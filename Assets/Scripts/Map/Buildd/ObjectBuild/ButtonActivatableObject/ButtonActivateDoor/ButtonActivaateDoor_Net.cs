using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class ButtonActivaateDoor_Net : NetworkBehaviour
{

    ButtonActivatedDoor door;
    ButtonActivatedDoor Door{
        get{
            if(door == null) door = GetComponent<ButtonActivatedDoor>();
            return door;
        }
    }

    [SyncVar(hook = nameof(OnDoorStateChanged))]
    private bool isOpen;

    #region  Init
    public bool onSync;
    [Server]
    public void Server_InitSync()
    {
        Rpc_InitSync(Door.ButtonActivatedObjectStruct);
    }
    [ClientRpc]
    private void Rpc_InitSync(ButtonActivatableObjectStruct data)
    {
        if(onSync) return;
        transform.position = data.position;
        transform.rotation = data.quaternion;
        transform.localScale = data.scale;
        onSync = true;
    }
    [Command(requiresAuthority =false)]
    public void Cmd_InitSync()
    {
        Server_InitSync();
    }
    public override void OnStartClient()
    {
        base.OnStartClient();
        if(!onSync)Cmd_InitSync();
    }
    #endregion

    [Server]
    private void SetState(bool open)
    {
        isOpen = open; 
    }

    [Command(requiresAuthority = false)]
    private void CmdSetState(bool newState)
    {
        SetState(newState);
    }


    public void HandleSetState(bool newState)
    {
        if(isServer)
        {
            SetState(newState);
        }else
        {
            CmdSetState(newState);
        }
    }

    private void OnDoorStateChanged(bool oldValue, bool newValue)
    {
       if(newValue){
        Door.Open();
       }else{
        Door.Close();
       }

    }




}
