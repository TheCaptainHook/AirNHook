using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DrawDronePath))]
public class DrawDronePath_Editor : Editor
{

private void Enable()
{
    DrawDronePath path = (DrawDronePath)target;
    path.Setting();
    EditorApplication.update -= path.DrawPath;
    EditorApplication.update += path.DrawPath;
}
private void Disable()
{
    DrawDronePath path = (DrawDronePath)target;
    EditorApplication.update -= path.DrawPath;
    path.Reset();
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

}
