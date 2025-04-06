using System.Collections;
using Mirror;
using UnityEngine;

public class Puzzle_1_LeftTrigger : MonoBehaviour, IInteractable
{
    // [SerializeField] Puzzle_1 puzzle_1;
    // [SerializeField] Puzzle_1_Button button;

    //Refectoring 0324
    public Vector3 offset;
    [SerializeField] Puzzle_1_Net net;
    [ReadOnly]
    public GameObject air;

    public Collider2D Col => GetComponent<Collider2D>();
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision != null)
        {
            if (collision.TryGetComponent(out AirSM air))
            {
                this.air = collision.gameObject;
                if(!net.onActive)
                net.Cmd_ShowE(collision.gameObject, true, true); //LEFT
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision != null)
        {
            if (collision.TryGetComponent(out AirSM air))
            {
                this.air = null;
                if(net != null)
                net.Cmd_ShowE(collision.gameObject, true, false); //LEFT
            }
        }


    }
    #region Interactable
    public ObjectTypeEnum _objectType = ObjectTypeEnum.Interaction;
    public Transform Hold_Pivot => transform;
    public void Interaction(Transform accessor = null)
    {
        if(air != null)
        {
            var id = accessor.root.gameObject.GetComponent<NetworkIdentity>().netId;
            if(!net.onActive){
                
                net.Cmd_ShowE(air,true,false); //LEFT
                net.Cmd_Interact(id,true,true);//LEFT
            }
            else{
                 net.Cmd_Interact(id,true,false);//LEFT
            }
        }
    }

    public bool CanInteract() { return true; }

    public void Interacting(bool value) { return; }

    public ObjectTypeEnum GetObjectType()
    {
        return _objectType;
    }

    public void ShowEButton()
    {
        return;
    }

    public void HideEButton()
    {
        return;
    }
    #endregion


    #region UI
    public void ShowE(bool onOff)
    {
        if (onOff)
        {   
            var ui = Managers.UI.ShowUI<UI_ShowEButton>();
            ui.transform.position = transform.position + offset;
           
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
