using Mirror;

using UnityEngine;
using UnityEngine.UIElements;

public class BatteryInteractable : InteractableObject
{

    private Collider2D Col => GetComponent<Collider2D>();


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
    [SyncVar]
    public GameObject batteryCharger;
    [SyncVar(hook = nameof(OnChangeBatteryCapacity))] 
    public float batteryCapacity; // hook


    [Server]
    private void Server_SetBatteryCharger(GameObject batteryCharger)
    {
        this.batteryCharger = batteryCharger;
    }

    [Server]
    private void Server_SetBatteryCapacity(float val)
    {
        batteryCapacity += val;
        if(batteryCapacity > maxCapacity) batteryCapacity = maxCapacity;
    }


    [Command(requiresAuthority = false)]
    public void Cmd_SetBatteryCapacity(float val)
    {
        Server_SetBatteryCapacity(val);
    }

    [Command(requiresAuthority = false)]
    public void Cmd_SetBatteryCharger(GameObject batteryCharger)
    {
        Server_SetBatteryCharger(batteryCharger);
    }


    //---------------------------------Hook
    private void OnChangeBatteryCapacity(float old,float newVal)
    {
        Animator.SetFloat(CAPACITY, newVal/maxCapacity);
    }

    #endregion



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
            BatteryRelease(batteryCharger.transform.position);
            //battery.InsertChargerSocket();
            Cmd_InsertChargerSocket(gameObject);
        }
        //else if (battery.powerSupply != null)
        //{
        //    BatteryRelease(battery.powerSupply.GetSocketPosition());
        //    battery.InsertPowerSocket();
        //}
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

        //CmdResetVelocity();
        //CmdSetTransform(releasePosition);
        Cmd_Release(releasePosition);
    }


    [Command(requiresAuthority = false)]
    private void Cmd_Release(Vector2 releasePosition)
    {
        Rpc_Release(releasePosition);
    }

    [ClientRpc]
    private void Rpc_Release(Vector2 releasePosition)
    {
        _rigidbody.velocity = Vector2.zero;
        _rigidbody.angularVelocity = 0;

        transform.position = releasePosition;
    }

    [Command(requiresAuthority = false)]
    public void Cmd_Recover()
    {
        Server_SetBatteryCharger(null);
        Rpc_Recover();
    }
    [ClientRpc]
    private void Rpc_Recover()
    {
        Col.enabled = true;
        _rigidbody.gravityScale = 1;
        RemoveEffect();
    }

    //-----------------------------------------------------------------------Insert Charger Socket

    [Command(requiresAuthority = false)]
    public void Cmd_InsertChargerSocket(GameObject battery)
    {
        Rpc_InsertChargerSocket(battery);
     
    }
    [ClientRpc]
    private void Rpc_InsertChargerSocket(GameObject battery)
    {
        if (batteryCharger)
        {
            Col.enabled = false;

            //batterCharget -> SetBattery -> Charging
            //batteryCharger.Charge(this);
            if(batteryCharger.TryGetComponent(out BatteryCharger component))
            {
                component.SetBattery(battery);
            }
        }
    }

    //-----------------------------------------------------------------------Insert Charger Socket


    private float horizontalVariation = 1f;
    private void RemoveEffect()
    {
        float xForce = Random.Range(-horizontalVariation, horizontalVariation);
        _rigidbody.AddForce(new Vector2(xForce, 3f), ForceMode2D.Impulse);
    }
}
