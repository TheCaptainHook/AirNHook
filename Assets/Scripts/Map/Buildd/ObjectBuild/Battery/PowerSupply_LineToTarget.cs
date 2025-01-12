using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;


#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class PowerSupply_LineToTarget : MonoBehaviour
{
    private PowerSupply powerSupply;
    private Transform debugTr;


    private List<LineRenderer> lineList;
    Vector3Int previousePosition;

   #if UNITY_EDITOR
    public void Setting()
    {
        powerSupply = GetComponent<PowerSupply>();
        previousePosition = ConvertPosition(transform.position);

        transform.position = previousePosition;

        CreateDebugTransform();

        lineList = new();
        for(int i = 0; i<powerSupply.targetObjects.Count;i++){
            LineRenderer line = GeneratorLineRenderer();
            SetLine(line,ConvertPosition(powerSupply.targetObjects[i].transform.position));
        }

    }

    
    public void Refrash(){
        if(powerSupply == null) return;

        if(previousePosition != transform.position){
            previousePosition = ConvertPosition(transform.position);
            transform.position = previousePosition;
        }

        CheckIPowerConsumers();
        
        CompareTargetListToLineList();

        for(int i = 0;i<powerSupply.targetObjects.Count;i++){
            Vector3Int pot = ConvertPosition(powerSupply.targetObjects[i].transform.position);
            powerSupply.targetObjects[i].transform.position = pot;
            SetLine(lineList[i],pot);
        }
    }

    private Vector3Int ConvertPosition(Vector3 pot){
        return new Vector3Int(
            Mathf.RoundToInt(pot.x),
            Mathf.RoundToInt(pot.y),
            Mathf.RoundToInt(pot.z)
        );
    }

    private void CompareTargetListToLineList(){
        if(powerSupply.targetObjects.Count > lineList.Count){
            for(int i =lineList.Count;i<powerSupply.targetObjects.Count;i++){
                GeneratorLineRenderer();
            }
        }else if(powerSupply.targetObjects.Count < lineList.Count){
            for (int i = lineList.Count - 1; i >= powerSupply.targetObjects.Count; i--)
            {
                GameObject lineObj = lineList[i].gameObject;
                lineList.RemoveAt(i);
                Undo.DestroyObjectImmediate(lineObj);
            }
        }
    }
    private void CheckIPowerConsumers(){
        if(powerSupply.targetObjects.Count == 0) return;

        for(int i=0;i<powerSupply.targetObjects.Count;i++){
            if(powerSupply.targetObjects[i] == null){
                powerSupply.targetObjects.RemoveAt(i);
                continue;
            }

            IPowerConsumer component = powerSupply.targetObjects[i].GetComponent<IPowerConsumer>();
            if(component == null){
                Debug.Log($"[{powerSupply.targetObjects[i].name}]\nThis object doesn’t have IPowerConsumer");
                powerSupply.targetObjects.RemoveAt(i);
            }
        }
    }
    public void Destroy(){
        if(debugTr == null) return;
        Undo.DestroyObjectImmediate(debugTr.gameObject);
    }

    #region  Get
    
    #endregion

    
   

    #region  Util
    private void CreateDebugTransform(){
        foreach(Transform tr in transform){
            if(tr.name == "DebugTransform"){
                Undo.DestroyObjectImmediate(tr.gameObject);
                break;
            }
        }
        debugTr = new GameObject("DebugTransform").transform;
        debugTr.SetParent(transform);
    }
     public bool IsObjectVisibleInInspector(){
        try{
            return gameObject.scene.IsValid() && gameObject.scene == SceneManager.GetActiveScene();
        }catch{
            return false;
        }
     
    }
    #endregion

    #region Draw Line
     private LineRenderer GeneratorLineRenderer(){

        GameObject obj = new GameObject("LineRenderer");
        LineRenderer lineRenderer = obj.AddComponent<LineRenderer>();
        lineList.Add(lineRenderer);
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.positionCount = 0;
        lineRenderer.sortingLayerName ="ForeGround";
        lineRenderer.sortingOrder = 10000;
        obj.transform.SetParent(debugTr);

        return lineRenderer;
    }
     private void SetLine(LineRenderer lineRenderer,Vector3Int end){
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0,transform.position);
        lineRenderer.SetPosition(1,end);
    }


    #endregion
   #endif
}
