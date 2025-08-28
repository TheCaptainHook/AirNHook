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
    #endregion


}
