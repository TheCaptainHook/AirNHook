using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;


#if UNITY_EDITOR
using UnityEditor;
#endif




[ExecuteInEditMode]
public class LineToTarget : MonoBehaviour
{
#if UNITY_EDITOR
    public Transform debugmodeTransform;

//------------------------------------------------------------Refactoring 0114
    // Vector2 previousPot;
    List<TargetTrackingField> targetList;
    // Queue<LineRenderer> lineQueue;
    //1 사이즈 차이
    //개별 오브젝트 비교, 차이나면 바로 리프레쉬
    ButtonEntity entity;

    



  public void Setting()
  {
    entity  = GetComponent<ButtonEntity>();
    CheckDebugTransform();

    targetList = new();
    // lineQueue = new();   
    
    CheckNullAndMissingValue();

    foreach(var target in entity.targetObjects)
    {
        targetList.Add(new TargetTrackingField(transform,GeneratorLineRenderer(),target.transform));
    }
    
  }

public void Reset()
{
    if(entity == null) return;
    foreach(Transform tr in transform)
    {
        if(tr.name == "DebugmodeTransform")
        {
            DestroyImmediate(tr.gameObject);
        }
    }
    // if(debugmodeTransform != null) DestroyImmediate(debugmodeTransform.gameObject);
    targetList.Clear();
    // lineQueue.Clear();
}



public void Tracking()
{
    CheckNullAndMissingValue();

    CheckEntityTargetValue();

    if(targetList.Count != entity.targetObjects.Count) return;

    EntityTargetRefrash();
    TargetRefrash();
}

private void CheckNullAndMissingValue()
{
    for(int i = entity.targetObjects.Count-1;i>=0;i--)
    {
        if(entity.targetObjects[i]==null)
        {
            entity.targetObjects.Remove(entity.targetObjects[i]);
        }
    }
}

private void CheckEntityTargetValue()
{
    if(targetList.Count != entity.targetObjects.Count)
    {
        if(targetList.Count > entity.targetObjects.Count)
        {
            for(int i = 0;i<targetList.Count - entity.targetObjects.Count;i++)
            {
                var lastVal = targetList[^1];
                targetList.Remove(lastVal);
                lastVal.DestroyLine();

            }


        }else{
            for(int i = 0;i < entity.targetObjects.Count - targetList.Count;i++)
            {
                targetList.Add(new TargetTrackingField(transform,GeneratorLineRenderer()));
            }
        }
    }
}
private void EntityTargetRefrash()
{
    for(int i =0; i<entity.targetObjects.Count;i++)
    {
        targetList[i].CompareTarget(entity.targetObjects[i]);
    }
}


private void TargetRefrash()
{
    foreach(var target in targetList)
    {
        target.Refrash();
    }
}

private void CheckDebugTransform()
{
     Transform debugTransform = gameObject.transform.Find("DebugmodeTransform");
        if(debugTransform !=null) Undo.DestroyObjectImmediate(debugTransform.gameObject);
        
        if(debugmodeTransform == null){
            GameObject obj = new GameObject("DebugmodeTransform");        
            obj.transform.SetParent(transform);            
            debugmodeTransform = obj.transform;
        }

}


public class TargetTrackingField
{
    public Transform main;
    public Transform target;
    public LineRenderer line;
    private Vector2 previousMainPot;
    private Vector2 previousPot;
    private Vector3 previousRot;

    public TargetTrackingField(Transform main,LineRenderer line,Transform target = null)
    {
        this.main = main;
        this.target = target;

        this.line = line;
        line.positionCount = 2;

        previousMainPot = main.position;
        line.SetPosition(0,main.position);

        if(target != null)
        {
            previousPot = target.position;
            previousRot = target.rotation.eulerAngles;
            
            line.SetPosition(1,target.position);
        }

    }
    #region  Check
    private bool CheckTargetPosition()
    {
        bool pot =  (Vector2)target.position == previousPot;
        bool rot = previousRot == target.rotation.eulerAngles;

        return pot && rot;
    }
    private bool CheckMainPosition()
    {
        return previousMainPot == (Vector2)main.position;
    }
    #endregion
   
    public void Refrash()
    {
        if(!CheckMainPosition())
        {
            line.SetPosition(0,main.position);
            previousMainPot = main.position;
        }
        if(!CheckTargetPosition())
        {
            line.SetPosition(1,target.transform.position);
            previousPot = target.position;
            previousRot = target.rotation.eulerAngles;
        }
    }


    public void CompareTarget(GameObject obj)
    {
        if(obj == null) return;
        if(target == null || obj != target.gameObject)
        {
            target = obj.transform;
            line.SetPosition(1,target.position);

        }
    }

    public void DestroyLine()
    {
        if(line == null) return;
        Undo.DestroyObjectImmediate(line.gameObject);
    }
}

//------------------------------------------------------------Refactoring 0114


#region  Line

    private LineRenderer GeneratorLineRenderer(){

        GameObject obj = new GameObject("LineRenderer");
        LineRenderer lineRenderer = obj.AddComponent<LineRenderer>();
        // lineRendererList.Add(lineRenderer);
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.positionCount = 0;
        lineRenderer.sortingLayerName ="ForeGround";
        lineRenderer.sortingOrder = 10000;
        obj.transform.SetParent(debugmodeTransform);

        return lineRenderer;
    }


#endregion

#endif
}
