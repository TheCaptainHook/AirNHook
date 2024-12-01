
using System.Collections;
using UnityEngine;

public class PullLever : ButtonEntity
{
   [CustomHeader("Pull Lever")]
    private readonly int _PullingAmount = Animator.StringToHash("PullingAmount");
    private readonly int _IsActive = Animator.StringToHash("IsActive");

   [Header("TEST CODE")]
    Animator animator;
    //TEST CODE
    public float duration; //revoke prograss, lever length
    public float curDuration;
    public float prograssSpeed; //lever pulling speed 
    public bool _OnPrograss;
    public float conditionWeight;
    public float curWeight;

    //TEST CODE
    private void Start(){
        animator = GetComponent<Animator>();
        StartCoroutine(Prograss());
    }


    private void OnDisable(){
        StopAllCoroutines();
    }
    private void Update(){
       
         if (!_OnPrograss && curDuration > 0)
        {
            curDuration -= Time.deltaTime;

        }
         // Check duration and only call LeverPulling when necessary
        if (curDuration <= 0){
            LeverPulling(-prograssSpeed * Time.deltaTime);
        }   
    }

    protected override void Activation()
    {
        
    }
    protected override void Deactivated()
    {
        
    }

    IEnumerator Prograss(){
          while (true){
            if(_OnPrograss){
                 curDuration = duration;
                 // 조건 충족 시 레버를 당기기
                if (conditionWeight <= curWeight)
                {
                 LeverPulling(prograssSpeed * Time.deltaTime);
                }else{
                 LeverPulling(-prograssSpeed * Time.deltaTime);
                }
            }
            yield return null;
        }
    }

    private void LeverPulling(float rate){

        float cur = animator.GetFloat(_PullingAmount);
        float newRate = Mathf.Clamp(cur + rate, 0, 1);

        if (Mathf.Approximately(cur, newRate)) // 위치가 변하지 않았으면 굳이 갱신하지 않음
        {
            return;
        }

        animator.SetFloat(_PullingAmount,newRate);

        if(newRate == 1){
            animator.SetBool(_IsActive,true);
            Debug.Log("Active");
        }else if(newRate == 0){
            animator.SetBool(_IsActive,false);
            Debug.Log("Deactive");
        }
        

        // 레버가 최대 또는 최소 길이에 도달했는지 확인
    }


}
