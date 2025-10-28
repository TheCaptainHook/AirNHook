
using Mirror;
using UnityEngine;

public class Battery : InteractableObjectEntity
{
    
    [CustomHeader("Battery")]

    [ReadOnly]
    public PowerSupply powerSupply;

    #region Network
    private BatteryInteractable Battery_Net => GetComponent<BatteryInteractable>();
    
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
            // respawnEvent += Battery_Net.Server_SetBatteryCapacity(-100);
            respawnEvent += Battery_Net.Server_ResetBattery;
        }
    }

    #region Network Sync
    public void Net_SetBatteryCharger(GameObject obj)
    {
        ////Battery_Net.Cmd_SetBatteryCharger(obj);
        //if (obj == null)
        //{
        //    Battery_Net.Cmd_SetBatteryCharger(9999);
        //}
        //else
        //{
        //    uint id = obj.TryGetComponent(out NetworkIdentity identity) ? identity.netId : 9999;
        //    Battery_Net.Cmd_SetBatteryCharger(id);


        //}
        Battery_Net.batteryCharger = obj;
    }
    public void Net_SetPowerSupply(GameObject obj)
    {
        //if(obj == null)
        //{
        //    Battery_Net.Cmd_SetPowerSupply(9999);
        //}
        //else
        //{
        //    uint id = obj.TryGetComponent(out NetworkIdentity identity) ? identity.netId : 9999;
        //    Battery_Net.Cmd_SetPowerSupply(id);


        //}
        Battery_Net.powerSupply = obj;
    }


    protected override void Clean_OtherValue()
    {
        if (NetworkServer.active) Battery_Net.batteryCapacity = 0;
    }
    #endregion
}
