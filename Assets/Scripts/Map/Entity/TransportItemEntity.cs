using System;
using System.Collections;
using Mirror;
using Steamworks;
using UnityEngine;

[RequireComponent(typeof(EncapsulationField))]
public class TransportItemEntity : InteractableObject, ITransportItem
{
    #region Transport Item
    protected Collider2D Col => GetComponent<Collider2D>();
    protected Rigidbody2D rb;
    public Rigidbody2D Rb     
    {
        get
        {
            if (rb == null) rb = GetComponent<Rigidbody2D>();
            return rb;
        }
    }
    
    private BuildObj _buildObj;
    // protected BuildObj BuildObj => GetComponent<BuildObj>();
    protected BuildObj BuildObj { get { _buildObj ??= GetComponent<BuildObj>(); return _buildObj; } }
    public void TransportItem_Constraint(uint netId) //Server
    {
        StartCoroutine(AllClientReadyChecker_Co(
            () => Rpc_InitSync(default,Vector2.zero,true),
            () => Rpc_Constraint(netId, SyncDirection.ServerToClient)
        ));
    }


    public void TransportItem_DropItem()
    {
        Rpc_DropItem();
    }
    public override void ShowEButton()
    {
        if(!MapEditor.Instance._onMapTransition_Complete)
        {
            HideEButton();
        }
        else
        {
            base.ShowEButton();    
        }
    }
    private void Transport_Drop()
    {
        Rb.gravityScale = _gravityScale;
        Col.enabled = true;

    }

    [ClientRpc]
    public void Rpc_Constraint(uint netId, SyncDirection direction)
    {
        Transport_Init(netId);
        ChangeSyncDirection(direction);

    }
    [ClientRpc]
    public void Rpc_DropItem()
    {
        BuildObj.canRespawn = true;
        ChangeSyncDirection(SyncDirection.ClientToServer);
        Transport_Drop();
    }

    // [ClientRpc]
    public void ChangeSyncDirection(SyncDirection direction)
    {
        var net_rb = GetComponent<NetworkRigidbodyUnreliable2D>();
        switch (direction)
        {
            case SyncDirection.ServerToClient:
                net_rb.syncDirection = direction;
                Rb.simulated = false;
                Rb.velocity = Vector2.zero;
                break;
            case SyncDirection.ClientToServer:
            default:
                net_rb.syncDirection = direction;
                Rb.simulated = true;
                break;
        }
    }
    [ReadOnly]
    public float defaultGravity;

    private void Transport_Init(uint netId)
    {
        if (NetworkClient.spawned.TryGetValue(netId, out NetworkIdentity identity))
        {
            var drone = identity.GetComponent<Drone_MultiPurpose>();
            Col.enabled = false;
            transform.position = drone.itemPlacementPosition.position;
            BuildObj.isTransportItem = true;
        }

    }

    IEnumerator AllClientReadyChecker_Co(params Action[] actions)
    {
        while (true)
        {
            int connectionClinetAmount = NetworkServer.connections.Count;
            int num = 0;
            foreach (var conn in NetworkServer.connections.Values)
            {
                if (conn.isReady) num++;
            }

            if (connectionClinetAmount == num) break;

            yield return null;
        }

        if(actions != null)
        {
            foreach (var action in actions)
            {
                action?.Invoke();
            }
        }
        // action?.Invoke();

    }
    #endregion
    #region Encapsulate Item
    private EncapsulationField field;
    public EncapsulationField EncapsulationField
    {
        get
        {
            field ??= GetComponent<EncapsulationField>();
            return field;
        }
    }

    [ClientRpc]
    public void Rpc_UnCapsuling() // Call only Server
    {
        if (EncapsulationField.isCapsuling)
            EncapsulationField.UnCapsuling();
    }

    [ClientRpc]
    public void Rpc_Capsuling() //only use Reset
    {
        _isDestroyed = true;
        EncapsulationField.Capsuling(BuildObj.ObjectData.position);
    }
    #endregion
    #region ---------------------------------------------Init Sync
    public bool onSync;

    // [Command(requiresAuthority = false)]
    // private void Cmd_OnChangeCanRespawn()
    // {
    //     Rpc_OnChangeCanRespawn();
    // }
    [ClientRpc]
    public void Rpc_OnChangeCanRespawn()
    {
        BuildObj.canRespawn = !BuildObj.canRespawn;
    }
    [Server]
    public void Server_InitSync()
    {
        StartCoroutine(AllClientCheckCo(() =>
        {
            Rpc_InitSync(BuildObj.ObjectData, transform.position, BuildObj.isTransportItem);
            Rb.AddForce(Vector2.up, ForceMode2D.Force);
        }));

    }

    private IEnumerator AllClientCheckCo(Action action)
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

    public ObjectData data;

    [ClientRpc]
    private void Rpc_InitSync(ObjectData data, Vector2 position, bool isTransportItem)
    {
        if (onSync) return;

        BuildObj.ObjectData = data;
        transform.position = position;
        transform.rotation = data.quaternion;
        this.data = data;

        BuildObj.DissolveInitSetting();
        
        if (isTransportItem)
        {
            defaultGravity = Rb.gravityScale;
            Col.enabled = false;
            BuildObj.canRespawn = false;
        }
        //0603 EnCapsulationField
        if (data.onEncapsulationItem)
        {
            EncapsulationField.onEncapsulationItem = true;
            EncapsulationField.activeRequirAmount = data.activeRequireAmount;
            EncapsulationField.indicator = data.indicator;

            if (!EncapsulationField.isCapsuling)
            {
                _isDestroyed = true;
                //StartCoroutine(DelayCapsuling(data.position));
                EncapsulationField.Capsuling(data.position);

            }
        }
        //0603 EnCapsulationField
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


    #region  RESET
    public void Reset_Interacable()
    {
        Col.enabled = true;
        Rb.gravityScale = _gravityScale;
        
        BuildObj.canRespawn = true;
        if (NetworkServer.active)
            CmdChnageDestroyState(false);
    }
    // public void CmdChangeDestroyState_False()
    // {
    //     CmdChnageDestroyState(false);
    // }
    #endregion

    #region  Indicator


    [ClientRpc]
    public virtual void Rpc_ApplyActive_Sync_var1(int curActiveAmount) //server
    {
        EncapsulationField.indicator_1.SetApplyActive_EncapsulationField(curActiveAmount);
    }

    [ClientRpc]
    public virtual void Rpc_ApplyActive_Sync_var2(int inc, uint id)
    {
        EncapsulationField.indicator_2.SetApplyActive_EncapsulationField(inc, id);
    }
    #endregion

    #region  Indicator_2 Path Chacking
    [Server]
    public void Server_Indicator_2_Path_Chacking(uint targetID)
    {
        StartCoroutine(AllClientCheckCo(() => Rpc_Indicator_2_Path_Chacking(targetID)));
    }
    [ClientRpc]
    public void Rpc_Indicator_2_Path_Chacking(uint targetID)
    {
        // Debug.Log("[3] Encapsulation Indicator 2 Path Chack->Net");
        // EncapsulationField.indicator_2.PathChacking(targetID);
        StartCoroutine(Wait_Path_Chacking(targetID));
    }


    private IEnumerator Wait_Path_Chacking(uint targetID)
    {
        yield return new WaitUntil(() => EncapsulationField.indicator_2 != null);
        Debug.Log("[3] Encapsulation Indicator 2 Path Chack->Net");
        EncapsulationField.indicator_2.PathChacking(targetID);
    }
    #endregion


    #region  Clean
    public virtual void Clean()
    {
        if(Accessor != null && Accessor.TryGetComponent(out PlayerSM sm))
        {
            sm.Reset();
        }

        onSync = false;
    }
    #endregion
}

