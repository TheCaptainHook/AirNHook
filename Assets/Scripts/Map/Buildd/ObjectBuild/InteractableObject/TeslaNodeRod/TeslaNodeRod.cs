using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeslaNodeRod : BuildObj
{
    private TeslaNodeRod_Net Net => GetComponent<TeslaNodeRod_Net>();
    void Awake()
    {
        DissolveInitSetting();
    }


    public override void SetData<T>(T data)
    {
        base.SetData(data);
        Net.onSync = true;
        Net.Server_InitSync();
    }

    
}
