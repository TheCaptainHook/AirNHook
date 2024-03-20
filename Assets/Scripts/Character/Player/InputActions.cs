using System;
using UnityEngine;

public class InputActions : MonoBehaviour
{
    private PlayerInputAction _playerInputAction;
    public PlayerInputAction.PlayerActions playerActions;

    private void Awake()
    {
        _playerInputAction = new PlayerInputAction();
        playerActions = _playerInputAction.Player;
        _playerInputAction.Enable();
    }
}
