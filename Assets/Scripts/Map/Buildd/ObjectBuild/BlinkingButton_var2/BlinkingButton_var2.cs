using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlinkingButton_var2 : BuildObj
{
   private BlinkingButton_var2_Net _net;
   private BlinkingButton_var2_Net Net { get { _net ??= GetComponent<BlinkingButton_var2_Net>(); return _net; } }   



#region Clean
    public override void Clean()
    {
        Net.Clean();
    }
    #endregion

    public override void SetData(ObjectData data)
    {
        base.SetData(data);
        // Net.Init();
    }


}
