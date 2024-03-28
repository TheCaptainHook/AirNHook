using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameThrower : MonoBehaviour
{
    
    BoxCollider2D _collider;
    BuildObj buildObj;


    float maxRate = 7f;
    float curRate;

    bool firstHit;

    public bool onActive;
    [SerializeField] LayerMask layerMask;

    [SerializeField] Flame flame;

    private void Awake()
    {
        _collider = GetComponent<BoxCollider2D>();
        buildObj = GetComponent<BuildObj>();
        buildObj.OnDisableAction += DisableParticle;
        curRate = maxRate;
    }
    private void Update()
    {
        Vector3 dir = Quaternion.AngleAxis(7, Vector3.forward) * transform.right;
        Vector3 dir1 = Quaternion.AngleAxis(-7, Vector3.forward) * transform.right;
        RaycastHit2D hit0 = Physics2D.Raycast(transform.position, dir, curRate, layerMask);
        RaycastHit2D hit1 = Physics2D.Raycast(transform.position, transform.right, curRate, layerMask);
        RaycastHit2D hit2 = Physics2D.Raycast(transform.position, dir1, curRate, layerMask);

        if (hit0)
        {
            CheckHit(hit0);
        }
        else if (hit1)
        {
            CheckHit(hit1);
        }
        else if (hit2)
        {
            CheckHit(hit2);
        }
        else
        {
            curRate = maxRate;
            flame.SetLifeTime();
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
        //flame.particle.startLifetime = 0.2f * hit.distance;
        var main = flame.particle.main;
        main.startLifetime = 0.2f * hit.distance;
    }

    void DisableParticle()
    {
        if (!onActive)
        {
            onActive = false;
            flame.GetComponent<Flame>().particle.Stop();
        }
        
    }

 


    private void OnDrawGizmos()
    {
        Vector3 dir = Quaternion.AngleAxis(7, Vector3.forward) * transform.right;
        Vector3 dir1 = Quaternion.AngleAxis(-7, Vector3.forward) * transform.right;
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, dir  * curRate);
        Gizmos.DrawRay(transform.position, transform.right * curRate);
        Gizmos.DrawRay(transform.position, dir1  * curRate);
    }
}
