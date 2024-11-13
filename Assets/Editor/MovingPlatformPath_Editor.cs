
using UnityEditor;
using UnityEngine;
[CustomEditor(typeof(MovingPlatformPath))]
public class MovingPlatformPath_Editor : Editor
{
   private GUIStyleGenerator _GUIStyleGenerator = new();
   private SerializedProperty serializedProperty;
   private MovingPlatformPath movingPlatformPath;
   private MovingPlatform movingPlatform;

    #region  Previous
    private Vector2 previousTransformPosition;
    private Vector2[] previousPaths;

    private bool IsPlay => Application.isPlaying;
    #endregion

    private void OnEnable(){
    movingPlatformPath = (MovingPlatformPath)target;
    movingPlatform = movingPlatformPath.GetComponent<MovingPlatform>();
    if(movingPlatform != null){
        //init
        SerializedObject serializedObject = new SerializedObject(movingPlatform);
        serializedProperty = serializedObject.FindProperty("paths");
        previousTransformPosition = movingPlatform.gameObject.transform.position;
        if(movingPlatform.paths != null) {
            previousPaths = movingPlatform.paths;
            movingPlatformPath.Init(previousPaths);
        }
        
    }

    EditorApplication.update += OnEditorUpdate;

   }
   private void OnDisable(){
    EditorApplication.update -= OnEditorUpdate;
    movingPlatformPath.Destroy_Parents();

   }

     public override void OnInspectorGUI()
   {
        if(serializedProperty == null){
            EditorGUILayout.HelpBox("Component is not found.", MessageType.Warning);
            return;
        }
        
        serializedObject.Update();
        DrawReadOnlyProperty(serializedProperty,"Target Paths");
        serializedObject.ApplyModifiedProperties();

    GUILayout.BeginHorizontal();
    GUILayout.FlexibleSpace();
    if(GUILayout.Button("ON",_GUIStyleGenerator.Generator("button",10,Color.white,TextAnchor.MiddleCenter,FontStyle.Normal),GUILayout.Width(45),GUILayout.Height(40))){
        movingPlatformPath.Enable();
    }
    if(GUILayout.Button("OFF",_GUIStyleGenerator.Generator("button",10,Color.white,TextAnchor.MiddleCenter,FontStyle.Normal),GUILayout.Width(45),GUILayout.Height(40))){
        movingPlatformPath.Disable();
    }
    GUILayout.FlexibleSpace();
    GUILayout.EndHorizontal();


    GUILayout.BeginHorizontal();
    GUILayout.FlexibleSpace();
    if(GUILayout.Button("Refrash",_GUIStyleGenerator.Generator("button",15,Color.white,TextAnchor.MiddleCenter,FontStyle.Bold),GUILayout.Width(100),GUILayout.Height(40))){
      
        EditorApplication.update -= OnEditorUpdate;
        movingPlatformPath.DestroyDebugTransform();
        movingPlatformPath.Init(previousPaths);
        EditorApplication.update += OnEditorUpdate;

        Debug.Log("Refrash");
        
    }
    GUILayout.FlexibleSpace();
    GUILayout.EndHorizontal();


   }


    private void OnEditorUpdate(){
    if(movingPlatform == null || !movingPlatformPath.onHierarchy) return;

    serializedProperty.serializedObject.Update();
    if(movingPlatform.paths != null){

        movingPlatformPath.SetPath(movingPlatform.paths);
        previousPaths = movingPlatform.paths;
       
    }
    movingPlatformPath.CheckTransform();

   }

     #region  Util
   
    private void DrawReadOnlyProperty(SerializedProperty property, string label)
    {
        // GUI.enabled를 false로 설정
        bool wasEnabled = GUI.enabled;
        GUI.enabled = false;


        float arrayHeight = EditorGUI.GetPropertyHeight(property, true);
         Rect arrayRect = new Rect(20,-10,EditorGUILayout.GetControlRect().width,arrayHeight);
        EditorGUI.PropertyField(arrayRect, property, new GUIContent(label), true);

        // 다음 요소를 그리기 위해 충분한 간격
        GUILayout.Space(arrayHeight);

        // EditorGUILayout.PropertyField(property, new GUIContent(label), true);
        
        GUI.enabled = wasEnabled;
    }
   #endregion
}
