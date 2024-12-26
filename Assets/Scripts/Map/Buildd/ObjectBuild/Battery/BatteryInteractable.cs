using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatteryInteractable : InteractableObject
{
    Battery battery;
    protected override void Awake()
    {
        base.Awake();
        battery = GetComponent<Battery>();
    }


    public override void Release()
    {
        if(battery.batteryCharger != null)
        {
            BatteryRelease();
            battery.InsertSocket();
        }
        else
        {
            base.Release();
        }
    }


    private void BatteryRelease()
    {
        _isFixed = false;
        _isGrab = false;
        _canInteract = true;
        ChangeState(false);

        _fixedPoint = null;
        _rigidbody.constraints = _originRot;
        _sortingGroup.sortingLayerID = _originSortingLayerID;
        _rigidbody.gravityScale = 0;

        CmdChangeSortingLayer(false);
        CmdResetVelocity();
        CmdSetTransform(battery.batteryCharger.transform.position);
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
    [Command(requiresAuthority = false)]
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
