using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Puzzle_1_RightTrigger : MonoBehaviour,IInteractable
{

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
                net.Cmd_ShowE(collision.gameObject, false, true); //Right
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
                if (NetworkClient.active)
                    net.Cmd_ShowE(collision.gameObject, false, false); //Right
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
            //var id = accessor.root.gameObject.GetComponent<NetworkIdentity>().netId;
            //if(!net.onActive){

            //    net.Cmd_ShowE(air,false,false); //Right
            //    net.Cmd_Interact(id,false,true);//Right
            //}
            //else{
            //     net.Cmd_Interact(id,false,false);//Right
            //}
            if (!net.onActive)
            {
                net.Cmd_ShowE(air, false, false); //Right
                net.HoldAndRecover(air, false, true);
            }
            else
            {
                net.HoldAndRecover(air, false, false);
                Col.enabled = false;
                Col.enabled = true;
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

}
