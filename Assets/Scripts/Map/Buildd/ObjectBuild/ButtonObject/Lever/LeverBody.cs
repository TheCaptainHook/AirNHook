using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;


public class LeverBody : ButtonEntity, IInteractable
{
    [CustomHeader("LeverBody")]
    [ReadOnly]
    //public LeverHead leverHead;
    //public Transform attachedLeverHead;

    ObjectTypeEnum objectTypeEnum = ObjectTypeEnum.Interaction;

    private LeverBodyNet Net => GetComponent<LeverBodyNet>();

    [Header("State")]
    [ReadOnly]
    //public bool onCompletionParts;
    //bool onAcitve;
    //bool onOperation;

    [Header("Components")]
    Animator animator;

    public override void SetData<T>(T data)
    {
        base.SetData(data);
        if(Application.isPlaying)
        Net.Server_InitSync();

    }


    //private void Awake()
    //{
    //    _leverBodyNet = GetComponent<LeverBodyNet>();
    //    animator = GetComponent<Animator>();
    //}

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<LeverHead>())
        {
            if (!Net.onCompletionParts)
            {
                //if(NetworkServer.active)
                //{
                //    LeverHead leverHead = collision.gameObject.GetComponent<LeverHead>();
                //    //Net.Server_SetLeverHead(leverHead);
                //    Net.Cmd_SetLeverHead(leverHead.GetComponent<NetworkIdentity>().netId);


                //}
                // LeverHead leverHead = collision.gameObject.GetComponent<LeverHead>();
                //Net.Server_SetLeverHead(leverHead);

                Net.Cmd_SetLeverHead(collision.gameObject.GetComponent<NetworkIdentity>().netId);
            }
           
        }
    }

    #region Network
    public void RpcActivation()
    {
        Activation();
    }

    #endregion

    public override void Activation()
    {
        PrograssButtonActivatedObject(true);
        Managers.Sound.PlaySound3D(GlobalText.BUTTON_LEVER_SOUND_1, transform.position, 0.5f);
    }
    public override void Deactivated()
    {
        PrograssButtonActivatedObject(false);
        Managers.Sound.PlaySound3D(GlobalText.BUTTON_LEVER_SOUND_1, transform.position, 0.5f);
    }
    public void Net_Act()
    {
        Activation();
    }
    public void Net_Deac()
    {
        Deactivated();
    }

    public override void TurnOn()
    {
        base.TurnOn();
        
    }
    //public override void TurnOff()
    //{
    //    if (onCompletionParts)
    //    {
    //        leverHead.DetachToLevelBody();
    //        onCompletionParts = false;
    //    }
    //    base.TurnOff();

    //}


    //IEnumerator Co_Operation()
    //{
    //    onOperation = true;
    //    if (onAcitve)
    //    {
    //        animator.SetBool(OnActive, true);
    //        AnimatorStateInfo animationState = animator.GetCurrentAnimatorStateInfo(0);
    //        Debug.Log(animationState.length);
    //        PrograssButtonActivatedObject(true);
    //        yield return new WaitForSeconds(animationState.length+0.5f);
    //    }
    //    else
    //    {
    //        animator.SetBool(OnActive, false);
    //        AnimatorStateInfo animationState = animator.GetCurrentAnimatorStateInfo(0);
    //        PrograssButtonActivatedObject(false);
    //        yield return new WaitForSeconds(animationState.length+0.5f);
            
    //    }
    //    onOperation = false;
        

    //}
    #region  Interaction

    public void Interaction(Transform accessor = null)
    {
        if (Net.onCompletionParts && !Net.onOperation)
        {
            // _leverBodyNet.CmdLeverActivate();
            //Activation();
            Net.Cmd_Active();
        }
        
    }

    public bool CanInteract()
    {
        return Net.onCompletionParts;
    }

    public bool Interacting(bool value, GameObject player)
    {
        return true;
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
