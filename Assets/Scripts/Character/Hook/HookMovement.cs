using System.Collections;
using Mirror;
using UnityEngine;

public class HookMovement : PlayerMovement, IInhalable
{
    public bool isSwinging;
    public bool swingJump = false;
    [SyncVar] public bool isAirAttached;
    public Vector2 ropeHook;
    public float swingForce;
    public Grappling grappling;
    private float _swingFloat;

    #region StringCache
    private static readonly int IsGrappling = Animator.StringToHash("IsGrappling");
    private static readonly int SwingingForce = Animator.StringToHash("SwingingForce");
    private static readonly int IsAirAttached = Animator.StringToHash("IsAirAttached");
    private static readonly int IsHookInhaled = Animator.StringToHash("IsHookInhaled");
    #endregion
    
    protected override void Awake()
    {
        base.Awake();
        grappling = GetComponent<Grappling>();
    }

    #region Movement
    protected override void Movement()
    {
        if (_horizontal != 0)
        {
            _rigidbd.drag = 0f;
            if (isSwinging)
            {
                _rigidbd.drag = 0.2f;
                var playerToHookDirection = (ropeHook - (Vector2)transform.position).normalized;
                Vector2 perpendicularDirection;
                if (_horizontal < 0)
                    perpendicularDirection = new Vector2(-playerToHookDirection.y, playerToHookDirection.x);
                else
                    perpendicularDirection = new Vector2(playerToHookDirection.y, -playerToHookDirection.x);

                var force = perpendicularDirection * swingForce;
                _rigidbd.AddForce(force, ForceMode2D.Force);
            }
            else if (!swingJump)
            {
                var groundForce = _moveSpeed * 2f;
                _rigidbd.AddForce(new Vector2((_horizontal * groundForce - _rigidbd.velocity.x) * groundForce, 0f));
                _rigidbd.velocity = new Vector2(_rigidbd.velocity.x, _rigidbd.velocity.y);
            }
            else
            {
                _rigidbd.AddForce(new Vector2(_horizontal * _moveSpeed, 0f));
                _rigidbd.velocity = new Vector2(_rigidbd.velocity.x, _rigidbd.velocity.y);
            }
        }
        else if(isGround)
        {
            _rigidbd.drag = 0f;
            var groundForce = _moveSpeed * 5f;
            _rigidbd.AddForce(new Vector2(-_rigidbd.velocity.x * groundForce, 0f));
            _rigidbd.velocity = new Vector2(_rigidbd.velocity.x, _rigidbd.velocity.y);
        }
        else if(isSwinging)
        {
            _rigidbd.drag = 0.2f;
        }
    }

    protected override void Jump()
    {
        if(!isSwinging)
            base.Jump();
    }

    protected override void IsFloor()
    {
        var velocity = (transform.position - previous) / Time.deltaTime;
        
        if(velocity.y > 0.15f) return;
        
        for (int i = -1; i < 2; i++)
        {
            if (Physics2D.Raycast(transform.position + (Vector3.right * (0.4f * i)), Vector2.down, 0.1f, _floorLayer))
            {
                if (!isGround)
                {
                    _landParticles.Play();
                }
                isGround = true;
                swingJump = false;
                coyoteTimeCount = _coyoteTime;
                if(isSwinging)
                    grappling.ResetRope();
                return;
            }
        }
        isGround = false;
        coyoteTimeCount -= Time.deltaTime;
    }

    protected override bool IsRightHead()
    {
        return !isSwinging && base.IsRightHead();
    }

    protected override bool IsLeftHead()
    {
        return !isSwinging && base.IsLeftHead();
    }
    #endregion

    #region Animation
    protected override void MoveAnimation()
    {
        _animator.SetBool(IsAirAttached, isAirAttached);
        _animator.SetBool(IsGrappling, isSwinging);
        if (_horizontal != 0 && isSwinging)
        {
            _swingFloat += Time.deltaTime;
            _animator.SetFloat(SwingingForce, _swingFloat);
        }

        if (_horizontal == 0 && isSwinging || !isSwinging)
        {
            _swingFloat = 0;
        }
        if (isGround)
        {
            _swingFloat = 0;
        }
        base.MoveAnimation();
    }
    #endregion

    #region Inhale
    private Transform _fixedPoint;
    private Coroutine _inhaleCoroutine;
    [field: SerializeField] private float _inhalePower;
    [SyncVar] private bool _isFixed;
    [SyncVar] private bool _canInteract;
    private WaitForFixedUpdate _waitForFixedUpdate = new();
    
    public void Inhalation(Transform accessor)
    {
        Debug.Log("a");
        canControl = false;
        grappling.canControl = false;
        _fixedPoint = accessor;
        _inhaleCoroutine = StartCoroutine(Co_Inhale());
    }

    private IEnumerator Co_Inhale()
    {
        while (true)
        {
            if (!_isFixed)
            {
                Debug.Log("b");
                yield return _waitForFixedUpdate;

                if (_fixedPoint is null) break;
                
                var direction = (_fixedPoint.position - transform.position).normalized;
                var power = _inhalePower * Time.fixedDeltaTime;
                _rigidbd.gravityScale = 0f;
                _rigidbd.AddForce(direction * power);

                if (Vector2.Distance(_fixedPoint.position, transform.position) <= 0.2f)
                    _isFixed = true;
            }
            else
            {
                Debug.Log("c");
                yield return null;
                _rigidbd.drag = 0f;
                _animator.SetBool(IsHookInhaled, true);
                _rigidbd.velocity = Vector2.zero;
                
                if (_fixedPoint is null) break;
                transform.position = _fixedPoint.position;
            }
        }

        _inhaleCoroutine = null;
    }

    public void StopInhale()
    {
        Debug.Log("d");
        if(_inhaleCoroutine is not null)
            StopCoroutine(_inhaleCoroutine);
        canControl = true;
        _isFixed = false;
        ChangeState(false);
        grappling.canControl = true;
        _fixedPoint = null;
        _rigidbd.gravityScale = _gravityScale;
        _animator.SetBool(IsHookInhaled, false);
    }

    public void Fixed(bool value)
    {
        _isFixed = value;
        _canInteract = !value;
    }

    public void Inhaling(bool value)
    {
        _canInteract = !value;
    }

    public void Shooting(Vector2 force)
    {
        Debug.Log("f");
        StopInhale();
        swingJump = true;
        _rigidbd.velocity = Vector2.zero;
        _rigidbd.AddForce(force, ForceMode2D.Impulse);
    }

    public bool CanInhale()
    {
        return !isSwinging;
    }
    #endregion

    #region Command
    private void ChangeState(bool value)
    {
        CmdChangeFixedState(value);
        CmdChangeInteractState(!value);
    }
    
    [Command(requiresAuthority = false)]
    private void CmdChangeFixedState(bool value)
    {
        _isFixed = value;
    }

    [Command(requiresAuthority = false)]
    private void CmdChangeInteractState(bool value)
    {
        _canInteract = value;
    }
    #endregion
}
