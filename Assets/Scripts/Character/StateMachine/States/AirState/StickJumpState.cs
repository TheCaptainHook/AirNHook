using UnityEngine;

public class StickJumpState : BaseState
{
    public StickJumpState(StateMachine stateMachine) : base(stateMachine) { }
    
    public override void EnterState()
    {
        stateMachine.player.animator.SetBool(GlobalText.AIR_ATTACHED_ANIMATION_STRING, false);
    }
    
    public override void Update()
    {
        OnMove();
        
        if (isGround)
            stateMachine.ChangeState(stateMachine.FallingState);
    }
    
    protected override void Move()
    {
        var horizontal = stateMachine.canMovable ? stateMachine.horizontal : 0;
        
        stateMachine.rigidbody2D.AddForce(new Vector2((horizontal * stateMachine.moveSpeed), 0f));
        rigidbd.velocity = new Vector2(rigidbd.velocity.x, rigidbd.velocity.y);
    }
}
