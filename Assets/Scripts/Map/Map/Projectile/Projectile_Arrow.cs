using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile_Arrow : MonoBehaviour
{

    bool onHit;
    Rigidbody2D rb;
    BoxCollider2D _collider;
    [SerializeField] float speed;
    public LayerMask layerMask;

    RaycastHit2D hit;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        _collider = GetComponent<BoxCollider2D>();
    }

    private void FixedUpdate()
    {
        if (!onHit)
        {
            hit = Physics2D.Raycast(transform.position, transform.right, 0.6f, layerMask);

            if (hit)
            {
                onHit = true;
                rb.velocity = Vector2.zero;
                rb.gravityScale = 0;

                if (hit.collider.TryGetComponent(out IDamageable damageable))
                {
                    damageable.TakeDamage();
                }
            }
            else
            {
                rb.AddForce(transform.right * speed, ForceMode2D.Impulse);
            }
        }

    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        //if (collision.gameObject.TryGetComponent(out IDamageable damageable))
        //{
        //    damageable.TakeDamage();
        //    _collider.enabled = false;
        //}

        if (collision.gameObject.layer == LayerMask.NameToLayer("Floor"))
        {
            rb.gravityScale = 0;
            rb.velocity = Vector2.zero;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            rb.gravityScale = 1;
        }
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
        Gizmos.DrawRay(transform.position, transform.right * 0.6f);
    }
}
