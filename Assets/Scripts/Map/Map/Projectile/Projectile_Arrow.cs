using System.Collections;
using UnityEngine;

public class Projectile_Arrow : ProjectileEntity
{
 
    public override void SpawnImpactEffect(Vector2 point)
    {
        spriteRenderer.sortingLayerID = MAPTILES_LAYERID;
        _collider.enabled = false;
        spriteRenderer.sortingOrder = 3;

        transform.position = point;
    }
    public override void Reset()
    {
        base.Reset();
 
        _collider.enabled = true;

    }
   protected override void ReleaseToPool_Projectile()
    {
        Managers.Pooling.D_ReleaseToPool(gameObject);
    }


}
