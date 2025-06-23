using System;
using UnityEngine;
using Mirror;


[RequireComponent(typeof(WDMP_Path))]
public class WeightDetectionMoveingPlatform : ActivatableObjectEntity
{
    [CustomHeader("Weight Detection Moving Platform")]

    #region Main

    #region  Main Field
    [Space(10)]
    [Header("Save Field")]
    public float moveDistance;
    public float moveSpeed;
    // private float maxRotate = 70; //only use server
    // [ReadOnly]
    // public float shotRayLength; //only use server
    // [Space(10)]
   
    #endregion


    // private RaycastHit2D[] leftHit;
    // private RaycastHit2D[] rightHit;

    // [ReadOnly]
    // [SerializeField] Transform leftPoint;
    // [ReadOnly]
    // [SerializeField] Transform rightPoint;

 
    // private bool onActive; 

    // [SerializeField] LayerMask layerMask;
    // [SerializeField] GameObject rail_Prefabs;
    // [SerializeField] LineRenderer rail_Line;
    
    // private float weight;
    // private float releaseCount =1;
    // private float curReleaseCount;
    #endregion



    #region  Get,Set
    public override T GetData<T>()
    {
        if(typeof(T)==typeof(ButtonActivatableObjectStruct)){
            return (T)(object)new ButtonActivatableObjectStruct(id,activeRequirAmount,transform.position,transform.rotation,transform.localScale,moveDistance,moveSpeed, indicator);
        }
        
        return default(T);

    }

    protected override void AdditionalInspectorConfig()
    {
        moveSpeed = ButtonActivatedObjectStruct.moveSpeed;
        moveDistance = ButtonActivatedObjectStruct.moveDistance;
    }

    #endregion

    #region  Activatable Object Entity
    public override void Activation()
    {
        Net.Server_ChangeOnActive(true);
    }
    public override void Deactivated()
    {
        Net.Server_ChangeOnActive(false);
    }
    #endregion



    private void Update()
    {
        // if(Net.onActive) ShootRay();
        
    }

    // - : right
    // + : left

    // bool onMove;
    // private void ShootRay()
    // {
    //     float lw = 0;
    //     float rw = 0;

    //     #if UNITY_EDITOR
    //     Debug.DrawRay(leftPoint.position, -transform.right * shotRayLength, Color.red);
    //     Debug.DrawRay(rightPoint.position, transform.right * shotRayLength, Color.red);
    //     #endif
        
    //     leftHit = Physics2D.RaycastAll(leftPoint.position,-transform.right, shotRayLength, layerMask);
    //     rightHit = Physics2D.RaycastAll(rightPoint.position,transform.right, shotRayLength, layerMask);


    //     foreach (RaycastHit2D hit in leftHit)
    //     {
    //         lw += Weight(hit);
    //     }

    //     foreach(RaycastHit2D hit in rightHit)
    //     {
    //         rw += Weight(hit);
    //     }

    //     //recover tilt
    //     if(leftHit.Length == 0 && rightHit.Length == 0)
    //     {
    //         if (_rb.rotation == 0) return;

    //         curReleaseCount +=Time.deltaTime;
    //         if(curReleaseCount >= releaseCount)
    //         {
    //             onMove = false;
    //             //transform.rotation = Quaternion.Lerp(transform.rotation,Quaternion.identity,Time.fixedDeltaTime);
    //             _rb.rotation = Mathf.Lerp(_rb.rotation, 0, Time.fixedDeltaTime);
    //             if (Mathf.Abs(_rb.rotation) < 0.95f) 
    //             {
    //                 _rb.rotation = 0;
    //             }
    //         }
    //     }else{
    //         curReleaseCount = 0;
    //         onMove = true;
    //     }
        
    //     if (!onMove) return;

            
    //     weight = lw-rw;

    //     //tilt platform
    //     Rotate(weight);
    //     //tilt animation

        
    //     //move platform
    //     if(moveDistance == 0) return;


    //     //------------- on hold ,,move
    //         // Vector2 dir = transform.rotation.z == 0 ? Vector2.zero : transform.rotation.z > 0 ? -Vector2.right : Vector2.right;
    //         // WDMP_Net.Server_SetDir(dir);

    //         // if (CheckMaxAndMinClamp(dir)){
    //         //     WDMP_Net.Server_SetStep(moveSpeed * rate * Time.fixedDeltaTime);
    //         // }else{
    //         //     //step = 0;
    //         //     WDMP_Net.Server_SetStep(0);
    //         // }

    //         // MoveTowards();   
    //         // MoveTowards(leftHit);
    //         // MoveTowards(rightHit);
    //     //------------- on hold

    // }
    
    // float rate = 0;
    // // z>0 : left , z<0 :right
    // private void Rotate(float weight)
    // {
    //     Vector3 euler = transform.rotation.eulerAngles;
    //     euler.z += weight;

    //     if(euler.z > 180){
    //         euler.z -= 360;
    //     }

    //     euler.z = Mathf.Clamp(euler.z , -maxRotate,maxRotate);
    //     rate = Mathf.Abs(euler.z) / maxRotate;

    //     _rb.rotation = euler.z;
    // }
    
    // private void MoveTowards(Vector2 dir)
    // {
    //     curTargetPot = _rb.position + dir;
    //     curTargetPot.x = Mathf.Clamp(curTargetPot.x, minDis_Clamp, maxDis_Clamp);
    //     _rb.position = Vector2.MoveTowards(_rb.position, curTargetPot, WDMP_Net.step);
    // }
    
    // private void MoveTowards(RaycastHit2D[] hits)
    // {
    //     foreach (RaycastHit2D hit in hits)
    //     {
    //         if (hit.collider != null && hit.collider.TryGetComponent(out Rigidbody2D component))
    //         {
    //             component.position = Vector2.MoveTowards(component.position, component.position + WDMP_Net.dir, WDMP_Net.step);
    //         }
    //     }
    // }





    #region  Util

 

        // private float Weight(RaycastHit2D hit)
        // {
        //     if (hit.collider.TryGetComponent(out HookSM hook))
        //     {
        //         if (hook.isSwinging)
        //         {
        //             return 0;
        //         }
            
        //     }
        //     if (hit.collider.TryGetComponent(out Rigidbody2D component))
        //     {
        //         float dis = Mathf.Floor(Vector3.Distance(transform.position, hit.point) * 100) / 100;
        //         float mass = component.mass;
        //         return dis * mass;
        //     }

        //     return 0;
   
        // }
        
    
#endregion
}
