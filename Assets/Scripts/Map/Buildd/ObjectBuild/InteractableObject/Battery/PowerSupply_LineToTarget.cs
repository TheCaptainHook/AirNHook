using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;



#if UNITY_EDITOR
using UnityEditor;
using Unity.EditorCoroutines.Editor;
#endif

[ExecuteInEditMode]
public class PowerSupply_LineToTarget : MonoBehaviour
{
    private PowerSupply powerSupply;
    private Transform debugTr;


    private List<LineRenderer> targetLineList;
    private List<LineRenderer> lightLineList;
    Vector3Int previousePosition;

   #if UNITY_EDITOR
    public void Setting()
    {
        StopAllCoroutines();

        powerSupply = GetComponent<PowerSupply>();
        previousePosition = ConvertPosition(transform.position);

        transform.position = previousePosition;
        

        CreateDebugTransform();

        //Target Object
        targetLineList = new();
        for(int i = 0; i<powerSupply.targetObjects.Count;i++){
            LineRenderer line = GeneratorLineRenderer(targetLineList,Color.red);
            SetLine(line,ConvertPosition(powerSupply.targetObjects[i].transform.position));
        }
        //Light Object
        lightLineList = new();
        for(int i = 0; i< lightLineList.Count;i++)
        {
            LineRenderer line = GeneratorLineRenderer(lightLineList,Color.blue);
            SetLine(line,powerSupply.lightObjects[i].transform.position);
        }

    }

    Coroutine coroutine;

    public void StartRefrash()
    {
        coroutine = StartCoroutine(RefrashCo());
    }
    public void StopRefrash()
    {
        if(coroutine != null)
        {
            StopCoroutine(coroutine);
            Destroy();
        }  
    }

    private IEnumerator RefrashCo()
    {

        while(true)
        {
            Refrash();
            yield return null;
        }

    }

    private void Refrash(){
        if(powerSupply == null) return;

        if(previousePosition != transform.position){
            previousePosition = ConvertPosition(transform.position);
            transform.position = previousePosition;
        }

        CheckIPowerConsumers();
        
        CompareTargetListToLineList();
        CompareLightListToLineList();

        for(int i = 0;i<powerSupply.targetObjects.Count;i++){
            Vector3Int pot = ConvertPosition(powerSupply.targetObjects[i].transform.position);
            powerSupply.targetObjects[i].transform.position = pot;
            SetLine(targetLineList[i],pot);
        }
        for(int i = 0;i<powerSupply.lightObjects.Count;i++){
            Vector3 pot = powerSupply.lightObjects[i].transform.position;
            powerSupply.lightObjects[i].transform.position = pot;
            SetLine(lightLineList[i],pot);
        }
    }



    private void CompareTargetListToLineList(){
        if(powerSupply.targetObjects.Count > targetLineList.Count){
            for(int i =targetLineList.Count;i<powerSupply.targetObjects.Count;i++){
                GeneratorLineRenderer(targetLineList,Color.red);
            }
        }else if(powerSupply.targetObjects.Count < targetLineList.Count){
            for (int i = targetLineList.Count - 1; i >= powerSupply.targetObjects.Count; i--)
            {
                GameObject lineObj = targetLineList[i].gameObject;
                targetLineList.RemoveAt(i);
                Undo.DestroyObjectImmediate(lineObj);
            }
        }
    }
    private void CompareLightListToLineList()
    {
        if(powerSupply.lightObjects.Count > lightLineList.Count){
            for(int i =lightLineList.Count;i<powerSupply.lightObjects.Count;i++){
                GeneratorLineRenderer(lightLineList,Color.blue);
            }
        }else if(powerSupply.lightObjects.Count < lightLineList.Count){
            for (int i = lightLineList.Count - 1; i >= powerSupply.lightObjects.Count; i--)
            {
                GameObject lineObj = lightLineList[i].gameObject;
                lightLineList.RemoveAt(i);
                Undo.DestroyObjectImmediate(lineObj);
            }
        }
    }
    private void CheckIPowerConsumers(){
        //Target Object
        if(powerSupply.targetObjects.Count == 0) return;

        for(int i=0;i<powerSupply.targetObjects.Count;i++){
            if(powerSupply.targetObjects[i] == null || !powerSupply.targetObjects[i].TryGetComponent(out IPowerConsumer _))
            {
                powerSupply.targetObjects.RemoveAt(i);
                continue;
            }

        }

        //Light Object
        if(powerSupply.lightObjects.Count == 0) return;
        for(int i = 0; i<powerSupply.lightObjects.Count;i++)
        {
            if(powerSupply.lightObjects[i] == null ||!powerSupply.lightObjects[i].TryGetComponent(out IPowerConsumer _))
            {
                powerSupply.lightObjects.RemoveAt(i);
                continue;
            }
        }

    }
    public void Destroy(){
        if(debugTr == null) return;
        Undo.DestroyObjectImmediate(debugTr.gameObject);
        
    }


    #region  Util
    private Vector3Int ConvertPosition(Vector3 pot)
    {
        return new Vector3Int(
            Mathf.RoundToInt(pot.x),
            Mathf.RoundToInt(pot.y),
            Mathf.RoundToInt(pot.z)
        );
    }
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
     private LineRenderer GeneratorLineRenderer(List<LineRenderer> list,Color color){

        GameObject obj = new GameObject("LineRenderer");
        LineRenderer lineRenderer = obj.AddComponent<LineRenderer>();
        list.Add(lineRenderer);

        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = color;
        lineRenderer.endColor = color;
        
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
     private void SetLine(LineRenderer lineRenderer,Vector3 end){
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0,transform.position);
        lineRenderer.SetPosition(1,end);
    }


    #endregion
   #endif
}
