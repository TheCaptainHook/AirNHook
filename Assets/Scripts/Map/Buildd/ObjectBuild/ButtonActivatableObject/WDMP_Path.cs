using UnityEditor;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

[ExecuteInEditMode]
public class WDMP_Path : MonoBehaviour
{
  private Transform debugTr;
  private LineRenderer line;
  public bool onHierarchy;
  private Vector2 previousTransformPosition;
  private float previousMoveDistance;

  private Vector2 target;

  public void Init(float moveDistance){
    onHierarchy = CheckFocusedObjectPresence();
    if(!onHierarchy) return;
    CheckDebugTr();

    debugTr = new GameObject("DebugTransfrom").transform;
    debugTr.SetParent(transform);
    line = GeneratorLineRenderer();

    line.SetPosition(0,transform.position);
    previousTransformPosition = transform.position;
    SetPath(moveDistance);
  }



    private void SetPath(float moveDistance){
        target.Set(previousTransformPosition.x + moveDistance,previousTransformPosition.y);
        line.SetPosition(1,target);
    }
    public void RefrashLine(float moveDistance){
        if((Vector2)transform.position != previousTransformPosition){
            previousTransformPosition = transform.position;
            line.SetPosition(0,previousTransformPosition);
        }
        if(!CheckMoveDistance(previousMoveDistance,moveDistance)){
            previousMoveDistance = moveDistance;
            target.Set(transform.position.x+previousMoveDistance,previousTransformPosition.y);
            line.SetPosition(1,target);

        }
    }

    

  #region  Util
  public void Destroy_Parents(){
    if(debugTr != null){
            Undo.DestroyObjectImmediate(debugTr.gameObject);
    }
  }
  private void CheckDebugTr(){
    foreach(Transform tr in transform){
            if(tr.gameObject.name == "DebugTransfrom"){
                Undo.DestroyObjectImmediate(tr.gameObject);
            }
        }
  }
  private bool CheckFocusedObjectPresence(){
        GameObject selectedObject = Selection.activeGameObject;
        if(selectedObject == null) return false;
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

    private LineRenderer GeneratorLineRenderer(){

        GameObject obj = new GameObject("LineRenderer");
        LineRenderer lineRenderer = obj.AddComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.sortingLayerName ="ForeGround";
        lineRenderer.sortingOrder = 10000;
        // lineRenderer.SetPosition(0,transform.position);

        obj.transform.SetParent(debugTr);

        return lineRenderer;
    }

    private bool CheckMoveDistance(float a, float b){
    return Mathf.Approximately(a,b);
}
  #endregion
}
