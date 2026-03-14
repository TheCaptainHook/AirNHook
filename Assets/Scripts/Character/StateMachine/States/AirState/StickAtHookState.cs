using UnityEngine;

public class StickAtHookState : BaseState
{
    public StickAtHookState(StateMachine stateMachine) : base(stateMachine) { }
    
    public override void EnterState()
    {
        stateMachine.player.animator.SetBool(GlobalText.AIR_ATTACHED_ANIMATION_STRING, true);
    }

    public override void ExitState()
    {
        stateMachine.player.animator.SetBool(GlobalText.AIR_ATTACHED_ANIMATION_STRING, false);
    }
    
    public override void Update()
    {
        if (!stateMachine.canMovable)
        {
            ((AirSM)(stateMachine.player)).StopSticking();
            stateMachine.ChangeState(stateMachine.IdleState);
        }
    }

    protected override void OnMove()
    {
        // Freeze
    }

    protected override void Move()
    {
        // Freeze
    }

    protected override void OnJump()
    {
        // Freeze
    }
}