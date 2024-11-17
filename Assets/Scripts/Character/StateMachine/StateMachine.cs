using UnityEngine;

public abstract class StateMachine
{
    public PlayerSM player { get; protected set; }
    
    public IState CurrentState { get; protected set; }

    public virtual void Initialize() { }
    
    public void ChangeState(IState newState)
    {
        CurrentState?.ExitState();
        CurrentState = newState;
        CurrentState?.EnterState();
    }

    public virtual void SubscribeInput() { }
    
    public virtual void UnsubscribeInput() { }

    public void HandleInput()
    {
        CurrentState?.HandleInput();
    }

    public void Update()
    {
        CurrentState?.Update();
    }

    public void PhysicsUpdate()
    {
        CurrentState?.PhysicsUpdate();
    }

    public virtual void Die(DamageType damageType) { }
}
