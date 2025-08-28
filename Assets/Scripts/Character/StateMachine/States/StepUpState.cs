using UnityEngine;

public class StepUpState : BaseState
{
    private bool _isJumped = false;
    public StepUpState(StateMachine stateMachine) : base(stateMachine) { }

    public override void EnterState()
    {
        stateMachine.moveSpeedMultiplier = 2f;
        stateMachine.player.animator.SetBool(stateMachine.player.animationData.WalkParameterHash, false);
        Jump();
    }

    public override void ExitState()
    {
        stateMachine.stepPower = 0f;
        _isJumped = false;
    }

    #region Loop Method
    public override void Update()
    {
        OnMove();

        if (!_isJumped) return;

        if (stateMachine.rigidbody2D.velocity.y <= 0f)
            stateMachine.ChangeState(stateMachine.FallingState);

        OnJump();
    }
    #endregion

    #region Movement
    protected override void OnMove()
    {
        stateMachine.player.animator.SetBool(stateMachine.player.animationData.FallingParameterHash, stateMachine.horizontal == 0);
        stateMachine.player.animator.SetBool(stateMachine.player.animationData.JumpParameterHash, stateMachine.horizontal != 0);

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
        var groundForce = stateMachine.moveSpeed * stateMachine.moveSpeedMultiplier;

        stateMachine.rigidbody2D.AddForce(new Vector2((horizontal * groundForce - rigidbd.velocity.x) * groundForce, 0f));
        rigidbd.velocity = new Vector2(rigidbd.velocity.x, rigidbd.velocity.y);
    }

    private void Jump()
    {
        float gravity = Mathf.Abs(Physics2D.gravity.y) * stateMachine.rigidbody2D.gravityScale;
        float requiredVelocity = Mathf.Sqrt(2 * gravity * stateMachine.stepPower);

        coyoteTimeCount = 0f;
        stateMachine.rigidbody2D.velocity = new Vector2(stateMachine.rigidbody2D.velocity.x, requiredVelocity);

        _isJumped = true;
        jumpParticle.Play();
        stateMachine.player.CmdJumpParticlePlay();
    }

    protected override void OnJump()
    {
        if (!stateMachine.canMovable) return;

        if (!stateMachine.isJumping && !stateMachine.isJumpPerformed) return;

        stateMachine.ChangeState(stateMachine.JumpState);
    }
    #endregion
}