
using JetBrains.Annotations;
using Mirror;
using UnityEngine;

public class JumpingPad : ActivatableObjectEntity
{
    [CustomHeader("Jumping Pad")]
    public int jumpingPower;


    #region Get,Set

    public override T GetData<T>()
    {
        if (typeof(T) == typeof(ButtonActivatableObjectStruct))
        {
            return (T)(object)new ButtonActivatableObjectStruct(id, transform.position, transform.rotation, transform.localScale,activeRequirAmount,indicatorStruct,jumpingPower);
        }

        return default(T);
    }
  
    protected override void AdditionalInspectorConfig()
    {
        jumpingPower = ButtonActivatedObjectStruct.jumpingPower;
    }
    #endregion

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!NetworkServer.active) return;

        if (!Net.onActive) return;
        if (collision != null && collision.TryGetComponent(out NetworkIdentity component))
        {
            if (collision.TryGetComponent(out JumpingPad _)) return;

            Net.Server_PlayUniqueEffect(component.netId);
            //Debug.Log(collision.name);
        }
    }



    public override void Activation()
    {
        //onActive = true;
        Net.Server_ChangeOnActive(true);
        // animator.SetBool(Activated,Net.onActive);
    }
    public override void Deactivated()
    {
        //onActive = false;
        Net.Server_ChangeOnActive(false);
        // animator.SetBool(Activated, Net.onActive);
    }
}
