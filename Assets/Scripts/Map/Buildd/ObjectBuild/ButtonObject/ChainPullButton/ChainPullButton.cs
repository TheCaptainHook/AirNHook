using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainPullButton : ButtonEntity
{
#region Get,Set
    private ChainPullButton_Net cpbn;
    private ChainPullButton_Net CPBN {get{cpbn ??= GetComponent<ChainPullButton_Net>(); return cpbn;}}
    public override T GetData<T>()
    {
        if (typeof(T) == typeof(ButtonObjectStruct))
        {
            return (T)(object)new ButtonObjectStruct(
                id,
                transform.position,
                transform.rotation,
                transform.localScale,

                GetTargetPositions(),
                GetLightPositions(),
                GetEncapsulationTiems(),
                CPBN._left_chain_condition_len,CPBN._right_chain_condition_len,CPBN._maxChainLength
                );
        }

        return default(T);
    }
    public override void SetData<T>(T data)
    {
        base.SetData(data);
    }
    public override void SetOtherDataParm(ButtonObjectStruct data)
    {
        CPBN._left_chain_condition_len = data.l_chain_len;
        CPBN._right_chain_condition_len = data.r_chain_len;
        CPBN._maxChainLength = data.max_chain_len;
    }
#endregion
   
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
