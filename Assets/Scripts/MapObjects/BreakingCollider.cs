using UnityEngine;

public class BreakingCollider : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.TryGetComponent(out IDamageable damageable)) return;

        damageable.TakeDamage();
    }
}
