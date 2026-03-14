
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
        Rpc_Init(Entity.ObjectData);

    }
    [ClientRpc]
    private void Rpc_Init(ObjectData data)
    {
        if (onSync) return;
        transform.position = data.position;
        transform.rotation = data.quaternion;
        transform.localScale = data.scale;
        chargeRequired = data.chargeRequired;
        if (chargeRequired)
        {
            if (hasPower == 0)
            {
                // Entity.LightOnOff(false);
             } 
        }

        onSync = true;
    }
    [Command(requiresAuthority = false)]
    private void Cmd_Init()
    {
        Server_Init();
    }
    public override void OnStartClient()
    {
        // if(isServer) return;
        base.OnStartClient();
        if (!onSync) Cmd_Init();

    }
    #endregion



    [SyncVar(hook = nameof(OnChangeHasPower))] 
    public int hasPower;

    [SyncVar] public bool chargeRequired;
    
    [Server]
    public void Server_SetHasPower(bool hasPower)
    {
        if(hasPower) this.hasPower++;
        else this.hasPower--;
        
        // this.hasPower = hasPower;
    }
    // [Command(requiresAuthority = false)]
    // public void Cmd_SetHasPower(bool hasPower)
    // {
    //     Server_SetHasPower(hasPower);
    // }


    //[Server]
    //private void Server_SetChargeRequired(bool chargeRequired)
    //{
    //    this.chargeRequired = chargeRequired;
    //    if(chargeRequired) Entity.PowerOff();
       
    //}
   
    // }
    //[Command]
    //public void Cmd_SetChargeRequired(bool chargeRequired)
    //{
    //    Server_SetChargeRequired(chargeRequired);
    //}




    private void OnChangeHasPower(int old,int newVal)
    {
        if(newVal>0)
        {
            // Entity.PowerOn();
            // Entity.LightOnOff(true);
            Debug.Log("Light PowerOn");
        }
        else
        {
            // Entity.LightOnOff(false);
            Debug.Log("Light PowerOff");
        }
    }

 



}
