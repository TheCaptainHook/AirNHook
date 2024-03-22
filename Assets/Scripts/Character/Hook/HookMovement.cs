using UnityEngine;

public class HookMovement : PlayerMovement
{
    public bool isSwinging;
    public bool swingJump = false;
    public Vector2 ropeHook;
    public float swingForce;
    public Grappling grappling;

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
        }
        else if(_isGround)
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
                _isGround = true;
                swingJump = false;
                _coyoteTimeCount = _coyoteTime;
                if(isSwinging)
                    grappling.ResetRope();
                return;
            }
        }

        _isGround = false;
        _coyoteTimeCount -= Time.deltaTime;
    }

    protected override bool IsRightHead()
    {
        return !isSwinging && base.IsRightHead();
    }

    protected override bool IsLeftHead()
    {
        return !isSwinging && base.IsLeftHead();
    }
}
