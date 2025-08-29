using Mirror;
using System;
using System.Collections;
using UnityEngine;



public enum DroneState{
    Forward,
    Idle,
    Back
}

/**
 *  1. Drone_Hook
 *  2. Drone_Lazer_var2
 *  3. Drone_Multipurpose
 *  4. MovingSaw
 * **/


[RequireComponent(typeof(NetworkIdentity),typeof(DroneEntity_Net),typeof(DrawDronePath))]
public class DroneEntity : BuildObj
{
    [CustomHeader("Drone")]
    public Vector2[] paths;

#if UNITY_EDITOR
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
#endif

    public float moveSpeed;
    
    private DroneStruct droneStruct;
    public DroneStruct DroneStruct {
        get{
            return droneStruct;
            }
        set{
            droneStruct = value;
            id = value.id;
            transform.position = value.position;
            transform.localScale = value.scale;
            paths = value.paths;
            moveSpeed = value.moveSpeed;
        }}
    private Coroutine animationMovingCoroutine;


    #region  Damageable Option
    private bool OnStop;
    public bool IsBroken;
    #endregion

    
    [Header("Components")]
    private Animator animator;
    private DroneEntity_Net net;
    protected DroneEntity_Net Net { get { net ??= GetComponent<DroneEntity_Net>(); return net; } }

    [Header("Animator")]
    private readonly int _Moveing = Animator.StringToHash("Moving");
    public float _Animation_Transition_Speed;


    #region GET,SET
    public override T GetData<T>()
    {
        if(typeof(T)==typeof(DroneStruct)){
            return (T)(object)new DroneStruct(id,transform.position,transform.localScale,ConvertPaths(paths),moveSpeed);
        }
        return default(T);

    }
    public override void SetData<T>(T data)
    {
        if(typeof(T)==typeof(DroneStruct)){
          DroneStruct dronsSt = (DroneStruct)(object)data;
          DroneStruct = dronsSt;
            //Init();
        //   Net.Server_InitSync();
        }

    }

   #endregion

    private void Awake(){
        animator = GetComponent<Animator>();
        
    }


    #region  Action

    private void Broken()
    {
        StopAllCoroutines();
        IsBroken = true;
        _rb.velocity = Vector2.zero;
        
    }
    #endregion



    #region  Main


    //-------------------------------------------------------------------------------------------------------------------------------------------------------------0412
    protected bool isStop;
    private void FixedUpdate()
    {
        if (Net.targetPosition == null) return;
        if(isStop) return;
        MoveToward();

    }
    private void MoveToward()
    {
        if (CheckDistanceAndDot()) return;
        if(isStop) return;
        _rb.MovePosition(_rb.position + Net.dir * DroneStruct.moveSpeed * Time.fixedDeltaTime);
    }
   
    private bool CheckDistanceAndDot()
    {
        bool t = Vector2.Distance(_rb.position, Net.targetPosition) < 0.1f;
        Vector2 curDir = (Net.targetPosition - _rb.position).normalized;
        bool d = Vector2.Dot(Net.dir, curDir) < 0.98f;

        return t || d;
        //-->
    }

  

    public virtual void DroneMovingAnimation(Vector2 dir){
        if(animator == null) return;
        if(animationMovingCoroutine != null){
            StopCoroutine(animationMovingCoroutine);
        }

        animationMovingCoroutine = StartCoroutine(DroneMovingAnimationCorountine(GetDroneState(dir)));
    }
    IEnumerator DroneMovingAnimationCorountine(DroneState state){

        if(!HasParameterOfType(animator,_Moveing,AnimatorControllerParameterType.Float)) yield break;
          
        float _Animator_MovingRate = animator.GetFloat(_Moveing);
        
        float targetRate = GetAnimatorMovingRate(state);
        while(!Mathf.Approximately(_Animator_MovingRate,targetRate)){  
            _Animator_MovingRate = Mathf.Lerp(_Animator_MovingRate,targetRate,_Animation_Transition_Speed * Time.fixedDeltaTime);
            animator.SetFloat(_Moveing,_Animator_MovingRate);
            yield return null;
        }
    }

    private bool HasParameterOfType(Animator animator,int stringToHash, AnimatorControllerParameterType type){
        foreach(AnimatorControllerParameter param in animator.parameters){
            if(param.nameHash == stringToHash){
                return true;
            }else{
                return false;
            }
        }
        return false;
    }

    #endregion

    public void SetDroneAnim(Vector2 dir)
    {
        DroneMovingAnimation(dir);
    }
    #region  Util
    private DroneState GetDroneState(Vector2 dir){
        if(dir.x >0){
            return DroneState.Forward;
        }else if(dir.x <0){
            return DroneState.Back;
        }
        else{
            return DroneState.Idle;
        }
    }
     protected Vector2[] ConvertPaths(Vector2[] paths){
        Vector2[] targetPaths = new Vector2[paths.Length+1];
        targetPaths[0] = transform.position;
        for(int i = 1; i<= paths.Length;i++){
            targetPaths[i] =  paths[i-1];
        }
        return targetPaths;
    }

    private float GetAnimatorMovingRate(DroneState state){
        switch(state){
            case DroneState.Forward:
            return 1;
            case DroneState.Idle:
            return 0;
            case DroneState.Back:
            return -1;
            default:
            return 0;
        }
    }
    #endregion

    #region  Editor

    public override void Editor_Setting(MapEditor mapEditor)
    {
        Vector2[] newVec = new Vector2[paths.Length-1];
        for(int i = 1;i<paths.Length;i++){
            newVec[i-1] = paths[i];
        }
        paths = newVec;
    }
    #endregion
}
