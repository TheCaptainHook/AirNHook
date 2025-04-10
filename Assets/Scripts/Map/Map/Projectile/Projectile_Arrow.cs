using System.Collections;
using UnityEngine;

public class Projectile_Arrow : ProjectileEntity
{
 
    public override void SpawnImpactEffect(Vector2 hitPoint)
    {
        spriteRenderer.sortingLayerID = MAPTILES_LAYERID;
        _collider.enabled = false;
        spriteRenderer.sortingOrder = 3;

        transform.position = hitPoint;
        // transform.SetParent(hit.collider.transform);
        
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

    public override void TransformChange(Transform tr)
    {
        transform.SetParent(tr);
        rb.velocity = Vector3.zero;
    }


}
