
using Mirror;

using UnityEngine;

public class InteractableObject_Puzzle_1_Item : InteractableObject
{
    [SyncVar] public Vector3 orgPosition;

    Puzzle_1_Item main;
    Puzzle_1_Item Main { get { main ??= GetComponent<Puzzle_1_Item>(); return main; } }


    Collider2D Col => GetComponent<Collider2D>();

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
    //[SyncVar(hook = nameof(OnPartsChange))]
    [SyncVar]
    public GameObject parts;
    private Vector2 PartsTransform => (parts)? parts.transform.position : Vector2.zero;

    //[SyncVar(hook = nameof(OnChangeOnSocket))] 
    //public bool onInsert;

    //public bool possibleInsertSocket;

    //[Command(requiresAuthority = false)]
    //private void Cmd_OnChangeCanRespawn()
    //{
    //    Rpc_OnChangeCanRespawn();
    //}

    //[ClientRpc]
    //private void Rpc_OnChangeCanRespawn()
    //{
    //    Main.canRespawn = !Main.canRespawn;
    //}


    //[Server]
    //public void SetParts(GameObject parts){
    //    this.parts = parts;

    //    if(parts != null) possibleInsertSocket = true;
    //    else possibleInsertSocket = false;

    //}
    //[Server]
    //public void SetInsert(bool val)
    //{
    //    onInsert = val;
    //}



    //public void OnChangeOnSocket(bool old,bool newVal)
    //{
    //    if(newVal)
    //    {
    //        //삽입
    //        _isFixed = false;
    //        _isGrab = false;
    //        _canInteract = true;
    //        ChangeState(false);

    //        //_fixedPoint = null;
    //        _rigidbody.constraints = _originRot;
    //        _sortingGroup.sortingLayerID = _originSortingLayerID;

    //        _rigidbody.gravityScale = 0;
    //        _rigidbody.velocity = Vector2.zero;
    //        _rigidbody.angularVelocity = 0;

    //        transform.position = PartsTransform;
    //        //_sortingGroup.sortingLayerID = _originSortingLayerID;

    //        Col.enabled = false;
    //        Main.canRespawn = false;
    //    }
    //    else
    //    {
    //        //해제
    //        Col.enabled = true;
    //        _rigidbody.gravityScale = 1;
    //        Main.RemoveSocketEffect();
    //        Main.canRespawn = true;
    //    }
    //}



    //[Command(requiresAuthority = false)]
    //public void HandleSetParts(GameObject parts){
    //    SetParts(parts);
    //}
    //[Command(requiresAuthority = false)]
    //public void Cmd_SetOnInsert(bool val)
    //{
    //    SetInsert(val);
    //}

//-------------------------------------------------------------------------Sync 1/30
    //[Command(requiresAuthority = false)]
    //private void Cmd_InserSocket()
    //{
    //    Cmd_SetOnInsert(true);
    //    Rpc_InserSocket();
    //}

    //[ClientRpc]
    //private void Rpc_InserSocket()
    //{
    //    Main.InsertSocket();
    //}


    [Server]
    public void Server_SetOrgPositon(Vector3 positon)
    {
        orgPosition = positon;
        Rpc_SetOrgPosition(positon);
    }
    [ClientRpc]
    private void Rpc_SetOrgPosition(Vector3 positon)
    {
        Main.position = positon;
    }


}
