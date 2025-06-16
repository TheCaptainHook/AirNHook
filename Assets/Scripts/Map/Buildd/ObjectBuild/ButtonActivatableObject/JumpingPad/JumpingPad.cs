
using JetBrains.Annotations;
using Mirror;
using UnityEngine;

public class JumpingPad : ActivatableObjectEntity
{
    [CustomHeader("Jumping Pad")]
    public int jumpingPower;

    private Animator animator;


    private bool onActive;


    
    private JumpingPad_Net Net;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        Net = GetComponent<JumpingPad_Net>();
    }

    #region Get,Set

    public override T GetData<T>()
    {
        if (typeof(T) == typeof(ButtonActivatableObjectStruct))
        {
            return (T)(object)new ButtonActivatableObjectStruct(id, activeRequirAmount, transform.position, transform.rotation, transform.localScale,jumpingPower,indicator);
        }

        return default(T);
    }
    public override async void SetData<T>(T data)
    {

        base.SetData(data);

            jumpingPower = ButtonActivatedObjectStruct.jumpingPower;

            if (Application.isPlaying)
            {
                Net.jumpingPower = ButtonActivatedObjectStruct.jumpingPower;
                Net.onSync = true;
                Net.Server_InitSync();

                await util.Delay(() => { CheckActiveRequirAmount(); });
            }

        
      

        
    }

    #endregion

    private void OnTriggerEnter2D(Collider2D collision)
    {
 
        if (collision != null && collision.TryGetComponent(out NetworkIdentity component))
        {
            if (collision.TryGetComponent(out JumpingPad _)) return;

            Net.Cmd_Jumping(component.netId);
            //Debug.Log(collision.name);
        }
    }

    #region Main
    //public void Jumping(Rigidbody2D rb)
    //{
    //    if (!onActive) return;
    //    rb.velocity = Vector2.zero;
    //    rb.AddForce(Vector2.up * Net.jumpingPower, ForceMode2D.Impulse);
    //}
    #endregion

    protected override void Activation()
    {
        //onActive = true;
        Net.Server_SetOnActive(true);
        // animator.SetBool(Activated,Net.onActive);
    }
    protected override void Deactivated()
    {
        //onActive = false;
        Net.Server_SetOnActive(false);
        // animator.SetBool(Activated, Net.onActive);
    }
}
