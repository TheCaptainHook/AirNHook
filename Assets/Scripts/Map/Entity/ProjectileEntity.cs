
using System.Collections;
using UnityEngine;

public class ProjectileEntity : MonoBehaviour,IPooling
{
    protected bool onHit;
    protected bool onFire;
    protected RaycastHit2D hit;
    #region Components
    protected Rigidbody2D rb;
    protected Collider2D _collider;
    #endregion


    

    public float speed;
    public LayerMask hitLayerMask;

    public virtual void Reset() { }
    public virtual void SpawnImpactEffect() { }

    #region Defalut

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        _collider = GetComponent<Collider2D>();
    }

    protected virtual void FixedUpdate()
    {
        if (!onHit && onFire)
        {
            float hitDistance = rb.velocity.magnitude * Time.fixedDeltaTime * 1f;
            hit = Physics2D.Raycast(transform.position + (transform.right * 0.5f), transform.right, hitDistance, hitLayerMask);
            Debug.DrawRay(transform.position + transform.right * 0.5f, transform.right * hitDistance, Color.red);
            if (hit)
            {
                onHit = true;
                rb.velocity = Vector2.zero;
                rb.gravityScale = 0;

                SpawnImpactEffect();
                
                if (hit.collider.TryGetComponent(out IDamageable damageable))
                {
                    damageable.TakeDamage();
                    N_ReleaseToPool();
                    return;
                }
                if(hit.collider.TryGetComponent(out Shield shield))
                {
                    N_ReleaseToPool();
                    return;
                }

                //transform.position = hit.point;
                rb.velocity = Vector2.zero;
                StartCoroutine(DelayRelease());
            }
            else
            {
                rb.AddForce(transform.right * speed, ForceMode2D.Impulse);
            }
        }
    }


  
    public virtual void Setting(Vector2 point, Vector3 dir)
    {
        transform.position = point;
        float z = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, z);
        onFire = true;
    }



    protected IEnumerator DelayRelease()
    {
        yield return new WaitForSeconds(5);
        N_ReleaseToPool();
    }

    protected T GetTypeEntity<T>() where T: class
    {
        return this as T;
    }

  

    protected virtual void ReleaseToPool_Projectile(){

    }

    
    public void D_ReleaseToPool(){}

    public void N_ReleaseToPool()
    {
        Reset();
        ReleaseToPool_Projectile();
    }

    #endregion
}
