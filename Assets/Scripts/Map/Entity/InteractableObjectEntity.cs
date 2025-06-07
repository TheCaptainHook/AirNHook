using System.Collections;
using System.Collections.Generic;
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

    protected virtual void Awake()
    {
        DissolveInitSetting();
    }

    public override T GetData<T>()
    {
        if (typeof(T) == typeof(ObjectData))
        {
            if (TryGetComponent(out EncapsulationField field))
                return (T)(object)new ObjectData(id, transform.position, transform.rotation, transform.localScale, false, field.onEncapsulationItem);
            else
                return (T)(object)new ObjectData(id, transform.position, transform.rotation, transform.localScale, false, false);
        }

        return default(T);
    }
    public override void SetData<T>(T data)
    {
        if(typeof(T) == typeof(ObjectData)){
            ObjectData objData = (ObjectData)(object)data;
            SetData(objData);
        }

        if (Application.isPlaying)
        {
            TransportItemEntity.onSync = true;
            TransportItemEntity.Server_InitSync();
                //TEST 0606
            if (ObjectData.onEncapsulationItem)
            {
                StartCoroutine(DelayedCapsuling());
            }
            //TEST 0606
        }
       
    }
    private IEnumerator DelayedCapsuling()
{
    yield return new WaitForSeconds(0.1f); // 한 프레임 대기
    EncapsulationField.Capsuling();
}
    public override void SetData(ObjectData data)
    {
        base.SetData(data);
        if (data.onEncapsulationItem) EncapsulationField.onEncapsulationItem = true;
    }
}
