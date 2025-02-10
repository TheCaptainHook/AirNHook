using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class BatteryCharger_Net : NetworkBehaviour
{
    BatteryCharger BatteryCharger => GetComponent<BatteryCharger>();

    [ReadOnly]
    [SyncVar(hook = nameof(OnChangeBattery))]
    public GameObject battery;



    [Server]
    private void Server_SetBattery(GameObject battery)
    {
        this.battery = battery;
    }

    [Command(requiresAuthority = false)]
    public void Cmd_SetBattery(GameObject battery)
    {
        Server_SetBattery(battery);
    }


    private void OnChangeBattery(GameObject old,GameObject newVal)
    {
        if (old != null)
        {
            //StopCharge
            StopAllCoroutines();
            //Remove battery
            if (old.TryGetComponent(out BatteryInteractable component))
            {
                component.Cmd_Recover();
            }
        }

        if (newVal != null)
        {
            Charge();
        }
    
    }

    //public void RemoveSocket()
    //{
    //    //if (chargeCorotine != null)
    //    //{
    //    //    StopCoroutine(chargeCorotine);
    //    //    chargeCorotine = null;
    //    //}

    //    if (battery == null) return;

    //    battery.RemoveSocket();
    //    battery = null;

    //}
    public void Charge()
    {
        //if (this.battery != null)
        //{
        //    RemoveSocket();
        //}

        //this.battery = battery;
      StartCoroutine(ChargeCo());
    }

    IEnumerator ChargeCo()
    {
        BatteryInteractable batteryNet = battery.GetComponent<BatteryInteractable>();

        while (batteryNet.batteryCapacity < 100)
        {
            //battery.BatteryCapacity = 1;
            batteryNet.Cmd_SetBatteryCapacity(1);
            yield return new WaitForSeconds(0.1f);
        }

        Cmd_SetBattery(null);
    }


    #region UI
    [Command(requiresAuthority = false)]
    public void Cmd_ShowE(GameObject player,bool onOff)
    {
        if(player.TryGetComponent(out NetworkIdentity component))
        {
            TRpc_ShowE(component.connectionToClient, onOff);
        }
    }

    [TargetRpc]
    private void TRpc_ShowE(NetworkConnection conn, bool onOff)
    {
        if (onOff) BatteryCharger.ShowE();
        else BatteryCharger.HideE();

    }
    #endregion
}


