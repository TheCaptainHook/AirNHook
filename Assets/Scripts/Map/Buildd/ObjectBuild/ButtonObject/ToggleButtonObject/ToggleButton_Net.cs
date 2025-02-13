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


    [SyncVar] public bool chargeRequired;
    [SyncVar] public bool hasPower;

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

    [Server]
    private void Server_SetHasPower(bool hasPower)
    {
        this.hasPower = hasPower;
    }

    [Command(requiresAuthority = false)]
    public void Cmd_SetHasPower(bool hasPower)
    {
        Server_SetHasPower(hasPower);
    }
    //[ClientRpc]
    //public void Rpc_SertHasPower(bool hasPower)
    //{
    //    Toggle.hasPower = hasPower;
    //}
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




    [Server] //Set sync chargeRequired
    public void Server_SetChargeRequired(bool chargeRequired)
    {
        this.chargeRequired = chargeRequired;
    }




}
