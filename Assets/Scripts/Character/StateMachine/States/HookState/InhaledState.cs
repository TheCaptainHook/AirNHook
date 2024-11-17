public class InhaledState : BaseState
{
    public InhaledState(StateMachine stateMachine) : base(stateMachine) { }

    public override void EnterState()
    {
        stateMachine.player.animator.SetBool(GlobalText.HOOK_INHALED_ANIMATION_STRING, true);
        stateMachine.player.invincible = true;
    }

    public override void ExitState()
    {
        stateMachine.player.animator.SetBool(GlobalText.HOOK_INHALED_ANIMATION_STRING, false);
        stateMachine.player.invincible = false;
    }

    protected override void OnMove() { }

    protected override void Move() { }
}
