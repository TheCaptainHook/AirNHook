using UnityEngine;

public interface IInteractable
{
    public void Interaction(Transform accessor = null);

    public bool CanInteract();

    public void Fixed(bool value);
    
    public ObjectTypeEnum GetObjectType();

    public void ShowEButton();
}