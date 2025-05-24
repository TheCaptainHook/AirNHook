using UnityEngine;

public interface IInteractable
{
    public void Interaction(Transform accessor);

    public bool CanInteract();

    public bool Interacting(bool value, GameObject player);
    
    public ObjectTypeEnum GetObjectType();

    public void ShowEButton();
    
    public void HideEButton();
}