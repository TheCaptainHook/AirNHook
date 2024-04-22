using UnityEngine;

public interface IInteractable
{
    public void Interaction(Transform accessor = null);

    public bool CanInteract();
    
    public ObjectTypeEnum GetObjectType();
}