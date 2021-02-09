using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class FSM {
    // 定义函数指针类型
    public delegate void FSMTranslationCallfunc();    /// <summary>
    public delegate void FSMUpdateCallFunc(Character character);    /// <summary>
    /// 状态类
    /// </summary>
    public class FSMState
    {
        public string name;
        public event FSMUpdateCallFunc updateEvent;
        public FSMState(string name,FSMUpdateCallFunc method = null)
        {
            this.name = name;
            updateEvent += method;
        }
        //update方法
        public void OnUpdate(Character curChara)
        {
            updateEvent?.Invoke(curChara);
        }
        ~FSMState()
        {
            
        }
        /// <summary>
        /// 存储事件对应的条转
        /// </summary>
        public Dictionary <string,FSMTranslation> TranslationDict = new Dictionary<string,FSMTranslation>();
    }
    /// <summary>
    /// 跳转类
    /// </summary>
    public class FSMTranslation
    {
        public FSMState fromState;
        public string name;
        public FSMState toState;
        public FSMTranslationCallfunc callfunc; // 回调函数
        
        public FSMTranslation(FSMState fromState,string name, FSMState toState,FSMTranslationCallfunc callfunc)
        {
            this.fromState = fromState;
            this.toState   = toState;
            this.name = name;
            this.callfunc = callfunc;
        }
    }
    // 当前状态
    private FSMState mCurState;

    Dictionary <string,FSMState> StateDict = new Dictionary<string,FSMState>();
    /// <summary>
    /// 添加状态
    /// </summary>
    /// <param name="state">State.</param>
    public void AddState(FSMState state)
    {
        if(!StateDict.ContainsKey(state.name))
            StateDict [state.name] = state;
    }
    /// <summary>
    /// 添加条转
    /// </summary>
    /// <param name="translation">Translation.</param>
    public void AddTranslation(FSMTranslation translation)
    {
        StateDict [translation.fromState.name].TranslationDict [translation.name] = translation;
    }
    /// <summary>
    /// 启动状态机
    /// </summary>
    /// <param name="state">State.</param>
    public void Start(FSMState state)
    {
        mCurState = state;
    }    
    public void Start(string stateName)
    {
        if(StateDict.ContainsKey(stateName))
            mCurState = StateDict[stateName];
    }
    /// <summary>
    /// 处理事件
    /// </summary>
    /// <param name="name">Name.</param>
    public void HandleEvent(string name)
    {
        if (mCurState != null && mCurState.TranslationDict.ContainsKey(name)) {
            Debug.LogWarning ("fromState:" + mCurState.name);

            mCurState.TranslationDict [name].callfunc ();
            mCurState = mCurState.TranslationDict [name].toState;


            Debug.LogWarning ("toState:" + mCurState.name);
        }
    }

    public void OnStateUpdate(Character curChara)
    {
        mCurState.OnUpdate(curChara);
    }

    public string GetCurrentStateName()
    {
        return mCurState.name;
    }
}