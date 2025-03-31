
using System.Collections;
using Unity.VisualScripting;
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
    [Header("Setting Field")]
    [SerializeField] protected SpriteRenderer spriteRenderer;
    [SerializeField] Transform firePoint;
    [Space(20)]
    protected int FOREGROUND_LAYERID;
    protected int MAPTILES_LAYERID;
    public GameObject main;
    public float speed;
    public LayerMask hitLayerMask;

    public virtual void Reset() 
    {
        _collider.enabled = true;
        onHit = false;
        spriteRenderer.sortingLayerID = FOREGROUND_LAYERID;
        spriteRenderer.sortingOrder = 10;
    }
    public virtual void SpawnImpactEffect(Vector2 point) { }

    #region Defalut
    /**
     * 1. Setting Field
     * 2. SpawnImpactEffect Override
     * 3. Reset Override
     * 4. ReleaseToPool_Projectile Override
     * **/
    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        _collider = GetComponent<Collider2D>();
        FOREGROUND_LAYERID = SortingLayer.NameToID("ForeGround");
        MAPTILES_LAYERID = SortingLayer.NameToID("Map/Tiles");
    }

    protected virtual void FixedUpdate()
    {
        if (!onHit && onFire)
        {
            float hitDistance = rb.velocity.magnitude * Time.fixedDeltaTime * 2f;
            hit = Physics2D.Raycast(firePoint.position, firePoint.right, hitDistance, hitLayerMask);
     
            if (hit)
            {
                onHit = true;
                rb.velocity = Vector2.zero;
                rb.gravityScale = 0;

                SpawnImpactEffect(hit.point);
                
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

                StartCoroutine(DelayRelease());
            }
            else
            {
                rb.AddForce(transform.right * speed, ForceMode2D.Impulse);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision!= null)
        {
            if (collision.gameObject == main) return;

            if (collision.TryGetComponent(out IDamageable damageable))
            {
                damageable.TakeDamage();
                Debug.Log($"Hit Projectile: [{gameObject.name}]\n{collision.name}");
                N_ReleaseToPool();
                return;
            }
            if (collision.TryGetComponent(out Shield shield))
            {
                N_ReleaseToPool();
                return;
            }
        }
    }


    public virtual void Setting(Vector2 point, Vector3 dir,GameObject obj)
    {
        main = obj;
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
