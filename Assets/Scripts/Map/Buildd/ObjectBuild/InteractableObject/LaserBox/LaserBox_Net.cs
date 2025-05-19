using Mirror;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Animations;

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


    public Vector2 curLaserDir;
    private ParentConstraint parentConstraint;
    private ParentConstraint ParentConstraint { get { parentConstraint ??= GetComponent<ParentConstraint>(); return parentConstraint; } }
   
    [Command(requiresAuthority = false)]
    public void Cmd_SetLaserDir(Vector2 dir)
    {
        Rpc_SetLaserDir(dir);
    }

    [ClientRpc]
    public void Rpc_SetLaserDir(Vector2 dir)
    {
        curLaserDir = dir;
    }



    protected override void Grab()
    {
        base.Grab();
        getDirCoroutine = StartCoroutine(GetDirCo());
    }

    public override void Release()
    {
        base.Release();
        StopCoroutine(getDirCoroutine);
    }


    Coroutine getDirCoroutine;

    IEnumerator GetDirCo()
    {
        while (true)
        {
            var dir = GetDir();
            if(curLaserDir != dir)
            {
                Cmd_SetLaserDir(dir);
            }
            yield return null;
        }
    }

    private Vector2 GetDir()
    {
        if (ParentConstraint.sourceCount > 0)
        {
            Transform source = parentConstraint.GetSource(0).sourceTransform;
            Transform parent = source.parent.parent;
            float y = parent.rotation.y;

            if (y == 0) return Vector2.right;
            else return Vector2.left;
        }

        return Vector2.right;
    }


    #region BOOM

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
    #endregion

}
