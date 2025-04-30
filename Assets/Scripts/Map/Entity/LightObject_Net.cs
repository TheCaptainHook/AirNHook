
using UnityEngine;
using Mirror;
using System.Collections;
using System;

public class LightObject_Net : NetworkBehaviour
{
    private LightObjectEntity entity;
    private LightObjectEntity Entity
    {
        get
        {
            if(entity == null) entity = GetComponent<LightObjectEntity>();
            return entity;
        }
    }
    
    
    [SyncVar(hook = nameof(OnChangeHasPower))] 
    public bool hasPower;

    [SyncVar] public bool chargeRequired;
    
    [Server]
    private void Server_SetHasPower(bool hasPower)
    {
        this.hasPower = hasPower;
    }
    [Command(requiresAuthority = false)]
    public void Cmd_SetHasPower(bool hasPower)
    {
        Server_SetHasPower(hasPower);
    }


    [Server]
    private void Server_SetChargeRequired(bool chargeRequired)
    {
        this.chargeRequired = chargeRequired;
        if(chargeRequired) Entity.PowerOff();
       
    }
    // [ClientRpc]
    // private void Power(bool hasPower)
    // {
    //     if(hasPower) Entity.PowerOn();
    //     else Entity.PowerOff();
    // }
    [Command]
    public void Cmd_SetChargeRequired(bool chargeRequired)
    {
        Server_SetChargeRequired(chargeRequired);
    }




    private void OnChangeHasPower(bool old,bool newVal)
    {
        if(newVal)
        {
            // Entity.PowerOn();
            Entity.gameObject.SetActive(true);
        }
        else
        {
            Entity.gameObject.SetActive(false);
        }
    }

    public override void OnStartClient()
    {
        // if(isServer) return;
        base.OnStartClient();

        if(chargeRequired)
        {
            if(hasPower)Entity.PowerOn();
            else Entity.PowerOff();
        }
        // Delay(()=>
        // {
        //     if(chargeRequired)
        //     {
        //         if(hasPower)Entity.PowerOn();
        //         else Entity.PowerOff();
        //     }
        // });
       
    }

    IEnumerator Delay(Action action)
    {
        yield return null;
        action?.Invoke();
    }


}
