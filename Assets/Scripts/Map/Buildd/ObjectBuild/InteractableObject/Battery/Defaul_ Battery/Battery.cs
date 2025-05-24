
using Mirror;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Battery : BuildObj
{
    
    [CustomHeader("Battery")]

    [ReadOnly]
    public PowerSupply powerSupply;


    public override void SetData<T>(T data)
    {
        base.SetData(data);
        // Battery_Net.Server_SetOrgPot(position);
        Battery_Net.onSync = true;
        Battery_Net.Server_InitSync();
    }

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

    #region Components
    Animator animator;
    Collider2D col;
    Rigidbody2D rb;
    #endregion

    private void Awake()
    {
        //col = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        DissolveInitSetting();
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
        //Battery_Net.Cmd_SetBatteryCharger(obj);
        if (obj == null)
        {
            Battery_Net.Cmd_SetBatteryCharger(9999);
        }
        else
        {
            uint id = obj.TryGetComponent(out NetworkIdentity identity) ? identity.netId : 9999;
            Battery_Net.Cmd_SetBatteryCharger(id);


        }
    }
    public void Net_SetPowerSupply(GameObject obj)
    {
        if(obj == null)
        {
            Battery_Net.Cmd_SetPowerSupply(9999);
        }
        else
        {
            uint id = obj.TryGetComponent(out NetworkIdentity identity) ? identity.netId : 9999;
            Battery_Net.Cmd_SetPowerSupply(id);


        }
    }

           
    #endregion
}
