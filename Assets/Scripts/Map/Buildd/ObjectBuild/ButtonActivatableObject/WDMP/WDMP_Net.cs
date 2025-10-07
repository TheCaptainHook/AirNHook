using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;


public class WDMP_Net : ActivatableObject_Net_Entity
{
    [SerializeField] GameObject rail_Node_Prefabs;
    [SerializeField] LineRenderer rail_Line_Prefabs;

    //Recovery Position
    private float recoveryPositionRate = 5;
    [ReadOnly]
    public float curRecoveryPositionRate;
    Coroutine recoveryPositionCoroutine;
    private bool onRecoverPosition;
    //Recovery Tilt
    private float recoveryTiltRate = 2;
    [ReadOnly]
    public float curRecoveryTiltRate;
    Coroutine recoveryTiltCoroutine;
    private bool onRecoverTilt;


    private (int leftCount, int rightCount) leftAndRightCounts;

    private float sendMsgRate = 0.1f;
    private float curSendMsgRate = 1;

    public float CurRotation => Rb.rotation;
    // private void Update()
    // {
    //     if (onActive)
    //     {
    //         //------------------[All Client]

    //         //------------------[All Client]

    //         if (isServer)
    //         {
    //             //------------------Recover Position,[Server]
    //             // RecoverPosition(); //onPositionRecover
    //             // RecoverTilt();

    //             //------------------Recover Position

    //             leftAndRightCounts = GetHitLeftAndRightCount();

    //             if (leftAndRightCounts.leftCount > 0 || leftAndRightCounts.rightCount > 0)
    //             {
    //                 Server_MainLogic(leftAndRightCounts);
    //             }

    //         }



    //     }

    // }
    //--------------------------------------------------------------------------------------------------------------Refactoring 1003
    private RaycastHit2D[] leftHitBuffer = new RaycastHit2D[10];
    private RaycastHit2D[] rightHitBuffer = new RaycastHit2D[10];

    [SerializeField] Transform leftPoint;
    [SerializeField] Transform rightPoint;


    
    [Header("Tilt Limits")]
    private float maxRotate = 70;
    private float sendInterval = 0.05f;      // 20Hz
    private float _c_curSendInterval = 0.05f;
    // ====== 서버 전용 상태 ======

    private int _s_serverTick;
    private float _s_lastestRec;

    // ====== 클라이언트 전용 상태 ======
    private float _c_localsoluteTilt;
    private float _c_curMoveSpeed;
    public float _c_curStep;
    private bool _c_didInterpThisFrame;
    public Vector2 _c_dir;

    struct Sample { public float tilt; public float tRec; public int tick; }
    Queue<Sample> _buf = new();

    private float _displayed;            // 화면에 표시 중인 각도
    private float _smoothTime = 0.06f;
    private float _maxDegPerSec = 720f;
    private float _snapEps = 0.25f;
    private float _angVel;               // SmoothDampAngle 내부속도


    private float tiltSpeed = 20;


    void FixedUpdate()
    {
        _c_didInterpThisFrame = false;
        var counts = GetHitLeftAndRightCount();

        if (counts.leftHitCount > 0 || counts.rightHitCount > 0)
        {
            float delta = GetWeight(counts) * Time.fixedDeltaTime * tiltSpeed;
            _c_localsoluteTilt = Mathf.Clamp(_c_localsoluteTilt + delta, -maxRotate, maxRotate);
            _c_curSendInterval += Time.fixedDeltaTime;

            UpdateDisplayedTilt(_c_localsoluteTilt);

            // if (_c_curSendInterval >= sendInterval)
            // {
            //     _c_curSendInterval = 0;
            //     Cmd_SendMessage(_c_localsoluteTilt, Time.time);
            // }

        }
        else      //Recover
        {
          
        }

        //Move Platform
        if (!_c_didInterpThisFrame && Mathf.Abs(_c_curStep) > 0)
        {
            var target = Rb.position + _c_dir * _c_curStep;
            target.x = Mathf.Clamp(target.x, minDis_Clamp, maxDis_Clamp);

            Rb.position = target;
        }
        //Move Platform

    }

    [Command(channel = Channels.Unreliable)]
    private void Cmd_SendMessage(float tilt, float tRec)
    {
        if (_s_lastestRec < tRec)
        {
            _s_lastestRec = tRec;
            _buf.Enqueue(new Sample { tilt = tilt, tRec = tRec, tick = ++_s_serverTick });
        }       
        
        if (_buf.Count >= 8) _buf.Dequeue();
    }

    [ClientRpc(channel = Channels.Unreliable)]
    private void Rpc_SetTilt(float tilt, float tRec)
    {
        //1. _c_localsoluteTilt 랑 tilt 비교
        if (Mathf.Abs(Mathf.DeltaAngle(_c_localsoluteTilt, tilt)) > _snapEps)
        {
            _c_didInterpThisFrame = true;
            ApplyInterpolation(tilt, tRec);
            return;
        }
        else
        {
            _c_didInterpThisFrame = true;
            _c_localsoluteTilt = tilt;
            Rb.rotation = tilt;
        }

    }
    //Right : +tilt, Left : -tilt
    //==================Client
      private void UpdateDisplayedTilt(float target)
    {
        float next = Mathf.SmoothDampAngle(
       _displayed,               // 현재 표시각을 기준으로
       target,                   // 목표(로컬/네트워크 모두 여기로 수렴)
       ref _angVel,
       _smoothTime,
       _maxDegPerSec,
       Time.fixedUnscaledDeltaTime
   );
        _displayed = Mathf.Clamp(next, -maxRotate, maxRotate);
        // Move from displayed
        _c_curMoveSpeed = Mathf.Abs(_displayed) / maxRotate * data.moveSpeed;
        _c_dir = _displayed > 0 ? -Vector2.right : Vector2.right;
        _c_curStep = _c_curMoveSpeed * Time.fixedUnscaledDeltaTime;

        Rb.rotation = _displayed;
    
    }
    private float kPredPos = 0.6f; // 0.5~0.8 사이로 튜닝

    private void ApplyInterpolation(float target, float tRect)
    {
        //Tilt interpolation
        float delay = Mathf.Min(Time.unscaledTime - tRect, 0.18f);
        float predicatedTarget = target + _angVel * delay * 0.7f;

        float prevDisplayed = _displayed;

        float next = Mathf.SmoothDampAngle(_displayed, predicatedTarget, ref _angVel, _smoothTime, _maxDegPerSec, Time.fixedUnscaledDeltaTime);
        _displayed = Mathf.Clamp(next, -maxRotate, maxRotate);
        Rb.rotation = _displayed;
        //Tilt interpolation
        //Move interpolation
        //현재 프레임 스텝 
        _c_curMoveSpeed = Mathf.Abs(_displayed) / maxRotate * data.moveSpeed;
        _c_dir = _displayed > 0 ? -Vector2.right : Vector2.right;
        _c_curStep = _c_curMoveSpeed * Time.fixedUnscaledDeltaTime;
        //예측 오프셋
        float speedPrev = Mathf.Abs(prevDisplayed) / maxRotate * data.moveSpeed;
        float avgSpeedDiff = (_c_curMoveSpeed - speedPrev) * 0.5f; // 속도 변화만 반영
        float predictedMoveOffset = avgSpeedDiff * delay * _c_dir.x * kPredPos;

        var pos = Rb.position;
        pos.x = Mathf.Clamp(pos.x + predictedMoveOffset, minDis_Clamp, maxDis_Clamp);

        Rb.position = pos;

        //Move interpolation

    }
    //==================Client


    // [ServerCallback]
    // void Update()
    // {
    //     if (_buf.Count == 0) return;

    //     var sample = _buf.Dequeue();

    //     while (_buf.Count > 0)
    //     {
    //         var term = _buf.Dequeue();
    //         if (term.tRec > sample.tRec) sample = term;
    //     }

    //     Rpc_SetTilt(sample.tilt,sample.tRec);
    // }


    // void Update()
    // {
    //     if (!isClient) return;

    //     // 1) 최신 샘플을 목표값으로 반영
    //     //    (지연이 있더라도 최신 틱을 따라가며 보간)
    //     float targetPlaybackTime = Time.time - _playbackDelay;
    //     while (_buffer.Count >= 2 && _buffer.Peek().time <= targetPlaybackTime)
    //     {
    //         var first = _buffer.Dequeue();               // [first]는 과거
    //         var second = _buffer.Peek();                 // [second]는 미래
    //                                                      // 두 샘플 사이에서 t를 계산
    //         float span = Mathf.Max(0.0001f, second.time - first.time);
    //         float t = Mathf.Clamp01((targetPlaybackTime - first.time) / span);

    //         float target = Mathf.LerpAngle(first.tilt, second.tilt, t);

    //         // 화면 회전(부드럽게 추종: SmoothDampAngle or MoveTowardsAngle)
    //         _displayed = Mathf.SmoothDampAngle(_displayed, target, ref _angVel, 0.10f, 720f, Time.deltaTime);

    //         // 물리와 부딪히지 않는 순수 연출이면 Transform 회전, 물리 반영 필요하면 rb.rotation
    //         // 여기선 렌더링 보간만 예시
    //         Rb.rotation = _displayed;
    //         return;
    //     }
    //     if (_buffer.Count == 1)
    //     {
    //         float target = _buffer.Peek().tilt;
    //         _displayed = Mathf.SmoothDampAngle(_displayed, target, ref _angVel, 0.10f, 720f, Time.deltaTime);
    //         Rb.rotation = _displayed;
    //     }




    //     // while (_buffer.Count > 0)
    //     // {
    //     //     var s = _buffer.Dequeue();
    //     //     _targetTilt = s.tilt;
    //     // }

    //     // float cur = Rb.rotation;
    //     // float next = Mathf.SmoothDampAngle(
    //     //     cur,
    //     //     _targetTilt,
    //     //     ref _angVel,
    //     //     smoothTime,
    //     //     maxDegPerSec,
    //     //     Time.deltaTime // 렌더 보간이므로 deltaTime 사용
    //     // );

    //     // if (Mathf.Abs(Mathf.DeltaAngle(next, _targetTilt)) <= snapEps)
    //     //     next = _targetTilt;
    //     // Rb.rotation = Mathf.Clamp(next, -maxRotate, maxRotate);


    // }
    //--------------------------------------------------------------------------------------------------------------Refactoring 1003


    public void Server_MainLogic((int leftCount, int rightCount) leftAndRightCounts)     //------------------[Server]
    {
        // curRecoveryPositionRate = 0;
        // curRecoveryTiltRate = 0;

        // curSendMsgRate += Time.deltaTime;
        // weightResult += GetWeight(leftAndRightCounts);
        // weightResult = Mathf.Clamp(weightResult, -maxRotate, maxRotate);

        // SendWeight(weightResult, Rb.rotation, transform.position);


        // if (curSendMsgRate > sendMsgRate)
        // {
        //     SendWeight(weightResult, Rb.rotation, transform.position);
        //     curSendMsgRate = 0;
        //     weightResult = 0;
        // }
    }

 
    // Coroutine tiltMoveCoroutine;

    // [ReadOnly]
    // [SyncVar]
    // public Vector2 moveDir = Vector2.zero;

    // [ReadOnly]
    // [SyncVar]
    // public float step;


    //------------------- 1003 Refactoring
    // [SyncVar(hook= nameof(Hook_Tilt))] public float _tilt;
    // //------------------- 1003 Refactoring


    // private void SendWeight(float weight, float curRot, Vector2 position)
    // {
    //     _tilt = GetTargetTilt(weight);
 
    // }
    // private void Hook_Tilt(float oldValue,float newValue)
    // {
    //     if (oldValue == newValue) return;
    //     Rb.rotation = newValue;
    // }


    [ReadOnly]
    public float tiltRate;

    // IEnumerator TiltCo()
    // {
    //     while (Mathf.DeltaAngle(Rb.rotation,targetTilt) > 0.5f)
    //     {
    //         Rb.rotation = Mathf.Lerp(Rb.rotation, targetTilt, Time.fixedDeltaTime*tiltSpeed);
    //         // tiltRate = Mathf.Abs(Rb.rotation) / maxRotate;
    //         // step = data.moveSpeed * tiltRate * Time.fixedDeltaTime;

    //         // moveDir = Rb.rotation > 0 ? -Vector2.right : Vector2.right;
    //         yield return null;
    //     }
    //     Rb.rotation = targetTilt;
    //     tiltCoroutine = null;
    // }
 
    // IEnumerator TiltMoveCo()
    // {
    //     while (Mathf.Abs(Rb.rotation) >0)
    //     {
    //         var target = Rb.position + moveDir * step;
    //         target.x = Mathf.Clamp(target.x, minDis_Clamp, maxDis_Clamp);

    //         Rb.position = target;

    //         yield return null;
    //     }
        
    //     tiltMoveCoroutine = null; 
    // }

    // private void CancelTilt()
    // {
    //     moveDir = Vector2.zero;
    //     step = 0;

    //     if (tiltCoroutine != null)
    //     {
    //         StopCoroutine(tiltCoroutine);
    //         tiltCoroutine = null;
    //     }
    //     if (tiltMoveCoroutine != null)
    //     {
    //         StopCoroutine(tiltMoveCoroutine);
    //         tiltMoveCoroutine = null;
    //     }

    // }

  


    #region  Get Hit Left And Right Count (All Client)

    
    
    // private float weight;


    [SerializeField] LayerMask layerMask;
    [ReadOnly]
    public float shotRayLength;
    private (int leftHitCount, int rightHitCount) GetHitLeftAndRightCount()
    {

#if UNITY_EDITOR
        Debug.DrawRay(leftPoint.position, -transform.right * shotRayLength, Color.red);
        Debug.DrawRay(rightPoint.position, transform.right * shotRayLength, Color.red);
#endif
                
        int hitLeftCount = Physics2D.RaycastNonAlloc(leftPoint.position, -transform.right, leftHitBuffer, shotRayLength, layerMask);
        int hitRightCount = Physics2D.RaycastNonAlloc(rightPoint.position, transform.right, rightHitBuffer, shotRayLength, layerMask);      

        return (hitLeftCount, hitRightCount);

    }
    

    #endregion

    #region Recover
       
    #region  Position
    // private void RecoverPosition() //Server
    // {
    //     if (!isServer) return;
    //     if (data.moveDistance != 0 && !Compare(transform.position, data.position) && !onRecoverPosition)
    //     {
    //         curRecoveryPositionRate += Time.deltaTime;
    //         if (curRecoveryPositionRate >= recoveryPositionRate)
    //         {
    //             onRecoverPosition = true;
    //             //Recover
    //             RecoverPosition(transform.position);
    //         }
    //     }
    // }

    // private void RecoverPosition(Vector2 startPosition)
    // {
    //     CancelTilt();
    //     transform.position = startPosition;
    //     recoveryPositionCoroutine = StartCoroutine(RecoverPosition_Co());
    // }
    IEnumerator RecoverPosition_Co()
    {
        Vector2 dir = ((Vector3)data.position - transform.position).normalized;

        while (!Compare(transform.position, data.position))
        {
            transform.position += (Vector3)(dir * data.moveSpeed * Time.fixedDeltaTime);
            yield return null;
        }

        curRecoveryPositionRate = 0;
        recoveryPositionCoroutine = null;
        transform.position = data.position;
        onRecoverPosition = false;
    }

    #endregion
    #region  Tilt
    // private void RecoverTilt() //Server
    // {
    //     if (!isServer || Rb.rotation == 0 || onRecoverTilt) return;
        
    //     curRecoveryTiltRate += Time.deltaTime;
    //     if (curRecoveryTiltRate >= recoveryTiltRate)
    //     {
    //         onRecoverTilt = true;
    //         RecoverTilt(Rb.rotation);
    //     }

    // }

    // private void RecoverTilt(float curRot)
    // {
    //     CancelTilt();
    //     Ani_ShutDown();
    //     Rb.rotation = curRot;
    //     recoveryTiltCoroutine = StartCoroutine(RecoverTilt_Co());
    // }
    IEnumerator RecoverTilt_Co()
    {
        while (Mathf.Abs(Rb.rotation) > 0)
        {
            Rb.rotation = Mathf.Lerp(Rb.rotation, 0, Time.fixedDeltaTime);
            if (Mathf.Abs(Rb.rotation) < 0.9f) Rb.rotation = 0;
            yield return null;
        }

        recoveryTiltCoroutine = null;
        curRecoveryTiltRate = 0;
        onRecoverTilt = false;
    }
    #endregion

    private void CancelRecover()
    {
        if (recoveryPositionCoroutine != null)
        {
            curRecoveryPositionRate = 0;
            StopCoroutine(recoveryPositionCoroutine);
            recoveryPositionCoroutine = null;
            onRecoverPosition = false;
        }
        if (recoveryTiltCoroutine != null)
        {
            curRecoveryTiltRate = 0;
            StopCoroutine(recoveryTiltCoroutine);
            recoveryTiltCoroutine = null;
            onRecoverTilt = false;
        }
    }
   
    #endregion
    
    #region Init Sync 0623
    private WeightDetectionMoveingPlatform wdmp;
    private WeightDetectionMoveingPlatform WDMP
    {
        get
        {
            wdmp ??= GetComponent<WeightDetectionMoveingPlatform>();
            return wdmp;
        }
    }
    protected override void Active()
    {
        Debug.Log("WDMP_Net Active");
    }
    protected override void Deactive()
    {
        Debug.Log("WDMP_Net Deactive");
    }

    [ReadOnly]
    public float maxDis_Clamp;
    [ReadOnly]
    public float minDis_Clamp;
    [ReadOnly]
    public Vector2 targetPosition;
   
    protected override void SetData(ButtonActivatableObjectStruct data) //all client
    {
        base.SetData(data);

        var items = GetPath(data);
        maxDis_Clamp = items.max;
        minDis_Clamp = items.min;
        targetPosition = items.target == Vector2.zero ? data.position : items.target;

        CreateRail(data.position, targetPosition);
        shotRayLength = Col.bounds.extents.x;
    }

    #endregion
    
    #region Refactoring 0622 Util
    
    public (Vector2 target, float min, float max) GetPath(ButtonActivatableObjectStruct data)
    {
        if (data.moveDistance == 0) return (default, 0, 0);

        Vector2 target = new Vector2(data.position.x + data.moveDistance, data.position.y); // 최대 이동거리

        float minDis_Clamp = data.position.x > target.x ? target.x : data.position.x;
        float maxDis_Clamp = data.position.x < target.x ? target.x : data.position.x;

        return (target, minDis_Clamp, maxDis_Clamp);

    }

     private float GetWeight((int left, int right) counts)
    {
        float lw = 0;
        float rw = 0;

        for (int i = 0; i < counts.left; i++)
        {
            lw += Weight(leftHitBuffer[i]);
        }

        for (int i = 0; i < counts.right; i++)
        {
            rw += Weight(rightHitBuffer[i]);
        }
        return lw - rw;
    }
    
    private float Weight(RaycastHit2D hit)
    {
        if (hit.collider.TryGetComponent(out HookSM hook))
        {
            if (hook.isSwinging)
            {
                return 0;
            }

        }
        if (hit.collider.TryGetComponent(out Rigidbody2D component))
        {
            float dis = Mathf.Floor(Vector3.Distance(transform.position, hit.point) * 100) / 100;
            float mass = component.mass;
            return dis * mass;
        }

        return 0;

    }


    private float GetTargetTilt(float weight)
    {
        var result = Rb.rotation + weight;
        return Mathf.Clamp(result, -maxRotate, maxRotate);
       
    }



    #endregion



    #region  Create Node
    Transform container;
    GameObject railNode_1;
    GameObject railNode_2;
    LineRenderer line;

    private void CreateRail(Vector2 org,Vector2 target) //rail node, rail lineRenderer
    {
        Transform parents = MapEditor.Instance.dontSaveObjectTransform;
        container = new GameObject("Rail_Container").transform;
        container.SetParent(parents);

        line = Instantiate(rail_Line_Prefabs, container);
        //Draw Line
        DrawLine(line,org,target);

        railNode_1 = Instantiate(rail_Node_Prefabs, container);
        railNode_1.transform.position = line.GetPosition(0);

        railNode_2 = Instantiate(rail_Node_Prefabs, container);
        railNode_2.transform.position = line.GetPosition(1);
    }
    private void DrawLine(LineRenderer line,Vector2 org,Vector2 target)
    {
        line.positionCount = 2;
        line.SetPosition(0, org); 
        line.SetPosition(1, target);
    }


    private bool Compare(Vector2 a, Vector2 b, float threshold = 0.01f)
    {
        return (a - b).sqrMagnitude < threshold;
    }
    #endregion


    #region Animation
    Animator animator;
    Animator Animator { get { animator ??= GetComponent<Animator>(); return animator; } }

    private readonly int leftDown = Animator.StringToHash("LeftDown");
    private readonly int rightDown = Animator.StringToHash("RightDown");
    bool leftAni;
    bool rightAni;

    public void Ani_Right()
    {
        if (leftAni)
        {
            leftAni = false;
            Animator.SetBool(leftDown, leftAni);
        }
        if(!rightAni)
        {
            rightAni = true;
            Animator.SetBool(rightDown, rightAni);
        }
    }
    public void Ani_Left()
    {
        if (rightAni)
        {
            rightAni = false;
            Animator.SetBool(rightDown, rightAni);
        }

        if (!leftAni)
        {
            leftAni = true;
            Animator.SetBool(leftDown, leftAni);
        }
    }

    public void Ani_ShutDown()
    {
        leftAni = false; 
        Animator.SetBool(leftDown, leftAni);
        rightAni = false; 
        Animator.SetBool(rightDown, rightAni);
    }

    #endregion
}
