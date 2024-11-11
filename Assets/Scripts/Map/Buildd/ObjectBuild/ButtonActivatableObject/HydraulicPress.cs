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
    public bool onPrograss;
    private RaycastHit2D hit;
    private float curPressLength;

    #region Animation
    readonly int Val = Animator.StringToHash("Val");
    #endregion

    #region Components
    private Animator animator;
    #endregion

    #region Steam Ani

    [SerializeField] Animator _UPStem;
    [SerializeField] Animator _DownStem;
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
            }else{
                if(onPrograss){
                    onPrograss = false;
                }
            }
        }
        else
        {
            if(curPressLength > 0)
            {
                if(!onPrograss) onPrograss = true;
                curPressLength -= Time.fixedDeltaTime * 0.1f;
                curPressLength = Mathf.Clamp(curPressLength, 0, 1);
                animator.SetFloat(Val, curPressLength);
            }else{
                if(onPrograss) onPrograss = false;
            }
        }
    }


    private void CheckRay()
    {
        if(!onPrograss) onPrograss = true;

        hit = Physics2D.Raycast(rayPoint.position, transform.right, rayDistance);
        if (!hit)
        {
            curPressLength += Time.fixedDeltaTime * 0.1f;
            curPressLength = Mathf.Clamp(curPressLength, 0, 1);
            animator.SetFloat(Val, curPressLength);
        }else{
            if(onPrograss){
                    onPrograss = false;
                }
        }
    }

    protected override void Activation()
    {   
        SteamOn(); 
        onActive = true;
    }
    protected override void Deactivated()
    {
        SteamOn();
        onActive = false;
    }

    private  void SteamOn(){
        IsAnimationPlaying("Steam");
    }

 public void IsAnimationPlaying(string animationName)
    {
        AnimatorStateInfo stateInfo1 = _UPStem.GetCurrentAnimatorStateInfo(0);
        AnimatorStateInfo stateInfo2 = _DownStem.GetCurrentAnimatorStateInfo(0);

        if(!stateInfo1.IsName(animationName)){
            _UPStem.Play(animationName);
        }
         if(!stateInfo2.IsName(animationName)){
            _DownStem.Play(animationName);
        }

    }
   
}
