using System.Collections;
using Mirror;
using UnityEngine;

public class Puzzle_1_LeftTrigger : MonoBehaviour, IInteractable
{

    //Refectoring 0324
    private Vector3 _offset = new Vector2(0.55f, 1.5f);
    [SerializeField] Puzzle_1_Net net;


    #region Interactable
    public ObjectTypeEnum _objectType = ObjectTypeEnum.AirGun;
    public Transform Hold_Pivot => transform;
    private UI_Base _eButtonUI;

    public void Interaction(Transform accessor = null)
    {
        if (!net.onActive)
        {
            net.HoldAndRecover(accessor.gameObject, true, true);
            ChangeEbutton(true);
        }
        else
        {
            net.HoldAndRecover(accessor.gameObject, true, false);
            ChangeEbutton(false);
        }
    }

    #region  Clean
    public void Clean()
    {
        if(_eButtonUI) ChangeEbutton(false);
        HideEButton();
    }
    #endregion

    public bool CanInteract() { return true; }

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
