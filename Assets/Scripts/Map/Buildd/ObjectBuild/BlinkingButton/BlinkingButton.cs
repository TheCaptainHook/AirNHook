using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlinkingButton : BuildObj,IInteractable
{
    private BlinkingButton_Net _net;
    private BlinkingButton_Net Net { get { _net ??= GetComponent<BlinkingButton_Net>(); return _net; } }


#region Clean
    public override void Clean()
    {
        
    }
#endregion


 #region Interactable
    public ObjectTypeEnum _objectType = ObjectTypeEnum.Interaction;
    private UI_Base _E_Btn;
    [SerializeField] float _BtnOffset;

    public void Interaction(Transform accessor = null)
    {
        
    }
    public bool CanInteract()
    {
        return true;
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
        _E_Btn.transform.position = transform.position + (transform.up * _BtnOffset);
    }
    public void HideEButton()
    {
        _E_Btn = null;
        Managers.UI.HideUI<UI_ShowEButton>();
    }
#endregion
}
