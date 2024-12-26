
using UnityEngine;

public class Battery : BuildObj
{
    
    private float maxCapacity = 100;
    [CustomHeader("Battery")]
    [ReadOnly]
    public float batteryCapacity;
    public float BatteryCapacity 
    { 
        get { return batteryCapacity; }
        set { 
            batteryCapacity += value;
            if(batteryCapacity > maxCapacity) batteryCapacity = maxCapacity;
        }
    }

    public BatteryCharger batteryCharger;

    public override void TakeDamage(DamageType damageType = DamageType.Default)
    {
        if(damageType == DamageType.Electric)
        {
            BatteryCapacity = 10;
        }
    }

    enum BatteryState
    {
        Idle,
        Charging,
        Discharging
    }

    private BatteryState batteryState;

    #region Components
    Animator Animator;
    Collider2D col;
    Rigidbody2D rb;
    #endregion

    #region Animation
    #endregion


    private void Awake()
    {
        col = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
    }

    public void InsertSocket()
    {
        if (batteryCharger)
        {
            col.enabled = false;
            batteryCharger.Charge(this);
        }
    }

    public void RemoveSocket()
    {
        col.enabled = true;
        rb.gravityScale = 1;
        RemoveEffect();
    }









    private float horizontalVariation = 1f;
    private void RemoveEffect()
    {
        float xForce = Random.Range(-horizontalVariation, horizontalVariation);
        rb.AddForce(new Vector2(xForce, 3f), ForceMode2D.Impulse);
    }
}
