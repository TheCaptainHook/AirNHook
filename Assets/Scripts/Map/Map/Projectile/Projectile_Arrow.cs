using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Projectile_Arrow : MonoBehaviour,IPooling
{

    bool onHit;
    Rigidbody2D rb;
    BoxCollider2D _collider;
    [SerializeField] float speed;
    public LayerMask layerMask;

    RaycastHit2D hit;

    bool onFire;
    Vector2 dir;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        _collider = GetComponent<BoxCollider2D>();
    }

    private void FixedUpdate()
    {
        if (!onHit && onFire)
        {
            hit = Physics2D.Raycast(transform.position+ (Vector3.right * 0.5f), transform.right, 0.6f, layerMask);

            if (hit)
            {
                onHit = true;
                rb.velocity = Vector2.zero;
                rb.gravityScale = 0;
                if (hit.collider.TryGetComponent(out IDamageable damageable))
                {
                    damageable.TakeDamage();
                    ReleaseToPool();
                    return;
                }
                transform.position = hit.point;
                StartCoroutine(DelayRelease());
            }
            else
            {
                rb.AddForce(transform.right * speed, ForceMode2D.Impulse);
            }
        }

    }



    #region Pooling
    public void ReleaseToPool()
    {
        Reset();
        Managers.Pooling.N_ReleaseToPool<Projectile_Arrow>(gameObject);
    }
    #endregion

    public void Setting(Vector2 point,Vector3 dir)
    {
        this.dir = dir;
        transform.position = point;
        float z = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, z);
        onFire = true;
    }

    IEnumerator DelayRelease()
    {
        yield return new WaitForSeconds(5);
        ReleaseToPool();
    }

    public void Reset()
    {
        rb.gravityScale = 0;
        _collider.enabled = true;
        onHit = false;
    }
    
    private void OnDrawGizmosSelected()
    {
        // 스피어 캐스트를 그리기 위해 씬 상에 범위를 표시
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position+(Vector3.right*0.5f), transform.right * 0.6f);
    }
}
