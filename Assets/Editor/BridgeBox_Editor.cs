
using UnityEngine;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine.SceneManagement;

[CustomEditor(typeof(BridgeConnect_Editor))]
public class BridgeBox_Editor : Editor
{

   private BridgeConnect_Editor bridgeConnect_Editor;

   private void OnEnable(){
        if(Application.isPlaying) return;

        bridgeConnect_Editor = (BridgeConnect_Editor)target;

        if(!IsObjectVisibleInInspector(bridgeConnect_Editor.gameObject)) return;

        bridgeConnect_Editor.Init();

        EditorApplication.update += bridgeConnect_Editor.Refrash;
   }
   private void OnDisable(){
    if(Application.isPlaying) return;

    bridgeConnect_Editor.Reset();
    EditorApplication.update -= bridgeConnect_Editor.Refrash;
   }

   
    private bool IsObjectVisibleInInspector(GameObject obj){
     return obj.scene.IsValid() && obj.scene == SceneManager.GetActiveScene();
        
    }
    
}
