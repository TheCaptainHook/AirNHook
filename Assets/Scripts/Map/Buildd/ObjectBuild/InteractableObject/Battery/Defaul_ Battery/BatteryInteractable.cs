using System;
using System.Collections;
using Mirror;
using UnityEngine;
using Random = UnityEngine.Random;


public class BatteryInteractable : TransportItemEntity,IRemoveSocketEffect
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
  
    #endregion

    #endregion


  
    public void Recover()
    {
        //------Interactable recover
        _stoppedTime = 0f;
        _isFixed = false;
        _isGrab = false;
        _canInteract = true;
        _canGrab = true;
   
        //-----etc recover
        _sortingGroup.sortingLayerID = _originSortingLayerID;
        BuildObj.canRespawn = true;
    }

    private float horizontalVariation = 1f;

    public void RemoveSocketEffect(bool val = false)
    {
        float xForce = Random.Range(-horizontalVariation, horizontalVariation);
        _rigidbody.AddForce(new Vector2(xForce, 4f), ForceMode2D.Impulse);
    }

}
