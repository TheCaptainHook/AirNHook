using System;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    private PlayerInputAction _playerInputAction;
    public PlayerInputAction.PlayerActions playerActions;
    public PlayerInputAction.UIActions uiActions;

    private void Awake()
    {
        _playerInputAction = new PlayerInputAction();
        playerActions = _playerInputAction.Player;
        uiActions = _playerInputAction.UI;
        _playerInputAction.Enable();
    }
}
