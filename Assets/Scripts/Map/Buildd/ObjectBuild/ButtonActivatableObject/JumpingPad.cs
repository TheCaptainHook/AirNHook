
using UnityEngine;

public class JumpingPad : ActivatableObjectEntity
{
    [CustomHeader("Jumping Pad")]
    public int jumpingPower;

    private Animator animator;


    private bool onActive;

    #region  Animation
    readonly int Activated = Animator.StringToHash("Activated");
    #endregion

    private Util util;
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    #region Get,Set

    public override T GetData<T>()
    {
        if (typeof(T) == typeof(ButtonActivatableObjectStruct))
        {
            return (T)(object)new ButtonActivatableObjectStruct(id, activeRequirAmount, transform.position, transform.rotation, transform.localScale,jumpingPower);
        }

        return default(T);
    }
    public override async void SetData<T>(T data)
    {
        try
        {
            if (typeof(T) == typeof(ButtonActivatableObjectStruct))
            {
                ButtonActivatableObjectStruct objData = (ButtonActivatableObjectStruct)(object)data;
                ButtonActivatedObjectStruct = objData;
                jumpingPower = objData.jumpingPower;
            }
        }
        catch
        {
            Debug.Log($"ERROR,{typeof(T)}");
        }

        if (Application.isPlaying)
        {
            util = new Util();
            await util.Delay(() => { CheckActiveRequirAmount(); });
        }
    }

    #endregion

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if(collision.TryGetComponent(out Rigidbody2D component)){
            Jumping(component);
        }
    }

    #region Main
    public void Jumping(Rigidbody2D rb)
    {
        if (!onActive) return;
        rb.velocity = Vector2.zero;
        rb.AddForce(Vector2.up * jumpingPower, ForceMode2D.Impulse);
    }
    #endregion
    protected override void Activation()
    {
        onActive = true;
        animator.SetBool(Activated,onActive);
    }
    protected override void Deactivated()
    {
        onActive = false;
        animator.SetBool(Activated,onActive);
    }
}
