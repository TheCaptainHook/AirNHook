using System.Collections;
using UnityEngine;
using System;


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


    private ToggleButton_Net toggleButton_Net;
    private ToggleButton_Net ToggleButton_Net
    {
        get
        {
            if(toggleButton_Net == null) toggleButton_Net = GetComponent<ToggleButton_Net>();
            return toggleButton_Net;
        }
    }
#region IPowerConsumer
    public bool hasPower
    {
        get { return ToggleButton_Net.hasPower; }
        set { ToggleButton_Net.Cmd_SetHasPower(value); }
    }
    public void PowerOn()
    {
        Debug.Log("Power");
        //hasPower = true;
        ToggleButton_Net.Cmd_SetHasPower(true);
    }
    public void PowerOff()
    {
        //hasPower = false;
        ToggleButton_Net.Cmd_SetHasPower(false);
        Debug.Log("Power Off");
        //Deactivated();
        ToggleButton_Net.Cmd_CallDeactivated();
    }
    public Vector2 GetPowerLineConnectionPoint(){
        return transform.position;
    }
    public Vector2 GetTransformPosition(){
        return transform.position;
    }
#endregion
    
    
    private void Awake(){
        animator = GetComponent<Animator>();
    }

 #region Get,Set
    public override T GetData<T>()
    {
        if (typeof(T) == typeof(ButtonObjectStruct))
        {
            return (T)(object)new ButtonObjectStruct(
                id, 
            GetTargetPositions(), 
            GetLightPositions(),
            transform.position, 
            transform.localScale, 
            chargeRequired);
        }

        return default(T);
    }
    public override void SetData<T>(T data)
    {
        try
        {
            if (typeof(T) == typeof(ButtonObjectStruct))
            {
                ButtonObjectStruct buttonData = (ButtonObjectStruct)(object)data;
                ButtonObjectData = buttonData;
                FindTargetObject();

                if(buttonData.lightPositions.Count >0) FindLightObject();
                
                chargeRequired = buttonData.chargeRequired;
                if(Application.isPlaying)
                ToggleButton_Net.Server_SetChargeRequired(buttonData.chargeRequired);
            }

        }
        catch (Exception ex)
        {
            Debug.Log($"name : {gameObject.name},{ex}");
        }
    }
#endregion


 
    protected override void Activation()
    {
        if (onPrograss || onActive) return;
        StartCoroutine(Co_Activation());
    }
    protected override void Deactivated()
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
       
    public void Net_Activation( )
    {
        Activation();
    }
    public void Net_Deactivated()
    { 
        Deactivated();
    }

#endregion
#region Client
    public void SetActive(bool val)
    {
        animator.SetBool(OnPressed,val);
    }
#endregion

#region  Interacte
    public void Interaction(Transform accessor = null){
        if (ToggleButton_Net.chargeRequired)
        {
            if (!ToggleButton_Net.hasPower)
            {
                return;
            }
        }

        if (onPrograss) return;

        if(onActive){
            //Deactivated();
            ToggleButton_Net.HandleSetState(false);
        }else{
            //Activation();
            ToggleButton_Net.HandleSetState(true);
        }
    }

    public bool CanInteract(){
        return true;
    }

    public void Interacting(bool value)
    {
        return;
    }

    public ObjectTypeEnum GetObjectType(){
        return _objectType;
    }

    public void ShowEButton(){
        if (ToggleButton_Net.chargeRequired)
        {
            if (!ToggleButton_Net.hasPower) return;
        }
        _E_Btn = Managers.UI.ShowUI<UI_ShowEButton>();
        _E_Btn.transform.position =  transform.position + (transform.up * _BtnOffset);
    }
    
    public void HideEButton(){
        _E_Btn = null;
        Managers.UI.HideUI<UI_ShowEButton>();
    }
#endregion
}
