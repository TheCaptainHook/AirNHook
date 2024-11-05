
using UnityEngine;

public class JumpingPad : ActivatableObjectEntity
{
    [CustomHeader("Jumping Pad")]
    public int jumpingPower;
    private bool onActive;


    private Util util;
    private void Awake()
    {
        util = new Util();
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
            await util.Delay(() => { CheckActiveRequirAmount(); });
        }
    }

    #endregion

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent(out Player component))
        {
            Jumping(component);
        }
    }

    #region Main
    public void Jumping(Player player)
    {
        if (!onActive) return;
        Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();
        playerRb.AddForce(Vector2.up * jumpingPower, ForceMode2D.Impulse);
    }
    #endregion
    protected override void Activation()
    {
        onActive = true;
    }
    protected override void Deactivated()
    {
        onActive = false;
    }
}
