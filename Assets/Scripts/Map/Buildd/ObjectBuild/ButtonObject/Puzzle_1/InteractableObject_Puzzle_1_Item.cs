using System.Collections;
using System.Collections.Generic;
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
        CmdChangeSortingLayer(false);

        _rigidbody.velocity = Vector3.zero;
        _rigidbody.simulated = false;

    }
}
