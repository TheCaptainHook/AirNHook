using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile_Plasma : ProjectileEntity
{
    private int maxBoundCount = 1;
    private int curBoundCount = 0;
    private bool onMaxBound;
    public override void SpawnImpactEffect(Vector2 hitPoint)
    {
        
    }
    public override void Reset()
    {
        
    }
    protected override void ReleaseToPool_Projectile()
    {
        base.ReleaseToPool_Projectile();
        Managers.Pooling.D_ReleaseToPool(gameObject);
    }
    protected override IEnumerator DelayRelease()
    {
        return base.DelayRelease();
    }
}
