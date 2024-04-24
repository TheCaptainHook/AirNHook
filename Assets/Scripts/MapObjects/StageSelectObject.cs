using Mirror;
using UnityEngine;

public class StageSelectObject : MonoBehaviour, IInteractable
{
    public ObjectTypeEnum objectType = ObjectTypeEnum.Interaction;
    public Vector2 offset;

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

    public void Fixed(bool value)
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
        eButtonUI.gameObject.transform.position = transform.position + (Vector3)offset;
    }
}
