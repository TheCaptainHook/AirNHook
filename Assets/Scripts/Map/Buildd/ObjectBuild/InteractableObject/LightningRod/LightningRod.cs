using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningRod : BuildObj
{

    // float dissolveRate = 0.05f;
    public Transform hitPoint;
    [ReadOnly]
    public bool onElectric;
    [SerializeField] float maxDurationRate; //Electric Duration
    public float curDurationRate;

    [SerializeField] ParticleSystem[] particles;



    private LightningRod_Net Net => GetComponent<LightningRod_Net>();

    private void Awake()
    {

        DissolveInitSetting();
        curDurationRate = maxDurationRate;
    }


    public override void SetData<T>(T data)
    {
        base.SetData(data);
        Net.onSync = true;
        Net.Server_InitSync();
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
