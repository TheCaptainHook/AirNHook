using System;
using Mirror;
using UnityEngine;

public class StageSelectObject : MonoBehaviour, IInteractable
{
    public ObjectTypeEnum objectType = ObjectTypeEnum.Interaction;
    public Vector2 offset;
    
    [SerializeField] StageSelectorComputer _StageSelectorComputer;
    //private void Awake()
    //{
    //    Managers.Sound.PlaySound(AudioType.Lobby,AudioMixerGroupType.BGM,true,1f, 0.6f);
    //}
    //todo 0605

    public void Interaction(Transform accessor = null)
    {
        if (!NetworkServer.active || !NetworkClient.isConnected)
            return;


        if (Managers.UI.GetUI<UI_StageSelect_var3>().GetComponent<UI_StageSelect_var3>().onPrograss) {  return; }
        

        

        if (!Managers.UI.IsActive<UI_StageSelect_var3>())
        {
            _StageSelectorComputer.Surprise_();
            Managers.UI.ShowUI<UI_StageSelect_var3>();
        }
        //else
        //{
        //    Debug.Log("EEEEE3");
        //    Managers.UI.GetUI<UI_StageSelect_var3>().GetComponent<UI_StageSelect_var3>().SetDown();
        //    _StageSelectorComputer.LineIdle();
        //}
            
        

    }

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
