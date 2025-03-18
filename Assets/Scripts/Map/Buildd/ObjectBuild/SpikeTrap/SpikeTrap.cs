using Org.BouncyCastle.Asn1.Esf;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeTrap : BuildObj
{
    [CustomHeader("Spike Trap")]
    public float attackStartTime;
    public float attackCooldown;

    private readonly int AttackTrigger = Animator.StringToHash("Attack");
    private Animator animator;

    private SpikeTrap_Net Net;
    private void Awake()
    {
        animator = GetComponent<Animator>();
        Net = GetComponent<SpikeTrap_Net>();
    }




    #region Get,Set
    public override T GetData<T>()
    {
        if (typeof(T) == typeof(ObjectData))
        {
            var val = GetInfo();

            return (T)(object)new ObjectData(id, transform.position, transform.rotation, transform.localScale,val.startTime,val.cooldown);
        }

        return default(T);
    }
    public override void SetData<T>(T data)
    {
        if (typeof(T) == typeof(ObjectData))
        {
            ObjectData = (ObjectData)(object)data;
            attackStartTime = ObjectData.attackStartTime;
            attackCooldown = ObjectData.attackCooldown;

            if(Application.isPlaying)
            {
                Net.Server_InitSync();
            }
            else
            {
                SetData(ObjectData);
            }
        }
    }




    #endregion

    private (float startTime,float cooldown) GetInfo()
    {
        float s = Mathf.Clamp(attackStartTime, 0, float.MaxValue);
        float c = Mathf.Clamp(attackCooldown,1, float.MaxValue);
        return (s, c);
    }


    public void Attack()
    {
        animator.SetTrigger(AttackTrigger);
    }



}
