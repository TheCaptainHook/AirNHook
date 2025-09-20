using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    private PlayerInputAction _playerInputAction;
    public PlayerInputAction.PlayerActions playerActions { get; private set; }
    public PlayerInputAction.UIActions uiActions { get; private set; }
    public PlayerInputAction.CutSceneActions cutSceneActions { get; private set; }
    private void Start()
    {
        _playerInputAction = new PlayerInputAction();
        playerActions = _playerInputAction.Player;
        uiActions = _playerInputAction.UI;
        
        cutSceneActions = _playerInputAction.CutScene;
        cutSceneActions.Disable();

        _playerInputAction.Enable();
        uiActions.Option.started += ToggleOption;

        Managers.Game.playerInput = this;
    }
    
    private void ToggleOption(InputAction.CallbackContext context)
    {
        if (!Managers.UI.IsActive<UI_Option>())
            Managers.UI.ShowUI<UI_Option>();
        else
        {
            Managers.UI.HideUI<UI_Option>();
        }
    }
}
