using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ChainLightningComponent : MonoBehaviour
{

    private List<Lightning> lightnings = new();
    [SerializeField] Transform lightningContainer;

    public void ChainLightning(Vector2 start,Collider2D target) // First Lightning
    {
        if(lightnings.Count == 0)
        {
            var lightning = GetLightning();
            lightnings.Add(lightning);
        }
         
            lightnings[0].SetTarget(start,target.gameObject);
            
    }


    #region Line
  
    [SerializeField] AnimationCurve lightningLineCurve;
    [SerializeField] Material lightningShaderMat;
    private Lightning GetLightning()
    {
        GameObject obj = new GameObject("LineRenderer");
        Lightning lightning = obj.AddComponent<Lightning>();
        LineRenderer lineRenderer = obj.AddComponent<LineRenderer>();
        // lineRenderer.widthCurve = lightningLineCurve;
        lineRenderer.startWidth = 0.1f;
        lineRenderer.sortingLayerName = "ForeGround";
        lineRenderer.sortingOrder = 100;
        // lineRenderer.material = lightningShaderMat;
        lineRenderer.positionCount  = 2;
        obj.transform.SetParent(lightningContainer);
        lightning.Init(lineRenderer);
        obj.SetActive(false);


        return lightning;
    }
    #endregion
    
}