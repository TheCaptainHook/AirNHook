using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(ShowLaser))]
public class ShowLaser_Editor : Editor
{
#if UNITY_EDITOR

    ShowLaser showLaser;
    bool onPrograss;

    private void OnEnable(){
        onPrograss = false;
        showLaser = (ShowLaser)target;
        showLaser.Setting();
        EditorApplication.update += OnEditorUpdate;
    }

    private void OnDisable(){        
        showLaser.laserObject.ResetLaser();
        onPrograss = false;
        EditorApplication.update -= OnEditorUpdate;
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.HelpBox("ONLY USE EDITOR",MessageType.Info);
        EditorGUILayout.Space(10);

        DrawDefaultInspector();

        if(GUILayout.Button("SHOW LASER",GUILayout.Width(200))){
            showLaser.laserObject.Editor_Awake();
            onPrograss = true;
        }
        if(GUILayout.Button("Shut Down LASER",GUILayout.Width(200))){
            onPrograss = false;
            showLaser.laserObject.ResetLaser();
            
        }
    }





    private void OnEditorUpdate(){
        if(onPrograss){
            showLaser.laserObject.Editor_UpdateLaser();
        }
    }

#endif
}

