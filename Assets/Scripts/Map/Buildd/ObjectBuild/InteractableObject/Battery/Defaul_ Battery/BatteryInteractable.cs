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
    [SyncVar] public GameObject batteryCharger;

    [ReadOnly]
    [SyncVar] 
    public float batteryCapacity;

    [ReadOnly]
    public GameObject powerSupply;

    [Server]    //  Set battery charger
    private void Server_SetBatteryCharger(GameObject batteryCharger)
    {
        this.batteryCharger = batteryCharger;

    }
     [Command(requiresAuthority = false)]
    public void Cmd_SetBatteryCharger(GameObject batteryCharger)
    {
        Server_SetBatteryCharger(batteryCharger);
    }


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

    //[Server]    //  Set PowerSupply
    //public void Server_SetPowerSupply(GameObject powerSupply)
    //{
    //    this.powerSupply = powerSupply;
    //}
    //[Command(requiresAuthority = false)]
    //public void Cmd_SetPowerSupply(GameObject powerSupply)
    //{
    //    Server_SetPowerSupply(powerSupply);
    //}


    [Server] //  Set PowerSupply
    private void Server_SetPowerSupply(uint id)
    {
        Rpc_SetPowerSupply(id);
    }

    [Command(requiresAuthority = false)]
    public void Cmd_SetPowerSupply(uint netId)
    {
        Debug.Log("Cmd _ SetPowrSUpply");
        Rpc_SetPowerSupply(netId);
    }
    [ClientRpc]
    private void Rpc_SetPowerSupply(uint id)
    {
        if(id == 9999) this.powerSupply = null;
        else
            this.powerSupply = NetworkClient.spawned.TryGetValue(id, out NetworkIdentity identity) ? identity.gameObject : null;
    }

    #endregion

    //-----------------------------------------------------------------------Interact
    Battery battery;
    //BuildObj BuildObj => GetComponent<BuildObj>();
    protected override void Awake()
    {
        base.Awake();
        battery = GetComponent<Battery>();
    }


    public override void Release()
    {
        if (batteryCharger != null)
        {
            //BatteryRelease();
            Cmd_Release(batteryCharger.transform.position);

            //battery.InsertChargerSocket();
            Cmd_InsertChargerSocket(gameObject);
        }
        else if (powerSupply != null)
        {
        //    BatteryRelease(battery.powerSupply.GetSocketPosition());
            Cmd_Release(powerSupply.transform.position);

            // battery.InsertPowerSocket();
            Cmd_InsertPowerSupplySocket(gameObject);
        }
        else
        {
            base.Release();
        }
    }



    [Server]
    private void Server_Release(Vector3 releasePosition)
    {
        Rpc_Release(releasePosition);
        // transform.position = batteryCharger.transform.position;
        //transform.position = releasePosition;
    }

    [Command(requiresAuthority = false)]
    private void Cmd_Release(Vector3 releasePosition)
    {
        // Rb.position = releasePosition;
        Server_Release(releasePosition);
    }

    [ClientRpc]
    private void Rpc_Release(Vector3 releasePosition)
    {
        _isFixed = false;
        _isGrab = false;
        _canInteract = true;
        ChangeState(false);

        //_fixedPoint = null;
        _rigidbody.position = releasePosition;
        _rigidbody.bodyType = _originType;
        _rigidbody.constraints = _originRot;
        _sortingGroup.sortingLayerID = _originSortingLayerID;

        CmdChangeSortingLayer(false);


        _rigidbody.velocity = Vector2.zero;
        _rigidbody.angularVelocity = 0;
        _rigidbody.gravityScale = 0;

        Col.enabled = false;
        BuildObj.canRespawn = false;

    }

    [Command(requiresAuthority = false)]
    public void Cmd_Recover()
    {
        Server_SetBatteryCharger(null);
        Server_SetPowerSupply(9999);

        RemoveEffect();

        Rpc_Recover();
    }
    [ClientRpc]
    private void Rpc_Recover()
    {
        Col.enabled = true;
        _rigidbody.gravityScale = 1;
        BuildObj.canRespawn = true;


    }
    //-----------------------------------------------------------------------Interact

    //-----------------------------------------------------------------------Insert Charger Socket

 
    [Command(requiresAuthority = false)]
    public void Cmd_InsertChargerSocket(GameObject battery)
    {
        //Server_InsertChargeSocket(battery);

        if (batteryCharger.TryGetComponent(out BatteryCharger component))
        {
            component.SetBattery(battery);
        }

        Rpc_InsertChargerSocket();

    }

    [ClientRpc]
    private void Rpc_InsertChargerSocket()
    {
        if (batteryCharger)
        {
            BuildObj.canRespawn = false;
            Col.enabled = false;

        }
    }

    //-----------------------------------------------------------------------Insert Charger Socket

    //-----------------------------------------------------------------------Insert PowerSupply Socket
  

    [Command(requiresAuthority = false)]
    public void Cmd_InsertPowerSupplySocket(GameObject battery)
    {
        if (powerSupply.TryGetComponent(out PowerSupply component))
        {
            component.SetBattery(battery);
        }

        Rpc_InsertPowerSupplySocket();

    }

    [ClientRpc]
    private void Rpc_InsertPowerSupplySocket()
    {
        if (powerSupply)
        {
            BuildObj.canRespawn = false;
            Col.enabled = false;

        }
    }
    //-----------------------------------------------------------------------Insert PowerSupply Socket

    private float horizontalVariation = 1f;
    private void RemoveEffect()
    {
        float xForce = Random.Range(-horizontalVariation, horizontalVariation);
        _rigidbody.AddForce(new Vector2(xForce, 6f), ForceMode2D.Impulse);
    }
}
