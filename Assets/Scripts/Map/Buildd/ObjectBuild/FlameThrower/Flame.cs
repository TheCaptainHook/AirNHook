using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flame : MonoBehaviour
{
    public ParticleSystem particle;
    float lifeTime;
    private void Awake()
    {
        particle = GetComponent<ParticleSystem>();
        GetParticleLifeTime();
    }



    private void GetParticleLifeTime()
    {
        var main = particle.main;
        lifeTime = main.startLifetime.constant;
        Debug.Log(lifeTime);
    }

    public void SetLifeTime()
    {
        var main = particle.main;
        main.startLifetime = lifeTime;
    }
}
