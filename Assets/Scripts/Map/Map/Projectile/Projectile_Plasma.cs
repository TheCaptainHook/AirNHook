using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Projectile_Plasma : ProjectileEntity
{
    [SerializeField] ParticleSystem particle;

    private int maxBoundCount = 1;
    private int curBoundCount = 0;
    private bool onMaxBound;
    private float defaultSpeed;
    public override void SpawnImpactEffect(Vector2 hitPoint)
    {
        //transform.position = hitPoint;
        particle.Play();
    }
    
    // protected override void ShootSound()
    // {
    //     Managers.Sound.PlaySound3D(GlobalText.DRONE_LASER_SOUND, transform.position, 0.5f);
    // }
    protected override void HitSound()
    {
        
    }
    public override void Reset()
    {
        StopAllCoroutines();
        curBoundCount = 0;
        onMaxBound = false;
        _collider.enabled = true;
        onHit = false;
        rb.isKinematic = false;
        spriteRenderer.enabled = true;
        onFire = false;
        curDurationRate = 0;
        transform.localScale = Vector3.one;
        rb.gravityScale = 0;

    }
    private void OnHit()
    {
        spriteRenderer.enabled = false;
        onMaxBound = true;
        onHit = true;
        _collider.enabled = false;
        rb.isKinematic = true;
        rb.velocity = Vector2.zero;
        HitSound();
    }

   
    protected override void OnHit(RaycastHit2D hit)
    {
        
        curBoundCount++;
        if (curBoundCount > maxBoundCount)
        {
            OnHit();
        }
        else
        {
            //reflection
            if(hit.collider == null)
            {
                OnHit();
            }
            else
            {
                if (Random.Range(0, 100) > 20) return;
                //Reflect Sound

                //Reflect Sound

                Vector2 refrection = Vector2.Reflect(transform.right, hit.normal);

                float randomAngle = Random.Range(-10f, 10f);

                Vector2 rotatedReflection = Quaternion.Euler(0f, 0f, randomAngle) * refrection;


                transform.right = rotatedReflection.normalized;
                StartCoroutine(Ricochet(rotatedReflection));
               
                transform.localScale = new Vector3(transform.localScale.x / 2, transform.localScale.y / 2, transform.localScale.z / 2);
                
                transform.position += transform.right;
                rb.AddForce(transform.right * speed, ForceMode2D.Impulse);
                rb.gravityScale = 1;
            }
        }
    }
    IEnumerator Ricochet(Vector2 reflect)
    {
        float percent = 0;
        float gra = rb.gravityScale;
        while(percent< 1)
        {
            percent += Time.deltaTime*2;
            rb.gravityScale = Mathf.Lerp(gra, 5, percent);
            yield return null;
        }
    }

    protected override void ReleaseToPool_Projectile(bool excution = false)
    {
        if (excution)
        {
            Reset();
            Managers.Pooling.D_ReleaseToPool(gameObject);
            return;
        }

        if (!onMaxBound) return;
        Reset();
        Managers.Pooling.D_ReleaseToPool(gameObject);
    }
    protected override IEnumerator DelayRelease()
    {

        if (onMaxBound)
        {
            yield return new WaitForSeconds(3);
            if (gameObject.activeSelf)
                ReleaseToPool_Projectile();
        }
        else
        {
            yield break;
        }
    }
}
