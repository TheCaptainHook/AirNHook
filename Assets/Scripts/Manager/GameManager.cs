using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public enum GameState
{
    Title,
    Lobby,
    Game,
}

public class GameManager : MonoBehaviour
{
    public GameState CurrentState { get; set; }

    private GameObject _player;
    public GameObject Player
    {
        get
        {
            _player = NetworkClient.localPlayer.gameObject;

            return _player;
        }
    }
    
    
}
