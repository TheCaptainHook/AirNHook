
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Serialization;


public class MovingSaw :  DroneEntity
{

    [Header("Main")]

    [SerializeField] private GameObject _greenLight;
  
    #region  SawObj

    //private void OnTriggerEnter2D(Collider2D other)
    //{
    //    // 충돌한 객체가 IDamageable 인터페이스를 가지고 있는지 확인
    //    if (other.TryGetComponent(out IDamageable damageable) && !turnOff)
    //    {
    //        // If successful, apply damage
    //        damageable.TakeDamage();
    //    }
    //}

    #endregion


}

