using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class CrumblingBox : BuildObj
{
    [Header("Info")]
    private float maxCrumblingRate = 2;
    public float curCrumblingRate;
    private int maxCrumblingAmount = 2;
    public int curCrumblingAmount;

    [Header("Components")]
    [SerializeField] Animator animator;
    [SerializeField] GameObject hitBox;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] ParticleSystem spark_Particle;
    [SerializeField] ParticleSystem bumb_Particle;
    [SerializeField] ShadowCaster2D shadowCaster2D;

    private string[] animationId = new string[] { "Green", "Yellow", "Red", "Explosion",};

    public Sprite[] sprites;
    private void Awake()
    {
        curCrumblingAmount= maxCrumblingAmount;
        curCrumblingRate = maxCrumblingRate;
    }

    CrumblingBox_Net net;
    CrumblingBox_Net Net { get { net ??= GetComponent<CrumblingBox_Net>(); return net; } }
    public override void SetData<T>(T data)
    {
        base.SetData(data);
        if (Application.isPlaying)
        {
            Net.Server_InitSync();
        }

    }

    public void Crumbling(int index)
    {
        animator.SetTrigger(animationId[index]);
        if(index >=0)
        spark_Particle.Play();
    }

    IEnumerator DestroyBox() //Animation trigger
    {
        bumb_Particle.Play();
        yield return new WaitForSeconds(0.2f);

        //Test Code
        GetComponent<Collider2D>().enabled = false;
        hitBox.SetActive(false);
        spriteRenderer.enabled = false;
        shadowCaster2D.enabled = false;
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

    //public override void TurnOff()
    //{
    //    base.TurnOff();
    //    hitBox.SetActive(false);
    //}
    //public override void TurnOn()
    //{
    //    base.TurnOn();
    //    hitBox.SetActive(true);
    //}
    public override void Reset()
    {
        curCrumblingAmount = maxCrumblingAmount;
        curCrumblingRate = maxCrumblingRate;

        //Test Code
        GetComponent<Collider2D>().enabled = true;
        hitBox.SetActive(true);
        spriteRenderer.enabled = true;
        shadowCaster2D.enabled = true;
        
        animator.SetTrigger(animationId[curCrumblingAmount]);
    }
}
