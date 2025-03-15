using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class TeslaTower_Net : NetworkBehaviour
{

    private TeslaTower Main => GetComponent<TeslaTower>();

    #region Init Sync
    public bool onSync;

    [Server]
    public void Server_InitSync()
    {
        Debug.Log("1111111");
        Rpc_InitSync(Main.ObjectData);
    }
    [ClientRpc]
    private void Rpc_InitSync(ObjectData data)
    {
        Debug.Log("222222");
        if(onSync) return;
        transform.position = data.position;
        transform.localScale = data.scale;
        transform.rotation = data.quaternion;

        onSync = true;
    }
    [Command(requiresAuthority = false)]
    private void Cmd_InitSync()
    {
        Debug.Log("444444444");
        Server_InitSync();
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        Debug.Log("333333");
        if(!onSync)Cmd_InitSync();
    }
    #endregion
}
