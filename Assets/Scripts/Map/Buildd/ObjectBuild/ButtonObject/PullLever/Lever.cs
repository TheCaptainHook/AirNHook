using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

public class Lever : MonoBehaviour,IInteractable
{
    [SerializeField] PullLever main;
    [SerializeField] PullLever_Net net;
    
    
    [Space(20)]
    [ReadOnly]
    public GameObject player;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (player == null && collision.TryGetComponent(out PlayerSM _))
        {
            if(!net.onActive)net.Cmd_ShowE(collision.gameObject,true);
            player = collision.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (player != null && collision.gameObject == player)
        {
            net.Cmd_ShowE(collision.gameObject,false);
            player = null;
        }
    }


    #region Interactable
    public ObjectTypeEnum _objectType = ObjectTypeEnum.Interaction;

    public Vector3 offset;
    public void Interaction(Transform accessor = null)
    {
        if(accessor.root.gameObject == player)
        {
            if(!net.onActive){
                net.Cmd_ShowE(player,false);
                net.Cmd_Interact(player.GetComponent<NetworkIdentity>().netId,true);
            }else{
                net.Cmd_Interact(player.GetComponent<NetworkIdentity>().netId,false);

            }
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
        return;
    }

    public void HideEButton()
    {
        return;
    }
    #endregion



    public void ShowE()
    {
        var ui = Managers.UI.ShowUI<UI_ShowEButton>();
        ui.transform.position = transform.position + offset;
    }
    public void HideE()
    {
        Managers.UI.HideUI<UI_ShowEButton>();
    }
}
