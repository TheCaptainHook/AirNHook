using Mirror;
using UnityEngine;

public class BreakingCollider : MonoBehaviour
{
    public SpikeBall spikeBall;
    private NetworkIdentity _networkIdentity;
    private GameObject _player => spikeBall.shootingPlayer;

    private void Start()
    {
        _networkIdentity = spikeBall.netIdentity;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("a");
        if (!spikeBall.isOwned) return;

        Debug.Log(collision.gameObject);

        Debug.Log("b");
        if (!collision.TryGetComponent(out Destructible destructible))
        {
            if (!collision.TryGetComponent(out IDamageable damageable)) return;

            if (_player != null && collision.gameObject.Equals(_player)) return;

            damageable.TakeDamage(DamageType.Destruction);
        }
        else
        {
            Debug.Log("c");
            destructible.TakeDestructionDamage(_networkIdentity);
        }
    }
}
