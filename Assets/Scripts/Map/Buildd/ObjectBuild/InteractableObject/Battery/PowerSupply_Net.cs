using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;
using Unity.VisualScripting;

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
         // 새 배터리가 없거나, 기존 배터리와 같다면 return
        if (this.battery != null && !Compare(this.battery, battery))
        {
            if (supplyCoroutine != null)
            {
                StopCoroutine(supplyCoroutine);
                supplyCoroutine = null;
            }

            PowerSupply.Net_Deactivated();
            Rpc_OnSupplyEffect(false);

            this.battery.GetComponent<BatteryInteractable>().Cmd_Recover();
            this.battery = null;
        }

        if (battery == null) return;


        this.battery = battery;

        // 1. Check battery capacity and Compare consumption    
        if (Check_BatteryCapacity())
        {
            //Use Battery
            Supply();
            Rpc_OnSupplyEffect(true);
            PowerSupply.Net_Activation();
        }
        else
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

    #region Draw,Eraser
    private List<PowerSupply_DrawLineUtility> pdu_List;
    [ReadOnly]
    public List<uint> targetObjectNetIdList;
    private Transform lineContainer;
    [SerializeField] Material lineMat;

    [ClientRpc]
    public void Rpc_SetTargetObject(List<uint> list)
    {
        targetObjectNetIdList = list;
        lineContainer = new GameObject("Line_Container").transform;
        lineContainer.SetParent(transform);
        pdu_List = new();

        for (int i = 0; i < list.Count; i++)
        {
            var item = new GameObject("Line").transform;
            item.SetParent(lineContainer);
            var line = item.AddComponent<PowerSupply_DrawLineUtility>();
            item.AddComponent<PathFinder>();

            pdu_List.Add(line);
            line.Setting(data.position, list[i],lineMat);
        }
    }
    
    private void DrawLine()
    {
        for (int i = 0; i < pdu_List.Count; i++)
        {
            var item = pdu_List[i];
            item.DrawOn();
        }
    }
    private void EraseLine()
    {
        for (int i = 0; i < pdu_List.Count; i++)
        {
            pdu_List[i].EraseOn_TargetToMain();
        }
    }
  
    #endregion
    private void Supply()//only server
    {
        if (supplyCoroutine != null)
        {
            StopCoroutine(supplyCoroutine);
        } 
       supplyCoroutine = StartCoroutine(SupplyCo());
    }
    

    IEnumerator SupplyCo() //only server
    {
        Debug.Log("Supply co");
        BatteryInteractable battery = this.battery.GetComponent<BatteryInteractable>();

        while(battery.batteryCapacity >0)
        {
            battery.Server_SetBatteryCapacity(-consumption);
            Debug.Log($"{battery.batteryCapacity}");
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
        if (onOff) DrawLine();
        else EraseLine();
        
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
