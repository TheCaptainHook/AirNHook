using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lever : MonoBehaviour,IInteractable
{
    [SerializeField] PullLever main;
    [SerializeField] PullLever_Net net;

    public GameObject player;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out PlayerSM player))
        {
            this.player = collision.gameObject;
        }


    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out PlayerSM player))
        {
            this.player = null;
        }

    }


    #region Interactable
    public ObjectTypeEnum _objectType = ObjectTypeEnum.Interaction;

    public Vector3 offset;
    public void Interaction(Transform accessor = null)
    {
        //Pulling(true);
        Debug.Log("EEEEE");
    }

    public bool CanInteract() { return true; }

    public void Interacting(bool value) { return; }

    public ObjectTypeEnum GetObjectType()
    {
        return _objectType;
    }

    public void ShowEButton()
    {
        if(NetworkClient.localPlayer)
        {
          var ui =  Managers.UI.ShowUI<UI_ShowEButton>();
            ui.transform.position = transform.position + offset;
        }

    }

    public void HideEButton()
    {
        if (NetworkClient.localPlayer)
        {
            Managers.UI.HideUI<UI_ShowEButton>();
        }
    }
    #endregion
}
