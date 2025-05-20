using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Shield_Net : TransportItemEntity
{

    // [SyncVar] public Vector2 orgPot;

    Shield shield;
    Shield Main { get { shield ??= GetComponent<Shield>(); return shield; } }

    //todo0423
    private float targetTime = 5;
    public float curTime;

    Coroutine co_timer;

    bool onHands;
    public bool OnHads
    {
        get { return onHands; }
        set
        {
            onHands = value;
            if (!onHands)
            {
                co_timer = StartCoroutine(Co_RefreshPosition());
            }
            else
            {
                if (co_timer != null) StopCoroutine(co_timer);
            }

        }
    }

    // [Server]
    // public void Server_SetOrgPot(Vector2 orgPot)
    // {
    //     this.orgPot = orgPot;
    // }


    //todo0423
    IEnumerator Co_RefreshPosition()
    {
        curTime = 0;
        while (curTime < targetTime)
        {
            curTime += Time.deltaTime;
            yield return null;
        }
        curTime = 0;
        //transform.position = BuildObj.ObjectData.position;
        Main.Respawn();
    }

    public override void Release()
    {
        base.Release();
        OnHads = false;
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;

    }

    protected override void Grab()
    {
        base.Grab();
        OnHads = true;
    }
}
