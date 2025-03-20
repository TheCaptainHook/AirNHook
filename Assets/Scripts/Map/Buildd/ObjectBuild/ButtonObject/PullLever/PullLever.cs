
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class PullLever : ButtonEntity,IInteractable
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
    private void Start(){
        animator = GetComponent<Animator>();
        //StartCoroutine(Prograss());
    }


    private void OnDisable(){
        StopAllCoroutines();
    }
    private void Update(){
       
        // if (!_OnPrograss && curDuration > 0)
        //{
        //    curDuration -= Time.deltaTime;

        //}
        // // Check duration and only call LeverPulling when necessary
        //if (curDuration <= 0){
        //    LeverPulling(-prograssSpeed * Time.deltaTime);
        //}   
        if(Input.GetKeyDown(KeyCode.P))
        {
            Pulling(true);
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            Pulling(false);
        }
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
        if(pullingCoroutine != null) StopCoroutine(pullingCoroutine);
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

        while (0 <= cur && cur<=1)
        {
            cur += t;
            animator.SetFloat(_PullingAmount, cur);
            yield return null;
        }

        cur = Mathf.Clamp(cur, 0, 1);
        animator.SetFloat(_PullingAmount,cur);

        if (cur == 1)
        {
            animator.SetBool(_IsActive, true);
            Debug.Log("Active");
        }
        
    }

    //private void LeverPulling(float rate){

    //    float cur = animator.GetFloat(_PullingAmount);
    //    float newRate = Mathf.Clamp(cur + rate, 0, 1);

    //    if (Mathf.Approximately(cur, newRate)) // 위치가 변하지 않았으면 굳이 갱신하지 않음
    //    {
    //        return;
    //    }

    //    animator.SetFloat(_PullingAmount,newRate);

    //    if(newRate == 1){
    //        animator.SetBool(_IsActive,true);
    //        Debug.Log("Active");
    //    }else if(newRate == 0){
    //        animator.SetBool(_IsActive,false);
    //        Debug.Log("Deactive");
    //    }


    //    // 레버가 최대 또는 최소 길이에 도달했는지 확인
    //}
    #region Net

    #endregion

    #region Interactable
    public ObjectTypeEnum _objectType = ObjectTypeEnum.Interaction;


    public void Interaction(Transform accessor = null) 
    {
        //Pulling(true);
    }

    public bool CanInteract() { return true; }

    public void Interacting(bool value) { return; }

    public ObjectTypeEnum GetObjectType()
    {
        return _objectType;
    }
    public void ShowEButton()
    {
        return;

    }

    public void HideEButton()
    {
        return;
    }
    #endregion
}


/**
 

 **/
