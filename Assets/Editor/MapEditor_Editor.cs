using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(MapEditor))]
public class MapEditor_Editor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        MapEditor mapEditor = target as MapEditor;

        EditorGUILayout.LabelField("테스트", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox($"Set Size -> 맵 에디터 크기조절 후 느르면 아웃라인 생성\nSave Data 오브젝트가 적절하게 각 트랜스폼에 들어가있으면 맵 데이터 Json파일 저장", MessageType.Info);



        if (GUILayout.Button("Set Size"))
        {
            mapEditor.Init();
            mapEditor.SetMapSize(mapEditor.width, mapEditor.height);
        }
        if (GUILayout.Button("Load Data"))
        {
            mapEditor.LoadMap(mapEditor.mapID);
        }

        if (GUILayout.Button("Save Data"))
        {
            Debug.Log("Buttom");
            mapEditor.SaveMapData();
        }
       


    }
}
