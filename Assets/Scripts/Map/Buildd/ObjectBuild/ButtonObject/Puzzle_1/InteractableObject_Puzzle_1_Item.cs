
using Mirror;

using UnityEngine;

public class InteractableObject_Puzzle_1_Item : InteractableObject
{

    Puzzle_1_Item item;

    protected override void Awake()
    {
        base.Awake();
        item = GetComponent<Puzzle_1_Item>();
    }

    public override void Release()
    {
        if(item.GetPossibleInsertSocket()){ 
            Puzzle_Item_Release();
            //item.InsertSocket();
            Cmd_InserSocket();
        }else{
            _rigidbody.simulated = true;
            base.Release();
        }

    }

    private void Puzzle_Item_Release(){
        _isFixed = false;
        _isGrab = false;
        _canInteract = true;
        ChangeState(false);
        // ShowEButton();

        _fixedPoint = null;
        _rigidbody.constraints = _originRot;
        _sortingGroup.sortingLayerID = _originSortingLayerID;
        CmdChangeSortingLayer(false);
        CmdResetVelocity();
        CmdSetTransform(item.GetPartsPosition());

    }
//-------------------------------------------------------------------------Sync 1/30
    [SyncVar(hook = nameof(OnPartsChange))] 
    public GameObject parts;


    [Server]
    public void SetParts(GameObject parts){
        this.parts = parts;
    }
    public void OnPartsChange(GameObject old,GameObject newVal)
    {
        if(newVal == null)
        {
            item.UnPossibleInsertSocket();
        }else{
            Puzzle_1_Parts parts = newVal.GetComponent<Puzzle_1_Parts>();
            item.PossibleInsertSocket(parts);
        }
    }

    [Command(requiresAuthority = false)]
    public void HandleSetParts(GameObject parts){
        SetParts(parts);
    }

//-------------------------------------------------------------------------Sync 1/30
    [Command(requiresAuthority = false)]
    private void Cmd_InserSocket()
    {
        Rpc_InserSocket();
    }
    [ClientRpc]
    private void Rpc_InserSocket()
    {
        item.InsertSocket();
    }




    [Command(requiresAuthority = false)]
    public void CmdResetVelocity()
    {
        RpcResetVelocity();
    }
    [ClientRpc]
    private void RpcResetVelocity()
    {
        _rigidbody.gravityScale = 0;
        _rigidbody.velocity = Vector2.zero;
        _rigidbody.angularVelocity = 0;
    }
    [Command(requiresAuthority =false)]
    public void CmdSetTransform(Vector2 position)
    {
        RpcSetTransfrom(position);
    }
    [ClientRpc]
    private void RpcSetTransfrom(Vector2 position)
    {
        transform.position = position;
    }
}
