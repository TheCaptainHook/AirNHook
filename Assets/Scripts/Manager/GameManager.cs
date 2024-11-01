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
                break;
            // MainScene
            case 1:
                Debug.Log("Scene Loaded 1");
                break;
            // EditorScene
            case 2:
                Debug.Log("Scene Loaded 2");
                Managers.Sound.PlayBGM(AudioType.Lobby,AudioMixerGroupType.BGM, true);
                break;
        }
    }

    public void StageStart(string mapID)
    {
        if (mapID is null or "Lobby") return;
        _stageID = mapID;
        _startTime = Time.time;
        _clearDeath = 0;
        //_totalDeath = Managers.Data.loadData.playData[_stageID].totalDeath; //TODO0726
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

    public void StageClear(string stageID,bool stageLevelUp = false)
    {
        if (stageID.Equals("Lobby")) return;
        //TODO0726
        //_stageID = stageID;
        //_clearTime = Time.time;
        //Managers.Data.loadData.playData[stageID].stageClear = true;

        //DeathCompare();
        //TimeCompare();

        //Managers.Data.loadData.Save();
        //_startTime = 0;
        //_clearTime = 0;
        //TODO0726
        if (stageLevelUp) stageLevel++;
        Managers.Data.saveData.ClearMap(stageID,stageLevelUp);
    }


    //public void DeathCompare()
    //{
    //    Managers.Data.loadData.playData[_stageID].deathCount = _clearDeath;
    //    Managers.Data.loadData.playData[_stageID].totalDeath = _totalDeath;
    //}

    //public void TimeCompare()
    //{
    //    var timeGap = _clearTime - _startTime;

    //    if (Managers.Data.loadData.playData[_stageID].clearTime == 0 || timeGap < Managers.Data.loadData.playData[_stageID].clearTime)
    //    {
    //        Managers.Data.loadData.playData[_stageID].clearTime = timeGap;
    //        DeathCompare();
    //    }
    //}
    //TODO 0726
    //public void PlayerAndMapSavaDataUpdate(string stageID)
    //{
    //    // string stageID = Managers.Stage.stageName;
    //    PlayerSaveData data = Managers.Data.saveData._SaveFileData._PlayerSaveData;
    //    MapSaveData mapData = Managers.Data.saveData.dic[stageID];
    //    //Updata MapSavaData//최단시간 클리어,가장 최근 클리어,해당맵 죽은 횟수,
    //    float clearTime = Time.time - _startTime;
    //    mapData.ClearMapDataUpdate(clearTime, _clearDeath);
    //    //Updata MapSavaData


    //    //Updata PlayerSavaData // 죽은 횟수 총합,클리어한 맵
    //    data.AddTotalDeath(_clearDeath);
    //    data.AddClearMapId(stageID);
    //    //Updata PlayerSavaData

    //    Managers.UI.ShowUI<UI_SaveAndLoad>();
    //    UI_SaveAndLoad uI_SaveAndLoad =  Managers.UI.GetUI<UI_SaveAndLoad>().GetComponent<UI_SaveAndLoad>();
       
       
    //    uI_SaveAndLoad.SaveData(Managers.Data.saveData.Save_SaveFile());
       
    //    // await Managers.Data.saveData.Save_SaveFile();


    //}

    public (float clearTIme,int deathCount) GetClearData()
    {
        return (Time.time - _startTime,_clearDeath);
    }
    //TODO 0726 
}
