using UnityEngine;

public class WalkState : BaseState
{
    public WalkState(StateMachine stateMachine) : base(stateMachine) { }

    public override void EnterState()
    {
        stateMachine.moveSpeedMultiplier = 2f;
    }

    #region Movement
    protected override void OnMove()
    {
        if(stateMachine.horizontal == 0)
            stateMachine.ChangeState(stateMachine.IdleState);
        else
        {
            stateMachine.player.animator.SetBool(stateMachine.player.animationData.WalkParameterHash, true);
            if (stateMachine.horizontal < 0)
                stateMachine.player.charPivot.rotation = Quaternion.Euler(0f, 180f, 0f);
            else if (stateMachine.horizontal > 0)
                stateMachine.player.charPivot.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
    }
    
    protected override void Move()
    {
        var groundForce = stateMachine.moveSpeed * stateMachine.moveSpeedMultiplier;
        
        stateMachine.rigidbody2D.AddForce(new Vector2((stateMachine.horizontal * groundForce - rigidbd.velocity.x) * groundForce, 0f));
        rigidbd.velocity = new Vector2(rigidbd.velocity.x, rigidbd.velocity.y);
    }
    #endregion
}
