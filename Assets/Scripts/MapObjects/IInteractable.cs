using UnityEngine;

public interface IInteractable
{
    public void Interaction(Transform accessor = null);

    public bool CanInteract();

    public void Interacting(bool value);
    
    public ObjectTypeEnum GetObjectType();

    public void ShowEButton();
    
    public void HideEButton();
}