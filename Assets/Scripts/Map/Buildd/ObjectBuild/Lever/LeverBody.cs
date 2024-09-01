using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class LeverBody : ButtonEntity, IInteractable
{
    [CustomHeader("LeverBody")]
    public LeverHead leverHead;
    public Transform attachedLeverHead;

    ObjectTypeEnum objectTypeEnum = ObjectTypeEnum.Interaction;
     private LeverBodyNet _leverBodyNet;

    [Header("State")]
    public bool onCompletionParts;
    bool onAcitve;
    bool onOperation;

    [Header("Components")]
    Animator animator;


    private static readonly int OnActive = Animator.StringToHash("OnActive");
    private static readonly int OnCompletion = Animator.StringToHash("OnCompletion");

    private void Awake()
    {
        _leverBodyNet = GetComponent<LeverBodyNet>();
        animator = GetComponent<Animator>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<LeverHead>())
        {
            if (!onCompletionParts)
            {
                LeverHead leverHead = collision.gameObject.GetComponent<LeverHead>();              
                leverHead.AttachToLevelBody();
                this.leverHead = leverHead;

                if(Managers.Game.CurrentState != GameState.Editor)
                {
                    collision.transform.GetChild(0).gameObject.SetActive(false);
                    Destroy(collision.gameObject, 1f);
                    attachedLeverHead.gameObject.SetActive(true);
                }
         
                onCompletionParts = true;
                animator.SetTrigger(OnCompletion);
            }
           
        }
    }

    public void RpcActivation(){
        Activation();
    }
    protected override void Activation()
    {
         onAcitve = !onAcitve;
            StartCoroutine(Co_Operation());
    }

    public override void TurnOn()
    {
        base.TurnOn();
        
    }
    public override void TurnOff()
    {
        if (onCompletionParts)
        {
            leverHead.DetachToLevelBody();
            onCompletionParts = false;
        }
        base.TurnOff();

    }


    IEnumerator Co_Operation()
    {
        onOperation = true;
        if (onAcitve)
        {
            animator.SetBool(OnActive, true);
            AnimatorStateInfo animationState = animator.GetCurrentAnimatorStateInfo(0);
            Debug.Log(animationState.length);
            PrograssButtonActivatedObject(true);
            yield return new WaitForSeconds(animationState.length+0.5f);
        }
        else
        {
            animator.SetBool(OnActive, false);
            AnimatorStateInfo animationState = animator.GetCurrentAnimatorStateInfo(0);
            PrograssButtonActivatedObject(false);
            yield return new WaitForSeconds(animationState.length+0.5f);
            
        }
        onOperation = false;
        

    }
    #region  Interaction

    public void Interaction(Transform accessor = null)
    {
        if (onCompletionParts && !onOperation)
        {
            _leverBodyNet.CmdLeverActivate();
        }
        
    }

    public bool CanInteract()
    {
        return onCompletionParts;
    }

    public void Interacting(bool value)
    {
        return;
    }

    public ObjectTypeEnum GetObjectType()
    {
        return objectTypeEnum;
    }

    public void ShowEButton()
    {
        var eButtonUI = Managers.UI.ShowUI<UI_ShowEButton>();
        eButtonUI.transform.position = transform.position + (Vector3)offset;
    }
    
    public void HideEButton()
    {
        Managers.UI.HideUI<UI_ShowEButton>();
    }

#endregion

}
