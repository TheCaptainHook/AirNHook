using System.Collections;

using UnityEngine;

public class StrongBox : BuildObj
{
    [SerializeField] SpriteRenderer mainSprite;

    private float health = 7f;
    private float curHealth = 0;
    private StrongBox_Net Net => GetComponent<StrongBox_Net>();

    private void Awake()
    {
        curHealth = health;
        DissolveInitSetting();
    }


    public override void SetData<T>(T data)
    {
        base.SetData(data);
        Net.onSync = true;
        Net.Server_InitSync();
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

    public override void TurnOff()
    {
        base.TurnOff();
        _rb.gravityScale = 0;
        _rb.velocity = Vector2.zero;
    }
    public override void TurnOn()
    {
        base.TurnOn();
        _rb.gravityScale = 1;
    }
}
