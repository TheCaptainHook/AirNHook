using Mirror;


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
    private void SetState(bool state)
    {
        isActive = state;
    }

    [Command(requiresAuthority = false)]
    private void CmdSetState(bool state)
    {
        SetState(state);
    }

    public void HandleSetState(bool state)
    {
        if(isServer)
        {
            SetState(state);
        }
        else
        {
            CmdSetState(state);
        }
    }

    public void OnStateChanged(bool oldVal,bool newVal)
    {
        // Toggle.SetActive(newVal);
        if(newVal)
        {
            Toggle.Net_Activation();
        }else
        {   
            Toggle.Net_Deactivated();
        }

    }


    [Command(requiresAuthority = false)]
    public void Cmd_SetHasPower(bool hasPower)
    {
        Rpc_SertHasPower(hasPower);
    }
    [ClientRpc]
    public void Rpc_SertHasPower(bool hasPower)
    {
        Toggle.hasPower = hasPower;
    }
    [Command]
    public void Cmd_CallDeactivated()
    {
        Rpc_CallDeactivated();
    }
    [ClientRpc]
    private void Rpc_CallDeactivated()
    {
        Toggle.Net_Deactivated();
    }    

    public override void OnStartClient()
    {
        base.OnStartClient();
        CmdSetState(isActive);
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
