using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class Portal : BuildObj,IInteractable
{
    public ObjectTypeEnum objectType = ObjectTypeEnum.Interaction;
    public Vector2 btnOffset;

    public Portal targetPortal;
    private Vector2 targetPosition;

    bool onPrograss;

    private Coroutine portalCoroutine;


    #region Get,Set

    public override void SetData(ObjectData data)
    {
        ObjectData = data;
        transform.position = data.position;
        targetPosition = data.talPot;

    }

    public override void SetTileData()
    {
        targetPosition = targetPortal.transform.position;
        ObjectData = new ObjectData(id, transform.position, transform.localScale, 0, targetPosition);
    }

    public void FindTargetPortal()
    {
        if (targetPortal != null) return;

        foreach(Transform tr in MapEditor.Instance.objectTransform)
        {
            Portal portal = tr.GetComponent<Portal>();

            if(portal != null)
            {
                if (portal.ObjectData.position == targetPosition)
                {
                    targetPortal = portal;
                    return;
                }
            }
        }
    }


    #endregion


    #region Portal Logic

    #endregion

    #region Interactable
    public void Interaction(Transform accessor = null)
    {
        if (!NetworkServer.active || !NetworkClient.isConnected)
            return;

        if (!onPrograss)
        {
            if(portalCoroutine != null)
            {
                StopCoroutine(CoPortal());
            }

            portalCoroutine = StartCoroutine(CoPortal());
        }
    }

    IEnumerator CoPortal()
    {
        onPrograss = true;

        FindTargetPortal();
        targetPortal.onPrograss = true;

        //TODO Take Care logic : Cant Move Player 

        // FadeOut
        yield return MapEditor.Instance.fadeInOutPanel.FadeIn();

        GameObject player = Managers.Game.Player;
        player.transform.position = targetPosition;


        //Finish
        yield return MapEditor.Instance.fadeInOutPanel.FadeOut();
        portalCoroutine = null;
        onPrograss = false;
        targetPortal.onPrograss = false;

        //TODO Take Care logic : Can Move Player 

    }

    public bool CanInteract()
    {
        return true;
    }

    public void Interacting(bool value)
    {
        return;
    }

    public ObjectTypeEnum GetObjectType()
    {
        return objectType;
    }

    public void ShowEButton()
    {
        var eButtonUI = Managers.UI.ShowUI<UI_ShowEButton>();
        eButtonUI.transform.position = transform.position + (Vector3)btnOffset;
    }

    public void HideEButton()
    {
        Managers.UI.HideUI<UI_ShowEButton>();
    }

    #endregion

}

