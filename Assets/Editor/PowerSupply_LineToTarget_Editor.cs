using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(PowerSupply_LineToTarget))]
public class PowerSupply_LineToTarget_Editor : Editor
{
    private SerializedProperty serializedProperty; //ButtonEntity-targetObjects
    private PowerSupply_LineToTarget powerSupply_LineToTarget;


    private void OnEnable(){
        powerSupply_LineToTarget = (PowerSupply_LineToTarget)target;
        if(!powerSupply_LineToTarget.IsObjectVisibleInInspector()) return;

        serializedProperty = serializedObject.FindProperty("targetObjects");
        powerSupply_LineToTarget.Setting();

        EditorApplication.update += powerSupply_LineToTarget.Refrash;

    }
    private void OnDisable(){
        if(!powerSupply_LineToTarget.IsObjectVisibleInInspector()) return;
        EditorApplication.update -= powerSupply_LineToTarget.Refrash;
        powerSupply_LineToTarget.Destroy();
    }


    #region  Util
     
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
    #endregion
}
