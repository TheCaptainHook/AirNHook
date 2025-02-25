using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class StrongBox : BuildObj
{

    public float health = 5f;

    private StrongBox_Net Net => GetComponent<StrongBox_Net>();

    private void Awake()
    {

        DissolveInitSetting();
    }


    public override void SetData<T>(T data)
    {
        base.SetData(data);
        Net.Server_SetOrgPosition(position);
    }

    public override void TakeDamage(DamageType damageType = DamageType.Default)
    {
        health -= 1f;
        if (health <= 0f)
        {
            position = Net.orgPosition;

            base.TakeDamage();
            health = 5f;
        }
    }
    

    public override void TurnOff()
    {
        base.TurnOff();
        _rb.gravityScale = 0;
        _rb.velocity = Vector2.zero;
    }
    public override void TurnOn()
    {
        base.TurnOn();
        _rb.gravityScale = 1;
    }
}
