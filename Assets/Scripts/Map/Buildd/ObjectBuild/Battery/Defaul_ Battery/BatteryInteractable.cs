using System;
using System.Collections;
using Mirror;
using UnityEngine;
using Random = UnityEngine.Random;


public class BatteryInteractable : TransportItemEntity
{
   
    //#region Transport Item
    //private Collider2D Col => GetComponent<Collider2D>();
    //private Rigidbody2D Rb => GetComponent<Rigidbody2D>();
    //private BuildObj BuildObj => GetComponent<BuildObj>();
    // public void TransportItem_Constraint(uint netId)
    // {
    //    StartCoroutine(AllClientReadyChecker_Co(()=>{Rpc_Transport_Init(netId);}));  
    // }
    //public void TransportItem_DropItem()
    //{
    //    Rpc_Transport_Drop();
    //}
    //[ClientRpc]
    //private void Rpc_Transport_Drop()
    //{
    //    Rb.gravityScale = 1;
    //    Col.enabled = true;
    //}

    //[ClientRpc]
    //private void Rpc_Transport_Init(uint netId)
    //{
    //    if(NetworkClient.spawned.TryGetValue(netId,out NetworkIdentity identity))
    //    {
    //        var drone = identity.GetComponent<Drone_MultiPurpose>();

    //        Rb.gravityScale = 0;
    //        Col.enabled = false;
    //        transform.position = drone.itemPlacementPosition.position;
    //        BuildObj.isTransformItem = true;
    //    }
    //}
    
    //IEnumerator AllClientReadyChecker_Co(Action action)
    //{
    //    while(true)
    //    {
    //        int connectionClinetAmount = NetworkServer.connections.Count;
    //        int num = 0;
    //        foreach(var conn in NetworkServer.connections.Values)
    //        {
    //            if(conn.isReady) num++;
    //        }

    //        if(connectionClinetAmount == num) break;

    //        yield return null;
    //    }
    //    action?.Invoke();

    //}
    //#endregion

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
    [SyncVar] public GameObject powerSupply;
    public Vector3 orgPosition;

    #region ---------------------------------------------Init Sync
    public bool onSync;
    [Server]
    public void Server_InitSync()
    {
        Rpc_InitSync(battery.ObjectData); 
    }

    [ClientRpc]
    private void Rpc_InitSync(ObjectData data)
    {
        Debug.Log("Server, Rpc, battery");
        if(onSync) return;
        transform.position = data.position;
        transform.rotation = data.quaternion;
        orgPosition = data.position;
        onSync = true;
    }

    #endregion

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

    [Server]    //  Set PowerSupply
    public void Server_SetPowerSupply(GameObject powerSupply)
    {
        this.powerSupply = powerSupply;
    }
    [Command(requiresAuthority = false)]
    public void Cmd_SetPowerSupply(GameObject powerSupply)
    {
        Server_SetPowerSupply(powerSupply);
    }

    // [Server]
    // public void Server_SetOrgPot(Vector3 pot)
    // {
    //     orgPosition = pot;
    // }
    
    #endregion

    //-----------------------------------------------------------------------Interact
    Battery battery;
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
        Rpc_Release();
        // transform.position = batteryCharger.transform.position;
        transform.position = releasePosition;
    }

    [Command(requiresAuthority = false)]
    private void Cmd_Release(Vector3 releasePosition)
    {
        Server_Release(releasePosition);
    }

    [ClientRpc]
    private void Rpc_Release()
    {
        _isFixed = false;
        _isGrab = false;
        _canInteract = true;
        ChangeState(false);

        _fixedPoint = null;
        _rigidbody.bodyType = _originType;
        _rigidbody.constraints = _originRot;
        _sortingGroup.sortingLayerID = _originSortingLayerID;

        CmdChangeSortingLayer(false);


        _rigidbody.velocity = Vector2.zero;
        _rigidbody.angularVelocity = 0;
        _rigidbody.gravityScale = 0;

        Col.enabled = false;

    }

    [Command(requiresAuthority = false)]
    public void Cmd_Recover()
    {
        Server_SetBatteryCharger(null);
        Server_SetPowerSupply(null);

        RemoveEffect();

        Rpc_Recover();
    }
    [ClientRpc]
    private void Rpc_Recover()
    {
        Col.enabled = true;
        _rigidbody.gravityScale = 1;
       
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
