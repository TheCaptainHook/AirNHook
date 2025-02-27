using System;
using Steamworks;
using UnityEngine;
using Random = UnityEngine.Random;

public class AchievementManager
{
    #region Event
        //Player
        private event Action playerJumpingEvent;
        private event Action playerDeathEvent;
        //Object
        private event Action usePortalEvent;
    #endregion

    public bool onRequestSteamUserState;
    // List<string> achievementList = new List<string>()
    // {
    //     GlobalText.PLAYER_JUMPING,
    //     GlobalText.USE_PORTAL_1,
    //     GlobalText.USE_PORTAL_50
    // };


    public void SetUp(){
        // if(SteamUserStats.RequestCurrentStats()){
        //     onRequestSteamUserState = true;
        // }

        //Object
        usePortalEvent += UsePortal;
        //Player
        playerJumpingEvent += PlayerJumping;
        playerDeathEvent += PlayerDeath;
    }

    #region Call Event
    #region  Obejct
    public void CallUsePortal()
    {
        usePortalEvent?.Invoke();
    }
    //
    //
    #endregion
    #region  Player
    //player
    public void CallPlayerJumping()
    {
        playerJumpingEvent?.Invoke();
    }
    public void CallPlayerDeath()
    {
        playerDeathEvent?.Invoke();
    }
    //PlayerDeath
    //PlayerDeath_Sucide
    //Clear_Toturial

    #endregion

    #endregion

    UI_EventEchoDialogue UI_EED => Managers.UI.ShowUI<UI_EventEchoDialogue>().gameObject.GetComponent<UI_EventEchoDialogue>();
    #region SteamWorks
    public bool IsAchievementUnlocked(string achievementID)
        {
                bool achieved = false;
                if (SteamManager.Initialized)
                {
                    SteamUserStats.GetAchievement(achievementID, out achieved);
                }
                return achieved;
        }
        private void AchievementUnlock(string achievementID){
            SteamUserStats.SetAchievement(achievementID);
            SteamUserStats.StoreStats();
        }
    #endregion




    #region Event
        #region Object
            private async void UsePortal(){ 
            int usePortal = ++Managers.Data.saveData._AchievementData.use_Portal;

            //test 1212
            //UI_EventEchoDialogue ui_EED = Managers.UI.ShowUI<UI_EventEchoDialogue>().gameObject.GetComponent<UI_EventEchoDialogue>();
        UI_EED.SetDialogue("use portal [/2] count");
            //test 1212

            if(onRequestSteamUserState)
            switch(usePortal){
                    case 1:
                        if(IsAchievementUnlocked(GlobalText.USE_PORTAL_1)){
                            AchievementUnlock(GlobalText.USE_PORTAL_1);
                            Debug.Log("Achievement Data Update");
                        }
                    break;
                    case 50:
                        if(IsAchievementUnlocked(GlobalText.USE_PORTAL_50)){
                            AchievementUnlock(GlobalText.USE_PORTAL_50);
                            Debug.Log("Achievement Data Update");
                        }
                    break;
            }
                await Managers.Data.saveData.Ac_Save();
            }
    #endregion
    #region Player
    private async void PlayerJumping()
    {
        int player_Jumping = ++Managers.Data.saveData._AchievementData.player_Jumping;

        if (onRequestSteamUserState)
            switch (player_Jumping)
            {
                case 100:
                    if (IsAchievementUnlocked(GlobalText.PLAYER_JUMPING_100))
                    {
                        AchievementUnlock(GlobalText.PLAYER_JUMPING_100);
                        Debug.Log("Achievement Data Update");
                    }
                    break;
            }
        await Managers.Data.saveData.Ac_Save();
    }

    //ID 70010~70012
    private int[] idList = new int[] {70010,70011,70012 };
    private int curDeathScriptPercent = 0;
    private async void PlayerDeath()
    {
        int playerDeath = ++Managers.Data.saveData._AchievementData.player_Death;

        switch(playerDeath)
        {
            case 1:
                UI_EED.SetDialogue("[끔찍하군!] 하지만, 걱정 말라! 우리 슈퍼 연구소의 기술력으로 얼마든지 재생성할 수 있으니!.");
                break;
            //case int n when n % 5 == 0:
            //    UI_EED.SetDialogue("Player [/1] Death.");
            //    break;
        }

        if(GetDeathPercent())
        {
            //print dialogue
            int num = Random.Range(0, idList.Length);
            UI_EED.SetDialogue($"Dialogue Id : [{num}]");
            curDeathScriptPercent = 0;
        }
        else
        {
            curDeathScriptPercent += 4;

        }


        await Managers.Data.saveData.Ac_Save();
    }

    private bool GetDeathPercent()
    {
        int num = Random.Range(1, 101);
        if(num <= curDeathScriptPercent)
        {
            return true;
        }

        return false;
    }


    #endregion
    #region  Map

    #endregion
    #endregion

}
