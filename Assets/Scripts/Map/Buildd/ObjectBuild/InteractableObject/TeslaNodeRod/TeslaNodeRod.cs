using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeslaNodeRod : ButtonEntity
{
    [CustomHeader("Tesla Node Rod")]
    public bool isPowerSupplied;
    private TeslaNodeRod_Net Net => GetComponent<TeslaNodeRod_Net>();
    void Awake()
    {
        DissolveInitSetting();
    }

    #region Get,Set
    public override T GetData<T>()
    {
        if (typeof(T) == typeof(ButtonObjectStruct))
        {
            return (T)(object)new ButtonObjectStruct(
                id, 
            GetTargetPositions(), 
            GetLightPositions(),
            transform.position, 
            transform.rotation,
            transform.localScale, 
            false);
        }

        return default(T);
    }
    public override void SetData<T>(T data)
    {
        base.SetData(data);
        Net.onSync = true;
        Net.Server_InitSync();
    }
    #endregion

    public float maxShutDownRate;
    private float curShutDownRate;
    void Update()
    {
        if(isPowerSupplied)
        {
            curShutDownRate+=Time.deltaTime;
            if(curShutDownRate> maxShutDownRate)
            {
                isPowerSupplied = false;
                curShutDownRate = 0;

                PowerSupply(false);
            }
        }
    }

    public override void TakeDamage(DamageType damageType = DamageType.Default)
    {
        if(damageType == DamageType.Electric)
        {

        }
        else base.TakeDamage(damageType);
    }


    private void PowerSupply(bool onOff)
    {
        if(isPowerSupplied) return;
        isPowerSupplied = true;

        for(int i = 0; i<targetObjects.Count;i++)
        {
            if(targetObjects[i].TryGetComponent(out IPowerConsumer component))
            {
                if(onOff)component.PowerOn();
                else component.PowerOff();
                //Effect Rpc

            }
        }
        for(int i =0;i<lightObjects.Count;i++)
        {
            if(lightObjects[i].TryGetComponent(out IPowerConsumer component))
            {
                if(onOff)component.PowerOn();
                else component.PowerOff();
                //Effect Rpc

            }
        }

    }
}
