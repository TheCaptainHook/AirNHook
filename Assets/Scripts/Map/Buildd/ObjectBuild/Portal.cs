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
    
    [Header("Animation")]
    [SerializeField] private Animator _animator;
    
    #region StringCache
    private static readonly int IsActive = Animator.StringToHash("IsActive");
    #endregion

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
    //TODO : 포탈 껏다 켜짐 옵션으로 애니메이터 조절
    //_animator.SetBool(IsActive, true);
    //_animator.SetBool(IsActive, false);
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
        GameObject player = Managers.Game.Player;
        Rigidbody2D rg = player.GetComponent<Rigidbody2D>();
        //TODO Take Care logic : Cant Move Player 
        rg.simulated = false;

        FindTargetPortal();
        targetPortal.onPrograss = true;

        // FadeOut
        yield return MapEditor.Instance.fadeInOutPanel.FadeIn();
        player.transform.position = targetPosition;


        //Finish
        yield return MapEditor.Instance.fadeInOutPanel.FadeOut();
        portalCoroutine = null;
        onPrograss = false;
        targetPortal.onPrograss = false;

        //TODO Take Care logic : Can Move Player 
        rg.simulated = true;
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

