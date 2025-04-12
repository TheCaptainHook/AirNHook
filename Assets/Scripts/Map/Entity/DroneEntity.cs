using System;
using System.Collections;
using UnityEngine;



public enum DroneState{
    Forward,
    Idle,
    Back
}

[RequireComponent(typeof(DrawDronePath))]
public class DroneEntity : BuildObj
{
    [CustomHeader("Drone")]
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
          Init();
         
        }

    }

   #endregion

    private void Awake(){
        animator = GetComponent<Animator>();
        
    }

    private void OnDisable(){
        StopAllCoroutines();
    }



    #region  Action

     private void Broken(){
        StopAllCoroutines();
        IsBroken = true;
        _rb.velocity = Vector2.zero;
    }
    #endregion

    #region  Main


    //-------------------------------------------------------------------------------------------------------------------------------------------------------------0412
    private bool onReady;
    private void FixedUpdate()
    {
        if(onReady)
        {
            Prograss();
        }
    }

    private int maxIndex;
    private int index;
    private int increment;
    private Vector2 targetPosition;
    private Vector2 dir;
    private void Prograss()
    {
        if(CheckDistance(_rb.position,targetPosition))
        {
            index += increment;
            if (index >= maxIndex || index < 0)
            {
                if (index >= maxIndex && paths[maxIndex - 1] == paths[0])
                {
                    index = 0;
                }
                else
                {
                    increment *= -1;
                    index += increment;
                }

            }

            targetPosition = paths[index];
            dir = (targetPosition - _rb.position).normalized *Time.fixedDeltaTime;
            DroneMovingAnimation(GetDroneState(dir));
        }

        MoveToward(dir);
    }
    private void MoveToward(Vector2 dir)
    {
        _rb.MovePosition(_rb.position + dir*moveSpeed);
    }

    private void Init()
    {
        if (paths == null || paths.Length <=0) return;

        maxIndex = paths.Length;
        index = 0;
        increment = 1;
        targetPosition = paths[0];
        onReady = true;
    }
    //-------------------------------------------------------------------------------------------------------------------------------------------------------------0412


    //public void Prograss(){
    //    if(paths.Length <=0) return;
    //    StartCoroutine(Prograss_Co(paths));
    //}

    //IEnumerator Prograss_Co(Vector2[] paths){
    //    int maxIndex = paths.Length;
    //    int index = 0;
    //    int increment = 1;
    //    Vector2 targetPosition = paths[index];
        
    //    while (!IsBroken)
    //    {
    //        Vector2 dir = Vector2.zero;

    //        while(OnStop)
    //        {
    //            if(_rb.velocity.magnitude > 0){
    //                _rb.velocity = Vector2.zero;
    //            }
    //            yield return null;
    //        }
            
    //        if (CheckDistance(_rb.position, targetPosition))
    //        {
    //            _rb.velocity = Vector2.zero;
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
    //        //Animation
    //        dir = (targetPosition - _rb.position).normalized;
    //        DroneMovingAnimation(GetDroneState(dir));
            
    //        //Move, normalized moveSpeed 
    //        _rb.AddForce(dir,ForceMode2D.Force);
    //        if (_rb.velocity.magnitude > moveSpeed)
    //        {
    //            _rb.velocity = _rb.velocity.normalized * moveSpeed;
    //        }
    //        yield return null; 
    //    }
    //}

    private bool CheckDistance(Vector2 curPos,Vector2 targetPos){
        if(Vector3.Distance(curPos,targetPos) < 0.1f){
            return true;
        }
        return false;
    }

    protected virtual void DroneMovingAnimation(DroneState state){
        if(animator == null) return;
        if(animationMovingCoroutine != null){
            StopCoroutine(animationMovingCoroutine);
        }

        animationMovingCoroutine = StartCoroutine(DroneMovingAnimationCorountine(state));
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
        DroneMovingAnimation(GetDroneState(dir));
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
