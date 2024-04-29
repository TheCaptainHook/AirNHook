using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SawObject : BuildObj
{
    [SerializeField] GameObject hitBox;
    [SerializeField] GameObject sawPivot;
    Animator animator;

    private void Awake()
    {
        animator = sawPivot.GetComponent<Animator>();
    }


    public override void TurnOff()
    {
        base.TurnOff();
        animator.enabled = false;
        hitBox.SetActive(false);
    }


    public override void TurnOn()
    {
        
        base.TurnOn();
        animator.enabled = true;
        hitBox.SetActive(true);
    }


    private void OnTriggerStay2D(Collider2D other)
    {
        // 충돌한 객체가 IDamageable 인터페이스를 가지고 있는지 확인
        if (other.TryGetComponent(out IDamageable damageable))
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
