using System;
using System.Collections;
using System.Collections.Generic;
using Org.BouncyCastle.Crypto.Engines;
using UnityEngine;

public class Shield : BuildObj
{

    private void Awake(){
        DissolveInitSetting();
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
