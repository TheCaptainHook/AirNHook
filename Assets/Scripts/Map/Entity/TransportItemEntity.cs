using System;
using System.Collections;
using Mirror;
using UnityEngine;

[RequireComponent(typeof(EncapsulationField))]
public class TransportItemEntity : InteractableObject, ITransportItem
{
    #region Transport Item
    protected Collider2D Col => GetComponent<Collider2D>();
    protected Rigidbody2D Rb => GetComponent<Rigidbody2D>();
    protected BuildObj BuildObj => GetComponent<BuildObj>();
    public void TransportItem_Constraint(uint netId) //Server
    {
        StartCoroutine(AllClientReadyChecker_Co(() =>
        {
            Rpc_Constraint(netId, SyncDirection.ServerToClient);
        }));

    }
    public void TransportItem_DropItem()
    {
        Rpc_DropItem();
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

    IEnumerator AllClientReadyChecker_Co(Action action)
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
        action?.Invoke();

    }
    #endregion
    #region Encapsulate ITem
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
        EncapsulationField.Capsuling(BuildObj.ObjectData.position);
    }
    #endregion
    #region ---------------------------------------------Init Sync
    public bool onSync;

    [Command(requiresAuthority = false)]
    private void Cmd_OnChangeCanRespawn()
    {
        Rpc_OnChangeCanRespawn();
    }
    [ClientRpc]
    private void Rpc_OnChangeCanRespawn()
    {
        BuildObj.canRespawn = !BuildObj.canRespawn;
    }
    [Server]
    public void Server_InitSync()
    {
        StartCoroutine(AllClientCheckCo(() =>
        {
            Rpc_InitSync(BuildObj.ObjectData, transform.position, BuildObj.isTransportItem);

            if (EncapsulationField.onEncapsulationItem)
            {
                if (!EncapsulationField.isCapsuling)
                {

                    Rpc_Capsuling(BuildObj.position);

                    //TEST
                    // StartCoroutine(DelayCapsuling(BuildObj.position));
                    //TEST
                }

                return;
            }

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
    [ClientRpc]
    private void Rpc_Capsuling(Vector2 startPot)
    {
        StartCoroutine(DelayCapsuling(startPot));
    }
    private IEnumerator DelayCapsuling(Vector2 startPot)
    {
        yield return new WaitForFixedUpdate();
        EncapsulationField.Capsuling(startPot);
    }
    [ClientRpc]
    private void Rpc_InitSync(ObjectData data, Vector2 position, bool isTransportItem)
    {
        if (onSync) return;
        BuildObj.ObjectData = data;
        transform.position = position;
        transform.rotation = data.quaternion;
        if (isTransportItem)
        {
            defaultGravity = Rb.gravityScale;
            Col.enabled = false;
        }
        //0603 EnCapsulationField

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







    #region  Interactable Object Component
    
    #endregion
}
