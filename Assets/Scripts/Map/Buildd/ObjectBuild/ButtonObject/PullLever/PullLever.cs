
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class PullLever : ButtonEntity
{
   
    private readonly int _PullingAmount = Animator.StringToHash("PullingAmount");
    private readonly int _IsActive = Animator.StringToHash("IsActive");
    Animator animator;

    [CustomHeader("Pull Lever")]
    [ReadOnly]
    [SerializeField] PullLever_Net Net;

    public float prograssSpeed; //lever pulling speed 

  

    //TEST CODE
    private void Start()
    {
        animator = GetComponent<Animator>();
   
    }


    public override void Activation()
    {
        PrograssButtonActivatedObject(true);
    }
    public override void Deactivated()
    {
        PrograssButtonActivatedObject(false);
    }

    Coroutine pullingCoroutine;
    public void Pulling(bool onOff)
    {
        if (pullingCoroutine != null) StopCoroutine(pullingCoroutine);
        pullingCoroutine = StartCoroutine(PullingCo(onOff));
    }



    IEnumerator PullingCo(bool onOff)
    {
        if (!onOff)
        {
            animator.SetBool(_IsActive, false);
            if (Net.onPrograssButtonActivatedObject)
            {
                Net.Cmd_SetonPrograssButtonActivatedObject(false);
                Deactivated();
            }
           
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
            Net.Cmd_SetonPrograssButtonActivatedObject(true);
            Activation();
        }

    }



}


/**
 0322
 player가 죽었을 때 걸어놓을 이벤트

 **/
