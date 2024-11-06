
using UnityEngine;

public class Turret : ActivatableObjectEntity
{
    
    [CustomHeader("Turret")]
    [SerializeField] float rotateRate;
    [SerializeField] float fireRate;
    [SerializeField] bool onLeft;
    private bool onFindTarget;
    private bool onActive;
    [Space(20)]
    [ReadOnly]
    [SerializeField] Transform firePoint;
    [ReadOnly]
    [SerializeField] ParticleSystem fireEffect;

    public float curTime;
    private float rayDistance;
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
        rayDistance = 50;
    }



    #region Get,Set
    public override T GetData<T>()
    {
        if (typeof(T) == typeof(ButtonActivatableObjectStruct))
        {
            return (T)(object)new ButtonActivatableObjectStruct(id, activeRequirAmount, transform.position, transform.rotation, transform.localScale, rotateRate,fireRate,onLeft);
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
        curTime += Time.deltaTime;
        curFireTime += Time.deltaTime;

        if(curTime >= rotateRate && !onFindTarget)
        {
            curTime = 0;
            onLeft = !onLeft;
            animator.SetBool(Left, onLeft);
        }

        if (curFireTime >= fireRate && !onFire && onFindTarget)
        {
            onFire = true;
        }

    }
    private void FixedUpdate()
    {

        TrackOrFire();

    }

    protected override void Activation()
    {
        onLeft = !onLeft;
        curTime = 0;
        animator.SetBool(Left, onLeft);
    }
    protected override void Deactivated()
    {
        base.Deactivated();
    }

    public override void CheckActiveRequirAmount()
    {
        Activation();
    }


    #region Main
    private void TrackOrFire()
    {
        RaycastHit2D hit = Physics2D.Raycast(firePoint.position, firePoint.right * rayDistance);

        if (hit)
        {
            if(hit.collider.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                onFindTarget = true;
                Fire();
            }
            else
            {
                onFindTarget = false;
            }
        }
        else
        {
            onFindTarget = false;
        }
       
    }


    private void Fire()
    {
        if (onFire && onFindTarget)
        {
            Debug.Log("fire");
            fireEffect.Play();
            onFire = false;
            curFireTime = 0;
            curTime = 0;
        }
    }
    #endregion

}
