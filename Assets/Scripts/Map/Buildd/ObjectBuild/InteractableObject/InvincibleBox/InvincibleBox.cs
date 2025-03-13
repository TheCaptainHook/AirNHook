
using UnityEngine;


public class InvincibleBox : BuildObj
{
    private void Awake()
    {
        DissolveInitSetting();
    }




    private InvincibleBox_Net Net => GetComponent<InvincibleBox_Net>();
    public override void SetData<T>(T data)
    {
        base.SetData(data);
        Net.onSync = true;
        Net.Server_InitSync();
    }

    public override void TurnOff()
    {
        base.TurnOff();
        _rb.gravityScale = 0;
        _rb.velocity = Vector2.zero;
    }
    public override void TurnOn()
    {
        base.TurnOn();
        _rb.gravityScale = 1;
    }
}
