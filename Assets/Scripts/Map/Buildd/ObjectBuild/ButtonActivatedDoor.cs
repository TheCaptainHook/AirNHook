using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class ButtonActivatedDoor : ActivatableObjectEntity
{
    [CustomHeader("ButtonActivatedDoor")]
    [SerializeField] private BoxCollider2D _collider;
    [SerializeField] NetworkAnimator _animator;

    #region StringCache
    private static readonly int UnlockTrigger = Animator.StringToHash("UnlockTrigger");
    private static readonly int LockTrigger = Animator.StringToHash("LockTrigger");
    #endregion

    public bool onOpen;

    public bool onPrograss;

    public override void ApplyActive(int num)
    {
        CurActiveBtn = num;
    }


    protected override void Activation()
    {
        if (onPrograss) return;
        if (onOpen) return;
        onOpen = true;
        _collider.enabled = false;
        _animator.SetTrigger(UnlockTrigger);
        Debug.Log("OpenOpen");
        //StartCoroutine(Co_Activation());
    }


    protected override void Deactivated()
    {
          if (onPrograss) return;
        if (!onOpen) return;

        onOpen = false;
        _collider.enabled = true;
        _animator.SetTrigger(LockTrigger);

        Debug.Log("Deactivate");
        //StartCoroutine(Co_Deactivated());
    }

}

    // IEnumerator Co_Activation()
    //     {
    //         onPrograss = true;

    //         onOpen = true;
    //         _collider.enabled = false;
    //         _animator.SetTrigger(UnlockTrigger);

    //         Debug.Log("OpenOpen");
    //         yield return new WaitForSeconds(0.5f);
    //         _animator.SetTrigger(UnlockTrigger);
    //         onPrograss = false;
    //     }


    //     IEnumerator Co_Deactivated()
    //     {
    //         onPrograss = true;

    //         onOpen = false;
    //         _collider.enabled = true;
    //         _animator.SetTrigger(LockTrigger);
    //         yield return new WaitForSeconds(0.5f);
    //         _animator.SetTrigger(LockTrigger);
    //         onPrograss = false;
    //     }






