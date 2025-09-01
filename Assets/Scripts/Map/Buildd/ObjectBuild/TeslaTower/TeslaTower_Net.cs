using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class TeslaTower_Net : NetworkBehaviour
{

    private TeslaTower Main => GetComponent<TeslaTower>();

    #region Init Sync
    public bool onSync;

    private AudioSourceController audioSourceController;

    private void OnDisable()
    {
        if(audioSourceController != null) Managers.Sound.StopSound(audioSourceController);
    }

    [Server]
    public void Server_InitSync()
    {
        Rpc_InitSync(Main.ObjectData);
    }
    [ClientRpc]
    private void Rpc_InitSync(ObjectData data)
    {
        if(onSync) return;
        transform.position = data.position;
        transform.localScale = data.scale;
        transform.rotation = data.quaternion;
        //Sound
        audioSourceController = Managers.Sound.PlaySound3D(GlobalText.TESLATOWER_ON, transform.position, 1, true);
        //Sound
        onSync = true;
    }
    [Command(requiresAuthority = false)]
    private void Cmd_InitSync()
    {
        Server_InitSync();
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        if(!onSync)Cmd_InitSync();
    }
    #endregion
}
