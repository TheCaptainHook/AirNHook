
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

    [ReadOnly]
    public GameObject main;
    public float speed;
    public LayerMask hitLayerMask;

    public virtual void Reset() 
    {
        _collider.enabled = true;
        rb.isKinematic = false;
        onHit = false;
        spriteRenderer.sortingLayerID = FOREGROUND_LAYERID;
        spriteRenderer.sortingOrder = 10;
    }
    public virtual void SpawnImpactEffect(Vector2 hitPoint) { }

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
                rb.isKinematic = true;
                SpawnImpactEffect(hit.point);
                
                if (hit.collider.TryGetComponent(out IDamageable damageable) && hit.collider.gameObject != main)
                {
                    if(hit.collider.TryGetComponent(out BuildObj buildObj))
                    {
                        if(buildObj.distructionStatus == DistructionStatus.Indestructible)
                        {
                            TransformChange(hit.collider.transform);
                            StartCoroutine(DelayRelease());
                            return;
                        }
                    }
                    damageable.TakeDamage();
                    N_ReleaseToPool();
                    return;
                }
                if(hit.collider.TryGetComponent(out Shield shield))
                {
                    // N_ReleaseToPool();
                    TransformChange(hit.transform);
                    StartCoroutine(DelayRelease());
                    return;
                }

                TransformChange(hit.transform);
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
        if(collision!= null && collision.gameObject != main)
        {
            // if (collision.gameObject == main) return;
            onHit = true;
            rb.velocity = Vector2.zero;
            rb.gravityScale = 0;

            Debug.Log($"{collision.gameObject.name}");
            
            SpawnImpactEffect(collision.ClosestPoint(transform.position));

            if (collision.TryGetComponent(out IDamageable damageable))
            {
                if(collision.TryGetComponent(out BuildObj buildObj))
                {
                    if(buildObj.distructionStatus == DistructionStatus.Indestructible)
                    {
                         TransformChange(collision.transform);
                         StartCoroutine(DelayRelease());
                         return;
                    }
                }
                damageable.TakeDamage();
                N_ReleaseToPool();
                return;
            }

            if (collision.TryGetComponent(out Shield shield))
            {
                // N_ReleaseToPool();
                TransformChange(collision.transform);
                StartCoroutine(DelayRelease());
                return;
            }

            TransformChange(collision.transform);
            StartCoroutine(DelayRelease());
        }
    }

    public virtual void TransformChange(Transform tr)
    {
        
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
