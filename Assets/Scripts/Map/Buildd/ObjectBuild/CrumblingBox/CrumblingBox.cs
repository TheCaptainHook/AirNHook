using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrumblingBox : BuildObj
{
    [Header("Info")]
    [SerializeField] float maxCrumblingRate;
    private float curCrumblingRate;
    [SerializeField] int maxCrumblingAmount;
    private int curCrumblingAmount;

    [Header("Components")]
    [SerializeField] GameObject hitBox;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] ParticleSystem spark_Particle;
    [SerializeField] ParticleSystem bumb_Particle;

    //Test Code
    public Sprite[] sprites;
    private void Awake()
    {
        curCrumblingAmount= maxCrumblingAmount;
        curCrumblingRate = maxCrumblingRate;
    }

    public void Crumbling()
    {
        curCrumblingAmount--;
        if(curCrumblingAmount < 0)
        {
            StartCoroutine(DestroyBox());
            return;
        }

        //Test Code
        spark_Particle.Play();
        spriteRenderer.sprite = sprites[curCrumblingAmount];
    }

  

    IEnumerator DestroyBox()
    {
        bumb_Particle.Play();
        yield return new WaitForSeconds(0.2f);

        //Test Code
        GetComponent<Collider2D>().enabled = false;
        spriteRenderer.enabled = false;
        //Test Code
        StartCoroutine(Timer());
    }
    IEnumerator Timer()
    {
        while (curCrumblingRate > 0)
        {
            curCrumblingRate -= Time.deltaTime;
            yield return null;

        }

        Reset();
    }


    public override void TurnOff()
    {
        base.TurnOff();
        hitBox.SetActive(false);
    }
    public override void TurnOn()
    {
        base.TurnOn();
        hitBox.SetActive(true);
    }
    public override void Reset()
    {
        curCrumblingAmount = maxCrumblingAmount;
        curCrumblingRate = maxCrumblingRate;

        //Test Code
        GetComponent<Collider2D>().enabled = true;
        spriteRenderer.enabled = true;
        spriteRenderer.sprite = sprites[maxCrumblingAmount];

    }
}
