using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrongBox : BuildObj
{

    private Rigidbody2D _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
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
