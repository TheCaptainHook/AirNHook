using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrel : BuildObj
{

    private Barrel_Net net;
    private Barrel_Net Net { get { net ??= GetComponent<Barrel_Net>(); return net; } }

    public override void SetData<T>(T data)
    {
        base.SetData(data);
        if(Application.isPlaying) Net.Server_InitSync();

    }

    public override void TakeDamage(DamageType damageType = DamageType.Default)
    {
        
    }
}
