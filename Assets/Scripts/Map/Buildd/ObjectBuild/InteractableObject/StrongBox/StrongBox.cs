using System.Collections;

using UnityEngine;

public class StrongBox : InteractableObjectEntity
{
    [SerializeField] SpriteRenderer mainSprite;

    private float health = 7f;
    private float curHealth = 0;
    private StrongBox_Net Net => GetComponent<StrongBox_Net>();

    // protected override void Awake()
    // {
    //     curHealth = health;
    //     base.Awake();
    // }

    void Awake()
    {
        curHealth = health;
    }


    private Coroutine hitEffectCoroutine;
    public override void TakeDamage(DamageType damageType = DamageType.Default)
    {
        curHealth -= 1f;
        if (hitEffectCoroutine != null) StopCoroutine(hitEffectCoroutine);
        hitEffectCoroutine = StartCoroutine(HitEffectCo());

        if (curHealth <= 0f)
        {
            base.TakeDamage();
            curHealth = health; 
        }
    }
   
    IEnumerator HitEffectCo()
    {
        float percent = 0;
        mainSprite.color = Color.red;
        while(percent < 0.5f)
        {
            percent += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        mainSprite.color = Color.white;


    }

}
