using Mirror;
using System.Collections;
using UnityEngine;

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
        if (collision.TryGetComponent(out Battery battery))
        {
            var interactable = battery.TryGetComponent(out InteractableObject component) ? component : null;
            if (interactable != null && interactable._isGrab)
            {
                B_Net.Cmd_ShowE(collision.gameObject, true);
                battery.Net_SetBatteryCharger(gameObject);
                return;
            }
            //Air Inhale object insert 0804
            if (ChackBatteryVelocity(battery) && NetworkServer.active)
            {
                //Insert Battery
                SetBattery(battery.gameObject);
                //Insert Battery
                return;
            }
            //Air Inhale object insert 0804
            
        }
    }
    private float condition_InsertBatteryChargerValue = 5;
    //--------Air Insert Object Logic 0804
    private bool ChackBatteryVelocity(Battery battery)
    {
        Debug.Log(battery._rb.velocity.magnitude);
        return battery._rb.velocity.magnitude >= condition_InsertBatteryChargerValue;
    }

    //--------Air Insert Object Logic 0804

    private void OnTriggerExit2D(Collider2D collider)
    {

        if (collider.TryGetComponent(out Battery battery))
        {
            B_Net.Cmd_ShowE(collider.gameObject, false);
            battery.Net_SetBatteryCharger(null);
        }
    }

  
    #region --------------------------------------------------------------------------------Network
    private BatteryCharger_Net B_Net => GetComponent<BatteryCharger_Net>();

    public void SetBattery(GameObject battery) //Server
    {
        //B_Net.Cmd_SetBattery(battery);
        B_Net.Server_SetBattery(battery);
       
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
        if(_E_Btn != null) Managers.UI.HideUI<UI_ShowEButton>();
        _E_Btn = null;
       
    }
    #endregion

}
