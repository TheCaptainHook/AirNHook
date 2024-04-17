using System;
using Mirror;
using UnityEngine;

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
    public CharacterType playerCharacterType = CharacterType.Hook;

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
}
