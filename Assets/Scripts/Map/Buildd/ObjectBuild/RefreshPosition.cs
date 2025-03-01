using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class RefreshPosition : InteractableObject
{
    // [SerializeField] BuildObj buildObj;


    [SyncVar] public Vector2 orgPot;

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

    [Server]
    public void Server_SetOrgPot(Vector2 orgPot)
    {
        this.orgPot = orgPot;
    }


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
        transform.position = orgPot;
    }

    public override void Release()
    {
        base.Release();
        OnHads = false;

    }

    protected override void Grab()
    {
        base.Grab();
        OnHads = true;
    }

}
