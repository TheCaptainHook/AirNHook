using UnityEngine;

public class WalkState : BaseState
{
    public WalkState(StateMachine stateMachine) : base(stateMachine) { }

    public override void EnterState()
    {
        stateMachine.moveSpeedMultiplier = 2f;
    }

    public override void PhysicsUpdate()
    {
        Move();
        TryStepOver();
    }

    #region Movement
    protected override void OnMove()
    {
        if (stateMachine.horizontal == 0)
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
        if (!stateMachine.canMovable)
        {
            stateMachine.ChangeState(stateMachine.IdleState);
            return;
        }

        var groundForce = stateMachine.moveSpeed * stateMachine.moveSpeedMultiplier;
        
        stateMachine.rigidbody2D.AddForce(new Vector2((stateMachine.horizontal * groundForce - rigidbd.velocity.x) * groundForce, 0f));
        rigidbd.velocity = new Vector2(rigidbd.velocity.x, rigidbd.velocity.y);
    }

    private void TryStepOver()
    {
        if (!stateMachine.canMovable)
            return;

        float maxStepHeight = 0.4f;
        float checkDistance = 1f;
        float groundCheckDistance = 0.1f;

        Vector2 origin = stateMachine.rigidbody2D.position;
        Vector2 dir = new Vector2(stateMachine.horizontal, 0).normalized;

        if (stateMachine.rigidbody2D.velocity.y > 0.1f)
            return;

        RaycastHit2D groundHit = Physics2D.Raycast(origin + Vector2.up * 0.001f, Vector2.down, groundCheckDistance, stateMachine.player.playerData.floorLayerMask);

        if (groundHit.collider == null)
            return;

        RaycastHit2D lowerHit = Physics2D.Raycast(origin + Vector2.up * 0.001f, dir, checkDistance, stateMachine.player.playerData.floorLayerMask);

        if (lowerHit.collider == null) return; // 계단이 없으면 종료

        float surfaceAngle = Vector2.Angle(lowerHit.normal, Vector2.up);

        if (surfaceAngle < 85f) return; // 경사진(slope) 곳이면 종료

        Vector2 verticalCheckOrigin = lowerHit.point + Vector2.up * maxStepHeight + new Vector2(0.01f, 0f) * dir;
        RaycastHit2D upperHit = Physics2D.Raycast(verticalCheckOrigin, Vector2.down, maxStepHeight, stateMachine.player.playerData.floorLayerMask);

        if (upperHit.collider == null) return; // 계단이 너무 높으면 종료

        float stepHeight = upperHit.point.y - origin.y;

        if (stepHeight <= maxStepHeight)
        {
            stateMachine.stepPower = stepHeight + (0.1f + 0.625f * stepHeight);
            stateMachine.ChangeState(stateMachine.StepUpState);
        }
    }
    #endregion
}
