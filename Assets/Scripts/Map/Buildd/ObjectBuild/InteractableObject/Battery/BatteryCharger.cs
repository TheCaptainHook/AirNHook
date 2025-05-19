using System.Collections;
using UnityEngine;
using Mirror;
public class BatteryCharger : BuildObj
{
    private UI_Base _E_Btn;
    [SerializeField] float _BtnOffset;

    private Battery battery;

    private Coroutine chargeCorotine;


    public override void SetData<T>(T data)
    {
        base.SetData(data);
        if(Application.isPlaying )
        {
            B_Net.onSync = true;
            B_Net.Server_SetInit();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent(out HookSM component))
        {
            Transform grabItem = component.GetGrabbedItem();
            if (grabItem != null)
            {
                if (grabItem.TryGetComponent(out Battery battery))
                {
                    B_Net.Cmd_ShowE(component.gameObject,true); //Show Button

                    //battery.batteryCharger = this;
                    if(NetworkServer.active)
                    battery.Net_SetBatteryCharger(gameObject); //Set BatteryCharger
                }
            }
          
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.TryGetComponent(out HookSM component))
        {

            Transform grabItem = component.GetGrabbedItem();
            if (grabItem != null)
            {
                if (grabItem.TryGetComponent(out Battery battery))
                {
                    B_Net.Cmd_ShowE(component.gameObject, false);//Hide

                    if (NetworkServer.active)
                        battery.Net_SetBatteryCharger(null); // remove batterycharger
                }

            }
        }
    }

    public void RemoveSocket()
    {
        if (chargeCorotine != null)
        {
            StopCoroutine(chargeCorotine);
            chargeCorotine = null;
        }

        if (battery == null) return;

        battery.RemoveSocket();
        battery = null;
  
    }
  

    //public void Charge(Battery battery)
    //{
    //    if(this.battery != null)
    //    {
    //        RemoveSocket();
    //    }

    //    this.battery = battery;
    //    chargeCorotine = StartCoroutine(ChargeCo());
    //}

    //IEnumerator ChargeCo()
    //{
    //    while (battery.BatteryCapacity < 100)
    //    {
    //        battery.BatteryCapacity =1;
    //        yield return new WaitForSeconds(0.1f);
    //    }
    //    chargeCorotine = null;
    //    RemoveSocket();
    //}

    #region --------------------------------------------------------------------------------Network
    private BatteryCharger_Net B_Net => GetComponent<BatteryCharger_Net>();

    public void SetBattery(GameObject battery)
    {
        B_Net.Cmd_SetBattery(battery);
    }
    #endregion


    #region UI
    public void ShowE()
    {
        _E_Btn = Managers.UI.ShowUI<UI_ShowEButton>();
        _E_Btn.transform.position = transform.position + (transform.up * _BtnOffset);
    }

    public void HideE()
    {
        _E_Btn = null;
        Managers.UI.HideUI<UI_ShowEButton>();
    }
    #endregion

}
