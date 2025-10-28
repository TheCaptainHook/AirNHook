using System;
using System.Collections;
using Mirror;
using UnityEngine;

public abstract class ButtonEntity_Net : NetworkBehaviour
{
    private ButtonEntity _main;
    protected ButtonEntity Main { get { _main ??= GetComponent<ButtonEntity>(); return _main; } }

    public bool _isActive;

    #region  Power
    [SyncVar(hook = nameof(Hook_ChargeRequired))]
    public bool _chargeRequired;
    
    [SyncVar] public int _hasPower;
    
    [Command(requiresAuthority = false)]
    public void Cmd_SetHasPower(bool power)
    {
        Server_SetHasPower(power);
    }

    [Server]
    private void Server_SetHasPower(bool power)
    {
        if (power) _hasPower++;
        else
        {
            _hasPower--;
            if (_hasPower < 0) _hasPower = 0;
        }

        _chargeRequired = _hasPower > 0 ? false : true;
    }
    
    #endregion

    #region  Init
    public bool _onSync;

    [Server]
    public void Server_SetInit()
    {
        var data = Main.ButtonObjectData;
        _chargeRequired = data.chargeRequired;
        Server_Sync_OtherValue();

        Rpc_SetInit(data);
    }
    [Command(requiresAuthority = false)]
    private void Cmd_SetInit()
    {
        Server_SetInit();
    }
    [ClientRpc]
    private void Rpc_SetInit(ButtonObjectStruct data)
    {
        if (_onSync) return;
        transform.position = data.position;
        transform.rotation = data.quaternion;

        // _chargeRequired = data.chargeRequired;

        _onSync = true;
    }
    public override void OnStartClient()
    {
        base.OnStartClient();
        Cmd_SetInit();
    }
    [Server]
    protected virtual void Server_Sync_OtherValue()
    {
        
    }

    //if need Energy Item, override
    protected virtual void Hook_ChargeRequired(bool old, bool newVal)
    {

    }
    #endregion

    #region  Set State
    [Command(requiresAuthority = false)]
    public void Cmd_SetState(bool state)
    {
        Rpc_SetState(state);
    }
    [ClientRpc]
    private void Rpc_SetState(bool state)
    {
        _isActive = state;
        if (_isActive) Main.Activation();
        else Main.Deactivated();
    }
    #endregion

    #region  Clean
    [Server]
    public virtual void Server_Clean()
    {
        _chargeRequired = false;
        _hasPower = 0;
        Rpc_Clean();
    }
    [ClientRpc]
    protected virtual void Rpc_Clean()
    {
        _onSync = false;
        _isActive = false;
        Main.Animation_Clean();
    }
    public virtual void Clean()
    {
        if (isServer) Server_Clean();
    }
    #endregion

    #region Util
    protected IEnumerator AllClientIsReadyCoroutine(Action action)
    {
        int connectClients = NetworkServer.connections.Count;
        bool onReady = false;
        while (!onReady)
        {
            int num = 0;
            foreach (var conn in NetworkServer.connections.Values)
            {
                if (conn.isReady) num++;
            }

            if (connectClients == num) onReady = true;
            yield return null;
        }

        action?.Invoke();
    }
    #endregion
}
