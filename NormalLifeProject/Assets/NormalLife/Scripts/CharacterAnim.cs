using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnim : MonoBehaviour
{
    private readonly int moveLeft = Animator.StringToHash("dodgeLeft");
    private readonly int moveRight = Animator.StringToHash("dodgeRight");
    private readonly int jumpUp = Animator.StringToHash("jump");
    private readonly int landing = Animator.StringToHash("landing");
    private readonly int falling = Animator.StringToHash("falling");
    private readonly int rolling = Animator.StringToHash("roll");
    private readonly int idle = Animator.StringToHash("idlePaint");
    public void PlayLeftAnim(Animator anim)
    {
        anim.Play(moveLeft);
    }
    public void PlayRightAnim(Animator anim)
    {
        anim.Play(moveRight);
    }public void PlayJumpUpAnim(Animator anim)
    {
        anim.Play(jumpUp);
    }public void CrossFadeJumpUpAnim(Animator anim,float time)
    {
        anim.CrossFadeInFixedTime(jumpUp, time);
    }public void PlayRollUpAnim(Animator anim)
    {
        anim.Play(rolling);
    }public void CrossFadeRollAnim(Animator anim,float time)
    {
        anim.CrossFadeInFixedTime(rolling, time);
    }public void PlayLandingAnim(Animator anim)
    {
        anim.Play(landing);
    }public void PlayFallingAnim(Animator anim)
    {
        anim.Play(falling);
    }public void PlayIdleAnim(Animator anim)
    {
        anim.Play(idle);
    }
    
    public static bool CompareCurAnimName(Animator anim, string name)
    {
        return anim.GetCurrentAnimatorStateInfo(0).IsName(name);
    }
}
