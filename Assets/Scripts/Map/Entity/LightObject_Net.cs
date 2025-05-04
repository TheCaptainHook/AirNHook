
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


    #region Init
    public bool onSync;
    [Server]
    public void Server_Init()
    {
        onSync = true;

    }
    [ClientRpc]
    private void Rpc_Init()
    {

    }
    [Command(requiresAuthority = false)]
    private void Cmd_Init()
    {

    }
    public override void OnStartClient()
    {
        // if(isServer) return;
        base.OnStartClient();

        if (chargeRequired)
        {
            if (hasPower>0) Entity.PowerOn();
            else Entity.PowerOff();
        }

    }
    #endregion



    [SyncVar(hook = nameof(OnChangeHasPower))] 
    public int hasPower;

    [SyncVar] public bool chargeRequired;
    
    [Server]
    private void Server_SetHasPower(bool hasPower)
    {
        if(hasPower) this.hasPower++;
        else this.hasPower--;
        // this.hasPower = hasPower;
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
            Entity._Light_Object.SetActive(true);
        }
        else
        {
            Entity._Light_Object.SetActive(false);
        }
    }

 



}
