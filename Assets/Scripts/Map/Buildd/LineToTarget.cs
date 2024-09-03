using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;




[ExecuteInEditMode]
public class LineToTarget : MonoBehaviour
{
    private List<LineRenderer> lineRendererList;
    private Transform debugmodeTransform;

    private List<Vector3> previousTargetVecList;


    private List<GameObject> curTargetObjectList;

    private bool readyForTracking;


    private Coroutine trackTargetCoroutine;
    public void Setting(){
        if(debugmodeTransform == null){
            GameObject obj = new GameObject("DebugmodeTransform");
            obj.transform.SetParent(transform);
            debugmodeTransform = obj.transform;
            lineRendererList = new();

            OverridePrefabWithoutDebugTransform();

        }
    }

    public void DestroyDebugmodeTransform(){
        lineRendererList = null;
        previousTargetVecList = null;
        readyForTracking = false;

        if(trackTargetCoroutine != null){
            if(trackTargetCoroutine != null)StopCoroutine(trackTargetCoroutine);
            trackTargetCoroutine = null;
        }

        Undo.DestroyObjectImmediate(debugmodeTransform.gameObject);
    }




    public void EnableToggle(bool onActive){
        
        foreach(var obj in lineRendererList){
            obj.enabled = onActive;
        }
    }


    IEnumerator TrackTargetObj(){
        bool onChange = false;

        while(true){
                if(!readyForTracking) yield return null;
           for(int i = 0 ;i<curTargetObjectList.Count;i++){
            if(curTargetObjectList[i].transform.position != previousTargetVecList[i]){
                previousTargetVecList = GetTargetPositionList(curTargetObjectList);
                onChange = true;
                break;
            }
           }

           if(onChange){
            RefrashLineRenderer_ThisTransform();
           }
           
            onChange = false;

            Debug.Log("Tracking");

            yield return null;
        }
    }

#region  Target Object
 
     public void SetTargetObjectList(List<GameObject> list){

        curTargetObjectList = list;
        if(trackTargetCoroutine != null){
            StopCoroutine(trackTargetCoroutine);
        }

            //INIT
            if(previousTargetVecList == null){
                previousTargetVecList = GetTargetPositionList(list);

                for(int i =0;i<list.Count; i++){
                    LineRenderer lineRenderer = GeneratorLineRenderer();
                    SetLine(lineRenderer,previousTargetVecList[i]);
                }

                readyForTracking = true;
            }
            //CHECK capacity
            if(CompareCapacityList(list)){
                StartCoroutine(CompareCapacityListCoroutine(list));
            }



        trackTargetCoroutine = StartCoroutine(TrackTargetObj());

    }

 //Compare the capacities of two lists
    private bool CompareCapacityList(List<GameObject> list){
        if(list.Count != previousTargetVecList.Count){
            return true;
        }

        return false;
    }
    

    IEnumerator CompareCapacityListCoroutine(List<GameObject> list){
        readyForTracking = false;

        if(list.Count > previousTargetVecList.Count){
            for(int i = 0; i<list.Count - previousTargetVecList.Count;i++){
                LineRenderer lineRenderer = GeneratorLineRenderer();
                SetLine(lineRenderer,list[list.Count - (i+1)].transform.position); 
            }
        }else{
            for(int i = previousTargetVecList.Count-1; i>list.Count-1 ; i--){
                GameObject obj = lineRendererList[i].gameObject;
                lineRendererList.RemoveRange(i,1);
                Undo.DestroyObjectImmediate(obj);
            }
        }

        previousTargetVecList = GetTargetPositionList(list);
        yield return new WaitForSeconds(0.1f);

        
        readyForTracking = true;
    }
    // private bool CheckTargetObjectTransform(List<GameObject> list){

    // } //TODO 0902

    private List<Vector3> GetTargetPositionList(List<GameObject> list){
        List<Vector3> vecList = new();
        foreach(var obj in curTargetObjectList){
            vecList.Add(obj.transform.position);
        }

        return vecList;
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

#endregion

#region Override Prefab
    public void OverridePrefabWithoutDebugTransform(){
         GameObject prefabRoot = PrefabUtility.GetNearestPrefabInstanceRoot(gameObject);

        if (prefabRoot == null)
        {
            Debug.LogWarning("The selected object is not part of a prefab instance.");
            return;
        }

        Transform debugTransform = gameObject.transform.Find("DebugTransform");
        if (debugTransform != null)
        {
            // DebugTransform 오브젝트의 변경 사항을 되돌리기
            PrefabUtility.RevertObjectOverride(debugTransform.gameObject, InteractionMode.UserAction);
            Debug.Log("DebugTransform has been excluded from prefab override.");
        }
        else
        {
            Debug.LogWarning("DebugTransform object not found.");
        }

        PrefabUtility.ApplyPrefabInstance(gameObject, InteractionMode.UserAction);
        Debug.Log("Prefab override applied, excluding DebugTransform.");
    }
#endregion

}
