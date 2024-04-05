using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SawObject : BuildObj
{
    [SerializeField] GameObject hitBox;
    [SerializeField] GameObject sawPivot;
    Animator animator;

    private void Awake()
    {
        animator = sawPivot.GetComponent<Animator>();
    }


    public override void TurnOff()
    {
        base.TurnOff();
        animator.enabled = false;
        hitBox.SetActive(false);
    }


    public override void TurnOn()
    {
        
        base.TurnOn();
        animator.enabled = true;
        hitBox.SetActive(true);
    }

}
