
using System.Collections;
using UnityEngine;
using System;
using Unity.VisualScripting;
using Mirror;
using System.Linq;



public class MovingPlatform :  ActivatableObjectEntity
{
    [CustomHeader("Moving Platform")]
    public Vector2[] paths;
    [ContextMenu("Add Current Position")]
    public void AddCurrentPosition() 
    {
        if(paths.Length > 0) 
        {
            Vector2[] newPaths = new Vector2[paths.Length + 1];
            for (int i = 0; i < paths.Length; i++)
            {
                newPaths[i] = paths[i];
            }
            newPaths[paths.Length] = transform.position;
            paths = newPaths;
        }
        else
        {
            paths = new Vector2[1];
            paths[0] = transform.position;
        }
       

    }


    public float moveSpeed;

    [Header("Main")]

    [ReadOnly]
    public float step;
    [ReadOnly]
    public Vector2 dir;
    // public event Action<Vector2> MoveAction;
    private AddForcePlatform addForcePlatform;
    private bool onActive;

    // [SerializeField] GameObject rail_Prefabs;
    // [SerializeField] LineRenderer rail_Line;


    private MovingPlatform_Net MovingPlatform_Net => GetComponent<MovingPlatform_Net>();
    private void Awake()
    {
        // _rb = GetComponent<Rigidbody2D>();
        addForcePlatform = GetComponent<AddForcePlatform>();
        addForcePlatform.Init();
        
    }


    #region  GET,SET (Will take care this logic)
    public override T GetData<T>()
    {
        if(typeof(T)==typeof(ButtonActivatableObjectStruct)){
            return (T)(object)new ButtonActivatableObjectStruct(id,activeRequirAmount,transform.position,transform.rotation,transform.localScale,paths,moveSpeed);
        }
        
        return default(T);

    }
    private bool onStartPrograss;
    public override async void SetData<T>(T data)
    {
         try{
            if (typeof(T) == typeof(ButtonActivatableObjectStruct))
            {   
                ButtonActivatableObjectStruct objData = (ButtonActivatableObjectStruct)(object)data;
                ButtonActivatedObjectStruct = objData;

                //Moving Platform
                paths = ConvertPaths(objData.paths);     
                moveSpeed = objData.moveSpeed;

            }
        }catch(Exception ex){
                Debug.Log($"{ex},{typeof(T)}");
        }

        if (Application.isPlaying)
        {
            // CreateRail(paths);
            MovingPlatform_Net.Server_CreateRail(paths);

            Util util = new Util();
            await util.Delay(() => { CheckActiveRequirAmount(); });
            //if(NetworkServer.active)Prograss();
            
        }
    }
    #endregion
//------------------------------------------------------0402
    // public void AddForce()
    // {
    //     StartCoroutine(AddForceCo());
    // }

    WaitForFixedUpdate waitSecond = new();
    // IEnumerator AddForceCo()
    // {
        
    //     while(true)
    //     {
    //         //MoveAction?.Invoke(MovingPlatform_Net.velocity);
    //         addForcePlatform.AddForce(MovingPlatform_Net.velocity);
            
    //         yield return waitSecond;
    //     }
    // }
    //------------------------------------------------------0402
    private void OnDestroy()
    {
        StopAllCoroutines();
    }

    //--------------------------------------------------------------------------------------------------------Refectoring 0406
    private void FixedUpdate()
    {
        if(onStartPrograss)
        {

        }
    }


    private void MovingPlatform_Prograss()
    {
        if (!onActive)
        {
            dir = Vector2.zero;
            return;
        }


    }
    //--------------------------------------------------------------------------------------------------------Refectoring 0406


    //public void Prograss(){
    //    if(paths.Length <=0) return;
    //    StartCoroutine(Prograss_Co(paths));
    //}

    //IEnumerator Prograss_Co(Vector2[] paths){
    //    int maxIndex = paths.Length;
    //    int index = 0;
    //    int increment = 1;
    //    Vector2 targetPosition = paths[index];

    //    while (true)
    //    {
    //            while(!onActive){
    //                dir = Vector2.zero;
    //                yield return null;
    //            }
    //        if (CheckDistance(_rb.position, targetPosition))
    //        {
    //            // _rb.velocity = Vector2.zero;
    //            _rb.position = targetPosition;

    //            index += increment;
    //            if (index >= maxIndex || index < 0)
    //            {
    //                if(index >=maxIndex && paths[maxIndex-1] == paths[0]){
    //                    index = 0;
    //                }else{
    //                    increment *= -1;
    //                    index += increment;
    //                }

    //            }

    //            targetPosition = paths[index];

    //        }
    //        MovingPlatform_Net.Server_MovePlatform(targetPosition);
    //        //MoveTowards(_rb.position, targetPosition);

    //        //MoveAction?.Invoke(dir*step);
    //        yield return waitSecond; 
    //    }
    //}

    #region  Activatable
    protected override void Activation()
    {
        onActive = true;
    }
    protected override void Deactivated()
    {
        onActive = false;
    }
    #endregion

    #region  Util
    private void MoveTowards(Vector2 curP,Vector2 target){
        dir = (target - curP).normalized;
        step = moveSpeed * Time.fixedDeltaTime;
        //----------------------------------------------------------------0402
        Vector2 moveDelta = dir * step;
        //_rb.MovePosition(_rb.position + moveDelta);
        //MovingPlatform_Net.Server_AddForce(dir*step);
        //----------------------------------------------------------------0402
        //_rb.position = Vector2.MoveTowards(_rb.position,_rb.position +dir,step);

        // addForcePlatform.AddForce(dir*step,step);
        //MovingPlatform_Net.Server_AddForce(dir*step,step);
    }

    private bool CheckDistance(Vector2 curPos,Vector2 targetPos){
        if(Vector3.Distance(curPos,targetPos) < 0.1f){
            return true;
        }
        return false;
    }
    /// <summary>
    /// This function adds the first index’s transform position to the paths array.
    /// </summary>
    /// <param name="paths"></param>
    /// <returns></returns>
     private Vector2[] ConvertPaths(Vector2[] paths){
        if (Application.isPlaying)
        {
            Vector2[] targetPaths = new Vector2[paths.Length + 1];
            targetPaths[0] = transform.position;
            for (int i = 1; i <= paths.Length; i++)
            {
                targetPaths[i] = paths[i - 1];
            }
            return targetPaths;
        }

        return paths;
    }

#endregion


}

