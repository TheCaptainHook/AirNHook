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
        hit = Physics2D.Raycast(transform.position, transform.right, 0.5f,layerMask);

        if (hit)
        {
            onHit = true;
            rb.velocity = Vector2.zero;

        }

        if (!onHit)
        {
            rb.AddForce(transform.right * speed, ForceMode2D.Force);
        }

    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out IDamageable damageable))
        {
            damageable.TakeDamage();
            _collider.enabled = false;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out IDamageable damageable))
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
}
