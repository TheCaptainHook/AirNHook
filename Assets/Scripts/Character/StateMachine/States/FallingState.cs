using Unity.VisualScripting;
using UnityEngine;

public class FallingState : BaseState
{
    public FallingState(StateMachine stateMachine) : base(stateMachine) { }

    public override void EnterState()
    {
        stateMachine.player.animator.SetBool(stateMachine.player.animationData.WalkParameterHash, false);
        stateMachine.moveSpeedMultiplier = 2f;
    }

    public override void ExitState()
    {
        stateMachine.player.animator.SetBool(stateMachine.player.animationData.JumpParameterHash, false);
        stateMachine.player.animator.SetBool(stateMachine.player.animationData.FallingParameterHash, false);
        
        //if (!stateMachine.player.canControl) return;
        
        // stateMachine.player.CmdLandParticlePlay();
    }

    #region Loop Method
    public override void Update()
    {
        OnMove();
        if (!isGround && coyoteTimeCount < 0f) return;

        if (!stateMachine.canMovable || (!stateMachine.isJumping && !stateMachine.isJumpPerformed))
        {
            stateMachine.player.isDownThroughPlatform = false;
            stateMachine.ChangeState(stateMachine.horizontal != 0 ? stateMachine.WalkState : stateMachine.IdleState);
            return;
        }

        if (stateMachine.isJumping || stateMachine.isJumpPerformed)
        {
            if (stateMachine.vertical < 0)
            {
                stateMachine.player.isDownThroughPlatform = true;
                if (stateMachine.player.isHalfPlatform)
                {
                    stateMachine.player.DownThroughHalfPlatform();
                    return;
                }
                else
                {
                    stateMachine.ChangeState(stateMachine.IdleState);
                    return;
                }
            }
            else
            {
                stateMachine.player.isDownThroughPlatform = false;
                stateMachine.ChangeState(stateMachine.JumpState);
                return;
            }
        }
    }
    #endregion

    #region Movement
    protected override void OnMove()
    {
        stateMachine.player.animator.SetBool(stateMachine.player.animationData.JumpParameterHash, stateMachine.horizontal != 0 && stateMachine.canMovable);
        stateMachine.player.animator.SetBool(stateMachine.player.animationData.FallingParameterHash, stateMachine.horizontal == 0 || !stateMachine.canMovable);

        if (!stateMachine.canMovable)
            return;

        if (stateMachine.horizontal < 0)
            stateMachine.player.charPivot.rotation = Quaternion.Euler(0f, 180f, 0f);
        else if (stateMachine.horizontal > 0)
            stateMachine.player.charPivot.rotation = Quaternion.Euler(0f, 0f, 0f);
    }

    protected override void Move()
    {   
        var horizontal = stateMachine.canMovable ? stateMachine.horizontal : 0f;
        var groundForce = stateMachine.moveSpeed * stateMachine.moveSpeedMultiplier;
        
        stateMachine.rigidbody2D.AddForce(new Vector2((horizontal * groundForce - rigidbd.velocity.x) * groundForce, 0f));
        rigidbd.velocity = new Vector2(rigidbd.velocity.x, rigidbd.velocity.y);
    }
    #endregion
}
