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

    [Server]
    private void SetState(bool open)
    {
        isOpen = open; // 이 시점에 hook 메서드가 클라이언트에서 실행됩니다.
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


    public override void OnStartClient()
    {
        base.OnStartClient();
        if(isOpen){
            Door.Open();
        }{
            Door.Close();
        }
    }

  

    //[Command(requiresAuthority = false)]
    //public void CmdOpen()
    //{
    //    RpcOpen();
    //}
    //[ClientRpc]
    //public void RpcOpen()
    //{
    //    isOpen = true;
    //    Door.Open();
    //}
    //[Command(requiresAuthority = false)]
    //public void CmdClose()
    //{
    //    RpcClose();
    //}
    //[ClientRpc]
    //public void RpcClose()
    //{
    //    isOpen = false;
    //    Door.Close();
    //}



}
