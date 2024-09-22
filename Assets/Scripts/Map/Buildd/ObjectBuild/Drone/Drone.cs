using System.Collections;
using System.Collections.Generic;
using Steamworks;
using UnityEngine;

public class Drone : BuildObj
{
    [CustomHeader("Drone")]
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


    //TEST
    public bool OnError;
    //TEST

    //Custom Editor
    // 1. DrawDronePath
    //     - previousPathList , cur PathList Check and refrash.
    //     - Create LineRenderer util func.
    // 2. DrawDronePath_Editor
    //     - Tracking SerializedProperty, and linerenderer refrash.



    // 1. MapEditor - Create Drone Transfrom,
    // 2. Map
    //    - Craete Drone data struct
    //    - Add Map Drone Data Struct list
    // 3. Create_Tool 
    //    - 

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
            Prograss(DroneStruct.paths);
        }
        
    }

    private Vector2[] ConvertPaths(Vector2[] paths){
        Vector2[] targetPaths = new Vector2[paths.Length+1];
        targetPaths[0] = transform.position;
        for(int i = 1; i<= paths.Length;i++){
            targetPaths[i] =  paths[i-1];
        }
        return targetPaths;
    }

    private void Awake(){
        _rb = GetComponent<Rigidbody2D>();
    }
    private void OnDisable(){
        StopAllCoroutines();
    }
    #region  Main
    public void Prograss(Vector2[] paths){
        StartCoroutine(Prograss_Co(paths));
    }
    IEnumerator Prograss_Co(Vector2[] paths){
        int maxIndex = paths.Length;
        int index = 0;
        int increment = 1;
        Vector2 targetPosition = paths[index];

        while (true)
        {
            while(OnError){
                yield return null;
            }
            if (CheckDistance(_rb.position, targetPosition))
            {
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
            
            float step = moveSpeed * Time.deltaTime; 
            _rb.position = Vector2.MoveTowards(_rb.position, targetPosition, step);
            yield return null; 
        }
    }

    private bool CheckDistance(Vector2 curPos,Vector2 targetPos){
        if(Vector3.Distance(curPos,targetPos) < 0.001f){
            return true;
        }
        return false;
    }
    #endregion



    #region  Editor
    // public void StopPrograss(){
    //     StopAllCoroutines();
    //     _rb.position = 

    // }
    public override void Editor_Setting(Transform transform = default)
    {
        Vector2[] newVec = new Vector2[paths.Length-1];
        for(int i = 1;i<paths.Length;i++){
            newVec[i-1] = paths[i];
        }
        paths = newVec;
    }
    #endregion
}
