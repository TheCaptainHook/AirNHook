using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameThrower : BuildObj
{
    
    BoxCollider2D _collider;

    float maxRate = 7f;
    float curRate;
    bool turnOff;
    public bool onActive;
    [SerializeField] LayerMask layerMask;

    [SerializeField] Flame flame;

    private void Awake()
    {
        _collider = GetComponent<BoxCollider2D>();
        curRate = maxRate;
    }
  

    private void FixedUpdate()
    {
        if (!MapEditor.Instance.stageClear || turnOff)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.right, curRate, layerMask);

            if (hit)
            {
                CheckHit(hit);
            }
            else
            {
                curRate = maxRate;
                flame.SetLifeTime();
            }

            curRate = Mathf.Clamp(curRate, 0, maxRate);
        }
        else
        {
            Disable();
        }
       
    }

    void CheckHit(RaycastHit2D hit)
    {
        curRate = hit.distance;

        if(hit.collider.gameObject.TryGetComponent(out IDamageable damageable))
        {
            damageable.TakeDamage();
        }

        flame.particle.Stop();
        flame.particle.Play();
        var main = flame.particle.main;
        main.startLifetime = 0.3f * hit.distance;
    }

    void Disable()
    {
        onActive = false;
        flame.GetComponent<Flame>().particle.Stop();
        curRate = 0;
        _collider.enabled = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("Projectile"))
        {
            Disable();
        }
    }


    private void OnDrawGizmos()
    {

        Gizmos.color = Color.blue;

        Gizmos.DrawRay(transform.position, transform.right * curRate);

    }


    public override void TurnOff()
    {
        base.TurnOff();
        turnOff = true;
        Disable();
        flame.gameObject.SetActive(false);
    }
    public override void TurnOn()
    {
        base.TurnOn();
        turnOff = false;
        flame.gameObject.SetActive(true);
        onActive = true;
        flame.particle.Play();
        curRate = maxRate;
        _collider.enabled = true;
    }
}
