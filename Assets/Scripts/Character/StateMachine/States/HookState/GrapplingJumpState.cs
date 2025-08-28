using UnityEngine;

public class GrapplingJumpState : BaseState
{
    private HookStateMachine _hookStateMachine;

    public GrapplingJumpState(StateMachine stateMachine) : base(stateMachine)
    {
        _hookStateMachine = (HookStateMachine)stateMachine;
    }

    public override void EnterState()
    {
        var power = rigidbd.velocity / 2f;
        rigidbd.AddForce(power);
        
        if (rigidbd.velocity.y > 0)
            rigidbd.AddForce(new Vector2(0, _hookStateMachine.swingJumpPower * rigidbd.velocity.magnitude * 0.1f), ForceMode2D.Impulse);
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
        stateMachine.player.animator.SetBool(stateMachine.player.animationData.WalkParameterHash, true);
        stateMachine.player.animator.SetBool(stateMachine.player.animationData.JumpParameterHash, true);

        if (!stateMachine.canMovable)
            return;

        if (stateMachine.horizontal < 0)
            stateMachine.player.charPivot.rotation = Quaternion.Euler(0f, 180f, 0f);
        else if (stateMachine.horizontal > 0)
            stateMachine.player.charPivot.rotation = Quaternion.Euler(0f, 0f, 0f);
    }

    protected override void Move()
    {
        var horizontal = stateMachine.canMovable ? stateMachine.horizontal : 0;

        rigidbd.AddForce(new Vector2(horizontal * stateMachine.moveSpeed, 0f));
        rigidbd.velocity = new Vector2(rigidbd.velocity.x, rigidbd.velocity.y);
    }
}
