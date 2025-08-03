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

    [SerializeField] GameObject left;
    [SerializeField] GameObject right;

    private float maxCharge = 0.2f;
    private float minCharge = 0.1f;
    private float maxCount = 500;
    private float curCount = 0;


    float percent;

    // [Server]
    // public void Server_DamageCount()
    // {
    //     if(shutDownCoroutine == null)
    //     {
    //         curShutDownDelayCount = maxShutDownDelayCount;
    //         shutDownCoroutine = StartCoroutine(ShutDownDelay());
    //     }
    //     else{
    //         curShutDownDelayCount = maxShutDownDelayCount;
    //     }

    //     curCount++;
    //     percent = curCount / maxCount;
    //     float scale = Mathf.Lerp(minCharge, maxCharge, percent);

    //     if(curCount>= maxCount)
    //     {
    //         StopCoroutine(ShutDownDelay());
    //         //BOOM,Rpc
    //         Server_Boom();
    //         //BOOM
    //     }
    //     else
    //     {
    //         Rpc_ChangeFillSprite(scale);
    //     }
    // }


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
        if (dir == Vector2.right)
        {
            if (left.activeSelf) left.SetActive(false);
            right.SetActive(true);
            
        }
        else
        {
            if (right.activeSelf) right.SetActive(false);
            left.SetActive(true);
        }
    }

    float offset = 0.4f;

    protected override void Grab()
    {
        base.Grab();
        if (getDirCoroutine != null)
        {
            StopCoroutine(getDirCoroutine);
            getDirCoroutine = null;
        }

        getDirCoroutine = StartCoroutine(GetDirCo(false));
    }

    public override void Release(GameObject accssor = null)
    {
        base.Release(accssor);

        if (getDirCoroutine != null)
        {
            StopCoroutine(getDirCoroutine);    
        }
        
        getDirCoroutine = null;
    }
    #region Hook Grab

    Coroutine getDirCoroutine;

    /// <summary>
    /// </summary>
    /// <param name="airHook">true : Air, false : Hook</param>
    /// <returns></returns>
    IEnumerator GetDirCo(bool airHook)
    {
        while (true)
        {
            var dir = GetDir(airHook);
            if(curLaserDir != dir)
            {
                Cmd_SetLaserDir(dir);
            }
            yield return null;
        }
    }
  
   
    private Vector2 GetDir(bool airHook)
    {
        if (ParentConstraint.sourceCount > 0)
        {
            Transform source = parentConstraint.GetSource(0).sourceTransform;
            Transform parent = airHook ? source.parent : source.parent.parent;
            float y = parent.rotation.y;

            if (y == 0) return Vector2.right;
            else return Vector2.left;
        }

        return Vector2.right;
    }

    #endregion

    #region Air Inhaling
    private Coroutine checkFixedGunCoroutine;
    public override void Inhalation(Transform accessor)
    {
        base.Inhalation(accessor);
        
        if (checkFixedGunCoroutine == null)
            checkFixedGunCoroutine = StartCoroutine(CheckFixedGun());
    }
    public override void Fixed(bool value)
    {
        base.Fixed(value);
        if (!value)
        {
            if (getDirCoroutine != null)
            {
                StopCoroutine(getDirCoroutine);
                getDirCoroutine = null;
            }

            if (checkFixedGunCoroutine != null)
            {
                StopCoroutine(checkFixedGunCoroutine);
                checkFixedGunCoroutine = null;
            }
        }

    }

    private IEnumerator CheckFixedGun()
    {
        while (!_isFixed)
        {
            yield return null;
        }

       if (getDirCoroutine != null)
        {
            StopCoroutine(getDirCoroutine);
            getDirCoroutine = null;
        }

        getDirCoroutine = StartCoroutine(GetDirCo(true));

    }
    /**
    에어가 흡입할때 InHaling인지
    에어가 흡입 취소할때 StopInhale 인지
    
    Air.Inhaling -> item.Inhalation -> 
    Air.FixInhaleTarget
    
        1. Air 에서 inhailing 호출
        2. inhailing 호출하면 실행하는 코루틴 실행
        3. 이이탬이 완전히 airgun에 붙었는지 까지 대기 후 airgun 위치에 따라 방향 전환

        코루틴으로 체크,
        -> _isFixed = true
        -> ConstraintParent.Source 가 없으면 
        
    **/
    #endregion

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
        // Rpc_Boom();
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

    // [ClientRpc]
    // private void Rpc_Boom()
    // {
    //     Main.onBoom = true;
    // }

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
