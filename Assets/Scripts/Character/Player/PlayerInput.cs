using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public PlayerInputAction playerInputAction { get; private set; }
    public PlayerInputAction.PlayerActions playerActions { get; private set; }

    private void Awake()
    {
        playerInputAction = new PlayerInputAction();
        playerActions = playerInputAction.Player;
    }

    private void OnEnable()
    {
        playerInputAction.Enable();   
    }
    private void OnDisable()
    {
        playerInputAction.Disable();
    }
}
