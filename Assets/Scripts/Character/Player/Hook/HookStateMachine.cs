public class HookStateMachine : PlayerStateMachine
{
    private HookSM hook => (HookSM)player;
    
    #region States
    public IState GrapplingState { get; private set; }
    public IState GrapplingJumpState { get; private set; }
    public IState InhaledState { get; private set; }
    public IState InhaledShotState { get; private set; }
    #endregion
    
    #region InputValue
    public float swingForce => ((HookDataSO)hook.playerData).swingForce;
    public float swingJumpPower => ((HookDataSO)hook.playerData).swingJumpForce;
    #endregion
    
    public HookStateMachine(PlayerSM player) : base(player)
    {
        GrapplingState = new GrapplingState(this);
        GrapplingJumpState = new GrapplingJumpState(this);
        InhaledState = new InhaledState(this);
        InhaledShotState = new InhaledShotState(this);

        Initialize();
    }
    
    #region Input
    public override void SubscribeInput()
    {
        input.playerActions.Move.started += MoveStarted;
        input.playerActions.Move.performed += MoveStarted;
        input.playerActions.Move.canceled += MoveStarted;
        input.playerActions.VerticalMove.started += VerticalMoveStarted;
        input.playerActions.VerticalMove.performed += VerticalMoveStarted;
        input.playerActions.VerticalMove.canceled += VerticalMoveStarted;
        input.playerActions.Jump.started += JumpStarted;
        input.playerActions.Jump.performed += JumpPerformed;
        input.playerActions.Jump.canceled += JumpCanceled;
    }
    
    public override void UnsubscribeInput()
    {
        input.playerActions.Move.started -= MoveStarted;
        input.playerActions.Move.performed -= MoveStarted;
        input.playerActions.Move.canceled -= MoveStarted;
        input.playerActions.VerticalMove.started -= VerticalMoveStarted;
        input.playerActions.VerticalMove.performed -= VerticalMoveStarted;
        input.playerActions.VerticalMove.canceled -= VerticalMoveStarted;
        input.playerActions.Jump.started -= JumpStarted;
        input.playerActions.Jump.performed -= JumpPerformed;
        input.playerActions.Jump.canceled -= JumpCanceled;
    }
    #endregion
}
