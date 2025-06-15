
using System;

using UnityEngine;

public class Turret : ActivatableObjectEntity
{
    
    [CustomHeader("Turret")]
    [SerializeField] float rotateRate;
    [SerializeField] float fireRate;
    [Header("Hold")]
    [SerializeField] bool onHoldRotation;
    [Header("Start Left")]
    [SerializeField] bool onLeft;
    private bool onActive;
    [Space(20)]
    [ReadOnly]
    [SerializeField] Transform firePoint;
    [ReadOnly]
    [SerializeField] ParticleSystem fireEffect;

    public float curTime;

    #region Network
    private Turret_Net Turret_Net => GetComponent<Turret_Net>();
    #endregion

    #region Fire
    public float curFireTime;
    private bool onFire;
    #endregion
   
    #region Animator
    private Animator animator;
    readonly int Left = Animator.StringToHash("Left");
    readonly int Activated = Animator.StringToHash("Activated");
    #endregion


    #region Get,Set
    public override T GetData<T>()
    {
        if (typeof(T) == typeof(ButtonActivatableObjectStruct))
        {
            return (T)(object)new ButtonActivatableObjectStruct(id, activeRequirAmount, transform.position, transform.rotation, transform.localScale, rotateRate,fireRate,onHoldRotation,onLeft);
        }

        return default(T);
    }
    public override void SetData<T>(T data)
    {
        base.SetData(data);
        
        animator = GetComponent<Animator>();
        rotateRate = ButtonActivatedObjectStruct.rotateRate;
        onLeft = ButtonActivatedObjectStruct.onLeft;
        onHoldRotation = ButtonActivatedObjectStruct.onHoldRotation;
        fireRate = ButtonActivatedObjectStruct.fireRate;
        
        if (Application.isPlaying)
        {
            animator = GetComponent<Animator>();
            Turret_Net.Server_InitSync();
        }
        
    }
    #endregion

    //private void Update()
    //{
    //    if (!onFire)
    //    {
    //        curFireTime += Time.deltaTime;
    //        curTime += Time.deltaTime;

    //        if (curTime >= rotateRate &&!onHoldRotation)
    //        {
    //            curTime = 0;
    //            curFireTime = 0;
    //            onLeft = !onLeft;
    //            animator.SetBool(Left, onLeft);
    //        }

    //        if (curFireTime >= fireRate)
    //        {
    //            onFire = true;
    //            //Fire();
    //            Turret_Net.Cmd_Fire();
    //        }
    //    }

    //}
    //public void RotateAnimation(bool onLeft)
    //{
    //    animator.SetBool(Left, onLeft);
    //}

    protected override void Activation()
    {
        //onLeft = !onLeft;
        //curTime = 0;
        //curFireTime = 0;
        //animator.SetBool(Left, onLeft);
        Turret_Net.Server_Activation();
    }
    protected override void Deactivated()
    {
        Turret_Net.Server_Activation();
    }

    public override void CheckActiveRequirAmount()
    {
        Activation();
    }

    //public void TurnOnAnimation()
    //{
    //    animator.SetBool(Activated, true);
    //}

    #region Main
    //private void TrackOrFire()
    //{
    //    RaycastHit2D hit = Physics2D.Raycast(firePoint.position, firePoint.right * rayDistance);

    //    if (hit)
    //    {
    //        if(hit.collider.gameObject.layer == LayerMask.NameToLayer("Player"))
    //        {
    //            onFindTarget = true;
    //            Fire();
    //        }
    //        else
    //        {
    //            onFindTarget = false;
    //        }
    //    }
    //    else
    //    {
    //        onFindTarget = false;
    //    }
       
    //}


    //private void Fire()
    //{
    //    ReloadAmmo();
    //    fireEffect.Play();

    //    onFire = false;
    //    curFireTime = 0;

    //}
    
    public void Fire_Effect()
    {
        fireEffect.Play();
    }


    //private void ReloadAmmo()
    //{
    //    // Projectile_Arrow arrow =  Managers.Pooling.N_GetItme<Projectile_Arrow>().GetComponent<Projectile_Arrow>();
    //    Projectile_Shell shell = Managers.Pooling.N_GetItme(typeof(Projectile_Shell).Name).GetComponent<Projectile_Shell>();
    //    Vector2 target = firePoint.TransformPoint(Vector2.zero);
    //    shell.Setting(target, firePoint.right);
    //    shell.gameObject.SetActive(true);

    //}
#endregion

}
