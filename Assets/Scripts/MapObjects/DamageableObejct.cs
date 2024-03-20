using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageableObejct : MonoBehaviour
{
    [SerializeField] Collider2D _collider2D;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 충돌한 객체가 IDamageable 인터페이스를 가지고 있는지 확인
        if (other.TryGetComponent(out IDamageable damageable))
        {
            // If successful, apply damage
            damageable.TakeDamage();
        }
    }
}
