using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(Fog))]
public class FogSetting : Editor
{
    private SerializedProperty serializedProperty;
    private Fog fog;

    private Vector2 previousSize;

    private void OnEnable(){
        fog = (Fog)target;
        fog.Init();
        if(fog != null){
            SerializedObject serializedObject = new SerializedObject(fog);
            serializedProperty = serializedObject.FindProperty("size");
            previousSize = serializedProperty.vector2Value;
        }

        EditorApplication.update += OnEditorUpdate;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
    }

    private void OnDisable(){
        EditorApplication.update -= OnEditorUpdate;
    }

    private void OnEditorUpdate(){
        if(previousSize != serializedProperty.vector2Value){
            fog.SetParticleSetting();
        }

        if(fog == null) return;

        serializedProperty.serializedObject.Update();

    }
}
