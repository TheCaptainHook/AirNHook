using System.Collections;
using Mirror;
using UnityEngine;

public class HookMovement : PlayerMovement, IInhalable
{
    public bool isSwinging;
    public bool swingJump = false;
    public bool isAirAttached;
    public Vector2 ropeHook;
    public float swingForce;
    public Grappling grappling;
    private float _swingFloat;

    #region StringCache
    private static readonly int IsGrappling = Animator.StringToHash("IsGrappling");
    private static readonly int SwingingForce = Animator.StringToHash("SwingingForce");
    private static readonly int IsAirAttached = Animator.StringToHash("IsAirAttached");
    #endregion
    protected override void Awake()
    {
        base.Awake();
        grappling = GetComponent<Grappling>();
    }

    protected override void Movement()
    {
        if (_horizontal != 0)
        {
            if (isSwinging)
            {
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
            var groundForce = _moveSpeed * 5f;
            _rigidbd.AddForce(new Vector2(-_rigidbd.velocity.x * groundForce, 0f));
            _rigidbd.velocity = new Vector2(_rigidbd.velocity.x, _rigidbd.velocity.y);
        }
    }

    protected override void Jump()
    {
        if(!isSwinging)
            base.Jump();
    }

    protected override void IsFloor()
    {
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

    #region Inhale
    private Transform _fixedPoint;
    private Coroutine _inhaleCoroutine;
    [field: SerializeField] private float _inhalePower;
    private bool _isFixed;
    private WaitForFixedUpdate _waitForFixedUpdate = new();
    
    public void Inhalation(Transform accessor)
    {
        canControl = false;
        _isFixed = false;
        _fixedPoint = accessor;
        _inhaleCoroutine = StartCoroutine(Co_Inhale());
        Debug.Log("a");
    }

    private IEnumerator Co_Inhale()
    {
        while (true)
        {
            if (!_isFixed)
            {
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
                yield return null;

                _rigidbd.velocity = Vector2.zero;
                transform.position = _fixedPoint.position;
            }
        }
    }

    public void StopInhale()
    {
        StopCoroutine(_inhaleCoroutine);
        Debug.Log("d");
        canControl = true;
        _isFixed = false;
        _fixedPoint = null;
        _rigidbd.gravityScale = _gravityScale;
    }

    public void Shooting(Vector2 force)
    {
        StopCoroutine(_inhaleCoroutine);
        canControl = true;
        _isFixed = false;
        _fixedPoint = null;
        _rigidbd.gravityScale = _gravityScale;
        swingJump = true;
        _rigidbd.velocity = Vector2.zero;
        _rigidbd.AddForce(force, ForceMode2D.Impulse);
    }

    public bool CanInhale()
    {
        return !isSwinging;
    }
    #endregion
}
