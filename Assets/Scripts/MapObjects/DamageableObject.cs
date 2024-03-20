using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageableObject : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 충돌한 객체가 IDamageable 인터페이스를 가지고 있는지 확인
        IDamageable damageable = other.GetComponent<IDamageable>();
        if (damageable != null)
        {
            // 대미지를 입힘
            damageable.Die();
        }
    }
}
