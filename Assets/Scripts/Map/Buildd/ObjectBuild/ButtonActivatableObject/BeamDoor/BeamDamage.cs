using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

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
                IsGrapCheck(obj);
                //Sound
                Managers.Sound.PlaySound3D(GlobalText.BEAMDOOR_ZAP, transform.position);
                //Sound
                AddForce(collision);
            }

        }   
    }

    private void IsGrapCheck(BuildObj obj)
    {
        if (obj.TryGetComponent(out TransportItemEntity component))
        {
            if (component._isGrab)
            {
                if (Managers.Game.Player.TryGetComponent(out HookSM hook))
                {
                    hook.ReleaseItem();
                }
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
