using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameThrower : BuildObj
{
    float maxRate = 6.3f;
    float curRate;

    public bool onActive;
    private bool onRecoveryRay;
    [SerializeField] LayerMask layerMask;

    [SerializeField] Flame flame;

    //todo test 0429
    Vector3 hitDown;
    Vector3 hitUp;

    private void Awake()
    {
        _collider = GetComponent<BoxCollider2D>();

        hitDown = (Quaternion.Euler(0, 0, -10) * transform.right).normalized ;
        hitUp= (Quaternion.Euler(0, 0, 10) * transform.right).normalized;
        StartCoroutine(Co_StartRay());
    }
  

    private void FixedUpdate()
    {
        if (!MapEditor.Instance.stageClear && !turnOff)
        {
            hitDown = (Quaternion.Euler(0, 0, -10) * transform.right).normalized;
            hitUp = (Quaternion.Euler(0, 0, 10) * transform.right).normalized;

            RaycastHit2D hit = Physics2D.Raycast(flame.transform.position, transform.right, curRate, layerMask);
            RaycastHit2D hit1 = Physics2D.Raycast(flame.transform.position,hitDown , curRate, layerMask);
            RaycastHit2D hit2 = Physics2D.Raycast(flame.transform.position, hitUp, curRate, layerMask);

            
            Debug.DrawRay(flame.transform.position, transform.right*curRate, Color.green);
            Debug.DrawRay(flame.transform.position, hitDown * curRate, Color.green);
            Debug.DrawRay(flame.transform.position, hitUp * curRate, Color.green);



            if (hit)
            {
                CheckHit(hit);
               
            }else if (hit1)
            {
                CheckHit(hit1);
                
            }
            else if (hit2)
            {
                CheckHit(hit2);
            }
            else
            {
                if (!onRecoveryRay && curRate < maxRate)
                {
                    StartCoroutine(Co_StartRay());
                }

                flame.SetLifeTime();
            }

            curRate = Mathf.Clamp(curRate, 0, maxRate);
        }
        else
        {
            Disable();
        }
       
    }


    IEnumerator Co_StartRay()
    {
        onRecoveryRay = true;
        while (curRate <= maxRate)
        {
            curRate += Time.deltaTime+0.015f;
            yield return null;
        }
        onRecoveryRay = false;
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

        if(curRate < 0.5f)
        {
            main.startLifetime = 0.05f;
        }
        else if(curRate < 1f)
        {
            main.startLifetime = 0.1f;
        }
        else
        {
            main.startLifetime = 0.3f * hit.distance;
        }

    }

    void Disable()
    {
        onActive = false;
        flame.GetComponent<Flame>().particle.Stop();
        curRate = 0;
        //_collider.enabled = false;
    }

    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    if(collision.gameObject.layer == LayerMask.NameToLayer("Projectile"))
    //    {
    //        Disable();
    //    }
    //}


    public override void TurnOff()
    {
        base.TurnOff();
        curRate = 0;
        turnOff = true;
        onActive = false;
        flame.GetComponent<Flame>().particle.Stop();
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
    }
}
