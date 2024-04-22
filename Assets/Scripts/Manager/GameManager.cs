using System;
using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    Title,
    Lobby,
    Game,
    Editor,
}

public class GameManager
{
    public GameState CurrentState { get; set; }
    public CharacterType playerCharacterType = CharacterType.Default;

    public string mapID;
    private string _stageID;
    private float _startTime;
    private float _clearTime;
    private int _clearDeath;
    private int _totalDeath;
    private bool _skip;

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
            catch (NullReferenceException e) { Debug.Log(e); }
            
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
            catch (MissingReferenceException e)
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
                Managers.UI.ShowUI<UI_Title>();
                break;
            // MainScene
            case 1:
                Debug.Log("Scene Loaded 1");
                break;
            // EditorScene
            case 2:
                Debug.Log("Scene Loaded 2");
                break;
        }
    }

    public void StageStart(string mapID)
    {
        if (mapID is null or "Lobby") return;
        
        _stageID = mapID;
        _startTime = Time.time;
        _clearDeath = 0;
        _totalDeath = Managers.Data.loadData.playData[_stageID].totalDeath;
        _skip = false;
    }

    //캐릭터 사망시 데스카운트추가
    public void IncreaseDeathCount(bool isLocalPlayer)
    {
        _totalDeath++;
        if(isLocalPlayer)
            _clearDeath++;
    }

    //스킵버튼클릭시 활성화
    public void Skip()
    {
        _skip = true;
    }

    public void StageClear(string stageID)
    {
        if (stageID.Equals("Lobby")) return;

        _stageID = stageID;
        _clearTime = Time.time;
        Managers.Data.loadData.playData[stageID].stageClear = true;

        DeathCompare();
        TimeCompare();

        Managers.Data.loadData.Save();
        _startTime = 0;
        _clearTime = 0;
    }

    public void DeathCompare()
    {
        Managers.Data.loadData.playData[_stageID].deathCount = _clearDeath;
        Managers.Data.loadData.playData[_stageID].totalDeath = _totalDeath;
    }

    public void TimeCompare()
    {
        var timeGap = _clearTime - _startTime;

        if (Managers.Data.loadData.playData[_stageID].clearTime == 0 || timeGap < Managers.Data.loadData.playData[_stageID].clearTime)
        {
            Managers.Data.loadData.playData[_stageID].clearTime = timeGap;
            DeathCompare();
        }
    }
}
