using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ChainLightningComponent : MonoBehaviour
{
    [SerializeField] TeslaTower_var2 main;
    [SerializeField] Transform attackPoint;
    [SerializeField] Transform lightningContainer;

    private Lightning[] lightnings;

    public List<Collider2D> targets;

    private void Awake()
    {
        lightnings = new Lightning[main.maxChainLightningCount];

        for (int i = 0; i < 4; i++)
        {
            lightnings[i] = GetLightning();
        }
    }


    public void ChainLightning(List<Collider2D> targets) 
    {
        this.targets = targets;
        lightnings[0].SetTarget(attackPoint.position, targets[0].gameObject);

        for (int i = 1; i < targets.Count; i++)
        {
            if (!lightnings[i]) return;

            lightnings[i].SetTarget(lightnings[i - 1].target.transform.position, targets[i].gameObject);
        }
      
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
        lineRenderer.startWidth = 0.5f;
        lineRenderer.sortingLayerName = "ForeGround";
        lineRenderer.sortingOrder = 100;
         lineRenderer.material = lightningShaderMat;
        lineRenderer.positionCount  = 2;
        obj.transform.SetParent(lightningContainer);
        lightning.Init(lineRenderer);
        obj.SetActive(false);


        return lightning;
    }
    #endregion
    
}