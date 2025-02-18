using System;
using Mirror;
using UnityEngine;

public class StageSelectObject : MonoBehaviour, IInteractable
{
    public ObjectTypeEnum objectType = ObjectTypeEnum.Interaction;
    public Vector2 offset;
    
    [ReadOnly]
    public bool onPower;

    [SerializeField] StageSelectorComputer _StageSelectorComputer;
    //private void Awake()
    //{
    //    Managers.Sound.PlaySound(AudioType.Lobby,AudioMixerGroupType.BGM,true,1f, 0.6f);
    //}
    //todo 0605

    void Start()
    {
        Net.Server_SetComputer(gameObject);
    }

    public void Interaction(Transform accessor = null)
    {
        if (!NetworkServer.active || !NetworkClient.isConnected)
            return;

        // if (Managers.UI.GetUI<UI_StageSelect_var3>().GetComponent<UI_StageSelect_var3>().onPrograss) {  return; }
        
        // if (!Managers.UI.IsActive<UI_StageSelect_var3>())
        // {
        //     _StageSelectorComputer.Surprise_();



        //     // NEtwork,
        //     // Managers.UI.ShowUI<UI_StageSelect_var3>();

        //     // ShowDummy();
        //     // onPower =true;

        //     // Managers.Game.Player.GetComponent<PlayerMovement>().canControl = false;
        //     // Managers.Game.Player.GetComponent<Rigidbody2D>().velocity  = Vector2.zero;
        // }

        //else
        //{
        //    Debug.Log("EEEEE3");
        //    Managers.UI.GetUI<UI_StageSelect_var3>().GetComponent<UI_StageSelect_var3>().SetDown();
        //    _StageSelectorComputer.LineIdle();
        //}
            
        if(Net.isOpen) return;
        Net.Server_SetOnPower();

    }

    //------------------------------------------------Network 250217
    public Computer_Net Net {get{return GetComponent<Computer_Net>();}}
    // [SerializeField] GameObject screen;
    // private UI_StageSelect_var3_Dummy dummy;
    // private void ShowDummy()
    // {
    //     var dummy =  Managers.UI.ShowUI<UI_StageSelect_var3_Dummy>();
    //     UI_StageSelect_var3_Dummy _dummy = dummy.GetComponent<UI_StageSelect_var3_Dummy>();
    //     Canvas canvas = dummy.GetComponent<Canvas>();
    //     canvas.worldCamera = CameraHolder.Instance.StageSelectCamera();
    // }

    //------------------------------------------------Network

    public bool CanInteract()
    {
        return true;
    }

    public void Interacting(bool value)
    {
        return;
    }

    public ObjectTypeEnum GetObjectType()
    {
        return objectType;
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
}
