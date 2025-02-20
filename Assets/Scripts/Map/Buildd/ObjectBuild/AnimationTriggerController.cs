using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AnimationTriggerType
{
    Idle,  // 선택되었을 때 아무 애니메이션도 실행되지 않음
    Work1,
    Work2
}

public class AnimatorTriggerController : MonoBehaviour
{
    [Header("Animation Settings")]
    private Animator animator;
    private Animator Animator {
        get{
            if(animator == null) animator = GetComponent<Animator>();
            return animator;
        }
    }
    // private AnimationTriggerType selectedTrigger = AnimationTriggerType.Idle;

    // private void Start()
    // {
    //     if (animator == null)
    //     {
    //         animator = GetComponent<Animator>();
    //     }
    // }

    public void PlayTrigger(AnimationTriggerType type)
    {
        if (Animator != null)
        {
            if (type == AnimationTriggerType.Idle)
            {
                return;
            }

            Animator.SetTrigger(type.ToString());
        }
        else
        {
            Debug.LogWarning("Animator가 설정되지 않았습니다!");
        }
    }
}
