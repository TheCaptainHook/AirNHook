using System;
using Mirror;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    Title,
    Lobby,
    Game,
    Editor,
}
//TODO 0726 Develop Code Line(Data) : 102,124,155,
public class GameManager
{
    public GameState CurrentState { get; set; }
    public CharacterType playerCharacterType = CharacterType.Default;
    public PlayerInput playerInput;
    public NewCameraShake cameraShake;

    // private string _stageID;
    private float _startTime;
    private int _clearDeath;
    // private bool _skip;

    public int stageLevel = 0;

    private GameObject _player;
    // 플레이어가 GameScene에서만 생성되고, NetworkManager에 의해 생성되기에
    // 이렇게 불러오는 방식을 채택.
    public GameObject Player
    {
        get
        {
            if (!NetworkClient.ready)
                return _player;

            try
            {
                _player = NetworkClient.localPlayer.gameObject;
            }
            catch (NullReferenceException) { }
            
            return _player;
        }
    }

    private GameObject _otherPlayer;
    public GameObject OtherPlayer
    {
        get
        {
            try
            {
                return _otherPlayer;
            }
            catch (Exception)
            {
                return null;
            }
        }
        set => _otherPlayer = value;
    }

    public GameManager()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Managers.UI.ClearUI();
        switch (scene.buildIndex)
        {
            // StartScene
            case 0:
                Debug.Log("Scene Loaded 0");
                Managers.Instance.CheckNetworkManager();
                Managers.Game.CurrentState = GameState.Title;
                Managers.UI.ShowUI<UI_ScreenSaver>();
                Managers.CursorManager.ClearCursor();
                break;
            // MainScene
            case 1:
                Debug.Log("Scene Loaded 1");
                Managers.Sound.PlayBGM(GlobalText.LOBBY_SOUND);
                Managers.CursorManager.ClearCursor();
                break;
            // EditorScene
            case 2:
                Debug.Log("Scene Loaded 2");
                Managers.CursorManager.ClearCursor();
                break;
        }
    }

    public void StageStart(string mapID)
    {
        if (mapID is null or "Lobby") return;
        // _stageID = mapID;
        _startTime = Time.time;
        _clearDeath = 0;
        //_totalDeath = Managers.Data.loadData.playData[_stageID].totalDeath; //TODO0726
        // _skip = false;
    }

    //캐릭터 사망시 데스카운트추가
    //public void IncreaseDeathCount(bool isLocalPlayer)
    //{
    //    //_totalDeath++;
    //    if(isLocalPlayer)
    //        _clearDeath++;
    //}
    // public  void IncreaseDeathCount()
    // {
    //         _clearDeath++;
    //     //Managers.Data.saveData._AchievementData.Update_Player_Death();
    //     //await Managers.Data.saveData.Ac_Save();

    // }

    // //스킵버튼클릭시 활성화
    // public void Skip()
    // {
    //     _skip = true;
    // }

    public void StageClear(string stageID,bool stageLevelUp = false)
    {
        if (stageID.Equals("Lobby")) return;

        //if (stageLevelUp && stageLevel <=1) stageLevel++; //The currently created stage is only up to 1.
        if(stageLevelUp)
        {
            stageLevel = MapEditor.Instance.CurMap.stageLevel+1;
            Debug.Log($"Clear 2, {stageLevel}, curMap StageLevel : {MapEditor.Instance.CurMap.stageLevel}");
        }

        Managers.Data.saveData.ClearMap(stageID,stageLevelUp);
    }

    public (float clearTime, int deathCount) GetClearData()
    {
        return (Time.time - _startTime,_clearDeath);
    }
    //TODO 0726 
}



/**
 *  플레이어가 죽으면
 *      1. GameManager clearDeath 올려주고
 *      2. AchievementManager 에서 플레이어 데스 이벤트 호출 -> achieve Data 에서 playerDeath 올려주고 관련 이벤트 실행 후 ac_save진행
 *      3. 맵 클리어시 GameManager clearDeath 맵 데이터 저장
**/