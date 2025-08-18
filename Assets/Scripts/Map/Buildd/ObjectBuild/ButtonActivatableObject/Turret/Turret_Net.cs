using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Turret_Net : ActivatableObject_Net_Entity
{
    private Turret Turret => GetComponent<Turret>();

    #region Animator
    readonly int Left = Animator.StringToHash("Left");
    readonly int Activated = Animator.StringToHash("Activated");
    #endregion
    [Space(20)]
    [SerializeField] Transform firePoint;
    [SerializeField] ParticleSystem fireEffect;

    [SerializeField] GameObject ammoPrefab;
    [SerializeField] Animator animator;
    [Header("--------------------------------------------")]
    [Space(20)]

    //[SyncVar] public ButtonActivatableObjectStruct data;

    ///Init Data
    [Header("Sync Data")]
    [SyncVar] public float rotateRate;
    [SyncVar] public bool onHoldRotation;
    [SyncVar] public float fireRate;

    // [SyncVar(hook =nameof(ChangeOnLeft))] 
    public bool onLeft;
 
    [SyncVar] public bool onFire;
    [SyncVar] public float curFireTime;
    [SyncVar] public float curRotateTime;

    protected override void SetData(ButtonActivatableObjectStruct data)
    {
        base.SetData(data);
        
        
        if (isServer)
        {
            fireRate = data.fireRate;
            onHoldRotation = data.onHoldRotation;
            onLeft = data.onLeft;
            if (data.onLeft) animator.SetBool(Left, true);
        }
    }
    //onLeft : 회전
    //curFireTime : 공격 타이머
    //curRotateTime : 회전 타이머
    protected override void Active()
    {
        animator.SetBool(Activated, true);
    }
    protected override void Deactive()
    {
        animator.SetBool(Activated, false);
    }
    private void Update()
    {
        if (!isServer || !onActive) return;
        MainLogic();
    }

    [Server]
    private void MainLogic()
    {
        if (!onFire)
        {
            curFireTime += Time.deltaTime;
            curRotateTime += Time.deltaTime;

            if (curRotateTime >= data.rotateRate && !onHoldRotation)
            {
                curRotateTime = 0;
                curFireTime = 0;
                onLeft = !onLeft;
                animator.SetBool(Left, onLeft);

            }

            if (curFireTime >= fireRate)
            {
                //Fire();
                Rpc_Fire();
                curFireTime = 0;
         
            }
        }

    }


    [ClientRpc]
    private void Rpc_Fire()
    {
        Turret.Fire_Effect();
        ReloadAmmo();
    }


    private void ReloadAmmo()
    {
        //Projectile_Shell shell = Managers.Pooling.N_GetItme(typeof(Projectile_Shell).Name).GetComponent<Projectile_Shell>();
        Projectile_Shell shell = Managers.Pooling.D_GetItem(ammoPrefab).GetComponent<Projectile_Shell>();
        Vector2 target = firePoint.TransformPoint(Vector2.zero);
        shell.Setting(target, firePoint.right,gameObject);
    }


}
