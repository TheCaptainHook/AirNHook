using System;
using System.Collections;
using Mirror;
using UnityEngine;


public class WDMP_Net : ActivatableObject_Net_Entity
{
    [SerializeField] GameObject rail_Node_Prefabs;
    [SerializeField] LineRenderer rail_Line_Prefabs;

    [SyncVar] public Vector2 dir;
    [SyncVar] public float step;


    private float recoveryRate = 5;
    [ReadOnly]
    public float curRecoveryRate;
    Coroutine recoveryCoroutine;
    private bool onRecover;
    // private void Awake()
    // {
    //     // rb = GetComponent<Rigidbody2D>();
    //     // AnimationTilt();
    // }
    private void Update()
    {
        //------------------Recover Position
        RecoverPosition(); //Server
        //------------------Recover Position

        if (onActive) ShootRay();


    }
    private void RecoverPosition()
    {
        if (!isServer) return;
        if (data.moveDistance != 0 && !Compare(transform.position, data.position) && !onRecover)
        {
            curRecoveryRate += Time.fixedDeltaTime;
            if (curRecoveryRate >= recoveryRate)
            {
                onRecover = true;
                //Recover
                Rpc_RecoverPosition(transform.position);
            }
        }
    }

    #region  Shot Ray
    private RaycastHit2D[] leftHit;
    private RaycastHit2D[] rightHit;
    [SerializeField] Transform leftPoint;
    [SerializeField] Transform rightPoint;
    private float shotRayLength;

    private bool onMove;
    private float weight;
    private float releaseCount =1;
    private float curReleaseCount;
    private float maxRotate = 70; //only use server
    [SerializeField] LayerMask layerMask;

    private void ShootRay()
    {
        float lw = 0;
        float rw = 0;

#if UNITY_EDITOR
        Debug.DrawRay(leftPoint.position, -transform.right * shotRayLength, Color.red);
        Debug.DrawRay(rightPoint.position, transform.right * shotRayLength, Color.red);
#endif

        leftHit = Physics2D.RaycastAll(leftPoint.position, -transform.right, shotRayLength, layerMask);
        rightHit = Physics2D.RaycastAll(rightPoint.position, transform.right, shotRayLength, layerMask);


        foreach (RaycastHit2D hit in leftHit)
        {
            lw += Weight(hit);
        }

        foreach (RaycastHit2D hit in rightHit)
        {
            rw += Weight(hit);
        }

        //recover tilt
        if (leftHit.Length == 0 && rightHit.Length == 0)
        {
            if (Rb.rotation == 0) return;

            curReleaseCount += Time.deltaTime;
            if (curReleaseCount >= releaseCount)
            {
                onMove = false;
                //transform.rotation = Quaternion.Lerp(transform.rotation,Quaternion.identity,Time.fixedDeltaTime);
                Rb.rotation = Mathf.Lerp(Rb.rotation, 0, Time.fixedDeltaTime);
                if (Mathf.Abs(Rb.rotation) < 0.95f)
                {
                    Rb.rotation = 0;
                }
            }
        }
        else
        {
            curReleaseCount = 0;
            onMove = true;
        }

        if (!onMove) return;


        weight = lw - rw;

        //tilt platform
        Rotate(weight);
        //tilt animation


        //move platform
        if (data.moveDistance == 0) return;


        //------------- on hold ,,move
        // Vector2 dir = transform.rotation.z == 0 ? Vector2.zero : transform.rotation.z > 0 ? -Vector2.right : Vector2.right;
        // WDMP_Net.Server_SetDir(dir);

        // if (CheckMaxAndMinClamp(dir)){
        //     WDMP_Net.Server_SetStep(moveSpeed * rate * Time.fixedDeltaTime);
        // }else{
        //     //step = 0;
        //     WDMP_Net.Server_SetStep(0);
        // }

        // MoveTowards();   
        // MoveTowards(leftHit);
        // MoveTowards(rightHit);
        //------------- on hold

    }
    #endregion

    #region Recover Position
    [ClientRpc]
    private void Rpc_CancelRecoverPosition(Vector2 startPosition)
    {
        if (recoveryCoroutine != null)
        {
            transform.position = startPosition;
            StopCoroutine(recoveryCoroutine);
            recoveryCoroutine = null;
            onRecover = false;
        }
    }
    [ClientRpc]
    private void Rpc_RecoverPosition(Vector2 startPosition)
    {
        transform.position = startPosition;
        recoveryCoroutine = StartCoroutine(Recover_Co());
    }
   
    IEnumerator Recover_Co()
    {
        Vector2 dir = ((Vector3)data.position - transform.position).normalized;

        while (!Compare(transform.position, data.position))
        {
            transform.position += (Vector3)(dir * data.moveSpeed * Time.fixedDeltaTime);
            yield return null;
        }
        recoveryCoroutine = null;

        transform.position = data.position;
        onRecover = false;
    }

    #endregion
    
    #region Refectoring 0622
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

    }
    protected override void Deactive()
    {
        
    }

    [ReadOnly]
    public float maxDis_Clamp;
    [ReadOnly]
    public float minDis_Clamp;
    [ReadOnly]
    public Vector2 targetPosition;
   
    protected override void SetData(ButtonActivatableObjectStruct data)
    {
        base.SetData(data);

        var items = GetPath(data);
        maxDis_Clamp = items.max;
        minDis_Clamp = items.min;
        targetPosition = items.target;

        CreateRail(data.position, targetPosition);

        shotRayLength = Col.bounds.extents.x;
    }

    #endregion
    
    #region Refactoring 0622 Util
    private bool CheckMaxAndMinClamp(Vector2 dir)
    {
        if (dir == Vector2.right)
        {
            if (transform.position.x > maxDis_Clamp)
            {
                return false;
            }
        }
        else if (dir == -Vector2.right)
        {
            if (transform.position.x < minDis_Clamp)
            {
                return false;
            }
        }
        else if (dir == Vector2.zero)
        {
            return false;
        }

        return true;
    }
    
    public (Vector2 target, float min, float max) GetPath(ButtonActivatableObjectStruct data)
    {
        if (data.moveDistance == 0) return (default, 0, 0);

        Vector2 target = new Vector2(data.position.x + data.moveDistance, data.position.y); // 최대 이동거리

        float minDis_Clamp = data.position.x > target.x ? target.x : data.position.x;
        float maxDis_Clamp = data.position.x < target.x ? target.x : data.position.x;

        return (target, minDis_Clamp, maxDis_Clamp);

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

    float rate = 0;
    // z>0 : left , z<0 :right
    private void Rotate(float weight)
    {
        Vector3 euler = transform.rotation.eulerAngles;
        euler.z += weight;

        if(euler.z > 180){
            euler.z -= 360;
        }

        euler.z = Mathf.Clamp(euler.z , -maxRotate,maxRotate);
        rate = Mathf.Abs(euler.z) / maxRotate;

        Rb.rotation = euler.z;
    }

    #endregion




    [Server]
    public void Server_SetDir(Vector2 dir)
    {
        this.dir = dir;
    }
    [Server]
    public void Server_SetStep(float step)
    {
        if (step != 0)
        {
            //Stop Recover
            if (recoveryCoroutine != null)
            {
                StopCoroutine(recoveryCoroutine);
                onRecover = false;
            }
            curRecoveryRate = 0;
        }

        this.step = step;
    }
    [Server]
    public void Server_SetClamp(float min, float max)
    {
        this.minDis_Clamp = min;
        this.maxDis_Clamp = max;
    }

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
    // private readonly int leftDown = Animator.StringToHash("LeftDown");
    // private readonly int rightDown = Animator.StringToHash("RightDown");
  
    public void AnimationTilt()
    {
        // StartCoroutine(AnimaionTiltCoroutine());
    }
    // Animator animator;
    // Animator Animator { get { animator ??= GetComponent<Animator>(); return animator; } }
    // bool leftAni;
    // bool rightAni;
    // WaitForSeconds wait = new WaitForSeconds(0.1f);
    // private IEnumerator AnimaionTiltCoroutine()
    // {
    //     while(true)
    //     {
    //         var z = Rb.rotation;
    //         if (z > 0)
    //         {
    //             if (!leftAni)
    //             {
    //                 leftAni = true;
    //                 Animator.SetBool(leftDown, leftAni);
    //             }
    //             if (rightAni)
    //             {
    //                 rightAni = false;
    //                 Animator.SetBool(rightDown, rightAni);
    //             }
    //         }
    //         else if (z < -0.1)
    //         {
    //             if (leftAni)
    //             {
    //                 leftAni = false;
    //                 Animator.SetBool(leftDown, leftAni);
    //             }
    //             if (!rightAni)
    //             {
    //                 rightAni = true;
    //                 Animator.SetBool(rightDown, rightAni);
    //             }
    //         }else if(Mathf.Abs(z) < 0.1)
    //         {
    //             if(leftAni) { leftAni = false; Animator.SetBool(leftDown, leftAni); }
    //             if (rightAni) {  rightAni = false; Animator.SetBool(rightDown, rightAni); }
    //         }
    //             yield return wait;
    //     }
    // }

    #endregion
}
