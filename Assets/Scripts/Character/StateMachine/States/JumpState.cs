using UnityEngine;

public class JumpState : BaseState
{
    private bool _isJumped = false;
    private LayerMask ceilingLayer;
    private LayerMask halfPlatformLayer;
    private Transform playerTransform;
    private bool _oneCheck;
    private float _rayLength = 0.25f;
    private float _baseNudgeAmount = 0.133f;
    private float _nudgeAmount = 0.05f;
    private float _headWidth = 0.8f;

    public JumpState(StateMachine stateMachine) : base(stateMachine)
    {
        halfPlatformLayer = stateMachine.player.halfPlatformLayer;
        ceilingLayer = stateMachine.player.playerData.floorLayerMask & ~(1 << halfPlatformLayer);
        playerTransform = stateMachine.player.transform;
    }
    
    public override void EnterState()
    {
        stateMachine.moveSpeedMultiplier = 2f;
        stateMachine.player.animator.SetBool(stateMachine.player.animationData.WalkParameterHash, false);
        _oneCheck = false;
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

        if (stateMachine.rigidbody2D.velocity.y <= 0f)
        {
            stateMachine.ChangeState(stateMachine.FallingState);
            return;
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        if (rigidbd.velocity.y >= 5f && !_oneCheck)
        {
            HandleCeilingSlide();
        }
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
        coyoteTimeCount = 0f;
        stateMachine.rigidbody2D.velocity = new Vector2(stateMachine.rigidbody2D.velocity.x, stateMachine.jumpPower);
        _isJumped = true;
        jumpParticle.Play();
        stateMachine.player.CmdJumpParticlePlay();
        //Achievement 0605
        Managers.AcManager.CallPlayerJumping();
        //Achievement 0605
        stateMachine.player.JumpSoundPlay();
    }

    private void HandleCeilingSlide()
    {
        if (!stateMachine.canMovable)
            return;

        Vector2 origin = playerTransform.position;
        float halfWidth = _headWidth / 2f;

        int rayCount = 7;
        bool[] rayHit = new bool[rayCount];

        for (int i = 0; i < rayCount; i++)
        {
            float t = i / 6f;
            float offsetX = Mathf.Lerp(-halfWidth, halfWidth, t);
            Vector2 rayOrigin = origin + new Vector2(offsetX, 0.9f);

            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up, _rayLength, ceilingLayer);
            //rayHit[i] = hit.collider != null;
            rayHit[i] = hit.collider != null && ((1 << hit.collider.gameObject.layer) & halfPlatformLayer.value) == 0;
        }

        if (rayHit[3]) return;

        if (rayHit[2]) return;

        if (rayHit[4]) return;

        //int leftHits = (rayHit[0] ? 1 : 0) + (rayHit[1] ? 1 : 0) + (rayHit[2] ? 1 : 0);
        int leftHits = (rayHit[0] ? 1 : 0) + (rayHit[1] ? 1 : 0);
        //int rightHits = (rayHit[4] ? 1 : 0) + (rayHit[5] ? 1 : 0) + (rayHit[6] ? 1 : 0);
        int rightHits = (rayHit[5] ? 1 : 0) + (rayHit[6] ? 1 : 0);

        if (leftHits > 0 && rightHits == 0)
        {
            if (rigidbd.velocity.x <= -0.02f) return;

            float nudge = _baseNudgeAmount * leftHits + _nudgeAmount;
            playerTransform.position += new Vector3(nudge, 0f, 0f);
            _oneCheck = true;
        }
        else if (rightHits > 0 && leftHits == 0)
        {
            if (rigidbd.velocity.x >= 0.02f) return;

            float nudge = _baseNudgeAmount * rightHits + _nudgeAmount;
            playerTransform.position += new Vector3(-nudge, 0f, 0f);
            _oneCheck = true;
        }
    }
    #endregion
}