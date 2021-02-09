using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class CharacterFSM : MonoBehaviour
{
    public string curFSMState;
    private Character mCharacter;

    private FSM playerFSM;

    private float runSpeed;

    // Start is called before the first frame update
    void Awake()
    {
        mCharacter = GetComponent<Character>();
        mCharacter.FSMController = this;
        mCharacter.fsm = new FSM();
        playerFSM = mCharacter.fsm;
        //        Idle,               闲置
        //        Run,                跑
        //        Jump,               一段跳
        //        DoubleJump,         二段跳
        //        Die,                挂彩

        // 创建状态
        FSM.FSMState idleState = new FSM.FSMState("idle",IdleStateUpdate);
        FSM.FSMState runState  = new FSM.FSMState("run",RunStateUpdate);
        FSM.FSMState jumpState = new FSM.FSMState("jump",JumpStateUpdate);
        //FSM.FSMState doubleJumpState = new FSM.FSMState("double_jump",DoubleJumpStateUpdate);
        FSM.FSMState dieState  = new FSM.FSMState("die",DieStateUpdate);
        FSM.FSMState rollState  = new FSM.FSMState("roll",RollStateUpdate);
        // 创建跳转
        FSM.FSMTranslation startTranslation1 = new FSM.FSMTranslation(idleState,"start",runState,StartRun);

        FSM.FSMTranslation touchTranslation1 = new FSM.FSMTranslation(runState,"touch_down",jumpState,EnterJump);
        // FSM.FSMTranslation touchTranslation2 = new FSM.FSMTranslation(jumpState,"touch_down",doubleJumpState,EnterDoubleJump);

        FSM.FSMTranslation rollTranslation1 = new FSM.FSMTranslation(runState,"roll",rollState,EnterRoll);


        FSM.FSMTranslation landTranslation1 = new FSM.FSMTranslation(jumpState,"land",runState,EnterRun);
        // FSM.FSMTranslation landTranslation2 = new FSM.FSMTranslation(doubleJumpState,"land",runState,EnterRun);
        FSM.FSMTranslation landTranslation3 = new FSM.FSMTranslation(rollState,"land",runState,EnterRun);

        // 添加状态
        mCharacter.fsm.AddState (idleState);
        mCharacter.fsm.AddState (runState);
        mCharacter.fsm.AddState (jumpState);
        // mCharacter.fsm.AddState (doubleJumpState);
        mCharacter.fsm.AddState (dieState);
        mCharacter.fsm.AddState (rollState);

        // 添加跳转
        mCharacter.fsm.AddTranslation (startTranslation1);
        mCharacter.fsm.AddTranslation (touchTranslation1);
        mCharacter.fsm.AddTranslation (rollTranslation1);
        mCharacter.fsm.AddTranslation (landTranslation3);
        // mCharacter.fsm.AddTranslation (touchTranslation2);
        mCharacter.fsm.AddTranslation (landTranslation1);
        // mCharacter.fsm.AddTranslation (landTranslation2);

        mCharacter.fsm.Start (idleState);
    }



    private void DoubleJumpStateUpdate(Character character)
    {
        Debug.Log("Double Jump Update!");
    }
    
    private void RollStateUpdate(Character character)
    {
        Debug.Log("Roll Update!");
        character.GetInputValue();
        mCharacter.MoveFuc();
        character.CheckCenter();
        character.CheckDir();
        //character.cc. = Vector3.zero;
        //下落方法绑定到jump里了
        mCharacter.Roll();
    }

    private void DieStateUpdate(Character character)
    {
        Debug.Log("Die Update!");
    }

    private void JumpStateUpdate(Character character)
    {
        Debug.Log("Jump Update!");
        character.GetInputValue();

        character.CheckCenter();
        character.CheckDir();
        character.MoveFuc();
        //character.cc. = Vector3.zero;
        //下落方法绑定到jump里了
        mCharacter.Jump();
    }

    private void RunStateUpdate(Character character)
    {
        Debug.Log("Run Update!");
        character.GetInputValue();
        character.MoveSidePos();
        character.TurnDirection();
        character.CheckCenter();
        character.CheckDir();
        character.MoveFuc();
        character.Jump();
        character.Roll();
    }

    private void IdleStateUpdate(Character character)
    {
        Debug.Log("Idle Update!");
        if (character.forwardSpeed != 0)
        {
            runSpeed = character.forwardSpeed;
        }
        character.PlayIdleState();
        character.forwardSpeed = 0;
        character.GetInputValue();
        character.CheckCenter();
        character.CheckDir();
        character.MoveFuc();
        character.MoveFuc();
        //character.cc. = Vector3.zero;
        //下落方法绑定到jump里了
        character.Jump();
    }
    //从idle进入runstate 调用方法
    private void StartRun()
    {
        if (runSpeed != 0)
        {
            mCharacter.forwardSpeed = runSpeed;
        }
        else
        {
            mCharacter.forwardSpeed = 7f;
        }

        mCharacter.ResetAnim();
    }
    //从idle以外的状态 进入runstate 调用方法
    private void EnterRun()
    {
        Debug.Log("Idle Enter!");
        if (runSpeed != 0)
        {
            mCharacter.forwardSpeed = runSpeed;
        }
        else
        {
            mCharacter.forwardSpeed = 7f;
        }
    }
    private void EnterRoll()
    {
        Debug.Log("Roll Enter!");
        if (mCharacter.forwardSpeed != 0)
        {
            runSpeed = mCharacter.forwardSpeed;
        }
        mCharacter.forwardSpeed = 10f;
    }

    private void EnterDoubleJump()
    {
        Debug.Log("DoubleJump Enter!");
    }

    private void EnterJump()
    {
        Debug.Log("Jump Enter!");
        if (mCharacter.forwardSpeed != 0)
        {
            runSpeed = mCharacter.forwardSpeed;
        }
        mCharacter.forwardSpeed = 2;
    }

    // Update is called once per frame
    void Update()
    {
        playerFSM = mCharacter.fsm;
        playerFSM.OnStateUpdate(mCharacter);
        //获取 状态的name
        curFSMState = mCharacter.fsm.GetCurrentStateName();
        if (Input.GetKey(KeyCode.S))
        {
            playerFSM.HandleEvent("start");
        }        
        if (Input.GetKey(KeyCode.X))
        {
            playerFSM.Start("idle");
        }
    }
    
}
