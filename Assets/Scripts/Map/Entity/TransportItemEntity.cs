using System;
using System.Collections;
using Mirror;
using UnityEngine;

public class TransportItemEntity : InteractableObject, ITransportItem
{
    #region Transport Item
    protected Collider2D Col => GetComponent<Collider2D>();
    protected Rigidbody2D Rb => GetComponent<Rigidbody2D>();
    protected BuildObj BuildObj => GetComponent<BuildObj>();
    public void TransportItem_Constraint(uint netId)
    {
        StartCoroutine(AllClientReadyChecker_Co(() => 
        {
            Rpc_Transport_Init(netId);
            Rpc_ChangeSyncDirection(SyncDirection.ServerToClient);
        }));
    }
    public void TransportItem_DropItem()
    {
        Rpc_ChangeSyncDirection(SyncDirection.ClientToServer);
        Rpc_Transport_Drop();
    }
    [ClientRpc]
    private void Rpc_Transport_Drop()
    {
        _gravityScale = defaultGravity;
        Rb.gravityScale = defaultGravity;
        Col.enabled = true;

    }

    [ClientRpc]
    public void Rpc_ChangeSyncDirection(SyncDirection direction)
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
    [ClientRpc]
    private void Rpc_Transport_Init(uint netId)
    {
        if (NetworkClient.spawned.TryGetValue(netId, out NetworkIdentity identity))
        {
            var drone = identity.GetComponent<Drone_MultiPurpose>();
            defaultGravity = Rb.gravityScale;
            Rb.gravityScale = 0;
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

    #region ---------------------------------------------Init Sync
    public bool onSync;

    //protected override void Grab()
    //{
    //    base.Grab();
    //    //BuildObj.canRespawn = false;
    //    //Cmd_OnChangeCanRespawn();
    //}
    //public override void Release()
    //{
    //    base.Release();
    //    //BuildObj.canRespawn = true;
    //    //Cmd_OnChangeCanRespawn();
    //}



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
        Rpc_InitSync(BuildObj.ObjectData,transform.position,BuildObj.isTransportItem);
        Rb.AddForce(Vector2.up, ForceMode2D.Force);
    }

    [ClientRpc]
    private void Rpc_InitSync(ObjectData data, Vector2 position,bool isTransportItem)
    {
        if (onSync) return;
        BuildObj.ObjectData = data;
        //BuildObj.position = data.position;
        //BuildObj.isTransportItem = isTransportItem;
        transform.position = position;
        transform.rotation = data.quaternion;
        //BuildObj.position = data.position;
        if(isTransportItem)
        {
            Col.enabled = false;
        }
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
}
