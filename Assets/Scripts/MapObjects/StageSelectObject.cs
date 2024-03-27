using UnityEngine;

public class StageSelectObject : MonoBehaviour, IInteractable
{
    public ObjectTypeEnum objectType = ObjectTypeEnum.Interaction;
    
    public void Interaction(Transform accessor = null)
    {
        if(!Managers.UI.IsActive<UI_StageSelect>())
            Managers.UI.ShowUI<UI_StageSelect>();
        else
            Managers.UI.HideUI<UI_StageSelect>();
    }

    public ObjectTypeEnum GetObjectType()
    {
        return objectType;
    }
}
