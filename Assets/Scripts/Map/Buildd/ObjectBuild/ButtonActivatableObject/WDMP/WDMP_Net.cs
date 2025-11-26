
using Mirror;
using UnityEngine;


public class WDMP_Net : ActivatableObject_Net_Entity
{
    [SerializeField] GameObject rail_Node_Prefabs;
    [SerializeField] LineRenderer rail_Line_Prefabs;


    private RaycastHit2D[] leftHitBuffer = new RaycastHit2D[10];
    private RaycastHit2D[] rightHitBuffer = new RaycastHit2D[10];

    [SerializeField] Transform leftPoint;
    [SerializeField] Transform rightPoint;


    
    [Header("Tilt Limits")]
    private float maxRotate = 70;

    // ====== 클라이언트 전용 상태 ======
    private float _c_localsoluteTilt;
    private float _c_curMoveSpeed;
    public float _c_curStep;
    // private bool _c_didInterpThisFrame;
    public Vector2 _c_dir;

    private float _displayed;            // 화면에 표시 중인 각도
    private float _smoothTime = 0.06f;
    private float _maxDegPerSec = 720f;
    // private float _snapEps = 0.25f;
    private float _angVel;               // SmoothDampAngle 내부속도

    private float tiltSpeed = 20;


    //Recover
    private float _recover_tilt_rate = 1f;
    private float _cur_recover_tilt_rate = 0;

    private enum TiltState
    {
        Tilting,         // 기울이는 중
        RecoveringTilt,  // 기울기 복귀 중
        RecoveringPos,    // 위치 복귀 중
        Idle
    }
    private TiltState _state = TiltState.RecoveringPos;

    [ServerCallback]
    void FixedUpdate()
    {
        var counts = GetHitLeftAndRightCount();
        bool hasHit = counts.leftHitCount > 0 || counts.rightHitCount > 0;

        switch (_state)
        {
            // ---------------------------
            // 기울이는 중
            // ---------------------------
            case TiltState.Tilting:
                if (hasHit && _onActive) //_onActive : Activation()
                {
                    _cur_recover_tilt_rate = 0f;
                    float delta = GetWeight(counts) * Time.fixedDeltaTime * tiltSpeed;
                    _c_localsoluteTilt = Mathf.Clamp(_c_localsoluteTilt + delta, -maxRotate, maxRotate);
                    UpdateDisplayedTilt(_c_localsoluteTilt);
                }
                else
                {
                    _cur_recover_tilt_rate = 0f;
                    _state = TiltState.RecoveringTilt;
                    Ani_ShutDown();
                }
                break;

            // ---------------------------
            // 기울기 복귀 중
            // ---------------------------
            case TiltState.RecoveringTilt:
                if (hasHit)
                {
                    _angVel = 0f;
                    _cur_recover_tilt_rate = 0f;
                    _state = TiltState.Tilting;
                    break;
                }

                _cur_recover_tilt_rate += Time.fixedDeltaTime;
                if (_cur_recover_tilt_rate >= _recover_tilt_rate)
                {

                    float next = Mathf.Abs(_c_localsoluteTilt) != 0 ? Mathf.MoveTowardsAngle(_c_localsoluteTilt, 0f, Time.fixedDeltaTime * 50f) : 0f;
                    _c_localsoluteTilt = next;
                     UpdateDisplayedTilt(_c_localsoluteTilt);
 
                    if (Mathf.Abs(_c_localsoluteTilt) < 0.01f)
                    {
                        _angVel = 0f;
                        _c_localsoluteTilt = 0f;
                        _displayed = 0f;
                        Rb.rotation = 0f;
                        _state = TiltState.RecoveringPos;
                    }
                }
                break;

            // ---------------------------
            // 위치 복귀 중
            // ---------------------------
            case TiltState.RecoveringPos:
                if (hasHit)
                {
                    _angVel = 0f;
                    _cur_recover_tilt_rate = 0f;
                    _state = TiltState.Tilting;
                    break;
                }

                Vector2 curPos = Rb.position;
                Vector2 nextPos = Vector2.MoveTowards(curPos, data.position, Time.fixedDeltaTime * data.moveSpeed);

                if ((nextPos - data.position).sqrMagnitude < 0.00001f)
                {
                    nextPos = data.position;
                    _state = TiltState.Idle;
                }

                Rb.position = nextPos;

                break;
            case TiltState.Idle:
                if (hasHit)
                {
                    _angVel = 0f;
                    _cur_recover_tilt_rate = 0f;
                    _state = TiltState.Tilting;
                    break;
                }

                break;
        }

        if (_state == TiltState.Tilting && Mathf.Abs(_c_curStep) > 0f)
        {
            Vector2 target = Rb.position + _c_dir * _c_curStep;
            target.x = Mathf.Clamp(target.x, minDis_Clamp, maxDis_Clamp);
            Rb.position = target;
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



    #region  Get Hit Left And Right Count (All Client)

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

    
    #region Init Sync 0623
    private bool _onActive;
    protected override void Active()
    {
        _onActive = true;
        Debug.Log("WDMP_Net Active");
    }
    protected override void Deactive()
    {
        _onActive = false;
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
        
        if (lw > rw)
        {
            Ani_Left();
        }
        else if (rw > lw)
        {
            Ani_Right();
        }
        else
        {
            Ani_ShutDown();
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


    // private float GetTargetTilt(float weight)
    // {
    //     var result = Rb.rotation + weight;
    //     return Mathf.Clamp(result, -maxRotate, maxRotate);
       
    // }

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


    // private bool Compare(Vector2 a, Vector2 b, float threshold = 0.01f)
    // {
    //     return (a - b).sqrMagnitude < threshold;
    // }
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



    #region  Clean
    public override void Clean_Value()
    {
        // Ani_ShutDown();
        Rb.rotation = 0;
        transform.rotation = Quaternion.Euler(0,0,0);

        _state = TiltState.Idle;

        _c_curStep  = 0;
        _angVel = 0;
       _displayed = 0;

        _cur_recover_tilt_rate = 0;
        _c_localsoluteTilt = 0;
        _c_dir = Vector2.zero;

    }
    #endregion
}
