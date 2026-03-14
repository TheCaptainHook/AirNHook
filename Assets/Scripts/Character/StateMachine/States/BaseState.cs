using UnityEngine;

public class BaseState : IState
{
    protected PlayerStateMachine stateMachine;
    protected Rigidbody2D rigidbd => stateMachine.rigidbody2D;
    protected bool isGround => stateMachine.player.isGround;
    protected float coyoteTimeCount
    {
        get => stateMachine.player.coyoteTimeCount;
        set => stateMachine.player.coyoteTimeCount = value;
    }

    protected ParticleSystem jumpParticle => stateMachine.player.jumpParticle;
    protected ParticleSystem landParticle => stateMachine.player.landParticle;
    
    public BaseState(StateMachine stateMachine)
    {
        this.stateMachine = (PlayerStateMachine)stateMachine;
    }
    
    public virtual void EnterState() { }
    
    public virtual void ExitState() { }

    #region Loop Method
    public virtual void HandleInput() { }
    
    public virtual void Update()
    {
        if (CheckFalling()) return;
        
        OnMove();

        OnJump();
    }
    
    public virtual void PhysicsUpdate()
    {
        Move();
    }
    #endregion
    
    #region Movement

    protected virtual void OnMove()
    {
        stateMachine.player.animator.SetBool(stateMachine.player.animationData.WalkParameterHash, (stateMachine.horizontal != 0 || !stateMachine.canMovable) && isGround);

        if (stateMachine.horizontal < 0)
        {
            stateMachine.player.charPivot.rotation = Quaternion.Euler(0f, 180f, 0f);
        }
        else if (stateMachine.horizontal > 0)
        {
            stateMachine.player.charPivot.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
    }

    protected virtual void OnJump()
    {
        if (!isGround) return;

        if (!stateMachine.canMovable) return;
        
        if (coyoteTimeCount < 0f) return;
        
        if (!stateMachine.isJumping && !stateMachine.isJumpPerformed) return;
        
        if (stateMachine.vertical < 0f)
        {
            stateMachine.player.isDownThroughPlatform = true;
            return;
        }

        stateMachine.ChangeState(stateMachine.JumpState);
    }
    
    protected virtual void Move() 
    {
        if (!stateMachine.canMovable) return;
    }
    
    protected virtual bool CheckFalling()
    {
        if (stateMachine.rigidbody2D.velocity.y >= 0f || isGround) return false;
        
        stateMachine.ChangeState(stateMachine.FallingState);
        return true;
    }
    #endregion
    
    #region Animation
    protected virtual void PlayAnimation() { }
    #endregion
}
