using UnityEngine;

public class JumpState : BaseState
{
    private bool _isJumped = false;
    
    public JumpState(StateMachine stateMachine) : base(stateMachine) { }
    
    public override void EnterState()
    {
        stateMachine.moveSpeedMultiplier = 2f;
        stateMachine.player.animator.SetBool(stateMachine.player.animationData.WalkParameterHash, false);
        Jump();
    }

    public override void ExitState()
    {
        _isJumped = false;
    }

    #region Loop Method
    public override void Update()
    {
        OnMove();
        
        if(!_isJumped) return;
        
        if(stateMachine.rigidbody2D.velocity.y <= 0f)
            stateMachine.ChangeState(stateMachine.FallingState);
    }
    #endregion

    #region Movement
    protected override void OnMove()
    {
        if (!stateMachine.canMovable) return;

        stateMachine.player.animator.SetBool(stateMachine.player.animationData.FallingParameterHash, stateMachine.horizontal == 0);
        stateMachine.player.animator.SetBool(stateMachine.player.animationData.JumpParameterHash, stateMachine.horizontal != 0);
        
        if (stateMachine.horizontal < 0)
            stateMachine.player.charPivot.rotation = Quaternion.Euler(0f, 180f, 0f);
        else if (stateMachine.horizontal > 0)
            stateMachine.player.charPivot.rotation = Quaternion.Euler(0f, 0f, 0f);
    }

    protected override void Move()
    {
        if (!stateMachine.canMovable) return;

        var groundForce = stateMachine.moveSpeed * stateMachine.moveSpeedMultiplier;
        
        stateMachine.rigidbody2D.AddForce(new Vector2((stateMachine.horizontal * groundForce - rigidbd.velocity.x) * groundForce, 0f));
        rigidbd.velocity = new Vector2(rigidbd.velocity.x, rigidbd.velocity.y);
    }

    private void Jump()
    {
        coyoteTimeCount = 0f;
        stateMachine.rigidbody2D.velocity = new Vector2(stateMachine.rigidbody2D.velocity.x, stateMachine.jumpPower);
        _isJumped = true;
        stateMachine.player.CmdJumpParticlePlay();
    }
    #endregion
}