using System;
using System.Collections.Generic;
using Steamworks;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class AchievementManager
{
    public bool _isInterrupted;

    #region Event
    //Player
    private event Action playerJumpingEvent;
    private event Action<DamageType> playerDeathEvent;
    private event Action _player_Puzzle_Wrong_Event;
    //Object
    //private event Action usePortalEvent;
    #endregion

    public bool onRequestSteamUserState;
    private Callback<UserStatsReceived_t> userStatsReceivedCallback;
    private List<int> _defaultDeathId = new List<int>() { 70010, 70011, 70012 };
    private List<int> _suicideDeathId = new List<int>() { 70100, 70101, 70102 };
    private List<int> _fireDeathId = new List<int>() { 70200, 70201, 70202 };
    private List<int> _electricDeathId = new List<int>() { 70300, 70301, 70302 };
    private List<int> _boomDeathId = new List<int>() { 70300};
    private Dictionary<DamageType, List<int>> _deathIdDict;

    #region  Puzzle
    private int[] _puzzle_taunt_Ids = new int[] { 50100, 50101, 50102 };
    #endregion

    public void SetUp()
    {

        //Object
        //usePortalEvent += UsePortal;
        //Player
        playerJumpingEvent += PlayerJumping;
        playerDeathEvent += PlayerDeath;
        _player_Puzzle_Wrong_Event += Player_Puzzle_Wrong;

        if (!SteamManager.Initialized)
        {
            return;
        }

        userStatsReceivedCallback = Callback<UserStatsReceived_t>.Create(OnUserStatsReceived);
        SteamUserStats.RequestCurrentStats();

        _deathIdDict = new Dictionary<DamageType, List<int>>
        {
            { DamageType.Default, _defaultDeathId },
            { DamageType.Suicide, _suicideDeathId },
            { DamageType.Fire, _fireDeathId },
            { DamageType.Electric, _electricDeathId },
            {DamageType.Boom,_boomDeathId}
        };
    }

    #region Call Event
    #region  Obejct
    //public void CallUsePortal()
    //{
    //    usePortalEvent?.Invoke();
    //}
    //
    //
    #endregion
    #region  Player
    //player
    public void CallPlayerJumping()
    {
        playerJumpingEvent?.Invoke();
    }
    public void CallPlayerDeath(DamageType damageType)
    {
        playerDeathEvent?.Invoke(damageType);
    }
    public void CallPlayer_Puzzle_Wrong()
    {
        _player_Puzzle_Wrong_Event?.Invoke();
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

    private void AchievementUnlock(string achievementID)
    {
        SteamUserStats.SetAchievement(achievementID);
        SteamUserStats.StoreStats();
    }
    private void OnUserStatsReceived(UserStatsReceived_t pCallback)
    {
        if ((ulong)pCallback.m_nGameID != SteamUtils.GetAppID().m_AppId)
        {
            Debug.LogWarning("Received stats for wrong game ID.");
            return;
        }

        if (pCallback.m_eResult == EResult.k_EResultOK)
        {
            onRequestSteamUserState = true;
            //TEST
            uint count = SteamUserStats.GetNumAchievements();
            for (uint i = 0; i < count; i++)
            {
                string apiName = SteamUserStats.GetAchievementName(i);
                Debug.Log(apiName);
            }
            //TEST
        }
        else
        {
            Debug.LogError("Failed to load user stats: " + pCallback.m_eResult);
        }
    }
    #endregion




    #region Event
    #region Object
    private async void UsePortal()
    {
        int usePortal = ++Managers.Data.saveData._AchievementData.use_Portal;

        //test 1212
        //UI_EventEchoDialogue ui_EED = Managers.UI.ShowUI<UI_EventEchoDialogue>().gameObject.GetComponent<UI_EventEchoDialogue>();
        UI_EED.SetDialogue("use portal [/2] count");
        //test 1212

        if (onRequestSteamUserState)
            switch (usePortal)
            {
                case 1:
                    if (IsAchievementUnlocked(GlobalText.USE_PORTAL_1))
                    {
                        AchievementUnlock(GlobalText.USE_PORTAL_1);
                        Debug.Log("Achievement Data Update");
                    }
                    break;
                case 50:
                    if (IsAchievementUnlocked(GlobalText.USE_PORTAL_50))
                    {
                        AchievementUnlock(GlobalText.USE_PORTAL_50);
                        Debug.Log("Achievement Data Update");
                    }
                    break;
            }
        await Managers.Data.saveData.Ac_Save();
    }
    private void Player_Puzzle_Wrong()
    {
        int id = Random.Range(0, _puzzle_taunt_Ids.Length);
        UI_EED.SetDialogue(Managers.Data.language.GetSentence(_puzzle_taunt_Ids[id]));
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

    private int curDeathScriptPercent = 0;

    private async void PlayerDeath(DamageType damageType)
    {
        int playerDeath = ++Managers.Data.saveData._AchievementData.player_Death;

        switch (playerDeath)
        {
            case 1:
                if (IsAchievementUnlocked(GlobalText.PLAYER_DEATH_1))
                {
                    AchievementUnlock(GlobalText.PLAYER_DEATH_1);
                    UI_EED.SetDialogue(Managers.Data.language.GetSentence(70001));
                }
                break;

        }

        if (GetDeathPercent())
        {
            //print dialogue
            _deathIdDict.TryGetValue(damageType, out var idList);
            int num = Random.Range(0, idList.Count);
            UI_EED.SetDialogue(Managers.Data.language.GetSentence(idList[num]));
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
        if (num <= curDeathScriptPercent)
        {
            return true;
        }

        return false;
    }


    #endregion

    #endregion

    #region  Interrupted
    public void SetInterrupted()
    {
        _isInterrupted = !_isInterrupted;
    }
    #endregion

}
