using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drone : BuildObj
{
    [CustomHeader("Drone")]
    public Vector2[] paths;

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


    // public override T GetData<T>()
    // {
        
    // }

    // public override void SetData<T>(T data)
    // {
        
    // }
}
