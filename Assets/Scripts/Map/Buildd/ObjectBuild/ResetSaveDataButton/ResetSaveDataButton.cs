using Mirror;
using UnityEngine;


public class ResetSaveDataButton : BuildObj,IInteractable
{
    private bool onReset;
    private float maxResetCount = 5;
    private float curResetCount = 0;

    void Update()
    {
        if(localPlayer)
        {
            if(Input.GetKeyDown(KeyCode.E) && !onReset)
            {
                onReset = true;
                ResetSaveData();
            }
        }
        
        if(onReset)
        {
            curResetCount += Time.deltaTime;
            if(curResetCount >= maxResetCount)
            {
                ResetBtn();
            }
        }

    }
    private void ResetBtn()
    {
        curResetCount = 0;
        onReset = false;
    }

    private PlayerSM localPlayer;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(localPlayer) return;

        if(collision.TryGetComponent(out PlayerSM player))
        {
            if(player.TryGetComponent(out NetworkIdentity identity))
            {
                if(identity.isLocalPlayer)
                {
                    ShowE();
                    localPlayer = player;
                }
            }
        }
    }
    void OnTriggerExit2D(Collider2D collision)
    {
        if(collision)
        {
            if(localPlayer)
            {
                if (localPlayer.gameObject == collision.gameObject)
                {
                    //ShutDown
                    localPlayer = null;
                    HideE();
                    //ShutDown
                }
            }
            
        }
    }



    #region  Main
    private  void ResetSaveData()
    {
        Managers.Data.saveData.DeleteSaveFile();
        Managers.Data.saveData.SetUp();
    }
    #endregion

    
    //#region  UI
    //private void ShowE()
    //{
    //    Managers.UI.ShowUI<UI_ShowEButton>();
    //}
    //private void HideE()
    //{
    //    Managers.UI.HideUI<UI_ShowEButton>();
    //}
    //#endregion


    #region  Interacable
    private UI_Base _E_Btn;
    public ObjectTypeEnum _objectType = ObjectTypeEnum.Interaction;
    [SerializeField] float _BtnOffset;

    public void Interaction(Transform accessor = null)
    {
     

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

    public void ShowE()
    {
        _E_Btn = Managers.UI.ShowUI<UI_ShowEButton>();
        _E_Btn.transform.position = transform.position + (transform.up * _BtnOffset);
    }
    public void HideE()
    {
        _E_Btn = null;
        Managers.UI.HideUI<UI_ShowEButton>();
    }



    #endregion


}




