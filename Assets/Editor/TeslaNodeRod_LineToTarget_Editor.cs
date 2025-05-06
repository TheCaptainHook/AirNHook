#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TeslaNodeRod_LineToTarget))]
public class TeslaNodeRod_LineToTarget_Editor : Editor
{
    private TeslaNodeRod_LineToTarget main;
    void OnEnable()
    {
        main = (TeslaNodeRod_LineToTarget)target;
    }

    public override void OnInspectorGUI()
    {

        if (GUILayout.Button("활성화"))
        {
            Activate();
        }

        if (GUILayout.Button("비활성화"))
        {
            Deactivate();
        }

    }

    private void Activate()
    {

        if (!main.IsObjectVisibleInInspector()) return;

        main.Setting();
        main.StartRefrash();
    }


    private void Deactivate()
    {
        if (main == null) return;

        if (!main.IsObjectVisibleInInspector()) return;
        main.StopRefrash();

    }

}

#endif