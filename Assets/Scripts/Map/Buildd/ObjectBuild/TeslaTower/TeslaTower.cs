using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeslaTower : BuildObj
{
    [SerializeField] ParticleSystem[] chargeEffects;
    [SerializeField] ParticleSystem[] lightningEffects;
    [SerializeField] Transform lightningBox;
    [SerializeField] TeslaBezierCurve bezierCurve;
    [SerializeField] Transform lineRendererContainer;
    [SerializeField] Material lightningShaderMat;
    public float lightningRate;
    public bool onCharge;

    Queue<GameObject> lineRendererQueue;

    private int enterBoundaryPlayerAmount;
    public int EnterBoundaryPlayerAmount
    {
        get
        {
            return enterBoundaryPlayerAmount;
        }
        set
        {
            enterBoundaryPlayerAmount += value;

            enterBoundaryPlayerAmount = Mathf.Clamp(enterBoundaryPlayerAmount, 0, 2);
            Debug.Log(enterBoundaryPlayerAmount);
            if (enterBoundaryPlayerAmount == 0)
            {
                StopChargeEffect();
            }
            else
            {
                StartChargeEffect();
            }
            

        }
    }


    private void Start()
    {
        lineRendererQueue = new Queue<GameObject>();

        for (int i = 0; i < 3; i++)
        {
            GameObject obj = new GameObject("LineRenderer");
            LineRenderer lineRenderer = obj.AddComponent<LineRenderer>();
            lineRenderer.startWidth = 1f;
            lineRenderer.sortingLayerName = "ForeGround";
            lineRenderer.sortingOrder = 100;
            lineRenderer.material = lightningShaderMat;
            obj.transform.SetParent(lineRendererContainer);
            obj.SetActive(false);
            lineRendererQueue.Enqueue(obj);
        }

    }

    #region Effect

    public void StartChargeEffect()
    {
        onCharge = true;
        foreach (ParticleSystem ps in chargeEffects)
        {
            ps.Play();
        }
        
    }

    public void StopChargeEffect()
    {
        onCharge = false;
        foreach (ParticleSystem ps in chargeEffects)
        {
            ps.Stop();
        }
    }

    private void StartLightningEffect()
    {
        foreach (ParticleSystem ps in lightningEffects)
        {
            ps.Play();
        }
    }

    #endregion

    public void Lightning(GameObject target)
    {
        ///
        /// If the Lightning Rod is within the attack range
        /// Unconditionally, a Lightning Rod attack.
        ///
        if (target.TryGetComponent(out LightningRod lightningRod1))
        {
            DrawLineRenderer(target.transform, lightningRod1.hitPoint);
            lightningRod1.Electric();
            return;
        }


        if (target.TryGetComponent(out Hook hook))
        {
            LightningRod lightningRod = hook.GetGrabbedItem<LightningRod>();
            if(lightningRod != null)
            {
                DrawLineRenderer(target.transform, lightningRod.hitPoint);
                return;
            }
            
        }

        //if(target.TryGetComponent(out Air air))
        //{
        //    Debug.Log("Air");
        //}

        if (target.gameObject.TryGetComponent(out IDamageable component))
        {
            DrawLineRenderer(target.transform, target.transform);
            component.TakeDamage();
        }

        return;


    }



    private void DrawLineRenderer(Transform target,Transform hitPoint)
    {
       Vector3 dir = (target.position - transform.position).normalized;
        GameObject newObj = lineRendererQueue.Dequeue();
        newObj.SetActive(true);
        StartLightningEffect();
        bezierCurve.Generator(newObj.GetComponent<LineRenderer>(), lightningBox.position, lightningBox.position + dir * 3, hitPoint.position);
        lineRendererQueue.Enqueue(newObj);
    }

}
