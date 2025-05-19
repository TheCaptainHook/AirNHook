using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EraseField_Character : ActivatableObjectEntity
{

    EraseField_Character_Net Net;
    private void Awake()
    {
      Net = GetComponent<EraseField_Character_Net>();
    }

    public override async void SetData<T>(T data)
    {
        try
        {
            if (typeof(T) == typeof(ButtonActivatableObjectStruct))
            {
                ButtonActivatableObjectStruct objData = (ButtonActivatableObjectStruct)(object)data;
                ButtonActivatedObjectStruct = objData;

            }
        }
        catch
        {
            Debug.Log($"ERROR,{typeof(T)}");
        }

        if (Application.isPlaying)
        {

            Util util = new Util();
            await util.Delay(() => { CheckActiveRequirAmount(); });
        }

    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        if(collision)
        {
            Debug.Log(collision.name);
        }
        if(collision.gameObject.TryGetComponent(out PlayerSM component))
        {
            component.TakeDamage(DamageType.Fire);
        }

    }

    protected override void Activation()
    {
        Net.Rpc_Active();
    }
    protected override void Deactivated()
    {
        Net.Rpc_Deactive();
    }

}
