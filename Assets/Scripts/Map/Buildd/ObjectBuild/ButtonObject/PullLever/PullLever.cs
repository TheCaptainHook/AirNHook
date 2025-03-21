
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class PullLever : ButtonEntity
{
    [CustomHeader("Pull Lever")]
    private readonly int _PullingAmount = Animator.StringToHash("PullingAmount");
    private readonly int _IsActive = Animator.StringToHash("IsActive");

    [Header("TEST CODE")]
    Animator animator;
    //TEST CODE
    //public float duration; //revoke prograss, lever length
    //public float curDuration;
    public float prograssSpeed; //lever pulling speed 
    //public bool _OnPrograss;

    //public float conditionWeight;
    //public float curWeight;

    //TEST CODE
    private void Start()
    {
        animator = GetComponent<Animator>();
        //StartCoroutine(Prograss());
    }


    //private void OnDisable(){
    //    StopAllCoroutines();
    //}
    private void Update()
    {

        // if (!_OnPrograss && curDuration > 0)
        //{
        //    curDuration -= Time.deltaTime;

        //}
        // // Check duration and only call LeverPulling when necessary
        //if (curDuration <= 0){
        //    LeverPulling(-prograssSpeed * Time.deltaTime);
        //}   
        //if(Input.GetKeyDown(KeyCode.P))
        //{
        //    Pulling(true);
        //}
        //if (Input.GetKeyDown(KeyCode.O))
        //{
        //    Pulling(false);
        //}
    }

    protected override void Activation()
    {
        PrograssButtonActivatedObject(true);
    }
    protected override void Deactivated()
    {
        PrograssButtonActivatedObject(false);
    }

    Coroutine pullingCoroutine;
    private void Pulling(bool onOff)
    {
        if (pullingCoroutine != null) StopCoroutine(pullingCoroutine);
        pullingCoroutine = StartCoroutine(PullingCo(onOff));
    }



    IEnumerator PullingCo(bool onOff)
    {
        if (!onOff)
        {
            animator.SetBool(_IsActive, false);
            Debug.Log("Deactive");
        }

        float t = onOff ? prograssSpeed * Time.deltaTime : -prograssSpeed * Time.deltaTime;
        float cur = animator.GetFloat(_PullingAmount);

        while (0 <= cur && cur <= 1)
        {
            cur += t;
            animator.SetFloat(_PullingAmount, cur);
            yield return null;
        }

        cur = Mathf.Clamp(cur, 0, 1);
        animator.SetFloat(_PullingAmount, cur);

        if (cur == 1)
        {
            animator.SetBool(_IsActive, true);
            Debug.Log("Active");
        }

    }


    #region Net

    #endregion

}


/**
 

 **/
