using System;
using System.Collections.Generic;
using JetBrains.Annotations;
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
            catch (NullReferenceException e) { }
            
            return _player;
        }
    }

}
