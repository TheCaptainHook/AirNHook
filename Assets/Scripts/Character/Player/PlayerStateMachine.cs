using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerStateMachine : StateMachine
{
    #region States
    public IState IdleState { get; protected set; }
    public IState WalkState { get; protected set; }
    public IState JumpState { get; protected set; }
    public IState FallingState { get; protected set; }
    public IState SuicideState { get; protected set; }
    public IState StepUpState { get; protected set; }
    #endregion

    #region InputValue
    protected PlayerInput input => Managers.Game.playerInput;
    public float horizontal { get; protected set; }
    public float vertical { get; protected set; }
    public bool canMovable => player.canMovable;
    public float moveSpeed => player.playerData.moveSpeed;
    public float moveSpeedMultiplier = 2f;
    public bool isJumping = false;
    public bool isJumpPerformed { get; protected set; }
    public float jumpPower => player.playerData.jumpPower;
    public float stepPower;
    public Rigidbody2D rigidbody2D { get; protected set; }
    #endregion
    
    /// <summary>
    /// PlayerStateMachine 생성자. MonoBehaviour를 매개변수로 받아서 코루틴 돌리기 가능. 
    /// </summary>
    /// <param name="player"> MonoBehaviour </param>
    public PlayerStateMachine(PlayerSM player)
    {
        this.player = player;
        rigidbody2D = player.rigidbody2D;

        IdleState = new IdleState(this);
        WalkState = new WalkState(this);
        JumpState = new JumpState(this);
        FallingState = new FallingState(this);
        SuicideState = new SuicideState(this);
        StepUpState = new StepUpState(this);

        Initialize();
    }

    public sealed override void Initialize()
    {
        CurrentState?.ExitState();
        CurrentState = IdleState;
        CurrentState?.ExitState();
    }

    #region Input
    public override void SubscribeInput()
    {
        input.playerActions.Move.started += MoveStarted;
        input.playerActions.VerticalMove.started += VerticalMoveStarted;
        input.playerActions.Jump.started += JumpStarted;
        input.playerActions.Jump.performed += JumpPerformed;
        input.playerActions.Jump.canceled += JumpCanceled;
    }

    public override void UnsubscribeInput()
    {
        input.playerActions.Move.started -= MoveStarted;
        input.playerActions.VerticalMove.started -= VerticalMoveStarted;
        input.playerActions.Jump.started -= JumpStarted;
        input.playerActions.Jump.performed -= JumpPerformed;
        input.playerActions.Jump.canceled -= JumpCanceled;
    }
    
    protected void MoveStarted(InputAction.CallbackContext context)
    {
        horizontal = context.ReadValue<Vector2>().x;
    }

    protected void VerticalMoveStarted(InputAction.CallbackContext context)
    {
        vertical = context.ReadValue<Vector2>().y;
    }

    protected void JumpStarted(InputAction.CallbackContext context)
    {
        isJumping = true;
    }

    protected void JumpPerformed(InputAction.CallbackContext context)
    {
        isJumpPerformed = true;
    }

    protected void JumpCanceled(InputAction.CallbackContext context)
    {
        player.isDownThroughPlatform = false;
        isJumping = false;
        isJumpPerformed = false;
    }
    #endregion
}
