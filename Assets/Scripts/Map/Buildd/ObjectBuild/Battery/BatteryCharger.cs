using System.Collections;
using UnityEngine;

public class BatteryCharger : BuildObj
{
    private UI_Base _E_Btn;
    [SerializeField] float _BtnOffset;

    private Battery battery;

    private Coroutine chargeCorotine;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent(out HookSM component))
        {
            Transform grabItem = component.GetGrabbedItem();
            if (grabItem != null)
            {
                if (grabItem.TryGetComponent(out Battery battery))
                {
                    ShowBtn();
                    battery.batteryCharger = this;
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
                    HideEButton();
                    battery.batteryCharger = null;
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
  

    public void Charge(Battery battery)
    {
        if(this.battery != null)
        {
            RemoveSocket();
        }

        this.battery = battery;
        chargeCorotine = StartCoroutine(ChargeCo());
    }

    IEnumerator ChargeCo()
    {
        while (battery.BatteryCapacity < 100)
        {
            battery.BatteryCapacity =1;
            yield return new WaitForSeconds(0.1f);
        }
        chargeCorotine = null;
        RemoveSocket();
    }

    #region UI
    private void ShowBtn()
    {
        _E_Btn = Managers.UI.ShowUI<UI_ShowEButton>();
        _E_Btn.transform.position = transform.position + (transform.up * _BtnOffset);
    }

    public void HideEButton()
    {
        _E_Btn = null;
        Managers.UI.HideUI<UI_ShowEButton>();
    }
    #endregion

}
