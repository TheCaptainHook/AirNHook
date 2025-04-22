using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Turret_Net : NetworkBehaviour
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

    [SyncVar(hook =nameof(ChangeOnLeft))] 
    public bool onLeft;
 
    [SyncVar] public bool onFire;
    [SyncVar] public float curFireTime;
    [SyncVar] public float curRotateTime;


    //onLeft : 회전
    //curFireTime : 공격 타이머
    //curRotateTime : 회전 타이머

    #region Init Sync
    public bool onSync;
    [Server]
    public void Server_InitSync()
    {
        Rpc_InitSync(Turret.ButtonActivatedObjectStruct);
    }
    [ClientRpc]
    private void Rpc_InitSync(ButtonActivatableObjectStruct data)
    {
        if (onSync) return;
        rotateRate = data.rotateRate;
        onHoldRotation = data.onHoldRotation;
        fireRate = data.fireRate;
        onLeft = data.onLeft;                                                                   

        gameObject.transform.position = data.position;
        gameObject.transform.rotation = data.quaternion;
        //Turret.TurnOnAnimation();
        animator.SetBool(Activated, true);
        onSync = true;
    }
    [Command(requiresAuthority = false)]
    private void Cmd_InitSync()
    {
        Server_InitSync();
    }
    public override void OnStartClient()
    {
        base.OnStartClient();
        if (!onSync) Cmd_InitSync();
    }
    #endregion


    private void Update()
    {
        if (!isServer) return;
        MainLogic();
    }

    [Server]
    private void MainLogic()
    {
        if (!onFire)
        {
            curFireTime += Time.deltaTime;
            curRotateTime += Time.deltaTime;

            if (curRotateTime >= rotateRate && !onHoldRotation)
            {
                curRotateTime = 0;
                curFireTime = 0;
                onLeft = !onLeft;

            }

            if (curFireTime >= fireRate)
            {
                //Fire();
                Rpc_Fire();
                curFireTime = 0;
         
            }
        }

    }

    #region Server
    //[Server]
    //public void SetData(ButtonActivatableObjectStruct data)
    //{
    //    rotateRate = data.rotateRate;
    //    onHoldRotation = data.onHoldRotation;
    //    fireRate = data.fireRate;
    //    onLeft = data.onLeft;

    //    gameObject.transform.position = data.position;
    //    //Turret.TurnOnAnimation();
    //    animator.SetBool(Activated, true);
    //}

    private void ChangeOnLeft(bool old, bool newVal)
    {
        //Turret.RotateAnimation(newVal);
        animator.SetBool(Left, newVal);

    }
    #endregion


    [Server]
    private void Server_Activation()
    {
        curFireTime = 0;
        curRotateTime = 0;
        onLeft = !onLeft; 
    }
    [Command]
    public void Cmd_Activation()
    {
        Server_Activation();
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
        shell.gameObject.SetActive(true);
    }


}
