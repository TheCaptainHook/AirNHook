
using Mirror;
using UnityEngine;
using UnityEngine.Animations;

public class Battery : InteractableObjectEntity
{
    
    [CustomHeader("Battery")]

    [ReadOnly]
    public PowerSupply powerSupply;

    #region Network
    private BatteryInteractable net ;
    private BatteryInteractable Battery_Net {get{net ??= GetComponent<BatteryInteractable>(); return net;}}
    
    #endregion

    public override void TakeDamage(DamageType damageType = DamageType.Default)
    {
        //position = Battery_Net.orgPosition;

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


    void Start()
    {
        if (NetworkServer.active)
        {
            respawnEvent += Battery_Net.Server_ResetBattery;
        }
    }

    #region Network Sync
  

    protected override void Clean_OtherValue()
    {
        if (NetworkServer.active) Battery_Net.batteryCapacity = 0;
    }

    #endregion
}
