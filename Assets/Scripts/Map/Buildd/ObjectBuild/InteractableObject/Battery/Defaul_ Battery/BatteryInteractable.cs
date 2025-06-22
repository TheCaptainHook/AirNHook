using System;
using System.Collections;
using Mirror;
using UnityEngine;
using Random = UnityEngine.Random;


public class BatteryInteractable : TransportItemEntity
{

    #region -------------------------------------------------------------------------------------Sync
    private float maxCapacity = 100;
    private readonly int CAPACITY = Animator.StringToHash("Capacity");
    private Animator animator;
    private Animator Animator
    {
        get
        {
            if(animator == null) animator = GetComponent<Animator>();  
            return animator;
        }
    }

    [Space(20)]
    [Header("---------------Sync")]
    [ReadOnly]
     public GameObject batteryCharger;

    [ReadOnly]
    [SyncVar] 
    public float batteryCapacity;

    [ReadOnly]
    public GameObject powerSupply;



    [Server]    // use Battery capacity
    public void Server_SetBatteryCapacity(float val)
    {
        batteryCapacity += val;
        if(batteryCapacity > maxCapacity) batteryCapacity = maxCapacity;
        if(batteryCapacity <0) batteryCapacity = 0;

        Animator.SetFloat(CAPACITY, batteryCapacity / maxCapacity);
    }
    [Command(requiresAuthority = false)]
    public void Cmd_SetBatteryCapacity(float val)
    {
        Server_SetBatteryCapacity(val);
    }
    [Server]
    public void Server_ResetBattery()
    {
        batteryCapacity = 0;
        Animator.SetFloat(CAPACITY, 0);
    }

    #region ---------------------------------------------------------------------Battery Charger
    [Server]    //  Set battery charger
    private void Server_SetBatteryCharger(uint netId)
    {
        Rpc_SetBatteryCharger(netId);

    }
    [Command(requiresAuthority = false)]
    public void Cmd_SetBatteryCharger(uint netId)
    {
        Server_SetBatteryCharger(netId);
    }
    [ClientRpc]
    private void Rpc_SetBatteryCharger(uint id)
    {
        if (id == 9999) this.batteryCharger = null;
        else
            this.batteryCharger = NetworkClient.spawned.TryGetValue(id, out NetworkIdentity identity) ? identity.gameObject : null;
    }
    #endregion
    #region ---------------------------------------------------------------------Power Supply
    [Server] //  Set PowerSupply
    private void Server_SetPowerSupply(uint id)
    {
        Rpc_SetPowerSupply(id);
    }

    [Command(requiresAuthority = false)]
    public void Cmd_SetPowerSupply(uint netId)
    {
        Server_SetPowerSupply(netId);
    }
    [ClientRpc]
    private void Rpc_SetPowerSupply(uint id)
    {
        if(id == 9999) this.powerSupply = null;
        else
            this.powerSupply = NetworkClient.spawned.TryGetValue(id, out NetworkIdentity identity) ? identity.gameObject : null;
    }
    #endregion

    #endregion

    //-----------------------------------------------------------------------Interact
    Battery battery;
    //BuildObj BuildObj => GetComponent<BuildObj>();
    protected override void Awake()
    {
        base.Awake();
        battery = GetComponent<Battery>();
    }


    public override void Release(GameObject accessor)
    {
        if (batteryCharger != null)
        {
            //BatteryRelease();
            Cmd_Release(batteryCharger.transform.position,false);

            //battery.InsertChargerSocket();
            Cmd_InsertChargerSocket(gameObject);
        }
        else if (powerSupply != null)
        {
        //    BatteryRelease(battery.powerSupply.GetSocketPosition());
            Cmd_Release(powerSupply.transform.position,true);

            // battery.InsertPowerSocket();
            Cmd_InsertPowerSupplySocket(gameObject);
        }
        else
        {
            base.Release(accessor); 
            Cmd_Reset();
        }
    }

    [Command(requiresAuthority = false)]
    private void Cmd_Reset()
    {
        Rpc_Reset();
    }
    [ClientRpc]
    private void Rpc_Reset()
    {
        if (batteryCharger != null)
        {
            batteryCharger = null;
        }
        if (powerSupply != null)
        {
            powerSupply = null;
        }
    }

    //[Server]
    //private void Server_Release(Vector3 releasePosition)
    //{
    //    Rpc_Release(releasePosition);
    //    // transform.position = batteryCharger.transform.position;
    //    //transform.position = releasePosition;
    //}

    [Command(requiresAuthority = false)]
    private void Cmd_Release(Vector3 releasePosition,bool isShowE)
    {
        // Rb.position = releasePosition;
        //Server_Release(releasePosition);
        Rpc_Release(releasePosition,isShowE);
    }

    [ClientRpc]
    private void Rpc_Release(Vector3 releasePosition,bool isShowE)
    {
        //--------------base Release(remove ShowEButton)
        _stoppedTime = 0f;
        _isFixed = false;
        _isGrab = false;
        _canInteract = true;
        _canGrab = true;
        CmdChangeFixedState(false);
        CmdChangeInteractState(true);
        CmdChangeGrabState(true);
        // if(isShowE) ShowEButton();
        
        
        _rigidbody.bodyType = _originType;

        _rigidbody.gravityScale = 0;
        _rigidbody.velocity = Vector2.zero;
        _rigidbody.angularVelocity = 0;

        _rigidbody.constraints = _originRot;
        _sortingGroup.sortingLayerID = _originSortingLayerID;
        CmdChangeSortingLayer(false);
        CmdRemovePermissionPlayer();
        //--------------base Release(remove ShowEButton)

        transform.position = releasePosition;

        Col.enabled = false;
        BuildObj.canRespawn = false;

    }


    [Command(requiresAuthority = false)]
    public void Cmd_Recover()
    {

        Rpc_Recover();
        

    }
    [ClientRpc]
    private void Rpc_Recover()
    {
        if (batteryCharger != null)
        {
            batteryCharger = null;
        }
        if (powerSupply != null)
        {
            powerSupply = null;
        }
        RemoveEffect();

        Col.enabled = true;
        _rigidbody.gravityScale = _gravityScale;
        BuildObj.canRespawn = true;


    }

    //-----------------------------------------------------------------------Interact

    //-----------------------------------------------------------------------Insert Charger Socket

 
    [Command(requiresAuthority = false)]
    public void Cmd_InsertChargerSocket(GameObject battery)
    {

        Rpc_InsertChargerSocket();

    }

    [ClientRpc]
    private void Rpc_InsertChargerSocket()
    {
        if (batteryCharger)
        {
            if (batteryCharger.TryGetComponent(out BatteryCharger component))
            {
                component.SetBattery(gameObject);
                BuildObj.canRespawn = false;
                Col.enabled = false;
            }

        }
    }

    //-----------------------------------------------------------------------Insert Charger Socket

    //-----------------------------------------------------------------------Insert PowerSupply Socket
  

    [Command(requiresAuthority = false)]
    public void Cmd_InsertPowerSupplySocket(GameObject battery)
    {

        Rpc_InsertPowerSupplySocket();

    }

    [ClientRpc]
    private void Rpc_InsertPowerSupplySocket()
    {
        if (powerSupply)
        {
            if (powerSupply.TryGetComponent(out PowerSupply component))
            {
                component.SetBattery(gameObject);
                BuildObj.canRespawn = false;
                Col.enabled = false;
            }

           

        }
    }
    //-----------------------------------------------------------------------Insert PowerSupply Socket

    private float horizontalVariation = 1f;
    private void RemoveEffect()
    {
        float xForce = Random.Range(-horizontalVariation, horizontalVariation);
        _rigidbody.AddForce(new Vector2(xForce, 4f), ForceMode2D.Impulse);
    }
}
