using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PowerSupply : ButtonEntity,IInteractable
{
    [CustomHeader("Power Supply")]
    [SerializeField] Transform socketPosition;
    private List<IPowerConsumer> targetObjectList;
    private Battery battery;
    private Coroutine batteryUseCoroutine;

    [Space(20)]
    [Header("Interacte")]
    public ObjectTypeEnum _objectType = ObjectTypeEnum.Interaction;
    [SerializeField] float _BtnOffset;
    private UI_Base _E_Btn;

    //SetData -> FindTargetObject -> Add List Target Object
    //Activation -> PrograssButtonActivatedObject

   

    #region  Get,Set
    public override T GetData<T>()
    {
        return base.GetData<T>();
    }
    public override void SetData<T>(T data)
    {
        base.SetData(data);
    }
    public Vector2 GetSocketPosition(){
        return socketPosition.position;
    }
    #endregion


    #region Active,Deactive
    protected override void Activation()
    {
        base.Activation();
    }
    protected override void Deactivated()
    {
        base.Deactivated();
    }

     protected override void PrograssButtonActivatedObject(bool onActivate)
    {
        
    }
    #endregion

    #region Find Target

    public override void FindTargetObject()
    {
        
    }
    #endregion


    #region  Main
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent(out HookSM component))
        {
            Transform grabItem = component.GetGrabbedItem();
            if (grabItem != null)
            {
                if (grabItem.TryGetComponent(out Battery battery))
                {
                    ShowEButton();
                    battery.powerSupply = this;
                    this.battery = battery;
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
                    battery.powerSupply = null;
                    this.battery = null;
                }

            }
        }
    }
    public void UseBattery(){
        
    }
    // private IEnumerator UseBatteryCo(){

    // }

    public void InsertSocket(Battery battery){
        if(this.battery != null){
            StopAllCoroutines();
            batteryUseCoroutine = null;
            this.battery.RemoveSocket();
        }
        this.battery = battery;
        TogglePowerSupply(true);
        UseBattery();
    }

    private void RemoveSocket(){
        if(battery){
            //Effect
            //Stop Use to Battery
            StopCoroutine(batteryUseCoroutine);
            batteryUseCoroutine = null;
            TogglePowerSupply(false);
            //Remove Socket
            battery.RemoveSocket();
            battery = null;
        }
    }

    private void TogglePowerSupply(bool toggle){
        foreach(var item in targetObjectList){
            if(toggle) item.PowerOn();
            else item.PowerOff();
        }
    }
    #endregion


#region  Interacable
     public void Interaction(Transform accessor = null){
       if (!NetworkServer.active || !NetworkClient.isConnected)
            return;
            if(battery){
                RemoveSocket();
                HideEButton();
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
         _E_Btn = Managers.UI.ShowUI<UI_ShowEButton>();
        _E_Btn.transform.position = transform.position + (transform.up * _BtnOffset);
    }
    
    public void HideEButton(){
        _E_Btn = null;
        Managers.UI.HideUI<UI_ShowEButton>();
    }

#endregion
}
