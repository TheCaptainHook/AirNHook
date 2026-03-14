using System;
using System.Collections;
using System.Linq.Expressions;
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
                //Loding -> MapTransition -> CutScene Page_1 Check
                var ui = Managers.UI.ShowUI<UI_MapOpenClosePanel>().GetComponent<UI_MapOpenClosePanel>();
                ui.Default_CloseOpen();
                //Loding -> MapTransition -> CutScene Page_1 Check

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

        _startTime = Time.time;
        _clearDeath = 0;

    }

    public void StageClear(string stageID, bool stageLevelUp = false)
    {
        if (stageID.Equals("Lobby")) return;

        if (stageLevelUp)
        {
            stageLevel = MapEditor.Instance.CurMap.stageLevel + 1;
        }

        Managers.Data.saveData.ClearMap(stageID, stageLevelUp);
    }

    public (float clearTime, int deathCount) GetClearData()
    {
        return (Time.time - _startTime, _clearDeath);
    }
    //TODO 0726 
    #region  CutScene
    private void CutScene_Page_1()
    {
        if (!Managers.Data.saveData._SaveFileData._PlayerSaveData._cutScene_Page_1)
        {
            Managers.Data.saveData._SaveFileData._PlayerSaveData._cutScene_Page_1 = true;
            Managers.Data.saveData.Save();

            Managers.UI.GetUI<UI_CutSceneController>().GetComponent<UI_CutSceneController>().StartCutScene(CutScenePageName.Page_1);
        }
    }

    #endregion
}

