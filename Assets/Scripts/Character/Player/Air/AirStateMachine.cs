public class AirStateMachine : PlayerStateMachine
{
    public AirSM air => (AirSM)player;

    #region States
    public IState StickAtHookState { get; private set; }
    public IState StickJumpState { get; private set; }
    #endregion

    #region InputValue
    //
    
    #endregion
    
    public AirStateMachine(PlayerSM player) : base(player)
    {
        StickAtHookState = new StickAtHookState(this);
        StickJumpState = new StickJumpState(this);
        
        Initialize();
    }
}
