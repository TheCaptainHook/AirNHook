using System;
using System.Collections;
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
    [ReadOnly]
    public float weightResult = 0;
    private float sendMsgRate = 0.05f;
    private float curSendMsgRate = 1;

    public float CurRotation => Rb.rotation;
    private void Update()
    {
        if (onActive)
        {
            //------------------[All Client]

            //------------------[All Client]

            if (isServer)
            {
                //------------------Recover Position,[Server]
                RecoverPosition(); //onPositionRecover
                RecoverTilt();

                //------------------Recover Position

                leftAndRightCounts = GetHitLeftAndRightCount();

                if (leftAndRightCounts.leftCount > 0 || leftAndRightCounts.rightCount > 0)
                {
                    Server_MainLogic(leftAndRightCounts);
                }
                
            }
            
  

        }

    }

    public void Server_MainLogic((int leftCount, int rightCount) leftAndRightCounts)     //------------------[Server]
    {
        curRecoveryPositionRate = 0;
        curRecoveryTiltRate = 0;

        curSendMsgRate += Time.deltaTime;
        weightResult += GetWeight(leftAndRightCounts);
        if (curSendMsgRate > sendMsgRate)
        {
            SendWeight(weightResult, Rb.rotation, transform.position);
            curSendMsgRate = 0;
            weightResult = 0;
        }
        
    }
    float targetTilt;
    Coroutine tiltCoroutine;
    Coroutine tiltMoveCoroutine;

    [ReadOnly]
    public Vector2 moveDir = Vector2.zero;

    [ReadOnly]
    public float step;

    // [ClientRpc]
    // private void Rpc_SendWeight(float weight,float curRot,Vector2 position)
    // {
    //     CancelRecover();

    //     Rb.rotation = curRot;
    //     transform.position = position;

    //     targetTilt = GetTargetTilt(weight);

    //     if (weight > 0) Ani_Left();
    //     else if (weight < 0) Ani_Right();


    //     if (tiltCoroutine == null)
    //     {
    //         tiltCoroutine = StartCoroutine(TiltCo());
    //     }

    //     if (data.moveDistance == 0) return;

    //     if (tiltMoveCoroutine == null)
    //     {
    //         tiltMoveCoroutine = StartCoroutine(TiltMoveCo());
    //     }
    // }

    private void SendWeight(float weight,float curRot,Vector2 position)
    {
        CancelRecover();

        Rb.rotation = curRot;
        Rb.position = position;

        targetTilt = GetTargetTilt(weight);

        if (weight > 0) Ani_Left();
        else if (weight < 0) Ani_Right();
        

        if (tiltCoroutine == null)
        {
            tiltCoroutine = StartCoroutine(TiltCo());
        }
        
        if (data.moveDistance == 0) return;
        
        if (tiltMoveCoroutine == null)
        {
            tiltMoveCoroutine = StartCoroutine(TiltMoveCo());
        }
    }

    [ReadOnly]
    public float tiltRate;
    private float tiltSpeed = 2;
    IEnumerator TiltCo()
    {
        while (Mathf.Abs(Rb.rotation - targetTilt) > 0.5f)
        {
            Rb.rotation = Mathf.Lerp(Rb.rotation, targetTilt, Time.fixedDeltaTime*tiltSpeed);
            tiltRate = Mathf.Abs(Rb.rotation) / maxRotate;
            step = data.moveSpeed * tiltRate * Time.fixedDeltaTime;

            moveDir = Rb.rotation > 0 ? -Vector2.right : Vector2.right;
            yield return null;
        }
        Rb.rotation = targetTilt;
        tiltCoroutine = null;
    }
    IEnumerator TiltMoveCo()
    {
        while (Mathf.Abs(Rb.rotation) >0)
        {
            var target = Rb.position + moveDir * step;
            target.x = Mathf.Clamp(target.x, minDis_Clamp, maxDis_Clamp);

            Rb.position = target;

            yield return null;
        }
        
        tiltMoveCoroutine = null; 
    }

    private void CancelTilt()
    {
        moveDir = Vector2.zero;
        step = 0;

        if (tiltCoroutine != null)
        {
            StopCoroutine(tiltCoroutine);
            tiltCoroutine = null;
        }
        if (tiltMoveCoroutine != null)
        {
            StopCoroutine(tiltMoveCoroutine);
            tiltMoveCoroutine = null;
        }

    }

  


    #region  Get Hit Left And Right Count (All Client)
    private RaycastHit2D[] leftHitBuffer = new RaycastHit2D[10];
    private RaycastHit2D[] rightHitBuffer = new RaycastHit2D[10];

    [SerializeField] Transform leftPoint;
    [SerializeField] Transform rightPoint;

    
    
    // private float weight;
    private float maxRotate = 70; //only use server

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
    
    // private void ShootRay()
    // {
    //     //------------- on hold ,,move
    //     // Vector2 dir = transform.rotation.z == 0 ? Vector2.zero : transform.rotation.z > 0 ? -Vector2.right : Vector2.right;
    //     // WDMP_Net.Server_SetDir(dir);

    //     // if (CheckMaxAndMinClamp(dir)){
    //     //     WDMP_Net.Server_SetStep(moveSpeed * rate * Time.fixedDeltaTime);
    //     // }else{
    //     //     //step = 0;
    //     //     WDMP_Net.Server_SetStep(0);
    //     // }

    //     // MoveTowards();   
    //     // MoveTowards(leftHit);
    //     // MoveTowards(rightHit);
    //     //------------- on hold

    // }
    #endregion

    #region Recover
       
    #region  Position
    private void RecoverPosition() //Server
    {
        if (!isServer) return;
        if (data.moveDistance != 0 && !Compare(transform.position, data.position) && !onRecoverPosition)
        {
            curRecoveryPositionRate += Time.deltaTime;
            if (curRecoveryPositionRate >= recoveryPositionRate)
            {
                onRecoverPosition = true;
                //Recover
                RecoverPosition(transform.position);
            }
        }
    }

    // [ClientRpc]
    // private void Rpc_RecoverPosition(Vector2 startPosition)
    // {
    //     CancelTilt();
    //     transform.position = startPosition;
    //     recoveryPositionCoroutine = StartCoroutine(RecoverPosition_Co());
    // }
    private void RecoverPosition(Vector2 startPosition)
    {
        CancelTilt();
        transform.position = startPosition;
        recoveryPositionCoroutine = StartCoroutine(RecoverPosition_Co());
    }
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
    private void RecoverTilt() //Server
    {
        if (!isServer || Rb.rotation == 0 || onRecoverTilt) return;
        
        curRecoveryTiltRate += Time.deltaTime;
        if (curRecoveryTiltRate >= recoveryTiltRate)
        {
            onRecoverTilt = true;
            RecoverTilt(Rb.rotation);
        }

    }
    // [ClientRpc]
    // private void Rpc_RecoverTilt(float curRot)
    // {
    //     CancelTilt();
    //     Ani_ShutDown();
    //     Rb.rotation = curRot;
    //     recoveryTiltCoroutine = StartCoroutine(RecoverTilt_Co());
    // }
    
    private void RecoverTilt(float curRot)
    {
        CancelTilt();
        Ani_ShutDown();
        Rb.rotation = curRot;
        recoveryTiltCoroutine = StartCoroutine(RecoverTilt_Co());
    }
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

    // float rate = 0;
    // z>0 : left , z<0 :right
    // private void Rotate(float weight)
    // {
    //     Vector3 euler = transform.rotation.eulerAngles;
    //     euler.z += weight;

    //     if(euler.z > 180){
    //         euler.z -= 360;
    //     }

    //     euler.z = Mathf.Clamp(euler.z , -maxRotate,maxRotate);
    //     rate = Mathf.Abs(euler.z) / maxRotate;

    //     Rb.rotation = euler.z;
    // }


    #endregion




    // [Server]
    // public void Server_SetDir(Vector2 dir)
    // {
    //     this.dir = dir;
    // }
    // [Server]
    // public void Server_SetStep(float step)
    // {
    //     if (step != 0)
    //     {
    //         //Stop Recover
    //         if (recoveryPositionCoroutine != null)
    //         {
    //             StopCoroutine(recoveryPositionCoroutine);
    //             onPositionRecover = false;
    //         }
    //         curRecoveryPositionRate = 0;
    //     }

    //     this.step = step;
    // }
    // [Server]
    // public void Server_SetClamp(float min, float max)
    // {
    //     this.minDis_Clamp = min;
    //     this.maxDis_Clamp = max;
    // }

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

    // public void AnimationTilt()
    // {
    //     // StartCoroutine(AnimaionTiltCoroutine());
    // }

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
