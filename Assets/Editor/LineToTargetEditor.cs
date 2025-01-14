

using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;


[CustomEditor(typeof(LineToTarget))]
public class LineToTargetEditor : Editor
{
   private LineToTarget lineToTarget;


    private void Enable()
    {
        lineToTarget = (LineToTarget)target;
        lineToTarget.Setting();
        EditorApplication.update -= lineToTarget.Tracking;
        EditorApplication.update += lineToTarget.Tracking;
    }
    private void Disable()
    {
        lineToTarget = (LineToTarget)target;
        EditorApplication.update -= lineToTarget.Tracking;
        lineToTarget.Reset();
    }
    public override void OnInspectorGUI()
    {
    
        GUILayout.BeginHorizontal(new GUIStyle(GUI.skin.window));
            GUILayout.FlexibleSpace();
            GUILayout.BeginVertical();
            if(GUILayout.Button("활성화",GUILayout.Width(100))){
                Enable();
            }
            if(GUILayout.Button("비활성화",GUILayout.Width(100))){
                Disable();
            }
            GUILayout.EndVertical();
            GUILayout.FlexibleSpace();

            EditorGUILayout.HelpBox("되돌리기(컨트롤 + z) 시 오류 날 수 있음.\n 오류시 비활성화 후 재 활성화",MessageType.Warning);
            


        GUILayout.EndHorizontal();

    }


    #region  Util

    private bool IsObjectVisibleInInspector(GameObject obj){
     return obj.scene.IsValid() && obj.scene == SceneManager.GetActiveScene();
        
    }
    #endregion
}
