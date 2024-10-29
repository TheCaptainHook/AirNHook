using System.Collections;
using Mirror;
using System.Collections.Generic;
using UnityEngine;

public class ToggleButtonObject : ButtonEntity,IInteractable
{
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
    private UI_Base _AorD_Btn;

    private void Awake(){
        animator = GetComponent<Animator>();
    }


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
