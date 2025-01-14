using UnityEngine;
using Vector2 = UnityEngine.Vector2;


#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class DrawDronePath : MonoBehaviour
{
   public Transform parents;
   
   
   public bool onHierarchy;


    private DroneEntity entity;
    public Transform debugmodeTransform;
    public Vector2[] curPath;
    public LineRenderer line;
    private Vector2 previousTransformPosition;


#if UNITY_EDITOR
public void Setting()
{
    entity = GetComponent<DroneEntity>();
    CheckDebugTransform();
    line = GeneratorLineRenderer();
}

    public void DrawPath()
    {
        line.positionCount = entity.paths.Length+1;
        line.SetPosition(0,transform.position);
        for(int i = 0;i<entity.paths.Length;i++)
        {
            line.SetPosition(i+1,entity.paths[i]);
        }
    }
    public void Reset()
    {
        foreach(Transform tr in transform){
            if(tr.name == "DebugmodeTransform")
            {
                DestroyImmediate(tr.gameObject);
            }
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

    private LineRenderer GeneratorLineRenderer(){

        GameObject obj = new GameObject("LineRenderer");
        LineRenderer lineRenderer = obj.AddComponent<LineRenderer>();
        lineRenderer.positionCount = 1;
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.positionCount = 0;
        lineRenderer.sortingLayerName ="ForeGround";
        lineRenderer.sortingOrder = 10000;
        obj.transform.SetParent(debugmodeTransform);

        return lineRenderer;
    }
#endif
}
