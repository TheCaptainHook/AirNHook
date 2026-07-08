using UnityEngine;

public class GrapplingState : BaseState
{
    private HookStateMachine _hookStateMachine;
    private HookSM _hook;
    private Vector2 ropeHook => _hook.grappling.hookAnchorPos;
    private float _swingFloat = 0f;

    public GrapplingState(StateMachine stateMachine) : base(stateMachine)
    {
        _hook = (HookSM)stateMachine.player;
        _hookStateMachine = (HookStateMachine)stateMachine;
    }

    public override void EnterState()
    {
        stateMachine.moveSpeedMultiplier = 2f;
        rigidbd.drag = 0.2f;
        _swingFloat = 0f;
        
        stateMachine.player.animator.SetBool(((HookAnimationData)stateMachine.player.animationData).GrapplingParameterHash, true);
    }

    public override void ExitState()
    {
        rigidbd.drag = 0f;
        _swingFloat = 0f;
        
        stateMachine.player.animator.SetBool(((HookAnimationData)stateMachine.player.animationData).GrapplingParameterHash, false);
    }

    public override void Update()
    {
        OnMove();
    }

    #region Movement
    protected override void OnJump() { }

    protected override void OnMove()
    {
        rigidbd.drag = stateMachine.horizontal == 0 ? 0.4f : 0.2f;

        if (stateMachine.canMovable)
        {
            if (stateMachine.horizontal < 0)
                stateMachine.player.charPivot.rotation = Quaternion.Euler(0f, 180f, 0f);
            else if (stateMachine.horizontal > 0)
                stateMachine.player.charPivot.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
        
        _swingFloat += (stateMachine.horizontal != 0 ? 1 : -1) * Time.deltaTime;
        _swingFloat = Mathf.Clamp(_swingFloat, 0f, 1f);
        
        stateMachine.player.animator.SetFloat(((HookAnimationData)stateMachine.player.animationData).SwingForceParameterHash, _swingFloat);
    }

    protected override void Move()
    {
        if (!stateMachine.canMovable) return;

        if (stateMachine.horizontal == 0) return;

        var playerToHookDirection = (ropeHook - (Vector2)_hookStateMachine.player.transform.position).normalized;
        var perpendicularDirection = _hookStateMachine.horizontal < 0 ? new Vector2(-playerToHookDirection.y, playerToHookDirection.x) : new Vector2(playerToHookDirection.y, -playerToHookDirection.x);

        var force = perpendicularDirection * _hookStateMachine.swingForce;
        rigidbd.AddForce(force, ForceMode2D.Force);
    }
    #endregion
}
