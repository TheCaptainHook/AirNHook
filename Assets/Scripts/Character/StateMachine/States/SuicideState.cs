using UnityEngine;
using UnityEngine.InputSystem;

public class SuicideState : BaseState
{
    public PlayerSM player;

    public SuicideState(StateMachine stateMachine) : base(stateMachine) 
    {
        player = stateMachine.player;
    }

    public override void EnterState()
    {
        Managers.Game.playerInput.playerActions.Suicide.canceled += SuicideCancel;
        player.animator.SetTrigger(player.animationData.SuicideParameterHash);
    }

    public override void ExitState()
    {
        Managers.Game.playerInput.playerActions.Suicide.canceled -= SuicideCancel;
        player.animator.SetBool(stateMachine.player.animationData.JumpParameterHash, false);
        player.animator.SetBool(stateMachine.player.animationData.FallingParameterHash, false);
    }

    public override void Update() {}

    public override void PhysicsUpdate()
    {
        Move();
    }

    protected override void Move()
    {
        rigidbd.AddForce(isGround ? new Vector2(- rigidbd.velocity.x * stateMachine.moveSpeed * stateMachine.moveSpeedMultiplier, 0f)
            : new Vector2(stateMachine.horizontal * stateMachine.moveSpeed, 0f));
        rigidbd.velocity = new Vector2(rigidbd.velocity.x, rigidbd.velocity.y);
    }

    private void SuicideCancel(InputAction.CallbackContext context)
    {
        Managers.Game.playerInput.playerActions.Suicide.canceled -= SuicideCancel;
        player.animator.SetTrigger(player.animationData.CancelSuicideParameterHash);
        stateMachine.ChangeState(isGround ? stateMachine.IdleState : stateMachine.FallingState);
    }
}