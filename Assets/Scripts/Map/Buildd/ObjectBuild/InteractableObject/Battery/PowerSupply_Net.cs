using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class PowerSupply_Net : NetworkBehaviour
{
    private PowerSupply powerSupply;
    private PowerSupply PowerSupply 
    {
        get
        {
            if(powerSupply == null) powerSupply = GetComponent<PowerSupply>();
            return powerSupply;
        }
    }

    //[ReadOnly]
    //public List<GameObject> targets;
    //[ReadOnly]
    //public List<Vector2> targetPositions;


    #region Init
    public bool onSync;
    [Server]
    public void Server_SetInit()
    {
        StartCoroutine(AllClientCheckCo(() =>
        {
            Rpc_SetInit(PowerSupply.ButtonObjectData);

        }));
        
    }
    private IEnumerator AllClientCheckCo(Action action)
    {
        int connectClients = NetworkServer.connections.Count;
        bool onReady = false;
        while (!onReady)
        {
            int num = 0;
            foreach (var conn in NetworkServer.connections.Values)
            {
                if (conn.isReady) num++;
            }

            if (connectClients == num) onReady = true;
            yield return null;
        }

        action?.Invoke();

    }
    private ButtonObjectStruct data;
    private List<uint> targetNetIdList;
    [Server]
    public void Server_SetTargetNetId(List<uint> list)
    {
        Rpc_SetTargetNetId(list);
    }
    [ClientRpc]
    private void Rpc_SetTargetNetId(List<uint> list)
    {
        targetNetIdList = list;
        //PathFind,
    }

    [ClientRpc]
    private void Rpc_SetInit(ButtonObjectStruct data)
    {
        if (onSync) return;
        this.data = data;
        transform.position = data.position;
        transform.rotation = data.quaternion;
        onSync = true;
    }


    [Command(requiresAuthority = false)]
    private void Cmd_SetInit()
    {
        Server_SetInit();
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        StartCoroutine(Delay(() => { Cmd_SetInit(); }));
    }



    #endregion

    #region Insert Battery

    [SyncVar] public float consumption;
    [SyncVar] public GameObject battery;
  
    Coroutine supplyCoroutine;
    [Server]    // insert battery and Use.
    public void Server_SetBatter(GameObject battery)
    {
        if (this.battery !=null && !Compare(this.battery,battery))
        {
            //Deactivated,
            if(supplyCoroutine != null)
            {
                StopCoroutine(supplyCoroutine);
                supplyCoroutine = null;
            }
            if(this.battery.GetComponent<BatteryInteractable>().batteryCapacity > consumption) PowerSupply.Net_Deactivated();

            Rpc_OnSupplyEffect(false);
            
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


    private bool Check_BatteryCapacity()
    {
        float capacity = battery.GetComponent<BatteryInteractable>().batteryCapacity;
        return capacity > consumption;
    }

    [Command(requiresAuthority = false)]
    public void Cmd_SetBattery(GameObject battery)
    {
        Server_SetBatter(battery);
    }


    //----------------------------------------Refectoring 0714
    /**
    먼저 드로우 코루틴, 이레이져 코루틴 먼저 만들기.
    1. Supply
        -> Rpc_OnSupply(bool onOff)
            ->if onOff -> DrawLineCoroutine ->다그려지면 -> if(isServer) Net_Activation,supplyCoroutine = StartCoroutine(SupplyCo());
            
        
    **/
    private void Supply()//only server
    {
        Rpc_OnSupplyEffect(true);
        PowerSupply.Net_Activation();
        supplyCoroutine = StartCoroutine(SupplyCo());
    }

    

    IEnumerator SupplyCo() //only server
    {
        BatteryInteractable battery = this.battery.GetComponent<BatteryInteractable>();

        while(battery.batteryCapacity >0)
        {
            battery.Server_SetBatteryCapacity(-consumption);
            yield return new WaitForSeconds(1);
        }
        supplyCoroutine = null;

        PowerSupply.Net_Deactivated();
        Rpc_OnSupplyEffect(false);

    }


    //----------------------------------------Refectoring 0714
    #endregion





    #region  Effect
    [ClientRpc]
    private void Rpc_OnSupplyEffect(bool onOff)
    {
        // PowerSupply.LineOn(onOff);
    }

    #endregion




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

   

    IEnumerator Delay(Action action)
    {
        yield return new WaitForSeconds(1);
        //yield return new WaitForSeconds(1f);
        //yield return null;
        action?.Invoke();
    }


    private bool Compare(GameObject a, GameObject b)
    {
        if (a == null || b == null) return false;

        NetworkIdentity aN = a.GetComponent<NetworkIdentity>();
        NetworkIdentity bN = b.GetComponent<NetworkIdentity>();

        return aN.netId == bN.netId;
    }

}
