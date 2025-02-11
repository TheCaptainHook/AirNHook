using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;


[Serializable]
public struct SupplyTargetStruct
{
    public List<GameObject> targets;
    public List<Vector2> targetPositions;
    public SupplyTargetStruct(List<GameObject> targets,List<Vector2> targetPositions)
    {
        this.targets = targets;
        this.targetPositions = targetPositions;
    }
}
public class PowerSupply_Net : NetworkBehaviour
{
    private IPowerConsumer IPowerConsumer => GetComponent<IPowerConsumer>();
    private PowerSupply powerSupply;
    private PowerSupply PowerSupply 
    {
        get
        {
            if(powerSupply == null) powerSupply = GetComponent<PowerSupply>();
            return powerSupply;
        }
    }

    [SyncVar] public SupplyTargetStruct targets;
    [SyncVar] public float consumption;
    [SyncVar] public GameObject battery;


    [Server]    // Power Supply Candidates
    public void Server_SetTargets(List<GameObject> targets,List<Vector2> positions)
    {
        this.targets = new SupplyTargetStruct(targets,positions);
        consumption = targets.Count;
    }

    
    Coroutine supplyCoroutine;
    [Server]    // insert battery and Use.
    public void Server_SetBatter(GameObject battery)
    {
        if(this.battery !=null && !Compare(this.battery,battery))
        {
            //Deactivated,
            if(supplyCoroutine != null)
            {
                StopCoroutine(supplyCoroutine);
                supplyCoroutine = null;
            }
            
            PowerSupply.Net_Deactivated();
            OnSupplyEffect(false);
            
            this.battery.GetComponent<BatteryInteractable>().Cmd_Recover();
            this.battery = null;
        }

        if(battery == null || Compare(this.battery,battery)) return;

        this.battery = battery;

        // 1. Check battery capacity and Compare consumption    
        if(Check_BatteryCapacity())
        {
            //Use Battery
            Supply();


        }else
        {
            //Cant use battery.
            Debug.Log("The battery doesn’t have much energy left");
        }
        


    }
    private bool Compare(GameObject a, GameObject b)
    {
        if (a == null || b == null) return false;

        NetworkIdentity aN = a.GetComponent<NetworkIdentity>();
        NetworkIdentity bN = b.GetComponent<NetworkIdentity>();

        return aN.netId == bN.netId;
    }

    private bool Check_BatteryCapacity()
    {
        float capacity = battery.GetComponent<BatteryInteractable>().batteryCapacity;
        return capacity >= consumption;
    }

    [Command(requiresAuthority = false)]
    public void Cmd_SetBattery(GameObject battery)
    {
        Server_SetBatter(battery);
    }



    public override void OnStartClient()
    {
        base.OnStartClient();
        StartCoroutine(Delay(()=>{PowerSupply.CreateLine(targets.targetPositions);}));
        //Create Supply line
        // PowerSupply.CreateLine(targets.targetPositions);

    }

    IEnumerator Delay(Action action)
    {
        yield return null;
        action?.Invoke();
    } 
    

    private void Supply()
    {
        OnSupplyEffect(true);
        PowerSupply.Net_Activation();
        supplyCoroutine = StartCoroutine(SupplyCo());
    }

    [ClientRpc]
    private void OnSupplyEffect(bool onOff)
    {
        PowerSupply.LineOn(onOff);
    }

    IEnumerator SupplyCo()
    {
        BatteryInteractable battery = this.battery.GetComponent<BatteryInteractable>();

        while(battery.batteryCapacity >0)
        {
            battery.Server_SetBatteryCapacity(-consumption);
            yield return new WaitForSeconds(1);
        }
        supplyCoroutine = null;

        PowerSupply.Net_Deactivated();
        OnSupplyEffect(false);

    }

    #region  UI
    [Command(requiresAuthority = false)]
    public void Cmd_ShowE(GameObject player, bool onOff)
    {
        if(player.TryGetComponent(out NetworkIdentity component))
        {
            TRpc_ShowE(component.connectionToClient,onOff);
        }
    }
    [TargetRpc]
    private void TRpc_ShowE(NetworkConnection conn,bool onOff)
    {
        if(onOff) PowerSupply.ShowE();
        else PowerSupply.HideE();
    }
    #endregion
}
