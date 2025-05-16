using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class ResetSaveDataButton : BuildObj
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
        if(collision && collision.gameObject == localPlayer.gameObject)
        {
            //ShutDown
            localPlayer = null;
            HideE();
            //ShutDown
        }
    }



    #region  Main
    private  void ResetSaveData()
    {
        Managers.Data.saveData.DeleteSaveFile();
        Managers.Data.saveData.SetUp();
    }
    #endregion

    
    #region  UI
    private void ShowE()
    {
        Managers.UI.ShowUI<UI_ShowEButton>();
    }
    private void HideE()
    {
        Managers.UI.HideUI<UI_ShowEButton>();
    }
    #endregion
}




