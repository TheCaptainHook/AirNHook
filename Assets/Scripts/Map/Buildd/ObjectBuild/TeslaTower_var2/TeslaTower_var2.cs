using System.Collections;
using System.Collections.Generic;
using System.Net.Cache;
using UnityEditor;
using UnityEditorInternal;
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
   public LayerMask obstacleLayerMask;

    private float maxDetectRate = 0.1f;
    private float curDetectRate = 0;
    void Update()
    {
        if (curDetectRate > maxDetectRate)
        {
            DetectArea();
            curDetectRate = 0;
        }
        else curDetectRate += Time.deltaTime;
        // DetectArea();
    }
    
    #region  Detect
    // private Vector2 detectAreaOffset;
    private Collider2D[] targets = new Collider2D[5];

    public Collider2D curDetectTarget;
    public float detectRadius;

    public int maxChainLightningCount = 4;
    private int curChainLightningCount = 0;

    public List<Collider2D> detectTargetList = new();
    float radius;
    Collider2D curTarget;

    private void DetectArea()
    {
        Vector2 targetPoint = attackPoint.position; //first start point
        radius = detectRadius;
        curTarget = null;

        detectTargetList.Clear();

        while (curChainLightningCount <maxChainLightningCount)
        {
            radius = curChainLightningCount > 0 ? detectRadius/2 : detectRadius;
            // radius = detectRadius/curChainLightningCount; //5

            int count = Physics2D.OverlapCircleNonAlloc(targetPoint, detectRadius, targets, detectLayerMask);
      
            if (count == 0) break;

            curTarget = AnalyzeDetectTargets(targetPoint,count,radius);
            if (!curTarget) break;
            if(curTarget.TryGetComponent(out LightningRod _)){
                detectTargetList.Add(curTarget);

                break;
            }

            detectTargetList.Add(curTarget);

            targetPoint = curTarget.transform.position;
            curChainLightningCount++;

        }

        //Attack
        if (detectTargetList.Count > 0)
        {
            chainLightningComponent.ChainLightning(detectTargetList);
        }

        curChainLightningCount = 0;

    }
    Vector2 detectDir;
    float distSq;

    private Collider2D AnalyzeDetectTargets(Vector2 start,int count,float radius)
    {
        if (count == 0) return null;
        Collider2D nearestTarget = null;

        float nearestDistSq = float.MaxValue;

        for (int i = 0; i < count; i++)
        {
            if (detectTargetList.Contains(targets[i])) continue;

            detectDir = (Vector2)targets[i].transform.position - start;
            distSq = detectDir.sqrMagnitude;

            // if (distSq > radius*radius) continue;
            if (IsBlocked(start,detectDir)) continue;

            if (targets[i].TryGetComponent(out LightningRod _))
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
    public RaycastHit2D hit;
    private bool IsBlocked(Vector2 start,Vector2 dir)
    {
        Debug.DrawRay(start,dir,Color.green);
        hit =  Physics2D.Raycast(start, dir.normalized,dir.magnitude, obstacleLayerMask);
        if(hit){
            Debug.Log($"hit : {hit.collider.name}");
        }
        return hit;
    }
    #endregion`

#if UNITY_EDITOR
    #region  Debug
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, detectRadius);
    }
    #endregion
#endif
}


//Tesla 타워에서 계산후 체인라이트닝에 콜라이더 배열만 전달,