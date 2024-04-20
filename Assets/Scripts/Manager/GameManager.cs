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
}
