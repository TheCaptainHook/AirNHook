using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleButton_Net : NetworkBehaviour
{
    bool HasClientsExceptHost => NetworkServer.connections.Count > 1;

    ToggleButtonObject toggle;
    ToggleButtonObject Toggle
    {
        get
        {
            if(toggle == null)toggle = GetComponent<ToggleButtonObject>();
            return toggle;
        }
    }

    // 클라이언트에서 서버로 명령을 전달하는 Command
    [Command]
    public void CmdActive()
    {
        RpcActive();
    }

    // 서버에서 클라이언트로 전달하는 ClientRpc
    [ClientRpc]
    private void RpcActive()
    {
        Toggle.Net_Activation();
    }

    [Command]
    public void CmdDeactived()
    {
        RpcDeactived();
    }

    // 서버에서 클라이언트로 전달하는 ClientRpc
    [ClientRpc]
    private void RpcDeactived()
    {
        Toggle.Net_Deactivated();
    }
}
