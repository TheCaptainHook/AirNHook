using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeamDamage : MonoBehaviour
{


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision != null)
        {
            if (collision.gameObject.TryGetComponent(out PlayerSM sm))
            {
                sm.TakeDamage(DamageType.Electric);
            }

            if (collision.gameObject.TryGetComponent(out BuildObj obj))
            {
                //Sound
                
                //Sound
                AddForce(collision);
            }

        }   
    }


    public float forcePower;
    private void AddForce(Collision2D collision)
    {
        var rb = collision.gameObject.GetComponent<Rigidbody2D>();
        if (rb == null) return;

        Vector2 normal = -collision.contacts[0].normal;
 
        rb.AddForce(normal*forcePower, ForceMode2D.Impulse);

    }
}
