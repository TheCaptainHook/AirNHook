using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEditor;
using UnityEditor.Build.Pipeline;
using UnityEngine;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.UI;



[ExecuteInEditMode]
public class LineToTarget : MonoBehaviour
{
    private List<LineRenderer> lineRendererList;
    private Transform debugmodeTransform;

    private List<Vector3> previousTargetVecList;


    private List<GameObject> curTargetObjectList;

    private bool readyForTracking;

    public void Setting(){
        if(debugmodeTransform == null){
            GameObject obj = new GameObject("DebugmodeTransform");
            obj.transform.SetParent(transform);
            debugmodeTransform = obj.transform;
            lineRendererList = new();
        }
    }


    public void DestroyDebugmodeTransform(){
        lineRendererList = null;
        previousTargetVecList = null;
        readyForTracking = false;
        Undo.DestroyObjectImmediate(debugmodeTransform.gameObject);
    }

    public void EnableToggle(bool onActive){
        
        foreach(var obj in lineRendererList){
            obj.enabled = onActive;
        }
    }



#region  Target Object
 
     public void SetTargetObjectList(List<GameObject> list){

        curTargetObjectList = list;
            //Check Previous TargetObject List;
            if(previousTargetVecList == null){
                previousTargetVecList = GetTargetPositionList();

                for(int i =0;i<list.Count; i++){
                    LineRenderer lineRenderer = GeneratorLineRenderer();
                    SetLine(lineRenderer,previousTargetVecList[i]);
                }

                readyForTracking = true;
            }
    }




 //Compare the capacities of two lists
    // private bool CompareCapacityList(List<GameObject> list){
    //     //if two list deff capacity 
            
    //     //
    // }
    // private bool CheckTargetObjectTransform(List<GameObject> list){

    // } //TODO 0902

    private List<Vector3> GetTargetPositionList(){
        List<Vector3> list = new();
        foreach(var obj in curTargetObjectList){
            list.Add(obj.transform.position);
        }

        return list;
    }

#endregion

#region  Line

    private LineRenderer GeneratorLineRenderer(){

        GameObject obj = new GameObject("LineRenderer");
        LineRenderer lineRenderer = obj.AddComponent<LineRenderer>();
        lineRendererList.Add(lineRenderer);
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.positionCount = 0;

        obj.transform.SetParent(debugmodeTransform);

        return lineRenderer;
    }



     private void SetLine(LineRenderer lineRenderer,Vector3 end){
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0,transform.position);
        lineRenderer.SetPosition(1,end);
    }



    public void RefrashLineRenderer_ThisTransform(){
        if(!readyForTracking) return;
        for(int i = 0;i<lineRendererList.Count;i++){
            SetLine(lineRendererList[i],previousTargetVecList[i]);
        }
        
    }

    public void RefrashLineRenderer_TargetObject(){
        
    }
#endregion


}
