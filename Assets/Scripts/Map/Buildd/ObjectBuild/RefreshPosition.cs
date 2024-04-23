using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RefreshPosition : InteractableObject
{
    [SerializeField] BuildObj buildObj;


    //todo0423
    private float targetTime = 2;
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
        transform.position = buildObj.ObjectData.position;
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
