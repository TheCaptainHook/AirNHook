using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : BuildObj
{
    Rigidbody2D rb;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }


    public override void TurnOff()
    {
        base.TurnOff();
        rb.gravityScale = 0;
    }
    public override void TurnOn()
    {
        base.TurnOn();
        rb.gravityScale = 1;
    }
}
