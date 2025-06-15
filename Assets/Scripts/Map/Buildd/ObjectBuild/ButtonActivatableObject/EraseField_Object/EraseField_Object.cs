using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EraseField_Object : ActivatableObjectEntity
{
    EraseField_Object_Net Net;
    private void Awake()
    {
        Net = GetComponent<EraseField_Object_Net>();
    }

    public override async void SetData<T>(T data)
    {
        base.SetData(data);

        if (Application.isPlaying)
        {
            await util.Delay(() => { CheckActiveRequirAmount(); });
        }

    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out BuildObj component))
        {
           
            component.Respawn();
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
