using System.Collections;
using UnityEngine;


public class Puzzle_1_Button : MonoBehaviour
{

    private Animator animator;
    
    private readonly int FULLNESS = Animator.StringToHash("Fullness");
    private readonly int EXPLODE = Animator.StringToHash("Explode");
 
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }


    #region NetWork

    public void SetAnimation(float rate)
    {
        float val = Mathf.Clamp01(rate);
        animator.SetFloat(FULLNESS, val);
    }
    public void SetAnimation_Explode()
    {
        animator.SetTrigger(EXPLODE);
    }
    #endregion


}
