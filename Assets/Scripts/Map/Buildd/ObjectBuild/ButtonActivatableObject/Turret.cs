
using Mirror;
using UnityEngine;

public class Turret : ActivatableObjectEntity
{
    
    [CustomHeader("Turret")]
    [SerializeField] float rotateRate;
    [SerializeField] float fireRate;
    [SerializeField] bool onHoldRotation;
    [SerializeField] bool onLeft;
    private bool onActive;
    [Space(20)]
    [ReadOnly]
    [SerializeField] Transform firePoint;
    [ReadOnly]
    [SerializeField] ParticleSystem fireEffect;

    public float curTime;

    #region Fire
    public float curFireTime;
    private bool onFire;
    #endregion
   
    #region Animator
    private Animator animator;
    readonly int Left = Animator.StringToHash("Left");
    readonly int Activated = Animator.StringToHash("Activated");
    #endregion

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }



    #region Get,Set
    public override T GetData<T>()
    {
        if (typeof(T) == typeof(ButtonActivatableObjectStruct))
        {
            return (T)(object)new ButtonActivatableObjectStruct(id, activeRequirAmount, transform.position, transform.rotation, transform.localScale, rotateRate,fireRate,onHoldRotation,onLeft);
        }

        return default(T);
    }
    public override  void SetData<T>(T data)
    {
        try
        {
            if (typeof(T) == typeof(ButtonActivatableObjectStruct))
            {
                ButtonActivatableObjectStruct objData = (ButtonActivatableObjectStruct)(object)data;
                ButtonActivatedObjectStruct = objData;
                rotateRate = objData.rotateRate;
                onLeft = objData.onLeft;
                onHoldRotation = objData.onHoldRotation;
                fireRate = objData.fireRate;

                if (onLeft) animator.SetBool(Left, onLeft);

                animator.SetBool(Activated, true);
            }
        }
        catch
        {
            Debug.Log($"ERROR,{typeof(T)}");
        }
    }
    #endregion

    private void Update()
    {
        if (!onFire)
        {
            curFireTime += Time.deltaTime;
            curTime += Time.deltaTime;

            if (curTime >= rotateRate &&!onHoldRotation)
            {
                curTime = 0;
                curFireTime = 0;
                onLeft = !onLeft;
                animator.SetBool(Left, onLeft);
            }

            if (curFireTime >= fireRate)
            {
                onFire = true;
                Fire();
                Debug.Log("Fire");
            }
        }

    }

    protected override void Activation()
    {
        onLeft = !onLeft;
        curTime = 0;
        curFireTime = 0;
        animator.SetBool(Left, onLeft);
    }
    protected override void Deactivated()
    {
        Activation();
    }

    public override void CheckActiveRequirAmount()
    {
        Activation();
    }


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


    private void Fire()
    {
        ReloadAmmo();
        fireEffect.Play();

        onFire = false;
        curFireTime = 0;

    }

    private void ReloadAmmo()
    {
        Projectile_Arrow arrow =  Managers.Pooling.N_GetItme<Projectile_Arrow>().GetComponent<Projectile_Arrow>();
        Vector2 target = firePoint.TransformPoint(Vector2.zero);
        arrow.Setting(target, firePoint.right);
        arrow.gameObject.SetActive(true);

    }
#endregion

}
