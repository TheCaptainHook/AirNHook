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
        player.canMovable = false;
        Managers.Game.playerInput.playerActions.Suicide.canceled += SuicideCancel;
        player.animator.SetTrigger(player.animationData.SuicideParameterHash);
    }

    public override void ExitState()
    {
        player.canMovable = true;
        Managers.Game.playerInput.playerActions.Suicide.canceled -= SuicideCancel;
    }

    public override void Update() {}

    public override void PhysicsUpdate()
    {
        Move();
    }

    protected override void Move()
    {
        var groundForce = stateMachine.moveSpeed * stateMachine.moveSpeedMultiplier * 4f;
        
        stateMachine.rigidbody2D.AddForce(new Vector2(- rigidbd.velocity.x * groundForce, 0f));
        rigidbd.velocity = new Vector2(rigidbd.velocity.x, rigidbd.velocity.y);
    }

    private void SuicideCancel(InputAction.CallbackContext context)
    {
        Managers.Game.playerInput.playerActions.Suicide.canceled -= SuicideCancel;
        player.animator.SetTrigger(player.animationData.CancelSuicideParameterHash);
        stateMachine.ChangeState(stateMachine.IdleState);
    }
}