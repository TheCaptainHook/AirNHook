
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

    [ReadOnly]
    [SerializeField] ParticleSystem fireEffect;

    // private Animator animator;
    // private Animator Animator
    // {
    //     get
    //     {
    //         animator ??= GetComponent<Animator>();
    //         return animator;
    //     }
    // }

    #region Network
    // private Turret_Net Turret_Net => GetComponent<Turret_Net>();
    #endregion

    #region Get,Set
    public override T GetData<T>()
    {
        if (typeof(T) == typeof(ButtonActivatableObjectStruct))
        {
            return (T)(object)new ButtonActivatableObjectStruct(id, transform.position, transform.rotation, transform.localScale,activeRequirAmount, rotateRate,fireRate,onHoldRotation,onLeft);
        }

        return default(T);
    }

    protected override void AdditionalInspectorConfig()
    {
        rotateRate = ButtonActivatedObjectStruct.rotateRate;
        onLeft = ButtonActivatedObjectStruct.onLeft;
        onHoldRotation = ButtonActivatedObjectStruct.onHoldRotation;
        fireRate = ButtonActivatedObjectStruct.fireRate;
    }
    #endregion


    private static readonly int IsActive = Animator.StringToHash("IsActive");
    public override void Activation()
    {
        Animator.SetBool(IsActive, true);

        Net.Server_ChangeOnActive(true);
    }
    public override void Deactivated()
    {
        Animator.SetBool(IsActive, false);
        
        Net.Server_ChangeOnActive(false);
    }


#region Main
    
    public void Fire_Effect()
    {
        fireEffect.Play();
    }


#endregion

}
