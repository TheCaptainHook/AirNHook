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
            lineRenderer.startWidth = .1f;
            lineRenderer.sortingLayerName = "ForeGround";
            lineRenderer.sortingOrder = 100;
            obj.transform.SetParent(lineRendererContainer);
            obj.SetActive(false);
            lineRendererQueue.Enqueue(obj);
        }

    }

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


    public void Lightning(Transform target)
    {
        Vector3 dir = (target.transform.position - transform.position).normalized;
        GameObject newObj = lineRendererQueue.Dequeue();
        newObj.SetActive(true);

        StartLightningEffect();
        bezierCurve.Generator(newObj.GetComponent<LineRenderer>(),lightningBox.position,lightningBox.position + dir*3, target.position);

        lineRendererQueue.Enqueue(newObj);

        if(target.gameObject.TryGetComponent(out LightningRod lightningRod))
        {
            lightningRod.Electric();

        }
        else if(target.gameObject.TryGetComponent(out IDamageable component))
        {
            component.TakeDamage();
        }
        ////target.GetComponent<IDamageable>().TakeDamage();
        //target.gameObject.SetActive(false);

    }

}
