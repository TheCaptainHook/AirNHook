
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(LineToTarget))]
public class LineToTargetEditor : Editor
{
   private SerializedProperty serializedProperty; //ButtonEntity-targetObjects
   private LineToTarget lineToTarget;


#region  Previous
private Vector3 previousThisTransfromPosition;
private int previousListSize;
private List<Object> previousList;
#endregion

    private void OnEnable(){

        lineToTarget = (LineToTarget)target;
        lineToTarget.Setting();
        ButtonEntity entity = lineToTarget.GetComponent<ButtonEntity>();

        if(entity != null && lineToTarget.onHierarchy){
            SerializedObject serializedObject = new SerializedObject(entity);
            serializedProperty = serializedObject.FindProperty("targetObjects");
            
            previousThisTransfromPosition = entity.gameObject.transform.position;
            previousListSize = serializedProperty.arraySize;

            if(previousList ==null){previousList = new();};
            UpdateList();
            lineToTarget.SetTargetObjectList(GetGameObjectListFromSerializedProperty(serializedProperty));

        }else{
            Debug.Log("Not Found Property");
        }
            EditorApplication.update += OnEditorUpdate;
   }


   private void OnDisable()
    {
        if(!lineToTarget.onHierarchy) return;

        EditorApplication.update -= OnEditorUpdate;

        if(lineToTarget != null){
            if(lineToTarget.debugmodeTransform != null){
                lineToTarget.DestroyDebugmodeTransform();
            }
        }
        
    }

    public override void OnInspectorGUI()
    {
        if(!lineToTarget.onHierarchy) return;

        // DrawDefaultInspector();

        if (serializedProperty != null && lineToTarget != null)
        {
            serializedObject.Update();
            // EditorGUILayout.PropertyField(serializedProperty, new GUIContent("Object List"),true);
             DrawReadOnlyProperty(serializedProperty, "Object List");

            serializedObject.ApplyModifiedProperties();
        }
        else
        {
            EditorGUILayout.HelpBox("Component is not found.", MessageType.Warning);
        }
            // 변경 사항을 적용
            // serializedObject.ApplyModifiedProperties();


        if(GUILayout.Button("활성화",GUILayout.Width(100))){
            lineToTarget.EnableToggle(true);
        }
         if(GUILayout.Button("비활성화",GUILayout.Width(100))){
            lineToTarget.EnableToggle(false);
        }

        if(GUILayout.Button("리프레쉬",GUILayout.Width(100))){
            lineToTarget.DestroyDebugmodeTransform();
            OnEnable();
        }

    }


    private void OnEditorUpdate(){
        if(!lineToTarget.onHierarchy) return;

        if (serializedProperty != null && lineToTarget != null)
        {
            serializedProperty.serializedObject.Update();

            if(((LineToTarget)target).gameObject.transform.position != previousThisTransfromPosition)
            {
                RefrashMainObjTransform();
                return;
            }

            int currentListSize = serializedProperty.arraySize;
            if (currentListSize != previousListSize)
            {
                RefrashPropertyList(currentListSize);
                return;
            }

            if(previousList.Count == 0) return;
            if(CheckDifferentProperty())
            {
                return;
            }
        }
    }

    private void RefrashMainObjTransform(){
        previousThisTransfromPosition = ((LineToTarget)target).gameObject.transform.position;
        lineToTarget.RefrashLineRenderer_ThisTransform();
        Debug.Log("Different this Transform");
    }
    private void RefrashPropertyList(int currentListSize){
        previousListSize = currentListSize;
        lineToTarget.SetTargetObjectList(GetGameObjectListFromSerializedProperty(serializedProperty));
        UpdateList();
        Debug.Log("Different Size");
    }
    private bool CheckDifferentProperty(){
        for(int i = 0; i< serializedProperty.arraySize;i++)
        {
            if(previousList[i] != serializedProperty.GetArrayElementAtIndex(i).objectReferenceValue)
            {
                UpdateList();
                lineToTarget.SetTargetObjectList(GetGameObjectListFromSerializedProperty(serializedProperty));
                Debug.Log("Different property");
                return true;
            }
        }   
        return false;
    }

    #region  Util

    
    private void UpdateList(){
       previousList.Clear();

       for(int i =0;i<serializedProperty.arraySize;i++){
        previousList.Add(serializedProperty.GetArrayElementAtIndex(i).objectReferenceValue);
       }
    }


    private List<GameObject> GetGameObjectListFromSerializedProperty(SerializedProperty property){
           List<GameObject> list = new List<GameObject>();

        if (property != null && property.isArray)
        {
            for (int i = 0; i < property.arraySize; i++)
            {
                SerializedProperty elementProperty = property.GetArrayElementAtIndex(i);
                GameObject obj = elementProperty.objectReferenceValue as GameObject;

                if(obj == null) continue;
                if(i>0){
                    GameObject previousObj = property.GetArrayElementAtIndex(i - 1).objectReferenceValue as GameObject;
                    if (previousObj == obj)
                    {
                        continue;
                    }
                }
                list.Add(obj);
            }
        }

        return list;
    }

 private void DrawReadOnlyProperty(SerializedProperty property, string label)
    {
        // GUI.enabled를 false로 설정
        bool wasEnabled = GUI.enabled;
        GUI.enabled = false;


        float arrayHeight = EditorGUI.GetPropertyHeight(property, true);
         Rect arrayRect = new Rect(20,-10,EditorGUILayout.GetControlRect().width,arrayHeight);
        // 배열을 그립니다.
        EditorGUI.PropertyField(arrayRect, property, new GUIContent("Target Object"), true);

        // 다음 요소를 그리기 위해 충분한 간격
        GUILayout.Space(arrayHeight);

        // EditorGUILayout.PropertyField(property, new GUIContent(label), true);
        
        GUI.enabled = wasEnabled;
    }

    #endregion
}
