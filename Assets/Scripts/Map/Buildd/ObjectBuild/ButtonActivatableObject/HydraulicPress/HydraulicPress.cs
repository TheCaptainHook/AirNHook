
using System.Collections;
using UnityEngine;

public class HydraulicPress : ActivatableObjectEntity
{
  
    public override void Activation()
    {
        Net.Server_ChangeOnActive(true);
    }
    
    public override void Deactivated()
    {
        Net.Server_ChangeOnActive(false);
    }
   
}
