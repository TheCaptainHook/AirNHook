using UnityEngine;
public class BridgeBox : ActivatableObjectEntity
{
    [CustomHeader("Bridge Box")]
    public float bridgeLength;

    public override T GetData<T>()
    {
            if (typeof(T) == typeof(ButtonActivatableObjectStruct))
        {
            return (T)(object)new ButtonActivatableObjectStruct(id, activeRequirAmount, transform.position, transform.rotation, transform.localScale,bridgeLength,GetConnectionPoint(),indicatorStruct);
        }

        return default(T);
    }

    protected override void AdditionalInspectorConfig()
    {
        bridgeLength = ButtonActivatedObjectStruct.bridgeLength;
    }
    
    #region  Main
    public override void Activation()
    {
        Net.Server_ChangeOnActive(true);

    }
    public override void Deactivated()
    {
        Net.Server_ChangeOnActive(false);

    }
    private Vector2 GetConnectionPoint()
    {
        Vector2 dir = transform.right;
        return (Vector2)transform.position + dir*bridgeLength;
            
    }
    #endregion
}

