
using UnityEngine;

public class Projectile_Shell : ProjectileEntity
{
    private SpriteRenderer spriteRenderer;
    
    protected override void Awake()
    {
        base.Awake();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }


    public override void Reset()
    {
        spriteRenderer.enabled = true;
        _collider.enabled = true;
        onHit = false;
    }
    public override void SpawnImpactEffect()
    {
        spriteRenderer.enabled = false;
        _collider.enabled = false;
    }
    protected override void ReleaseToPool_Projectile()
    {
        // Managers.Pooling.N_ReleaseToPool<Projectile_Shell>(gameObject);
        Managers.Pooling.D_ReleaseToPool(gameObject);
    }
}
