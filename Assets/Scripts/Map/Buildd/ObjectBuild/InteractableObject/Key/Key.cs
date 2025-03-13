using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : BuildObj
{
    private Key_Net Net => GetComponent<Key_Net>();

    private void Awake()
    {
        DissolveInitSetting();
    }

    public override void SetData<T>(T data)
    {
        base.SetData(data);
        Net.onSync = true;
        Net.Server_InitSync();
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
