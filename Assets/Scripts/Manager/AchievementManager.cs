using System;
using System.Collections;
using System.Collections.Generic;
using Steamworks;
using UnityEngine;

public class AchievementManager
{
    #region Event    
    //Player
    private event Action playerJumpingEvent;
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
  }

#region Call Event
  public void CallUsePortal(){
    usePortalEvent?.Invoke();
  }
  public void CallPlayerJumping(){
    playerJumpingEvent?.Invoke();
  }
  #endregion


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
    #region  Object

private async void UsePortal(){ 
   int usePortal = ++Managers.Data.saveData._AchievementData.use_Portal;

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
    private async void PlayerJumping(){
        int player_Jumping = ++Managers.Data.saveData._AchievementData.player_Jumping;
       
        if(onRequestSteamUserState)
        switch (player_Jumping){
            case 100:
            if(IsAchievementUnlocked(GlobalText.PLAYER_JUMPING_100)){
                AchievementUnlock(GlobalText.PLAYER_JUMPING_100);
                Debug.Log("Achievement Data Update");
            }
            break;
        }
        await Managers.Data.saveData.Ac_Save();
    }
    #endregion
#endregion

}
