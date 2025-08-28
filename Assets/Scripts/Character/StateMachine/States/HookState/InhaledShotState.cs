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

    protected override void OnMove()
    {
        if (!stateMachine.canMovable) return;

        if (stateMachine.horizontal < 0)
            stateMachine.player.charPivot.rotation = Quaternion.Euler(0f, 180f, 0f);
        else if (stateMachine.horizontal > 0)
            stateMachine.player.charPivot.rotation = Quaternion.Euler(0f, 0f, 0f);
    }
    
    protected override void Move()
    {
        var horizontal = stateMachine.canMovable ? stateMachine.horizontal : 0f;

        rigidbd.AddForce(new Vector2(horizontal * stateMachine.moveSpeed, 0f));
        rigidbd.velocity = new Vector2(rigidbd.velocity.x, rigidbd.velocity.y);
    }
}
