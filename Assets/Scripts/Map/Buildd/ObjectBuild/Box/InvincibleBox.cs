
using UnityEngine;


public class InvincibleBox : BuildObj
{
    public float health = 1f;


    private void Awake()
    {
        DissolveInitSetting();
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
