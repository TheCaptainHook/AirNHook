
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
        curDurationRate = 0;
    }
    public virtual void SpawnImpactEffect(Vector2 hitPoint) { }

    #region Defalut
    /**
     * 1. Setting Field
     * 2. SpawnImpactEffect Override
     * 3. Reset Override
     * 4. ReleaseToPool_Projectile Override
     * **/
     private float maxDurationRate=5;
     protected float curDurationRate=0;
    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        _collider = GetComponent<Collider2D>();
        FOREGROUND_LAYERID = SortingLayer.NameToID("ForeGround");
        MAPTILES_LAYERID = SortingLayer.NameToID("Map/Tiles");
    }

    protected virtual void OnHit(RaycastHit2D hit)
    {
        onHit = true;
        rb.velocity = Vector2.zero;
        rb.gravityScale = 0;
        rb.isKinematic = true;
    }
    protected virtual void FixedUpdate()
    {
        if (!onHit && onFire)
        {
            curDurationRate += Time.fixedDeltaTime;
            if(curDurationRate >= maxDurationRate)
            {
                ReleaseToPool_Projectile(true);
                return;
            }

            float hitDistance = rb.velocity.magnitude * Time.fixedDeltaTime * _collider.bounds.extents.x*1.2f;
            hit = Physics2D.Raycast(firePoint.position, firePoint.right, hitDistance, hitLayerMask);
            Debug.DrawRay(firePoint.position,firePoint.right*hitDistance,Color.red);
            if (hit)
            {
                // onHit = true;
                // rb.velocity = Vector2.zero;
                // rb.gravityScale = 0;
                // rb.isKinematic = true;
                Debug.Log($"Fixed Hit : {hit.collider.name}");
                OnHit(hit);
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
                    // N_ReleaseToPool();
                    ReleaseToPool_Projectile(true);
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
                rb.AddForce(transform.right * speed, ForceMode2D.Force);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision!= null && collision.gameObject != main)
        {
            // if (collision.gameObject == main) return;
            // onHit = true;
            // rb.velocity = Vector2.zero;
            // rb.gravityScale = 0;
            OnHit(hit);
            Debug.Log($"Trigger  : {collision.gameObject.name}");
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
                // N_ReleaseToPool();
                ReleaseToPool_Projectile();
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



    protected virtual IEnumerator DelayRelease()
    {
        yield return new WaitForSeconds(3);
        // N_ReleaseToPool();
        if(gameObject.activeSelf)
        ReleaseToPool_Projectile();
    }

    protected T GetTypeEntity<T>() where T: class
    {
        return this as T;
    }

  

    protected virtual void ReleaseToPool_Projectile(bool excution = false)
    {
        Reset();
    }

    
    public void D_ReleaseToPool()
    {
        Managers.Pooling.D_ReleaseToPool(gameObject);
    }

    public void N_ReleaseToPool()
    {
        // Reset();
        // ReleaseToPool_Projectile();
    }

    #endregion
}
