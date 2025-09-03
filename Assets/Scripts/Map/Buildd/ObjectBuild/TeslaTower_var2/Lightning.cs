using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Lightning : MonoBehaviour
{
    public LineRenderer line;
    public GameObject target;

    public Vector2 start;
    private bool onPrograss;
    private float delay;
    private float maxDelay = 0.1f;

    void Update()
    {
        if (onPrograss)
        {
            if (delay >= maxDelay)
            {
                onPrograss = false;
                delay = 0;
                LineReset();
            }
            else
            {
                delay += Time.deltaTime;
            }
        }
    }
    public void Init(LineRenderer line)
    {
        this.line = line;
    }

    public void SetTarget(Vector2 start, GameObject target)
    {
        onPrograss = true;
        delay = 0;
        this.start = start;
        this.target = target;

        line.gameObject.SetActive(true);
        line.SetPosition(0, start);

        AttackSound(target.transform.position);
        //LightningRod
        if (target.TryGetComponent(out LightningRod rod))
        {
            line.SetPosition(1,rod.hitPoint.position);
            rod.Electric();
            return;
        }
        
        if(target.TryGetComponent(out TeslaRelayObject teslaRelayObject1))
        {
            line.SetPosition(1,teslaRelayObject1.headPoint.position);
            teslaRelayObject1.TakeDamage(DamageType.Electric);
            return;
        }

        if(target.TryGetComponent(out TeslaNodeRod nodeRode))
        {
            line.SetPosition(1, nodeRode.head.position);
            nodeRode.TakeDamage(DamageType.Electric);
            return;
        }

        //Hook Grap Item Check
        if(target.TryGetComponent(out HookSM hook))
        {
            var item = hook.GetGrabbedItem();
            if(item)
            {
                 if(item.TryGetComponent(out LightningRod rod2))
                {
                    line.SetPosition(1,rod2.hitPoint.position);
                    rod2.Electric();
                    return;
                }

                if(item.TryGetComponent(out TeslaRelayObject teslaRelayObject2))
                {
                    line.SetPosition(1,teslaRelayObject2.headPoint.position);
                    teslaRelayObject2.TakeDamage(DamageType.Electric);
                    return;
                }
            } 

           
        }
        //Hook Grap Item Check
        
        line.SetPosition(1, target.transform.position);
       
        if (target.TryGetComponent(out IDamageable component))
        {
            component.TakeDamage(DamageType.Electric);
        }
       
        //Target TakeDamage
    }

    private void AttackSound(Vector2 target)
    {
        Managers.Sound.PlaySound3D(GlobalText.ELECTRIC_SHOCK_SOUND, target,0.2f);
    }

    private void LineReset()
    {
        line.gameObject.SetActive(false);
    }

}
