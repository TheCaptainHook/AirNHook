using System.Collections;
using Mirror;
using UnityEngine;

public class Puzzle_1_LeftTrigger : MonoBehaviour, IInteractable
{

    //Refectoring 0324
    private Vector3 _offset = new Vector2(0.55f, 1.5f);
    [SerializeField] Puzzle_1_Net net;

    //public Collider2D Col => GetComponent<Collider2D>();
    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    if (collision != null)
    //    {
    //        if (collision.TryGetComponent(out AirSM air))
    //        {
    //            this.air = collision.gameObject;
    //            if(!net.onActive)
    //                net.Cmd_ShowE(collision.gameObject, true, true); //LEFT
    //        }
    //    }
    //}
    //
    //private void OnTriggerExit2D(Collider2D collision)
    //{
    //    if (collision != null)
    //    {
    //
    //        if (collision.TryGetComponent(out AirSM air))
    //        {
    //            if(this.air == collision.gameObject)
    //            {
    //                this.air = null;
    //
    //            }
    //           
    //        }
    //    }
    //
    //
    //}

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

    //Refectoring 0324


    #region before
    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    if (collision != null)
    //    {
    //        if (collision.TryGetComponent(out AirSM component))
    //        {
    //            air = component;
    //            //dir
    //        }
    //    }
    //}

    //private void OnTriggerExit2D(Collider2D collision)
    //{
    //    if (collision != null)
    //    {
    //        if (collision.TryGetComponent(out AirSM component))
    //        {
    //            air = null;
    //        }
    //    }
    //}

    //private Vector3 GetAirDir()
    //{
    //    if (air == null) return Vector3.zero;

    //    if(airWeaponPivot == null)
    //    {
    //        foreach (Transform tr in air.transform)
    //        {
    //            if (tr.name == "WeaponPivot")
    //            {
    //                airWeaponPivot = tr;
    //            }
    //        }
    //    }

    //    return airWeaponPivot.rotation.eulerAngles;

    //}

    //private bool GetReadyToCharge(Vector3 rot)
    //{
    //    float z = rot.z - 360;
    //    if(rot.y ==0 && (z >=-10 && z <= 0))
    //    {
    //        return true;
    //    }
    //    else
    //    {
    //        return false;
    //    }
    //}
    #endregion
}
