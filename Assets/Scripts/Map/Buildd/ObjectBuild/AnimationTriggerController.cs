using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AnimationTrigger
{
    Idle,  // 선택되었을 때 아무 애니메이션도 실행되지 않음
    Work1,
    Work2
}

public class AnimatorTriggerController : MonoBehaviour
{
    [Header("Animation Settings")]
    [SerializeField] private Animator animator;
    [SerializeField] private AnimationTrigger selectedTrigger = AnimationTrigger.Idle;

    private void Start()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
    }

    public void PlayTrigger()
    {
        if (animator != null)
        {
            if (selectedTrigger == AnimationTrigger.Idle)
            {
                return;
            }

            animator.SetTrigger(selectedTrigger.ToString());
        }
        else
        {
            Debug.LogWarning("Animator가 설정되지 않았습니다!");
        }
    }
}
