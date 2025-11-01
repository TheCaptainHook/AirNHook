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

    // private LeverBodyNet Net => GetComponent<LeverBodyNet>();
    private LeverBodyNet body_net;
    private LeverBodyNet B_Net { get { body_net ??= GetComponent<LeverBodyNet>(); return body_net; } }
    // [Header("State")]
    // [ReadOnly]
    //public bool onCompletionParts;
    //bool onAcitve;
    //bool onOperation;

    // [Header("Components")]
    // Animator animator;

    // public override void SetData<T>(T data)
    // {
    //     base.SetData(data);
    //     if(Application.isPlaying)
    //     Net.Server_InitSync();

    // }


    //private void Awake()
    //{
    //    _leverBodyNet = GetComponent<LeverBodyNet>();
    //    animator = GetComponent<Animator>();
    //}

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!MapEditor.Instance._onMapTransition_Complete) return;

        if (collision.gameObject.GetComponent<LeverHead>())
        {
            if (!B_Net.onCompletionParts)
            {
                //if(NetworkServer.active)
                //{
                //    LeverHead leverHead = collision.gameObject.GetComponent<LeverHead>();
                //    //Net.Server_SetLeverHead(leverHead);
                //    Net.Cmd_SetLeverHead(leverHead.GetComponent<NetworkIdentity>().netId);


                //}
                // LeverHead leverHead = collision.gameObject.GetComponent<LeverHead>();
                //Net.Server_SetLeverHead(leverHead);

                B_Net.Cmd_SetLeverHead(collision.gameObject.GetComponent<NetworkIdentity>().netId);
            }

        }
    }

    #region Network
    // public void RpcActivation()
    // {
    //     Activation();
    // }

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
    // public void Net_Act()
    // {
    //     Activation();
    // }
    // public void Net_Deac()
    // {
    //     Deactivated();
    // }

    // public override void TurnOn()
    // {
    //     base.TurnOn();

    // }
    #region  Interaction

    public void Interaction(Transform accessor = null)
    {
        if (B_Net.onCompletionParts && !B_Net.onOperation)
        {
            B_Net.Cmd_Active();
        }

    }

    public bool CanInteract()
    {
        return B_Net.onCompletionParts;
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

    public override void Clean()
    {
        onActive = false;
        Net.Clean();
    }
    #endregion

}
