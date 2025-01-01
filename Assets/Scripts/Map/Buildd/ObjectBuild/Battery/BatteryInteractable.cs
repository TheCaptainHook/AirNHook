using Mirror;

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
            BatteryRelease(battery.batteryCharger.transform.position);
            battery.InsertChargerSocket();
        }
        else if(battery.powerSupply != null)
        {
            BatteryRelease(battery.powerSupply.GetSocketPosition());
            battery.InsertPowerSocket();
        }
        else
        {
            base.Release();
        }
    }


    private void BatteryRelease(Vector2 releasePosition)
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
        CmdSetTransform(releasePosition);
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
