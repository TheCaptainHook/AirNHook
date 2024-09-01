using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEditor.Build.Pipeline;
using UnityEngine;
using UnityEngine.UI;



[ExecuteInEditMode]
public class LineToTarget : MonoBehaviour
{
   private List<LineRenderer> lineRendererList;
   private Transform debugmodeTransform;

   public void TrackTarget(List<GameObject> targets){
    if(targets.Count == 0) return;
    
    for(int i =0 ; i<targets.Count ; i++){
        if(targets[i] != null){
           GameObject obj = GeneratorLineRenderer();
           SetLine(obj.GetComponent<LineRenderer>(),targets[i].transform.position);

        }
    }
   }

    public void Setting(){
        if(debugmodeTransform == null){
            GameObject obj = new GameObject("DebugmodeTransform");
            obj.transform.SetParent(transform);
            debugmodeTransform = obj.transform;
            lineRendererList = new();
        }
    }

    private GameObject GeneratorLineRenderer(){

        GameObject obj = new GameObject("LineRenderer");
        LineRenderer lineRenderer = obj.AddComponent<LineRenderer>();
        lineRendererList.Add(lineRenderer);
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.positionCount = 0;

        obj.transform.SetParent(debugmodeTransform);

        return obj;
    }

    private void SetLine(LineRenderer lineRenderer,Vector3 end){
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0,transform.position);
        lineRenderer.SetPosition(1,end);
    }

    private void Reset(){
       
    }


    public void EnableToggle(bool onActive){
        
        foreach(var obj in lineRendererList){
            obj.enabled = onActive;
        }
    }

}
