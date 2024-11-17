using UnityEngine;

public class InhaledShotState : BaseState
{
    public InhaledShotState(StateMachine stateMachine) : base(stateMachine) { }

    public override void EnterState()
    {
        stateMachine.player.animator.SetBool(GlobalText.HOOK_INHALED_ANIMATION_STRING, false);
    }

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
