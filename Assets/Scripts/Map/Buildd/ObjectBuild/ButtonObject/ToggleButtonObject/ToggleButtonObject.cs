using System.Collections;
using UnityEngine;
using System;
using System.Collections.Generic;
using Mirror;


public class ToggleButtonObject : ButtonEntity,IInteractable,IPowerConsumer
{
    [CustomHeader("Toggle")]
   
#region Components
    private Animator animator;
#endregion

#region Animation
    readonly int OnPressed = Animator.StringToHash("OnPressed");
#endregion

    [Header("Interacte")]
    [SerializeField] float _BtnOffset;
    public ObjectTypeEnum _objectType = ObjectTypeEnum.Interaction;
    private UI_Base _E_Btn;
    [SerializeField] private GameObject energyIcon;

    // private ToggleButton_Net toggleButton_Net;
    // private ToggleButton_Net ToggleButton_Net
    // {
    //     get
    //     {
    //         if(toggleButton_Net == null) toggleButton_Net = GetComponent<ToggleButton_Net>();
    //         return toggleButton_Net;
    //     }
    // }
#region IPowerConsumer
    // public bool hasPower
    // {
    //     get { return ToggleButton_Net.hasPower > 0 ? true : false; }
    //     set { ToggleButton_Net.Cmd_SetHasPower(value); }
    // }
    // public int GetConsumption()
    // {
    //     return 1;
    // }
    // public void PowerOn()
    // {
    //     Debug.Log("Power");
    //     //hasPower = true;
    //     ToggleButton_Net.Cmd_SetHasPower(true);
    // }
    // public void PowerOff()
    // {
    //     //hasPower = false;
    //     ToggleButton_Net.Cmd_SetHasPower(false);
    //     Debug.Log("Power Off");
    //     //Deactivated();
    //     //ToggleButton_Net.Cmd_CallDeactivated();
    //     ToggleButton_Net.HandleSetState(false);
    // }
    // public Vector2 GetPowerLineConnectionPoint(){
    //     return transform.position;
    // }
    // public Vector2 GetTransformPosition(){
    //     return transform.position;
    // }
    #endregion


    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    #region Get,Set
    #region  Clean
    public override void Clean()
    {
        // ToggleButton_Net.onSync = false;
        // onActive = false;
        if(NetworkServer.active)
        {
            Net.Server_Clean();
        }
    }
    #endregion

    // public override void SetData<T>(T data)
    // {
    //     base.SetData(data);
    //     chargeRequired = ButtonObjectData.chargeRequired;

    //     if (Application.isPlaying)
    //     {
    //         ToggleButton_Net.onSync = true;
    //         ToggleButton_Net.Server_SetInit();
    //         ToggleButton_Net.Server_SetChargeRequired(chargeRequired);
    //     }   
    // }
#endregion


 
    public override void Activation()
    {
        if (onPrograss || onActive) return;
        StartCoroutine(Co_Activation());
    }
    public override void Deactivated()
    {
        if (onPrograss || !onActive) return;
        StartCoroutine(Co_Deactivated());
    }

   

    protected override IEnumerator Co_Activation()
    {
        onPrograss = true;
        onActive = true;
        //network
        animator.SetBool(OnPressed,onActive);
        //
        PrograssButtonActivatedObject(onActive);
        yield return new WaitForSeconds(0.8f);
        onPrograss = false;
        
    }
    protected override IEnumerator Co_Deactivated()
    {
        onPrograss = true;
        onActive = false;
        ////network
        animator.SetBool(OnPressed,onActive);
        //
        PrograssButtonActivatedObject(onActive);

        yield return new WaitForSeconds(0.8f);
        onPrograss = false;
    }

#region  NetWork
       
    // public void Net_Activation( )
    // {
    //     Activation();
    // }
    // public void Net_Deactivated()
    // { 
    //     Deactivated();
    // }

#endregion
#region Client
    // public void SetActive(bool val)
    // {
    //     animator.SetBool(OnPressed,val);
    // }
#endregion

#region  Interacte
    public void Interaction(Transform accessor = null){
        if (Net._chargeRequired)
        {
            if (!hasPower) //if (ToggleButton_Net.hasPower == 0)
            {
                return;
            }
        }
        if (onPrograss) return;

        if(onActive){
            //Deactivated();
            // ToggleButton_Net.HandleSetState(false);
            Net.Cmd_SetState(false);
        }else{
            //Activation();
            // ToggleButton_Net.HandleSetState(true);
            Net.Cmd_SetState(true);
        }
    }

    public bool CanInteract(){
        return true;
    }

    public bool Interacting(bool value, GameObject player)
    {
        return true;
    }

    public ObjectTypeEnum GetObjectType(){
        return _objectType;
    }

    public void ShowEButton(){
        if (Net._chargeRequired)
        {
            if (!hasPower) return;
        }
        _E_Btn = Managers.UI.ShowUI<UI_ShowEButton>();
        _E_Btn.transform.position =  transform.position + (transform.up * _BtnOffset);
    }
    
    public void HideEButton(){
        _E_Btn = null;
        Managers.UI.HideUI<UI_ShowEButton>();
    }
    #endregion

    #region
    public void OnSoundEventButton()
    {
        Managers.Sound.PlaySound3D(GlobalText.BUTTON_PRESS_SOUND_1,transform.position,0.55f);
    }
    public void OnSoundEventLever()
    {
        Managers.Sound.PlaySound3D(GlobalText.BUTTON_LEVER_SOUND_1, transform.position, 0.55f);
    }
    #endregion
}
