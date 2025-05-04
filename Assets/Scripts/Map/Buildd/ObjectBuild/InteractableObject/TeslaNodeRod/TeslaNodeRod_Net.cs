using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class TeslaNodeRod_Net : NetworkBehaviour
{
    private TeslaNodeRod main;

    private ButtonObjectStruct data;

    private void Awake()
    {
        main = GetComponent<TeslaNodeRod>();
    }

    #region Init Sync
    public bool onSync;
    [Server]
    public void Server_InitSync()
    {
        onSync = true;
        Rpc_InitSync(main.ButtonObjectData);
    }
    [ClientRpc]
    private void Rpc_InitSync(ButtonObjectStruct data)
    {
        if (onSync) return;
        this.data = data;
        transform.position = data.position;
        transform.rotation = data.quaternion;

    }
    [Command(requiresAuthority = false)]
    private void Cmd_InitSync()
    {
        Server_InitSync();
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (!onSync) Cmd_InitSync();
    }

    #endregion

    #region IPowerConsumer
    [SyncVar(hook = nameof(Hook_OnChangeHasPower))]
    public int hasPower;

    private void Hook_OnChangeHasPower(int old,int newVal)
    {
        if (newVal == 1)
        {
            if (!isActive)
            {
                isActive = true;
                main.Net_Active();
            }
            //Active
        }else if(newVal == 0)
        {
            isActive = false;
            main.Net_DeActive();
            //Deactive
        }
    }
    public bool isActive;
    [Server]
    public void Server_SetHasPower(bool value)
    {
        if(value)
        {   
            hasPower++;

        }
        else
        {
            hasPower--;
        }
    }

    #endregion
    [Server]
   public void Server_Effect()
   {

   }
   [ClientRpc]
   private void Rpc_Effect()
   {
    
   }

}
