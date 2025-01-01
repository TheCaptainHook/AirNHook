using System.Collections;
using Mirror;
using UnityEngine;
using System;

public class ToggleButtonObject : ButtonEntity,IInteractable,IPowerConsumer
{
    [CustomHeader("Toggle")]
    [ReadOnly]
    public bool hasPower;
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

    #region  Power
    public void PowerOn(){hasPower = true;}
    public void PowerOff(){hasPower = false;}
    public Vector2 GetPowerLineConnectionPoint(){
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
            return (T)(object)new ButtonObjectStruct(id, GetTargetPositions(), transform.position, transform.localScale, chargeRequired);
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
                chargeRequired = buttonData.chargeRequired;
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
        animator.SetBool(OnPressed,onActive);
        PrograssButtonActivatedObject(onActive);
        yield return new WaitForSeconds(0.5f);
        onPrograss = false;
        
    }
    protected override IEnumerator Co_Deactivated()
    {
        onPrograss = true;
        onActive = false;
        
        animator.SetBool(OnPressed,onActive);
        PrograssButtonActivatedObject(onActive);

        yield return new WaitForSeconds(0.5f);
        onPrograss = false;
    }


    #region  Interacte
    public void Interaction(Transform accessor = null){
       if (!NetworkServer.active || !NetworkClient.isConnected)
            return;
        if (chargeRequired)
        {
            if (!hasPower)
            {
                return;
            }
        }

        if(onActive){
            Deactivated();
        }else{
            Activation();
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
        _E_Btn.transform.position =  transform.position + (transform.up * _BtnOffset);
    }
    
    public void HideEButton(){
        _E_Btn = null;
        Managers.UI.HideUI<UI_ShowEButton>();
    }
    #endregion
}
