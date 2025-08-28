using UnityEditor.Build.Pipeline;
using UnityEngine;

public class IdleState : BaseState
{
    public IdleState(StateMachine stateMachine) : base(stateMachine)
    {
        
    }

    public override void EnterState()
    {
        stateMachine.moveSpeedMultiplier = 5f;
        stateMachine.player.animator.SetBool(stateMachine.player.animationData.FallingParameterHash, false);
        stateMachine.player.animator.SetBool(stateMachine.player.animationData.JumpParameterHash, false);
    }

    #region Movement
    protected override void OnMove()
    {
        if (!stateMachine.canMovable)
        {
            stateMachine.player.animator.SetBool(stateMachine.player.animationData.WalkParameterHash, false);
            return;
        }

        if (stateMachine.horizontal != 0)
            stateMachine.ChangeState(stateMachine.WalkState);
        else
            stateMachine.player.animator.SetBool(stateMachine.player.animationData.WalkParameterHash, false);
    }

    protected override void Move()
    {
        var groundForce = stateMachine.moveSpeed * stateMachine.moveSpeedMultiplier;
        
        stateMachine.rigidbody2D.AddForce(new Vector2(- rigidbd.velocity.x * groundForce, 0f));
        rigidbd.velocity = new Vector2(rigidbd.velocity.x, rigidbd.velocity.y);
    }
    #endregion
}
