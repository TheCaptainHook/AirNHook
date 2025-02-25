
using System.Runtime.CompilerServices;
using UnityEngine;

public class Battery : BuildObj
{
    

    //private float maxCapacity = 100;
    [CustomHeader("Battery")]

    //public float batteryCapacity;
    //public float BatteryCapacity 
    //{ 
    //    get { return batteryCapacity; }
    //    set { 
    //        batteryCapacity += value;
    //        if(batteryCapacity > maxCapacity) batteryCapacity = maxCapacity;
    //        animator.SetFloat(CAPACITY,batteryCapacity/maxCapacity);
    //    }
    //}
    //[ReadOnly]
    //public BatteryCharger batteryCharger;

    [ReadOnly]
    public PowerSupply powerSupply;


    

    #region Network
    private BatteryInteractable Battery_Net => GetComponent<BatteryInteractable>();
    
    #endregion

    public override void TakeDamage(DamageType damageType = DamageType.Default)
    {
        if(damageType == DamageType.Electric)
        {
            //BatteryCapacity = 10;
            Battery_Net.Cmd_SetBatteryCapacity(10);
        }
        else
        {
            Battery_Net.Cmd_SetBatteryCapacity(-100);
            base.TakeDamage();
        }


    }

    #region Components
    Animator animator;
    Collider2D col;
    Rigidbody2D rb;
    #endregion

    //#region Animation
    //private readonly int CAPACITY = Animator.StringToHash("Capacity");
    //#endregion


    private void Awake()
    {
        //col = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
        animator= GetComponent<Animator>();
        DissolveInitSetting();
    }

    //public void InsertChargerSocket()
    //{
    //    //if (batteryCharger)
    //    //{
    //    //    col.enabled = false;
    //    //    batteryCharger.Charge(this);
    //    //}
    //    if (Battery_Net.batteryCharger)
    //    {
    //        col.enabled = false;

    //        //batterCharget -> SetBattery -> Charging
    //        batteryCharger.Charge(this);
    //    }
    //}


   

    public void InsertPowerSocket()
    {
        if(powerSupply){
            col.enabled = false;
            // powerSupply.InsertSocket(this);

        }
    }
    public void RemoveSocket()
    {
        //col.enabled = true;
        //rb.gravityScale = 1;
        //RemoveEffect();
        Battery_Net.Cmd_Recover();
    }





    //public void Net_SetBatteryCapacity(float val)
    //{
    //    Battery_Net.Server_SetBatteryCapacity(val);
    //}
    //public float BatteryCapacity()
    //{
    //    return Battery_Net.batteryCapacity;
    //}


    //private float horizontalVariation = 1f;
    //private void RemoveEffect()
    //{
    //    float xForce = Random.Range(-horizontalVariation, horizontalVariation);
    //    rb.AddForce(new Vector2(xForce, 3f), ForceMode2D.Impulse);
    //}



    #region Network Sync
    public void Net_SetBatteryCharger(GameObject obj)
    {
        Battery_Net.Cmd_SetBatteryCharger(obj);
    }
    public void Net_SetPowerSupply(GameObject obj)
    {
        Battery_Net.Cmd_SetPowerSupply(obj);
    }
    #endregion
}
