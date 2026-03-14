using System;
using UnityEngine;
using Mirror;


[RequireComponent(typeof(WDMP_Path))]
public class WeightDetectionMoveingPlatform : ActivatableObjectEntity
{
    [CustomHeader("Weight Detection Moving Platform")]

    #region Main

    #region  Main Field
    [Space(10)]
    [Header("Save Field")]
    public float moveDistance;
    public float moveSpeed;

    #endregion

    #endregion



    #region  Get,Set
    public override T GetData<T>()
    {
        if(typeof(T)==typeof(ButtonActivatableObjectStruct)){
            return (T)(object)new ButtonActivatableObjectStruct(id,transform.position,transform.rotation,transform.localScale,activeRequirAmount,indicatorStruct,moveDistance,moveSpeed);
        }
        
        return default(T);

    }

    protected override void AdditionalInspectorConfig()
    {
        moveSpeed = ButtonActivatedObjectStruct.moveSpeed;
        moveDistance = ButtonActivatedObjectStruct.moveDistance;
    }

    #endregion

    #region  Activatable Object Entity
    public override void Activation()
    {
        Net.Server_ChangeOnActive(true);
    }
    public override void Deactivated()
    {
        Net.Server_ChangeOnActive(false);
    }
    #endregion

}
