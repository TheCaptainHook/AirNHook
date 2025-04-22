using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class LaserObject_Net : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnChangeOnActive))]
    public bool onActive;


    public bool shouldRunFixedUpdate;

    //LaserObject laser;
    LaserObject Laser;
    //{
    //    get
    //    {
    //        if (laser == null) laser = GetComponent<LaserObject>();
    //        return laser;
    //    }
    //}
    private void Awake()
    {
        Laser = GetComponent<LaserObject>();
    }

    #region Init
    public bool onSync;
    [Server]
    public void Server_InitSync()
    {
        Rpc_InitSync(Laser.ButtonActivatedObjectStruct);
    }

    [ClientRpc]
    private void Rpc_InitSync(ButtonActivatableObjectStruct data)
    {
        if (onSync) return;
        transform.position = data.position;
        transform.rotation = data.quaternion;

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
        if (!onSync) Cmd_InitSync();
    }
    #endregion


    [Server]
    public void Server_SetOnActive(bool active)
    {
        if(onActive != active)
        onActive = active;
    }
    private void OnChangeOnActive(bool old, bool newVal)
    {
        if (newVal)
        {
            Laser.Net_Active();
        }
        else
        {
            Laser.Net_Deactive();
        }
    }


}
