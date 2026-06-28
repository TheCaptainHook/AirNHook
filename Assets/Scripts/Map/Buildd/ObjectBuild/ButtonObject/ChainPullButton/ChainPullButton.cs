using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainPullButton : ButtonEntity
{
    public override void SetData<T>(T data)
    {
        base.SetData(data);

    }
    #region Clean
    public override void Clean()
    {
        Net.Clean();
    }
    #endregion


    public override void Activation()
    {
        // base.Activation();
        PrograssButtonActivatedObject(true);
    }
    public override void Deactivated()
    {
        PrograssButtonActivatedObject(false);
        // base.Deactivated();
    }
}
