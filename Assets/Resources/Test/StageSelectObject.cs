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

    // void Start()
    // {
    //     Net.Server_SetComputer(gameObject);
    // }

    public void Interaction(Transform accessor = null)
    {
        if (!NetworkServer.active || !NetworkClient.isConnected)
            return;
            
        if(Net.isOpen) return;
        Net.Server_SetOnPower();

    }

    //------------------------------------------------Network 250217
    public Computer_Net Net {get{return GetComponent<Computer_Net>();}}

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
