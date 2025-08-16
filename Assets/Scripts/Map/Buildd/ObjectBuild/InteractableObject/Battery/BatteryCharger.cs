using Mirror;
using System.Collections;
using UnityEngine;

public class BatteryCharger : BuildObj, IInteractable
{
    private UI_Base _E_Btn;
    [SerializeField] float _BtnOffset;

    // private BatteryInteractable battery;
    protected ObjectTypeEnum _objectType = ObjectTypeEnum.Mount;
    // public bool _onSocket = false;
    public BatteryCharger_Net net;
    private Vector2 _topOfObj = new Vector2(0, 1.2f);
    // private Coroutine chargeCorotine;

    void Awake()
    {
        net = GetComponent<BatteryCharger_Net>();
    }
    public override void SetData<T>(T data)
    {
        base.SetData(data);
        if(Application.isPlaying )
        {
            B_Net.onSync = true;
            B_Net.Server_SetInit();
        }
    }

    #region Interaction
    //    private void OnTriggerEnter2D(Collider2D collision)
    //     {
    //         if (collision.TryGetComponent(out Battery battery))
    //         {
    //             var interactable = battery.TryGetComponent(out InteractableObject component) ? component : null;
    //             if (interactable != null && interactable._isGrab)
    //             {
    //                 // B_Net.Cmd_ShowE(collision.gameObject, true);
    //                 battery.Net_SetBatteryCharger(gameObject);
    //                 return;
    //             }
    //             //Air Inhale object insert 0804
    //             if (ChackBatteryVelocity(battery) && NetworkServer.active)
    //             {
    //                 //Insert Battery
    //                 SetBattery(battery.gameObject);
    //                 //Insert Battery
    //                 return;
    //             }
    //             //Air Inhale object insert 0804
    //         }
    //     }

    //private void OnTriggerExit2D(Collider2D collider)
    //{
    //
    //    if (collider.TryGetComponent(out Battery battery))
    //    {
    //        B_Net.Cmd_ShowE(collider.gameObject, false);
    //        battery.Net_SetBatteryCharger(null);
    //    }
    //}

    public void Interaction(Transform accessor)
    {
        if (net.battery != null) return;

        if (accessor != null)
        {
            if (accessor.TryGetComponent(out NetworkIdentity identity))
            {
                net.Cmd_SetBattery(identity.netId);
                HideEButton();
            }
        }
      
    }
    private float condition_Insert_BatteryVelocityValue = 5;
    //--------Air Insert Object Logic 0804
    private bool ChackBatteryVelocity(Battery battery)
    {
        Debug.Log(battery._rb.velocity.magnitude);
        return battery._rb.velocity.magnitude >= condition_Insert_BatteryVelocityValue;
    }

    //--------Air Insert Object Logic 0804

    public bool CanInteract()
    {

        return Hook_IsInteractionValid();
    }
    private bool Hook_IsInteractionValid()
    {
        var hook = Managers.Game.Player.TryGetComponent(out HookSM component) ? component : null;
        if (hook == null || net.battery != null || hook.GetGrabbedItem() == null) return false;

        if (hook.GetGrabbedItem().TryGetComponent<Battery>(out var battery))
            return true;
        else
            return false;
        
    }
    public bool Interacting(bool value, GameObject player)
    {
        return true;
    }

    public ObjectTypeEnum GetObjectType()
    {
        return _objectType;
    }

    public void ShowEButton()
    {
        _E_Btn = Managers.UI.ShowUI<UI_ShowEButton>();
        _E_Btn.transform.position = transform.position + (Vector3)_topOfObj;
    }

    public void HideEButton()
    {
        _E_Btn = null;
        Managers.UI.HideUI<UI_ShowEButton>();
    }
    #endregion


    #region --------------------------------------------------------------------------------Network
    private BatteryCharger_Net B_Net => GetComponent<BatteryCharger_Net>();

    public void SetBattery(GameObject battery) //Server
    {
        //B_Net.Cmd_SetBattery(battery);
        B_Net.Server_SetBattery(battery);
       
    }
    #endregion


}
