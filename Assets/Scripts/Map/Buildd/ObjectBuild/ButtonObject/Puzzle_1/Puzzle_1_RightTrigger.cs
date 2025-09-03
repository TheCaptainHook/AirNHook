using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Puzzle_1_RightTrigger : MonoBehaviour, IInteractable
{

    //Refectoring 0324
    private Vector3 _offset = new Vector2(-0.35f, 1.5f);
    [SerializeField] Puzzle_1_Net net;

    #region Interactable
    public ObjectTypeEnum _objectType = ObjectTypeEnum.AirGun;
    public Transform Hold_Pivot => transform;
    private UI_Base _eButtonUI;

    public void Interaction(Transform accessor = null)
    {
        if (!net.onActive)
        {
            net.HoldAndRecover(accessor.gameObject, false, true);
            ChangeEbutton(true);
        }
        else
        {
            net.HoldAndRecover(accessor.gameObject, false, false);
            ChangeEbutton(false);
        }
    }

    public bool CanInteract() { return true; }

    public bool Interacting(bool value, GameObject player) { return true; }

    public ObjectTypeEnum GetObjectType()
    {
        return _objectType;
    }

    public void ShowEButton()
    {
        _eButtonUI = Managers.UI.ShowUI<UI_ShowEButton>();
        _eButtonUI.transform.position = transform.position + _offset;
    }

    public void HideEButton()
    {
        _eButtonUI = null;
        Managers.UI.HideUI<UI_ShowEButton>();
    }

    private void ChangeEbutton(bool isInteracting)
    {
        ((UI_ShowEButton)_eButtonUI).ChangeSprite(isInteracting);
    }
    #endregion

    #region UI
    public void ShowE(bool onOff)
    {
        if (onOff)
        {   
            var ui = Managers.UI.ShowUI<UI_ShowEButton>();
            ui.transform.position = transform.position + _offset;
           
        }
        else
        {
            Managers.UI.HideUI<UI_ShowEButton>();
        }
    }
    #endregion

}
