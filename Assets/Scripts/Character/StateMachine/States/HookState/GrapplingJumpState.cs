using UnityEngine;

public class GrapplingJumpState : BaseState
{
    public GrapplingJumpState(StateMachine stateMachine) : base(stateMachine) { }

    public override void Update()
    {
        if (isGround)
        {
            stateMachine.ChangeState(stateMachine.FallingState);
            return;
        }

        OnMove();
    }

    protected override void OnMove() { }

    protected override void Move()
    {
        rigidbd.AddForce(new Vector2(stateMachine.horizontal * stateMachine.moveSpeed, 0f));
        rigidbd.velocity = new Vector2(rigidbd.velocity.x, rigidbd.velocity.y);
    }
}
