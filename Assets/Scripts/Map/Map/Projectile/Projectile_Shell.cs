
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Projectile_Shell : ProjectileEntity
{
    private Light2D Light=> GetComponent<Light2D>();


    protected override void ShootSound()
    {
        Managers.Sound.PlaySound3D(GlobalText.CANNON_FIRE_SOUND, transform.position, 0.25f);
    }
    protected override void HitSound()
    {
        
    }
    
    public override void Reset()
    {
        base.Reset();
        spriteRenderer.enabled = true;
        Light.intensity = 1;
    }
    public override void SpawnImpactEffect(Vector2 hitPoint)
    {
        spriteRenderer.enabled = false;
        _collider.enabled = false;
        Light.intensity = 0;
    }
    protected override void ReleaseToPool_Projectile(bool excution = false)
    {
        base.ReleaseToPool_Projectile();
        
        // Managers.Pooling.N_ReleaseToPool<Projectile_Shell>(gameObject);
        Managers.Pooling.D_ReleaseToPool(gameObject);
    }
}
