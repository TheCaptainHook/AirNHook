
using UnityEngine;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine.SceneManagement;

[CustomEditor(typeof(BridgeConnect_Editor))]
public class BridgeBox_Editor : Editor
{
   private SerializedProperty serializedProperty;
   private BridgeConnect_Editor bridgeConnect_Editor;
   private BridgeBox bridgeBox;
   private float previousBridgeLength;
   private Transform previousTransform;

   private bool onActiveEditor;

   private void OnEnable(){
        if(Application.isPlaying) return;

        bridgeConnect_Editor = (BridgeConnect_Editor)target;
        bridgeBox = bridgeConnect_Editor.GetComponent<BridgeBox>();
        if(bridgeBox != null){
            if(!IsObjectVisibleInInspector(bridgeBox.gameObject)){
                bridgeBox = null;
                return;
            }
            bridgeConnect_Editor.Init();
            serializedProperty = new SerializedObject(bridgeBox).FindProperty("bridgeLength");
            previousBridgeLength = serializedProperty.floatValue;
            previousTransform = bridgeBox.transform;
            if(previousBridgeLength >= 1) bridgeConnect_Editor.RefrashBridge(previousTransform,previousBridgeLength);
        }

    EditorApplication.update += OnEditorUpdate;
   }
   private void OnDisable(){
    if(Application.isPlaying) return;

    bridgeConnect_Editor.Reset();
    EditorApplication.update -= OnEditorUpdate;
   }

    public override void OnInspectorGUI()
    {
        if(bridgeBox == null) return;

        serializedProperty.serializedObject.Update();
        if(!CheckBridgeLengthValue()){
            previousBridgeLength = Mathf.Floor(serializedProperty.floatValue);
            
            if(previousBridgeLength <1){
                bridgeConnect_Editor.DisconnectBridge();
                return;
            }

            //refrash
            bridgeConnect_Editor.RefrashBridge(previousTransform,previousBridgeLength);
        }
    }

    private void OnEditorUpdate(){
        if(bridgeBox == null) return;

        if(!CheckTransform()){
            previousTransform = bridgeBox.gameObject.transform;

            bridgeConnect_Editor.RefrashTrnasform(previousTransform,previousBridgeLength);
            //refrash
        }
    }



    private bool CheckTransform(){ //EditorUdpate
        serializedProperty.serializedObject.Update();

        bool position = false;
        bool rotate = false;

        if(previousTransform.position == bridgeBox.gameObject.transform.position){
            position = true;
        }
        
        if(!previousTransform.rotation.Equals(bridgeBox.gameObject.transform.rotation))
            rotate = true;
        
        return position && rotate;
    }
    private bool CheckBridgeLengthValue(){ //Inspector Check
        if(previousBridgeLength == serializedProperty.floatValue){
            return true;
        }
        return false;
    }

    private bool IsObjectVisibleInInspector(GameObject obj){
     return obj.scene.IsValid() && obj.scene == SceneManager.GetActiveScene();
        
    }
    
}
