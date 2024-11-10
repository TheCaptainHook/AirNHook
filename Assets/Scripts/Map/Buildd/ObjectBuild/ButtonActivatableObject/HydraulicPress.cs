using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HydraulicPress : ActivatableObjectEntity
{
    [CustomHeader("HydraulicPress")]
    [SerializeField] Transform rayPoint;
    [SerializeField] float rayDistance;
    public bool onActive;

    private RaycastHit2D hit;
    private float curPressLength;

    private Vector2 hitPoint;

    #region Animation
    readonly int Activated = Animator.StringToHash("Activated");
    readonly int Val = Animator.StringToHash("Val");
    #endregion

    #region Components
    private Animator animator;
    #endregion




    private void Awake()
    {
        animator = GetComponent<Animator>();
    }


    private void Update()
    {
        Debug.DrawRay(rayPoint.position, transform.right * rayDistance, Color.red);
        if (onActive)
        {
            if (curPressLength < 1)
            {
                CheckRay();
            }
        }
        else
        {
            if(curPressLength > 0)
            {
                curPressLength -= Time.fixedDeltaTime * 0.1f;
                curPressLength = Mathf.Clamp(curPressLength, 0, 1);
                animator.SetFloat(Val, curPressLength);
            }
        }
    }


    private void CheckRay()
    {
        hit = Physics2D.Raycast(rayPoint.position, transform.right, rayDistance);
        if (!hit)
        {
            curPressLength += Time.fixedDeltaTime * 0.1f;
            curPressLength = Mathf.Clamp(curPressLength, 0, 1);
            animator.SetFloat(Val, curPressLength);
        }
    }

    protected override void Activation()
    {    
        onActive = true;
        animator.SetBool(Activated, onActive);
    }
    protected override void Deactivated()
    {
        onActive = false;
        animator.SetBool(Activated, onActive);
    }
}
