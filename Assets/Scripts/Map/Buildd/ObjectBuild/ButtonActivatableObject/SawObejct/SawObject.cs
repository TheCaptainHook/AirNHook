using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class SawObject : ActivatableObjectEntity
{
    [CustomHeader("Saw Object")]
    public float addForcePower;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out IDamageable damageable) && !turnOff)
        {
            var rb = other.TryGetComponent(out Rigidbody2D _rb) ? _rb : null;
            if (rb != null)
            {
                rb.velocity = Vector2.zero;
                rb.AddForce(GetTargetDir(other) * addForcePower, ForceMode2D.Impulse);
            }
            damageable.TakeDamage();
        }
    }

    private Vector2 GetTargetDir(Collider2D target)
    {
        return (target.transform.position - transform.position).normalized;

    }


    #region  Main
    public override void Activation()
    {
        Net.Server_ChangeOnActive(true);
    }
    public override void Deactivated()
    {
        Net.Server_ChangeOnActive(false);
    }
    #endregion


}
