using System.Collections;
using System.Collections.Generic;
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


    private void OnTriggerEnter2D(Collider2D other)
    {
        // 충돌한 객체가 IDamageable 인터페이스를 가지고 있는지 확인
        if (other.TryGetComponent(out IDamageable damageable) && !turnOff)
        {
            // If successful, apply damage
            damageable.TakeDamage();
        }
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
