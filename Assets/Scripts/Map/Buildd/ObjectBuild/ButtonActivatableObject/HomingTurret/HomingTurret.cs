using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingTurret : ActivatableObjectEntity
{


    protected override void AdditionalInspectorConfig()
    {
        
    }



    public override void Activation() //Server
    {
        Net.Server_ChangeOnActive(true);
    }
    public override void Deactivated() //Server
    {
        Net.Server_ChangeOnActive(false);
    }

    public override void Clean()
    {
        base.Clean();
    }
}
