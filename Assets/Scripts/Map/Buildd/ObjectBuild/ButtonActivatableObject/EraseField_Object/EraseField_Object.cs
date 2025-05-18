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
        if (collision.gameObject.TryGetComponent(out BuildObj component))
        {
            //var root = collision.GetComponent<InteractableObject>().GetFixedPointRootTransform();
            //if (root != null)
            //{
            //    if (root.TryGetComponent(out HookSM hook)) hook.ReleaseItem();
            //    else if (root.TryGetComponent(out AirSM air)) air.StopGun();
            //}
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
