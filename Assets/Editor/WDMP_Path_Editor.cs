using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WDMP_Path))]
public class WDMP_Path_Editor : Editor
{
    private SerializedProperty serializedProperty;
    private WeightDetectionMoveingPlatform _WDMP;
    private WDMP_Path _WDMP_Path;

    float previousMoveDistance;

    private void OnEnable(){
        _WDMP_Path  = (WDMP_Path)target;
        _WDMP = _WDMP_Path.GetComponent<WeightDetectionMoveingPlatform>();
        if(_WDMP !=null){
            SerializedObject so = new SerializedObject(_WDMP);
            serializedProperty = so.FindProperty("moveDistance");
            previousMoveDistance = serializedProperty.floatValue;
            _WDMP_Path.Init(previousMoveDistance);
        }

        EditorApplication.update += OnEditorUpdate;
    }
    private void OnDisable(){
        EditorApplication.update -= OnEditorUpdate;
        _WDMP_Path.Destroy_Parents();
    }

    public override void OnInspectorGUI()
{
    serializedObject.Update(); // 대상 객체의 데이터를 가져옴
    serializedObject.ApplyModifiedProperties();
}

    private void OnEditorUpdate(){
        if(_WDMP == null || !_WDMP_Path.onHierarchy){
            Debug.Log("Can't find Weight Detect Moving Platform");
            return;
        }
        _WDMP_Path.RefrashLine(_WDMP.moveDistance);
    }

private bool CheckMoveDistance(float a, float b){
    return Mathf.Approximately(a,b);
}


}
