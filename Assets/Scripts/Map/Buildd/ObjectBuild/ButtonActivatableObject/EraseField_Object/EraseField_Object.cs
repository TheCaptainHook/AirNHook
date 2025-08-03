using Mirror;
using UnityEngine;

public class EraseField_Object : ActivatableObjectEntity
{

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!NetworkServer.active) return;
        if (collision.gameObject.TryGetComponent(out BuildObj component))
        {
            component.Respawn();
        }
    }

    public override void Activation()
    {
        Net.Server_ChangeOnActive(true);
    }
    public override void Deactivated()
    {
        Net.Server_ChangeOnActive(false);
    }
}
