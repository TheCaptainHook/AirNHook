using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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

    private AudioSourceController audioSourceController;
    #region Get,Set

    protected override void StartSound()
    {
        if(Application.isPlaying)
        audioSourceController = Managers.Sound.PlaySound3D(GlobalText.TESLATOWER_ON, transform.position, .7f, true);
    }
 
    private void OnDisable()
    {
        if(audioSourceController!= null) Managers.Sound.StopSound(audioSourceController); 
    }
    #endregion

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

            curTarget = AnalyzeDetectTargets(targetPoint,count);
            if (!curTarget) break;
            if(curTarget.TryGetComponent(out LightningRod _)){
                detectTargetList.Add(curTarget);

                break;
            }

            detectTargetList.Add(curTarget);
            //----------------- Set Start Point-----------------
            if(curTarget.TryGetComponent(out LightningRod rod))
            {
                targetPoint = rod.hitPoint.position;
            }else if(curTarget.TryGetComponent(out TeslaRelayObject tro))
            {
                targetPoint = tro.headPoint.position;
            }else if(curTarget.TryGetComponent(out TeslaNodeRod noderode))
            {
                targetPoint = noderode.head.position;
            }else
                targetPoint = curTarget.transform.position;
            //----------------- Set Start Point-----------------

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
    private bool Contains(Collider2D col)
    {
        if(detectTargetList.Contains(col)) return true;
        
        //----------------- HOOK Item Check-----------------
        if (col.TryGetComponent(out HookSM hook))
        {
            var item = hook.GetGrabbedItem();
            if (item && item.TryGetComponent(out Collider2D itemCol))
            {
                return detectTargetList.Contains(itemCol);
            }
        }
        //----------------- HOOK Item Check-----------------

        return false;
    }
    private Collider2D AnalyzeDetectTargets(Vector2 start,int count)
    {
        if (count == 0) return null;
        Collider2D nearestTarget = null;

        float nearestDistSq = float.MaxValue;

        for (int i = 0; i < count; i++)
        {
            // if (detectTargetList.Contains(targets[i])) continue;
            if(Contains(targets[i])) continue;
            if (targets[i].TryGetComponent(out TeslaTower_var2 _)) continue;

            //----------------- Set Dir -----------------
            if(targets[i].TryGetComponent(out LightningRod rod))
            {
                detectDir = (Vector2)rod.hitPoint.position - start;
            }else if(targets[i].TryGetComponent(out TeslaRelayObject tro))
            {
                detectDir = (Vector2)tro.headPoint.position - start;
            }else detectDir = (Vector2)targets[i].transform.position - start;
            //----------------- Set Dir -----------------

            distSq = detectDir.sqrMagnitude;

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
        //----------------- HOOK Item Check-----------------
        if(nearestTarget && nearestTarget.TryGetComponent(out HookSM hook))
        {
            var item = hook.GetGrabbedItem();
            if(item)
            {
                if (item.TryGetComponent(out  LightningRod rod))
                {
                    return rod.GetComponent<Collider2D>();
                }
                if(item.TryGetComponent(out TeslaRelayObject tro))
                {
                    return tro.GetComponent<Collider2D>();
                }
               
            }
            
        }
        //----------------- HOOK Item Check-----------------

        return nearestTarget;

    }
    public RaycastHit2D hit;
    private bool IsBlocked(Vector2 start,Vector2 dir)
    {

        hit =  Physics2D.Raycast(start, dir.normalized,dir.magnitude, obstacleLayerMask);

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