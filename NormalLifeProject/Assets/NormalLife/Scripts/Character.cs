using System;

using UnityEngine;
[System.Serializable]
public enum SIDE
{
    Left,Mid,Right
}
public enum HitX { Left, Mid, Right, None }
public enum HitY { Up, Mid, Down, None }
public enum HitZ { Forward, Mid, BackWard, None }

public class Character : MonoBehaviour
{
    [Header("===== Status =====")]
    public SIDE mSide=SIDE.Mid;
    public bool swipeLeft;
    public bool swipeRight;    
    public bool isHorizontal;
    public bool isTurnLeft;
    public bool isTurnRight;
    public bool swipeUp;
    public bool swipeDown;
    [Header("===== Move Setting =====")]
    public float forwardSpeed;
    public float xOffset;
    public float xCenter;
    public float xSpeedMult;
    public float jumpPower;
    public Vector3 cusForwardDir;
    public Vector3 cusRightDir;
    public FSM fsm;
    
    private bool isJumping;
    private bool isRoll;
    private float newPos;
    //[SerializeField]
    private float centerX;
    //[SerializeField]
    private float curX;
    private float curY;
    private float colHeight;
    private float colCenterY;
    public CharacterController cc;
    private Animator mAnimator;
    private CharacterAnim mAnimCtl;
    private TurnStateTrigger triggerObj;
    [SerializeField]
    private HitX hitX= HitX.None;
    [SerializeField] private HitY hitY = HitY.None;
    [SerializeField] private HitZ hitZ = HitZ.None;

    public CharacterFSM FSMController { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        //transform.position = Vector3.zero;
        cc = GetComponent<CharacterController>();
        colHeight = cc.height;
        colCenterY = cc.center.y;
        mAnimCtl = GetComponent<CharacterAnim>();
        mAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        // swipeLeft = Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow);
        // swipeRight = Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow);
        // swipeUp = Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow);
        // swipeDown = Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow);
        
        
        //  没有到转弯处
        // if(!isTurnLeft && !isTurnRight)
        // {
        //     if (swipeLeft && !isRoll)
        //     {
        //         if (mSide == SIDE.Mid)
        //         {
        //             newPos = -xOffset * isGlobalRightMirror;
        //             mSide = SIDE.Left;
        //             mAnimCtl.PlayLeftAnim(mAnimator);
        //         }
        //         else if (mSide == SIDE.Right)
        //         {
        //             newPos = 0;
        //             mSide = SIDE.Mid;
        //             mAnimCtl.PlayLeftAnim(mAnimator);
        //         }
        //     }
        //     else if (swipeRight && !isRoll)
        //     {
        //         if (mSide == SIDE.Mid)
        //         {
        //             newPos = xOffset * isGlobalRightMirror;
        //             mSide = SIDE.Right;
        //             mAnimCtl.PlayRightAnim(mAnimator);
        //         }
        //         else if (mSide == SIDE.Left)
        //         {
        //             newPos = 0;
        //             mSide = SIDE.Mid;
        //             mAnimCtl.PlayRightAnim(mAnimator);
        //         }
        //     }
        //     
        // }
        //进入转向状态时执行的逻辑
        // if (swipeLeft && isTurnLeft)
        // {
        //     //旋转
        //     transform.Rotate(0.0f, -90.0f, 0.0f, Space.Self);
        //     isHorizontal = !isHorizontal;
        //     //方向矫正
        //     CheckDir();
        //     newPos *= isPosMirror;
        //     curX = newPos;
        //     //标记
        //     isTurnLeft = false;
        // }      
        
        //局部x轴向中心点计算
        
        
        // //各轴向速度增量计算
        // Vector3 forwardVec = cusForwardDir * (forwardSpeed * Time.deltaTime);
        // Vector3 rightVec = cusRightDir * ((float)curX - centerX);
        // Vector3 upVec = transform.up * (curY * Time.deltaTime);
        // //叠加
        // Vector3 moveVec = forwardVec + rightVec + upVec;
        // cc.Move(moveVec);
        
        // GetInputValue();
        // MoveSidePos();
        // TurnDirection();
        // CheckCenter();
        // CheckDir();
        // MoveFuc();
        // Jump();
        // Roll();
        //插值
        curX = Mathf.Lerp(curX, newPos, xSpeedMult * Time.deltaTime);
    }

    private void Init()
    {
        var transform1 = transform;
        transform.position = new Vector3(0.0f, 1.26f, 0.98f);
        transform.rotation = Quaternion.identity;
        cusForwardDir = Vector3.forward;
        cusRightDir = Vector3.right;
        mSide=SIDE.Mid;
        newPos = 0;
        curX = newPos;
        isGlobalRightMirror = 1;
        isPosMirror = 1;
    }
    //输入获取
    public void GetInputValue()
    {
        swipeLeft = Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow);
        swipeRight = Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow);
        swipeUp = Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow);
        swipeDown = Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow);
    }
    //道路的左中右方法
    public void MoveSidePos()
    {
        if(!isTurnLeft && !isTurnRight)
        {
            if (swipeLeft && !isRoll)
            {
                if (mSide == SIDE.Mid)
                {
                    newPos = -xOffset * isGlobalRightMirror;
                    mSide = SIDE.Left;
                    mAnimCtl.PlayLeftAnim(mAnimator);
                }
                else if (mSide == SIDE.Right)
                {
                    newPos = 0;
                    mSide = SIDE.Mid;
                    mAnimCtl.PlayLeftAnim(mAnimator);
                }
            }
            else if (swipeRight && !isRoll)
            {
                if (mSide == SIDE.Mid)
                {
                    newPos = xOffset * isGlobalRightMirror;
                    mSide = SIDE.Right;
                    mAnimCtl.PlayRightAnim(mAnimator);
                }
                else if (mSide == SIDE.Left)
                {
                    newPos = 0;
                    mSide = SIDE.Mid;
                    mAnimCtl.PlayRightAnim(mAnimator);
                }
            }
            
        }
    }
    //转向方法
    public void TurnDirection()
    {
        if (swipeLeft && isTurnLeft)
        {
            //旋转
            transform.Rotate(0.0f, -90.0f, 0.0f, Space.Self);
            isHorizontal = !isHorizontal;
            //方向矫正
            CheckDir();
            newPos *= isPosMirror;
            curX = newPos;
            //标记
            isTurnLeft = false;
        }        
        if (swipeRight && isTurnRight)
        {
            print("Turn Right!");
            transform.Rotate(0.0f, 90.0f, 0.0f, Space.Self);
            isHorizontal = !isHorizontal;
            CheckDir();
            newPos *= isPosMirror;
            curX = newPos;
            isTurnRight = false;
        }
    }
    
    private float isGlobalRightMirror = 1 ;
    //转弯后的位置是否镜像
    private float isPosMirror = 1 ;
    //移动方向的矫正
    public void CheckDir()
    {
        isPosMirror = 1;
        float isForwardMirror = 1;
        float isRightMirror = 1 ;
        Vector3 forwardDir = transform.forward;
        // if (forwardDir.z > 0.1f)
        // {
        //     isForwardMirror = 1;
        //     isRightMirror = 1;
        // }

        if (forwardDir.z < -0.1f)
        {
            isForwardMirror = 1;
            isRightMirror = -1;
        }

        if (forwardDir.x > 0.1f)
        {
            isRightMirror = -1;
            isForwardMirror = 1;
        }

        // if (forwardDir.x < -0.1f)
        // {
        //     isRightMirror = 1;
        //     isForwardMirror = 1;
        // }
        //-1 -1 no 1 -1 -1 1 yes 
        //x轴向 需要镜像变为不需要镜像 或者 不需要镜像变为需要镜像，newPos需要取反
        if (Math.Abs(isGlobalRightMirror - (-1f)) < 0.1f && Math.Abs(isRightMirror - (1)) < 0.1f||
        Math.Abs(isGlobalRightMirror - (1f)) < 0.1f && Math.Abs(isRightMirror - (-1)) < 0.1f)
        {
            isPosMirror = -1;
        }
        isGlobalRightMirror = isRightMirror;
        var transform1 = transform;
         cusForwardDir = transform1.forward * isForwardMirror;
         cusRightDir = transform1.right * isRightMirror;
    }
    //中心区域的矫正
    internal void CheckCenter()
    {
        if(isTurnLeft || isTurnRight) return;
        Vector3 triPos = Vector3.zero;

        if (isHorizontal)
        {
            if (triggerObj != null)
            {
                triPos = triggerObj.gameObject.transform.parent.transform.position;
                xCenter = triPos.z; 
            }
          
            float pos = transform.position.z;
            centerX = pos - xCenter;
        }
        else
        {            
            if (triggerObj != null)
            {
                triPos = triggerObj.gameObject.transform.parent.transform.position;
                xCenter = triPos.x; 
            }
            float pos = transform.position.x;
            centerX = pos - xCenter;
        }
        
    }

    public void PlayIdleState()
    {
        mAnimCtl.PlayIdleAnim(mAnimator);
    }
    public void MoveFuc()
    {
        //各轴向速度增量计算
        Vector3 forwardVec = cusForwardDir * (forwardSpeed * Time.deltaTime);
        Vector3 rightVec = cusRightDir * ((float)curX - centerX);
        Vector3 upVec = transform.up * (curY * Time.deltaTime);
        //叠加
        Vector3 moveVec = forwardVec + rightVec + upVec;
        cc.Move(moveVec);
    }
    public void Jump()
    {
        if (cc.isGrounded)
        {
            
            if(CharacterAnim.CompareCurAnimName(mAnimator,"falling"))
            {
                mAnimCtl.PlayLandingAnim(mAnimator);
                isJumping = false;
                fsm.HandleEvent("land");
            }
            if(swipeUp)
            {
                curY = jumpPower;
                mAnimCtl.CrossFadeJumpUpAnim(mAnimator, 0.1f);
                isJumping = true;
                fsm.HandleEvent("touch_down");
            }
        }
        else
        {
            curY -= jumpPower * 2 * Time.deltaTime;
            if (cc.velocity.y < -0.1f)
                mAnimCtl.PlayFallingAnim(mAnimator);
        }
    }
    float rollCounter = 0;
    public void Roll()
    {
        rollCounter -= Time.deltaTime;
        if (rollCounter < 0)
        {
            if(fsm.GetCurrentStateName().Equals("roll"))
            fsm.HandleEvent("land");
            rollCounter = 0f;
            isRoll = false;
            cc.center = new Vector3(0, colCenterY, 0);
            cc.height = colHeight;
        }
        if(swipeDown)
        {
            fsm.HandleEvent("roll");
            rollCounter = 0.2f;
            curY -= 10f;
            cc.center = new Vector3(0, colCenterY/2, 0);
            cc.height = colHeight/2;
            mAnimCtl.CrossFadeRollAnim(mAnimator, 0.1f);
            isRoll = true;
            isJumping = false; 
        }
    }
    public void OnCharacterColliderHit(Collider col)
    {
        hitX = GetHitX(col);
        hitY = GetHitY(col);
        hitZ = GetHitZ(col);
    }
    //设置状态可以右转
    public void SetTurnRight(bool isturn = false , TurnStateTrigger triObj = null)
    {
        isTurnRight = isturn;
        triggerObj = triObj;
    }
    //设置状态可以左转
    public void SetTurnLeft(bool isturn = false, TurnStateTrigger triObj = null)
    {
        isTurnLeft = isturn;
        triggerObj = triObj;
    }
    /// <summary>
    /// X,Y,Z方向计算碰撞部位
    /// </summary>
    /// <param name="col"></param>
    /// <returns></returns>
    public HitX GetHitX(Collider col)
    {
        Bounds char_bounds = cc.bounds;
        Bounds col_bounds = col.bounds;
        float minX = Mathf.Max(col_bounds.min.x, char_bounds.min.x);
        float maxX = Mathf.Min(col_bounds.max.x, char_bounds.max.x);
        float average = (minX + maxX) / 2f - col_bounds.min.x;
        HitX hit;
        //print(average);
        if (average > col_bounds.size.x - 0.33f)
            hit = HitX.Left;
        else if (average < 0.33f)
            hit = HitX.Right;
        else
            hit = HitX.Mid;
        return hit;
    } 
    public HitY GetHitY(Collider col)
    {
        Bounds char_bounds = cc.bounds;
        Bounds col_bounds = col.bounds;
        float minY= Mathf.Max(col_bounds.min.y, char_bounds.min.y);
        float maxY = Mathf.Min(col_bounds.max.y, char_bounds.max.y);
        float average = (minY + maxY) / 2f - col_bounds.min.y;
        average /= char_bounds.size.y;
        HitY hit;
       // print(average);
        if (average < 0.33f)
            hit = HitY.Up;
        else if (average < 0.66f)
            hit = HitY.Mid;
        else
            hit = HitY.Down;
        return hit;
    }   
    public HitZ GetHitZ(Collider col)
    {
        Bounds char_bounds = cc.bounds;
        Bounds col_bounds = col.bounds;
        float minZ= Mathf.Max(col_bounds.min.z, char_bounds.min.z);
        float maxZ = Mathf.Min(col_bounds.max.z, char_bounds.max.z);
        float average = (minZ + maxZ) / 2f - col_bounds.min.z;
        average /= char_bounds.size.z;
        HitZ hit;
//        print(average);
        if (average < 0.33f)
            hit = HitZ.Forward;
        else if (average < 0.66f)
            hit = HitZ.Mid;
        else
            hit = HitZ.BackWard;
        return hit;
    }

    public void OnGUI()
    {
        GUIStyle fontStyle=new GUIStyle();
        fontStyle.normal.background = null;    //这是设置背景填充的
        fontStyle.normal.textColor=new Color(1,0,0);   //设置字体颜色的
        fontStyle.fontSize = 60;   
        
        if (GUILayout.Button("重开",GUILayout.Width(300),GUILayout.Height(50)))
        {
            SceneHelper.Instance().ReloadCurrentScene();
        }
        GUILayout.Space(10);
        string state = fsm.GetCurrentStateName();
        GUILayout.Label("当前状态是 :"+state,fontStyle);
        GUILayout.Label("当前速度是 :"+forwardSpeed.ToString(),fontStyle);
        GUILayout.Space(10);
        fontStyle.fontSize = 40;
        GUILayout.Label("S开始Run X进入Idle",fontStyle);
        // if (GUILayout.Button("换个地图",GUILayout.Width(300),GUILayout.Height(30)))
        // {
        //     SceneHelper.Instance().ChangeRoadSample();
        //     Init();
        // }
    }

    public void ResetAnim()
    {
        mAnimCtl.PlayFallingAnim(mAnimator);
    }
}
