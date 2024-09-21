using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Org.BouncyCastle.Crypto.Modes;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Vector2 = UnityEngine.Vector2;

[ExecuteInEditMode]
public class DrawDronePath : MonoBehaviour
{
   public Transform parents;
   private Vector2 previousTransformPosition;
   public Vector2[] curPath;
   public LineRenderer line;
   public bool onHierarchy;

   public void Init(Vector2[] paths){
    onHierarchy = CheckFocusedObjectPresence();
    if(!onHierarchy) return;

    //setting parents Transform
    DestroyDebugTransform();

    parents = new GameObject("DebugTransfrom").transform;
    parents.SetParent(transform);
    line = GeneratorLineRenderer();

    previousTransformPosition = transform.position;

    if(paths != null){
        curPath = paths;
        SetPath(curPath);
    }
    //
    
   }
    
    #region  Main
     public void SetPath(Vector2[] paths){
        curPath = paths;
        line.positionCount = paths.Length+1;
        line.SetPosition(0,previousTransformPosition);
            for(int i =1 ;i<=paths.Length; i++){
                line.SetPosition(i,paths[i-1]);
                Debug.Log(paths[i-1]);
            }
    }


    #endregion
  

   #region  Util
   public void CheckTransform(){
    if(previousTransformPosition != (Vector2)transform.position){
        previousTransformPosition = transform.position;
        
        if(curPath == null) return;

        RefrashLine();
    }
   }
    private void DestroyDebugTransform(){
        var tr = transform.Find("DebugTransform");
        if(tr != null) Undo.DestroyObjectImmediate(tr.gameObject);
    }
    private bool CheckFocusedObjectPresence(){
        GameObject selectedObject = Selection.activeGameObject;
        GameObject obj = GameObject.Find(selectedObject.name);

        if (obj != null)
        {
            // 선택된 오브젝트가 하이라키에 존재하는지 확인
            return true;
        }
        else
        {
            // 선택된 오브젝트가 없는 경우
            return false;
        }
    }

   public void RefrashLine(){
    line.SetPosition(0,previousTransformPosition);
   }


    public void Disable(){
        Undo.DestroyObjectImmediate(parents.gameObject);
    }

    private LineRenderer GeneratorLineRenderer(){

        GameObject obj = new GameObject("LineRenderer");
        LineRenderer lineRenderer = obj.AddComponent<LineRenderer>();
        lineRenderer.positionCount = 1;
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.positionCount = 0;
        lineRenderer.sortingLayerName ="ForeGround";
        lineRenderer.sortingOrder = 10000;
        obj.transform.SetParent(parents);

        return lineRenderer;
    }
   #endregion

}
