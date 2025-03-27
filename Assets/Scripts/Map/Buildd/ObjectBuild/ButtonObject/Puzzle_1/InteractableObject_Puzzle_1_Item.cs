
using Mirror;

using UnityEngine;
using UnityEngine.UIElements;

public class InteractableObject_Puzzle_1_Item : InteractableObject
{
    [SyncVar] public Vector3 orgPosition;

    Puzzle_1_Item Main => GetComponent<Puzzle_1_Item>();

    Collider2D Col => GetComponent<Collider2D>();

    protected override void Awake()
    {
        base.Awake();
    }

    public override void Release()
    {
        //if(item.GetPossibleInsertSocket()){
        if (possibleInsertSocket)
        {
            //Puzzle_Item_Release();

            //onInsert = false;
            //item.InsertSocket();
            Cmd_InserSocket();
        }else{
            _rigidbody.simulated = true;
            Cmd_OnChangeCanRespawn();
            base.Release();
        }

    }
    protected override void Grab()
    {
        base.Grab();
        //Main.canRespawn = false;
        Cmd_OnChangeCanRespawn();
    }

    //private void Puzzle_Item_Release(){
    //    _isFixed = false;
    //    _isGrab = false;
    //    _canInteract = true;
    //    ChangeState(false);
    //    // ShowEButton();

    //    _fixedPoint = null;
    //    _rigidbody.constraints = _originRot;
    //    _sortingGroup.sortingLayerID = _originSortingLayerID;
    //    CmdChangeSortingLayer(false);
    //    CmdResetVelocity();
    //    CmdSetTransform(item.GetPartsPosition());

    //}
    //-------------------------------------------------------------------------Sync 1/30
    //[SyncVar(hook = nameof(OnPartsChange))]
    [SyncVar]
    public GameObject parts;
    private Vector2 PartsTransform => (parts)? parts.transform.position : Vector2.zero;

    [SyncVar(hook = nameof(OnChangeOnSocket))] 
    public bool onInsert;
    [SyncVar] public bool possibleInsertSocket;

    [Command(requiresAuthority = false)]
    private void Cmd_OnChangeCanRespawn()
    {
        Rpc_OnChangeCanRespawn();
    }
    [ClientRpc]
    private void Rpc_OnChangeCanRespawn()
    {
        Main.canRespawn = !Main.canRespawn;
    }


    [Server]
    public void SetParts(GameObject parts){
        this.parts = parts;

        if(parts != null) possibleInsertSocket = true;
        else possibleInsertSocket = false;

    }
    [Server]
    public void SetInsert(bool val)
    {
        onInsert = val;
    }

    //public void OnPartsChange(GameObject old,GameObject newVal)
    //{
    //    if(newVal == null)
    //    {
    //        item.UnPossibleInsertSocket();
    //    }else{
    //        Puzzle_1_Parts parts = newVal.GetComponent<Puzzle_1_Parts>();
    //        item.PossibleInsertSocket(parts);
    //    }
    //}

    public void OnChangeOnSocket(bool old,bool newVal)
    {
        if(newVal)
        {
            //삽입
            _isFixed = false;
            _isGrab = false;
            _canInteract = true;
            ChangeState(false);

            _fixedPoint = null;
            _rigidbody.constraints = _originRot;
            _sortingGroup.sortingLayerID = _originSortingLayerID;

            _rigidbody.gravityScale = 0;
            _rigidbody.velocity = Vector2.zero;
            _rigidbody.angularVelocity = 0;

            transform.position = PartsTransform;
            //_sortingGroup.sortingLayerID = _originSortingLayerID;

            Col.enabled = false;
            Main.canRespawn = false;
        }
        else
        {
            //해제
            Col.enabled = true;
            _rigidbody.gravityScale = 1;
            Main.RemoveSocketEffect();
            Main.canRespawn = true;
        }
    }



    [Command(requiresAuthority = false)]
    public void HandleSetParts(GameObject parts){
        SetParts(parts);
    }
    [Command(requiresAuthority = false)]
    public void Cmd_SetOnInsert(bool val)
    {
        SetInsert(val);
    }

//-------------------------------------------------------------------------Sync 1/30
    [Command(requiresAuthority = false)]
    private void Cmd_InserSocket()
    {
        Cmd_SetOnInsert(true);
        Rpc_InserSocket();
    }

    [ClientRpc]
    private void Rpc_InserSocket()
    {
        Main.InsertSocket();
    }


    [Server]
    public void Server_SetOrgPositon(Vector3 positon)
    {
        orgPosition = positon;
        Main.position = positon;
    }

    //[Command(requiresAuthority = false)]
    //public void CmdResetVelocity()
    //{
    //    RpcResetVelocity();
    //}
    //[ClientRpc]
    //private void RpcResetVelocity()
    //{
    //    _rigidbody.gravityScale = 0;
    //    _rigidbody.velocity = Vector2.zero;
    //    _rigidbody.angularVelocity = 0;
    //}
    //[Command(requiresAuthority =false)]
    //public void CmdSetTransform(Vector2 position)
    //{
    //    RpcSetTransfrom(position);
    //}
    //[ClientRpc]
    //private void RpcSetTransfrom(Vector2 position)
    //{
    //    transform.position = position;
    //}
}
