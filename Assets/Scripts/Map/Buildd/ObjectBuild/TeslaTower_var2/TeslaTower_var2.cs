using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
enum Insulator
{
    LightningRod,
    LeverHead,
    Battery
}
public class TeslaTower_var2 : BuildObj
{
    [CustomHeader("Tesla Tower_var2")]
   [SerializeField] Transform attackPoint;
   [SerializeField] ChainLightningComponent chainLightningComponent;
   public LayerMask detectLayerMask;
   public LayerMask obstacleLyaerMask;

    void Update()
    {
        DetectArea();
    }

    #region  Detect
    // private Vector2 detectAreaOffset;

    private Collider2D[] targets = new Collider2D[5];
    public Collider2D curDetectTarget;
    public float detectRadius;
private void DetectArea()
{

    int count = Physics2D.OverlapCircleNonAlloc(attackPoint.position,detectRadius,targets,detectLayerMask);
    //Analyze
    curDetectTarget = AnalyzeDetectTargets(count);
    if(curDetectTarget == null) return;
    //Analyze

    //Attack
    chainLightningComponent.ChainLightning(attackPoint.position,curDetectTarget);
    //Attack


}
private Collider2D AnalyzeDetectTargets(int count)
{
    if(count == 0) return null;
     Collider2D nearestTarget = null;
 
    float nearestDistSq = float.MaxValue;

    for(int i =0; i<count;i++)
    {
        if(IsBlocked(targets[i],out float distSq)) continue;
        if(targets[i].TryGetComponent(out LightningRod _))
        {
            return targets[i];
        }

        if (distSq < nearestDistSq)
        {
            nearestDistSq = distSq;
            nearestTarget = targets[i];
        }
    }
    return nearestTarget;
    
}

private bool IsBlocked(Collider2D target, out float distSq)
{
    Vector2 dir = target.transform.position - attackPoint.position;
    distSq = dir.sqrMagnitude;
    RaycastHit2D hit = Physics2D.Raycast(attackPoint.position, dir.normalized, Mathf.Sqrt(distSq), obstacleLyaerMask);
    return hit; // 막혀있으면 true
}
    #endregion`


    #region  Debug
    void OnDrawGizmos()
    {
        Gizmos.color= Color.red;
        Gizmos.DrawWireSphere(attackPoint.position,detectRadius);
    }
    #endregion
}


