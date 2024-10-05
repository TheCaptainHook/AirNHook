
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using System;

public class MovingPlatform :  BuildObj
{
    [CustomHeader("Moving Platform")]
    public Vector2[] paths;
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


    [Header("Main")]

    public float step;
    public Vector2 dir;
    public event Action<Vector2> MoveAction;

  private void Awake(){
    _rb = GetComponent<Rigidbody2D>();
  }


    #region  GET,SET (Will take care this logic)
   public override T GetData<T>()
    {
        if(typeof(T)==typeof(DroneStruct)){
            Debug.Log("Drone");
            return (T)(object)new DroneStruct(id,transform.position,transform.localScale,ConvertPaths(paths),moveSpeed);
        }
        return default(T);

    }
    public override void SetData<T>(T data)
    {
        if(typeof(T)==typeof(DroneStruct)){
          DroneStruct dronsSt = (DroneStruct)(object)data;
          DroneStruct = dronsSt;
        }
        //Test
        if(Application.isPlaying){
            Debug.Log("Drone Prograss");
            Prograss();
        }
        
    }
    #endregion

    // private void Start(){
    //     paths = ConvertPaths(paths);
    //     Prograss();
    // }

    public void Prograss(){
        if(paths.Length <=0) return;
        StartCoroutine(Prograss_Co(paths));
    }

    IEnumerator Prograss_Co(Vector2[] paths){
        int maxIndex = paths.Length;
        int index = 0;
        int increment = 1;
        Vector2 targetPosition = paths[index];
        
        while (true)
        {

            if (CheckDistance(_rb.position, targetPosition))
            {
                // _rb.velocity = Vector2.zero;
                _rb.position = targetPosition;

                index += increment;
                if (index >= maxIndex || index < 0)
                {
                    if(index >=maxIndex && paths[maxIndex-1] == paths[0]){
                        index = 0;
                    }else{
                        increment *= -1;
                        index += increment;
                    }
                    
                }

                targetPosition = paths[index];

            }
            
            MoveTowards(_rb.position, targetPosition);
            MoveAction?.Invoke(dir*step);
            yield return null; 
        }
    }

    private void MoveTowards(Vector2 curP,Vector2 target){
        dir = (target-curP).normalized;
        step = moveSpeed * Time.fixedDeltaTime; 
        _rb.position = Vector2.MoveTowards(_rb.position,_rb.position +dir,step);
        
    }

    private bool CheckDistance(Vector2 curPos,Vector2 targetPos){
        if(Vector3.Distance(curPos,targetPos) < 0.1f){
            return true;
        }
        return false;
    }

     private Vector2[] ConvertPaths(Vector2[] paths){
        Vector2[] targetPaths = new Vector2[paths.Length+1];
        targetPaths[0] = transform.position;
        for(int i = 1; i<= paths.Length;i++){
            targetPaths[i] =  paths[i-1];
        }
        return targetPaths;
    }

}

