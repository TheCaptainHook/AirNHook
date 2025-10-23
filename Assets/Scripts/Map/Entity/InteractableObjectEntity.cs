
using UnityEngine;

public class InteractableObjectEntity : BuildObj
{
    private TransportItemEntity transportItemEntity;
    public TransportItemEntity TransportItemEntity
    {
        get
        {
            transportItemEntity ??= GetComponent<TransportItemEntity>();
            return transportItemEntity;
        }
    }

    private EncapsulationField field;
    public EncapsulationField EncapsulationField
    {
        get
        {
            field ??= GetComponent<EncapsulationField>();
            return field;
        }
    }

    // protected virtual void Awake()
    // {
    //     DissolveInitSetting();
    // }

    public override T GetData<T>()
    {
        if (typeof(T) == typeof(ObjectData))
        {
            if (TryGetComponent(out EncapsulationField field))
                return (T)(object)new ObjectData(id, transform.position, transform.rotation, transform.localScale, field.onEncapsulationItem, field.activeRequirAmount,field.indicator);
            else
                return (T)(object)new ObjectData(id, transform.position, transform.rotation, transform.localScale, false);
        }

        return default(T);
    }
    
    public override void SetData<T>(T data)
    {
        if (typeof(T) == typeof(ObjectData))
        {
            ObjectData objData = (ObjectData)(object)data;
            SetData(objData);
        }

        if (Application.isPlaying)
        {
            //TransportItemEntity.onSync = true;
            TransportItemEntity.Server_InitSync();
        }

    }
    public override void SetData(ObjectData data)
    {
       base.SetData(data);
       if (data.onEncapsulationItem)
       {
           EncapsulationField.onEncapsulationItem = true;
           EncapsulationField.activeRequirAmount = data.activeRequireAmount;
           EncapsulationField.indicator = data.indicator;
       }

    }


    public override void Clean()
    {
        DissolveClean();
        EncapsulationField.Clean(); //indicator Clean
        TransportItemEntity.onSync = false;
    }

}
