
using Mirror;

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



    //Refectoring

    public override void Release()
    {
        if (Main.parts != null)
        {
            //Connect Parts Cmd
            base.Release();

            var id = Main.parts.TryGetComponent(out NetworkIdentity identity) ? identity.netId : 9999;
            if(id != 9999)
            {
                Cmd_ConnectParts(id);
            }
        
        }
        else
        {
            base.Release();
        }
    }


    public bool onConnect;

    [Command(requiresAuthority = false)]
    private void Cmd_ConnectParts(uint netId)
    {
        Rpc_ConnectAndDisConnectParts(netId);    
    }
    [ClientRpc]
    private void Rpc_ConnectAndDisConnectParts(uint netId)
    {
        var item = NetworkClient.spawned.TryGetValue(netId, out NetworkIdentity identity) ? identity.gameObject : null;
        if (item == null) return;

        var parts = item.TryGetComponent(out Puzzle_1_Parts value);
        if (!parts) return;

        value.Connect(Main);

    }

    //-------------------------------------------------------------------------Sync 1/30
   
    [SyncVar]
    public GameObject parts;
   


}
