using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SparkTrap : BuildObj
{


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision && collision.TryGetComponent(out IDamageable component))
        {
            component.TakeDamage(DamageType.Electric);
        }
    }
}
