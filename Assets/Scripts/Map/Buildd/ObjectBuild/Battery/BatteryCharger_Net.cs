using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class BatteryCharger_Net : NetworkBehaviour
{
    BatteryCharger BatteryCharger => GetComponent<BatteryCharger>();

    [ReadOnly]
    [SyncVar]
    public GameObject battery;


    Coroutine charge;
    [Server]
    private void Server_SetBattery(GameObject newBattery)
    {

        if(battery != null && !Compare(battery,newBattery))
        {
            StopCoroutine(charge);
            //Remove battery
            if (battery.TryGetComponent(out BatteryInteractable component))
            {
                component.Cmd_Recover();
            }
        }

        
        if(newBattery != null && !Compare(battery,newBattery))
        {
            battery = newBattery;
            Charge();
        }


    }
    private bool Compare(GameObject a, GameObject b)
    {
        if (a == null || b == null) return false;

        NetworkIdentity aN = a.GetComponent<NetworkIdentity>();
        NetworkIdentity bN = b.GetComponent<NetworkIdentity>();

        return aN.netId == bN.netId;
    }

    [Command(requiresAuthority = false)]
    public void Cmd_SetBattery(GameObject battery)
    {
        Server_SetBattery(battery);
    }


    public void Charge()
    {
     charge = StartCoroutine(ChargeCo());
    }

    IEnumerator ChargeCo()
    {
        Battery battery = this.battery.GetComponent<Battery>();

        while (battery.BatteryCapacity() < 100)
        {
            //battery.BatteryCapacity = 1;
            battery.Net_SetBatteryCapacity(1);
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


