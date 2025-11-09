
using Mirror;
using Org.BouncyCastle.Ocsp;
using System;
using System.Collections;
using UnityEngine;

public class InteractableObject_Puzzle_1_Item : InteractableObject
{
    [SyncVar] public Vector3 orgPosition;

    Puzzle_1_Item main;
    Puzzle_1_Item Main { get { main ??= GetComponent<Puzzle_1_Item>(); return main; } }


    //Collider2D Col => GetComponent<Collider2D>();

    protected override void Awake()
    {
        base.Awake();
    }

    public bool _onSync;
    
    [Server]
    public void Server_InitSync()
    {
        StartCoroutine(AllClientCheckCo(() =>
      {
          Rpc_InitSync();
      }));
      

    }
    [ClientRpc]
    private void Rpc_InitSync()
    {
        if (_onSync) return;
        Main.DissolveInitSetting();
        _onSync = true;
    }
    public override void OnStartClient()
    {
        base.OnStartClient();
        if (!_onSync) Cmd_InitSync();
    }
    [Command(requiresAuthority = false)]
    private void Cmd_InitSync()
    {
        Server_InitSync();
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



    //Refectoring

    public override void Release(GameObject accessor)
    {

        base.Release(accessor);
    }


}
