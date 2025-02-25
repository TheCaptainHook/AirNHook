using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : BuildObj
{
    
    private Box_Net Net => GetComponent<Box_Net>();

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
        position = Net.orgPosition;
        base.TakeDamage(damageType);
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
