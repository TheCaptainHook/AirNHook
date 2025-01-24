using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleButton_Net : NetworkBehaviour
{

    ToggleButtonObject toggle;
    ToggleButtonObject Toggle
    {
        get
        {
            if(toggle == null)toggle = GetComponent<ToggleButtonObject>();
            return toggle;
        }
    }

    [SyncVar(hook = nameof(OnStateChanged))]
    private bool isActive;

    [Server]
    public void SetState(bool state)
    {
        isActive = state;
    }


    public void OnStateChanged(bool oldVal,bool newVal)
    {
        // Toggle.SetActive(newVal);
        if(newVal)
        {
            Debug.Log("Act");
            Toggle.Net_Activation();
        }else
        {   
            Debug.Log("Dac");
            Toggle.Net_Deactivated();
        }

    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        Toggle.SetActive(isActive);
    }


    // // 클라이언트에서 서버로 명령을 전달하는 Command
    // [Command(requiresAuthority =false)]
    // public void CmdActive()
    // {
    //         RpcActive();        
    // }

    // // 서버에서 클라이언트로 전달하는 ClientRpc
    // [ClientRpc]
    // private void RpcActive()
    // {
    //     Toggle.Net_Activation();
    // }

    // [Command(requiresAuthority = false)]
    // public void CmdDeactived()
    // {
    //     RpcDeactived();
    // }

    // // 서버에서 클라이언트로 전달하는 ClientRpc
    // [ClientRpc]
    // private void RpcDeactived()
    // {
    //     Toggle.Net_Deactivated();
    // }


}
