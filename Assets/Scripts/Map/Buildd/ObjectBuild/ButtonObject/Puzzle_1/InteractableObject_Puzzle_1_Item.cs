
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
            item.InsertSocket();
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
        _rigidbody.gravityScale = 0;
        CmdChangeSortingLayer(false);
        CmdResetVelocity();
        CmdSetTransform(item.GetPartsPosition());

    }

    [Command(requiresAuthority = false)]
    public void CmdResetVelocity()
    {
        RpcResetVelocity();
    }
    [ClientRpc]
    private void RpcResetVelocity()
    {
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
