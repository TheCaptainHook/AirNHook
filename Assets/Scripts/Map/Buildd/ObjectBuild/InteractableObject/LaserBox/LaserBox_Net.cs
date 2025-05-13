using Mirror;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class LaserBox_Net : TransportItemEntity
{

    LaserBox main;
    LaserBox Main { get { main ??= GetComponent<LaserBox>(); return main; } }

    [SerializeField] Transform chargeTransform;

    private float maxCharge = 0.2f;
    private float minCharge = 0.1f;
    private float maxCount = 500;
    private float curCount = 0;


    float percent;

    [Server]
    public void Server_DamageCount()
    {
        if(shutDownCoroutine == null)
        {
            curShutDownDelayCount = maxShutDownDelayCount;
            shutDownCoroutine = StartCoroutine(ShutDownDelay());
        }
        else{
            curShutDownDelayCount = maxShutDownDelayCount;
        }

        curCount++;
        percent = curCount / maxCount;
        float scale = Mathf.Lerp(minCharge, maxCharge, percent);

        if(curCount>= maxCount)
        {
            StopCoroutine(ShutDownDelay());
            //BOOM,Rpc
            Server_Boom();
            //BOOM
        }
        else
        {
            Rpc_ChangeFillSprite(scale);
        }
    }
    Coroutine boomCoroutine;
    [Server]
    private void Server_Boom()
    {
        //Boom
        boomCoroutine = StartCoroutine(BoomCoroutine());
        //Boom

    }
    IEnumerator BoomCoroutine()
    {
        //Boom
        //OverlapCircle.

        Rpc_Boom();
        //Boom
        yield return new WaitForSeconds(1f);
        Rpc_Reset();
        Main.Respawn();
        boomCoroutine = null;
    }
    float maxShutDownDelayCount = 5;
    public float curShutDownDelayCount = 0;
    Coroutine shutDownCoroutine;
    Coroutine recoverCoroutine;

    IEnumerator ShutDownDelay()
    {
        if(recoverCoroutine != null) StopCoroutine(recoverCoroutine);

        while(0 < curShutDownDelayCount)
        {
            curShutDownDelayCount -= Time.deltaTime;
            yield return null;
        }
        shutDownCoroutine = null;

        recoverCoroutine = StartCoroutine(Recover());
       
    }
    IEnumerator Recover()
    {
        while(0 < curCount)
        {
            curCount--;
            percent = curCount / maxCount;
            float scale = Mathf.Lerp(minCharge, maxCharge, percent);
            Rpc_ChangeFillSprite(scale);
            yield return null;
        }

    }    

   
    [ClientRpc]
    public void Rpc_ChangeFillSprite(float scale)
    {

        chargeTransform.localScale = new Vector2(scale, scale);

    }

    [ClientRpc]
    private void Rpc_Boom()
    {
        Main.onBoom = true;
    }
    [ClientRpc]
    private void Rpc_Reset()
    {
        Main.Reset();

        curCount = 0;
        curShutDownDelayCount = 0;
        chargeTransform.localScale = new Vector3(minCharge, minCharge);
    }


}
