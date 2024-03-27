using UnityEngine;

public interface IInteractable
{
    public void Interaction(Transform accessor = null);

    public ObjectTypeEnum GetObjectType();
}