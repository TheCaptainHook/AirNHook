using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeslaTower : BuildObj
{
    [SerializeField] ParticleSystem[] chargeEffects;
    [SerializeField] Transform lightningBox;
    [SerializeField] TeslaBezierCurve bezierCurve;
    [SerializeField] Transform lineRendererContainer;

    Queue<LineRenderer> lineRendererQueue;

    /// <summary>
    /// 0614
    /// Queue<LineRenderer> , Craete Lightning function,
    /// 
    /// </summary>

    [SerializeField] GameObject TestObj;


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
        lineRendererQueue = new Queue<LineRenderer>();
    }

    public void StartChargeEffect()
    {
        foreach(ParticleSystem ps in chargeEffects)
        {
            ps.Play();
        }


        //bezierCurve.Generator(lightningBox.position, lightningBox.position + new Vector3(3, 1, 0), TestObj.transform.position);
        
    }

    public void StopChargeEffect()
    {
        foreach (ParticleSystem ps in chargeEffects)
        {
            ps.Stop();
        }
    }


    public void Lightning(Vector3 target)
    {
       
    }






}
