using System;
using Mirror;
using UnityEngine;

public class StageSelectObject : MonoBehaviour, IInteractable
{
    public ObjectTypeEnum objectType = ObjectTypeEnum.Interaction;
    public Vector2 offset;

    //private void Awake()
    //{
    //    Managers.Sound.PlaySound(AudioType.Lobby,AudioMixerGroupType.BGM,true,1f, 0.6f);
    //}

    public void Interaction(Transform accessor = null)
    {
        if (!NetworkServer.active || !NetworkClient.isConnected)
            return;

        if(!Managers.UI.IsActive<UI_StageSelect>())
            Managers.UI.ShowUI<UI_StageSelect>();
        else
            Managers.UI.HideUI<UI_StageSelect>();
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
        eButtonUI.transform.position = transform.position + (Vector3)offset;
    }
    
    public void HideEButton()
    {
        Managers.UI.HideUI<UI_ShowEButton>();
    }
}
