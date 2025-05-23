using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class SawObject : BuildObj
{
    //[SerializeField] GameObject hitBox;
    [SerializeField] GameObject sawPivot;
    Animator animator;

    private void Awake()
    {
        animator = sawPivot.GetComponent<Animator>();
        // _collider = GetComponent<Collider2D>();
    }


    public override void TurnOff()
    {
        base.TurnOff();
        animator.enabled = false;
        turnOff = true;
    }


    public override void TurnOn()
    {
        
        base.TurnOn();
        turnOff = false;
        animator.enabled = true;

    }

    public float addForcePower;
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 충돌한 객체가 IDamageable 인터페이스를 가지고 있는지 확인
        if (other.TryGetComponent(out IDamageable damageable) && !turnOff)
        {
                var rb = other.TryGetComponent(out Rigidbody2D _rb) ? _rb : null;
                if (rb != null)
                {
                    rb.velocity = Vector2.zero;
                    
                    rb.AddForce(GetTargetDir(other) * addForcePower, ForceMode2D.Impulse);
                }
            // If successful, apply damage
            damageable.TakeDamage();
        }
    }

    private Vector2 GetTargetDir(Collider2D target)
    {
        return (target.transform.position - transform.position).normalized;
    
    }
    //private void OnCollisionEnter2D(Collision2D other)
    //{
    //    if (other.gameObject.TryGetComponent(out IDamageable damageable))
    //    {
    //        // If successful, apply damage
    //        damageable.TakeDamage();
    //    }
    //}


}
