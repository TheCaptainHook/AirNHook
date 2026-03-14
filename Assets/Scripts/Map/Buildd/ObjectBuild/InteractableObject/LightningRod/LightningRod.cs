using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningRod : InteractableObjectEntity
{

    // float dissolveRate = 0.05f;
    public Transform hitPoint;
    [ReadOnly]
    public bool onElectric;
    [SerializeField] float maxDurationRate; //Electric Duration
    public float curDurationRate;

    [SerializeField] ParticleSystem[] particles;


    // protected override void Awake()
    // {
    //     base.Awake();
    //     curDurationRate = maxDurationRate;
    // }

    void Awake()
    {
        curDurationRate = maxDurationRate;
    }

    public void Electric()
    {
        if (!onElectric)
        {
            Activate();
            StartCoroutine(Timer());
        }
        else
        {
            curDurationRate = maxDurationRate;
        }

    }


   public void Activate()
    {
        onElectric = true;
        foreach (ParticleSystem ps in particles)
        {
            ps.Play();
        }
    }
    public void Deactivate()
    {
        onElectric = false;
        foreach (ParticleSystem ps in particles)
        {
            ps.Stop();
            
        }
    }





    IEnumerator Timer()
    {
        while(curDurationRate > 0 && onElectric)
        {
            curDurationRate -= Time.deltaTime;
            yield return null;
        }
        Deactivate();
        curDurationRate = maxDurationRate;
    }

}
