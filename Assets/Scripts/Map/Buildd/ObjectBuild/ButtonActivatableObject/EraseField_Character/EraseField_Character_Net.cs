using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EraseField_Character_Net : NetworkBehaviour
{
    private EraseField_Character Main => GetComponent<EraseField_Character>();

    #region Init Sync
    public bool onSync;
    [Server]
    public void Server_InitSync()
    {
        Rpc_InitSync(Main.ButtonActivatedObjectStruct);
    }
    [ClientRpc]
    private void Rpc_InitSync(ButtonActivatableObjectStruct data)
    {
        if (onSync) return;
        transform.position = data.position;
        transform.rotation = data.quaternion;
        transform.localScale = data.scale;
        onSync = true;
    }
    [Command]
    private void Cmd_InitSync()
    {
        Server_InitSync();
    }
    public override void OnStartClient()
    {
        base.OnStartClient();
        if(!onSync) Cmd_InitSync();
    }
    #endregion


    [ClientRpc]
    public void Rpc_Active()
    {
        Main.Net_Active();
    }
    [ClientRpc]
    public void Rpc_Deactive()
    {
        Main.Net_Deactive();
    }
}
