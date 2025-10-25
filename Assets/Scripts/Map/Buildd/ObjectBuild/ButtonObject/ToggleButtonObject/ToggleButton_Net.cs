using Mirror;
using UnityEngine;

public class ToggleButton_Net : ButtonEntity_Net
{
    [SerializeField] private GameObject energyIcon;
    protected override void Hook_ChargeRequired(bool old, bool newVal)
    {
        if (newVal)
        {
            energyIcon.SetActive(true);
        }
        else
        {
            energyIcon.SetActive(false);
        }
    }
    // ToggleButtonObject toggle;
    // ToggleButtonObject Toggle
    // {
    //     get
    //     {
    //         if (toggle == null) toggle = GetComponent<ToggleButtonObject>();
    //         return toggle;
    //     }
    // }

    // [SyncVar(hook = nameof(OnStateChanged))]
    // public bool isActive;

    // [SyncVar(hook = nameof(onChangeChargeRequired))]
    // public bool chargeRequired;


    // [SyncVar] public int hasPower;

    // [Server]
    // private void SetState(bool state)
    // {
    //     // isActive = state;
    //     Rpc_SetState(state);
    // }

    // [ClientRpc]
    // private void Rpc_SetState(bool state)
    // {
    //     isActive = state;
    //     if (state)
    //     {
    //         Toggle.Net_Activation();
    //     }
    //     else
    //     {
    //         Toggle.Net_Deactivated();
    //     }
    // }

    // [Command(requiresAuthority = false)]
    // private void CmdSetState(bool state)
    // {
    //     SetState(state);
    // }

    // public void HandleSetState(bool state)
    // {
    //     // if (isServer)
    //     // {
    //     //     SetState(state);
    //     // }
    //     // else
    //     // {
    //     //     CmdSetState(state);
    //     // }
    //     CmdSetState(state);
    // }

    // public void OnStateChanged(bool oldVal, bool newVal)
    // {
    //     // Toggle.SetActive(newVal);
    //     if (newVal)
    //     {
    //         Toggle.Net_Activation();
    //     } else
    //     {
    //         Toggle.Net_Deactivated();
    //     }

    // }

    #region Has Power
    // [Server]
    // private void Server_SetHasPower(bool hasPower)
    // {
    //     if (hasPower) this.hasPower++;
    //     else
    //     {
    //         this.hasPower--;
    //         if (this.hasPower < 0) this.hasPower = 0;
    //     }
    //     //this.hasPower = hasPower;  //this.hasPower++;

    //     Server_SetChargeRequired(this.hasPower > 0 ? false : true);
    // }

    // [Command(requiresAuthority = false)]
    // public void Cmd_SetHasPower(bool hasPower)
    // {
    //     Server_SetHasPower(hasPower);
    // }
    #endregion

    // [Command]
    // public void Cmd_CallDeactivated()
    // {
    //     Rpc_CallDeactivated();
    // }
    // [ClientRpc]
    // private void Rpc_CallDeactivated()
    // {
    //     Toggle.Net_Deactivated();
    // }




    #region Init
    // public bool onSync;
    // [Server]
    // public void Server_SetInit()
    // {
    //     Rpc_SetInit(Toggle.ButtonObjectData);
    // }
    // [ClientRpc]
    // private void Rpc_SetInit(ButtonObjectStruct data)
    // {
    //     if (onSync) return;
    //     transform.position = data.position;
    //     transform.rotation = data.quaternion;

    //     energyIcon.SetActive(chargeRequired);

    //     onSync = true;
    // }
    // [Command]
    // private void Cmd_SetInit()
    // {
    //     Server_SetInit();
    // }

    #endregion


    // [Server] //Set sync chargeRequired
    // public void Server_SetChargeRequired(bool onOff)
    // {
    //     if (onOff) this.chargeRequired = true;
    //     else this.chargeRequired = false;

    // }


    // private void onChangeChargeRequired(bool old, bool newVal)
    // {
    //     if (newVal)
    //     {
            // energyIcon.SetActive(true);
    //     }
    //     else
    //     {
    //         energyIcon.SetActive(false);
    //     }
    // }

    #region  Server_Clean
    // [Server]
    // public void Server_Clean()
    // {
    //     Rpc_Clean();
    //     chargeRequired = false;
    //     hasPower = 0;
    // }
    // [ClientRpc]
    // private void Rpc_Clean()
    // {
    //     isActive = false;
    // }
    #endregion

}
