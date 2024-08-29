using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivatableObjectEntity : BuildObj
{
   protected virtual void Activation(){}
   protected virtual void Deactivated(){}

   protected virtual void PrograssButtonActivatedObject(int num){}
}
