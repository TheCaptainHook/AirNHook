using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(Test_DroneLaser))]
public class Test_DroneLaserAngleTest : Editor
{
    Test_DroneLaser main;


    private void OnEnable()
    {
        main = (Test_DroneLaser)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if(GUILayout.Button("START"))
        {
            main.parts.TrackingTarget(main.targetObj);
            //main.parts.SetTarget(main.targetObj);

        }
    }
}
