using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DrawDronePath))]
public class DrawDronePath_Editor : Editor
{
   private GUIStyleGenerator _GUIStyleGenerator = new();
   private SerializedProperty serializedProperty;
   private DrawDronePath drawDronePath;
   private DroneEntity drone;

    #region  Previous
    private Vector2 previousTransformPosition;
    private Vector2[] previousPaths;

    private bool IsPlay => Application.isPlaying;
    #endregion

   private void OnEnable(){
    drawDronePath = (DrawDronePath)target;
    drone = drawDronePath.GetComponent<DroneEntity>();
    if(drone != null){
        //init
        SerializedObject serializedObject = new SerializedObject(drone);
        serializedProperty = serializedObject.FindProperty("paths");
        previousTransformPosition = drone.gameObject.transform.position;
        if(drone.paths != null) {
            previousPaths = drone.paths;
            drawDronePath.Init(previousPaths);
        }
        
    }

    EditorApplication.update += OnEditorUpdate;

   }
   private void OnDisable(){
    EditorApplication.update -= OnEditorUpdate;
    drawDronePath.Destroy_Parents();

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
        drawDronePath.Enable();
    }
    if(GUILayout.Button("OFF",_GUIStyleGenerator.Generator("button",10,Color.white,TextAnchor.MiddleCenter,FontStyle.Normal),GUILayout.Width(45),GUILayout.Height(40))){
        drawDronePath.Disable();
    }
    GUILayout.FlexibleSpace();
    GUILayout.EndHorizontal();


    GUILayout.BeginHorizontal();
    GUILayout.FlexibleSpace();
    if(GUILayout.Button("Refrash",_GUIStyleGenerator.Generator("button",15,Color.white,TextAnchor.MiddleCenter,FontStyle.Bold),GUILayout.Width(100),GUILayout.Height(40))){
      
        EditorApplication.update -= OnEditorUpdate;
        // drawDronePath.DestroyDebugTransform();
        drawDronePath.Init(previousPaths);
        EditorApplication.update += OnEditorUpdate;

        Debug.Log("Refrash");
        
    }
    GUILayout.FlexibleSpace();
    GUILayout.EndHorizontal();


   }


   private void OnEditorUpdate(){
    if(!drawDronePath.onHierarchy || drawDronePath == null) return;

    serializedProperty.serializedObject.Update();
    if(drone.paths != null){
       
        drawDronePath.SetPath(drone.paths);
        previousPaths = drone.paths;
       
    }
    drawDronePath.CheckTransform();

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
