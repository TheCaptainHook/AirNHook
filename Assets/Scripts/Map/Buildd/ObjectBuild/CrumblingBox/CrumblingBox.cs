using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrumblingBox : BuildObj
{
    [Header("Info")]
    [SerializeField] float maxCrumblingRate;
    public float curCrumblingRate;
    [SerializeField] int maxCrumblingAmount;
    public int curCrumblingAmount;

    [Header("Components")]
    [SerializeField] Animator animator;
    [SerializeField] GameObject hitBox;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] ParticleSystem spark_Particle;
    [SerializeField] ParticleSystem bumb_Particle;
    bool onPrograss;

    private string[] animationId = new string[] { "Red", "Yellow", "Green" };

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
        curCrumblingAmount = Mathf.Clamp(curCrumblingAmount, -1, 2);
        if(curCrumblingAmount == -1 && !onPrograss)
        {
            animator.SetTrigger("Explosion");
            return;
        }

        spark_Particle.Play();
        animator.SetTrigger(animationId[curCrumblingAmount]);
    }

  

    IEnumerator DestroyBox()
    {
        onPrograss = true;
        bumb_Particle.Play();
        yield return new WaitForSeconds(0.2f);

        //Test Code
        GetComponent<Collider2D>().enabled = false;
        hitBox.SetActive(false);
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
        hitBox.SetActive(true);
        spriteRenderer.enabled = true;
        
        animator.SetTrigger(animationId[curCrumblingAmount]);
        onPrograss = false;

    }
}
