using UnityEngine;
using Mirror;
public class EraseField_Character : ActivatableObjectEntity
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!NetworkServer.active) return;
        
        if (collision.gameObject.TryGetComponent(out PlayerSM component))
        {
            component.TakeDamage(DamageType.Fire);
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
