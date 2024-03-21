using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    //점프 버퍼 체크
    private bool _isJumpBufferCheck;
    private bool _isJumpPerformed;
    //코요테 타임
    [SerializeField] private float _coyoteTime = 0.2f;
    private float _coyoteTimeCount;
    //플레이어 점프체크
    private bool _isJumping;

    //점프력
    [SerializeField] private float _jumpingPower = 10f;
    //땅 체크
    [SerializeField] private Transform _floorCheck;
    [SerializeField] private LayerMask _floorLayer;

    [SerializeField] private Rigidbody2D _rigidbd;
    public PlayerInput playerInput { get; private set; }
    //좌우 움직임
    private float _horizontal;
    //움직임속도
    [SerializeField] private float _moveSpeed = 2f;
    
    private Animator _animator;
    [SerializeField] private Transform _charPivot;

    #region StringCache
    private static readonly int IsMoving = Animator.StringToHash("IsMoving");
    private static readonly int IsJumping = Animator.StringToHash("IsJumping");
    private static readonly int IsStayJumping = Animator.StringToHash("IsStayJumping");
    #endregion
    
    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _rigidbd = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        playerInput = GetComponent<PlayerInput>();

        //움직임 입력
        playerInput.playerActions.Move.started += MoveStarted;
        //점프 입력
        playerInput.playerActions.Jump.started += JumpStarted;
        playerInput.playerActions.Jump.performed += JumpPerformed;
        playerInput.playerActions.Jump.canceled += JumpCanceled;
    }

    private void Update()
    {
        // 땅 체크
        if (IsFloor())
        {
            _coyoteTimeCount = _coyoteTime;
        }
        else
        {
            _coyoteTimeCount -= Time.deltaTime;
        }

        if (CheckJumpBuffer())
        {
            _isJumpBufferCheck = true;
        }
        else
        {
            _isJumpBufferCheck = false;
            _isJumping = false;
        }
    }

    private void FixedUpdate()
    {
        //머리충돌검사
        IsLeftHead();
        IsRightHead();

        //좌우무브
        Movement();
        //점프
        Jump();
        
        //애니메이션체크
        MoveAnimation();
    }

    public virtual void Movement()
    {
        //플레이어 이동속도 코드
        var groundForce = _moveSpeed * 5f;
        _rigidbd.AddForce(new Vector2((_horizontal * groundForce - _rigidbd.velocity.x) * groundForce, 0f));
        _rigidbd.velocity = new Vector2(_rigidbd.velocity.x, _rigidbd.velocity.y);
    }

    private void MoveStarted(InputAction.CallbackContext context)
    {
        _horizontal = context.ReadValue<Vector2>().x;
    }

    private void Jump()
    {
        if (_coyoteTimeCount > 0f && (_isJumpPerformed || (_isJumping && _isJumpBufferCheck)))
        {
            _coyoteTimeCount = 0f;
            _isJumping = false;
            _rigidbd.velocity = new Vector2(_rigidbd.velocity.x, _jumpingPower);
        }
    }
    
    private void JumpStarted(InputAction.CallbackContext context)
    {
        _isJumping = true;
    }

    //점프 꾹 누르고있을때
    private void JumpPerformed(InputAction.CallbackContext context)
    {
        _isJumpPerformed = true;
    }

    private void JumpCanceled(InputAction.CallbackContext context)
    {
        _isJumpPerformed = false;
    }
    
    //점프체크
    private bool IsFloor()
    {
        //Ray발사
        for (int i = -1; i < 2; i++)
        {
            if (Physics2D.Raycast(transform.position + (Vector3.right * 0.5f * i), Vector2.down, 0.1f, _floorLayer))
            {
                return true;
            }
        }
        return false;
    }

    //점프 버퍼 체크
    private bool CheckJumpBuffer()
    {
        if(_rigidbd.velocity.y > 0f)
            return false;
        
        //Ray발사
        for (int i = -1; i < 2; i++)
        {
            if (Physics2D.Raycast(transform.position + (Vector3.right * 0.5f * i), Vector2.down, 0.5f, _floorLayer))
            {
                return true;
            }
        }
        return false;
    }

    //오른쪽 머리 충돌체크
    private bool IsLeftHead()
    {
        //레이 발사 / 정상 작동
        for (int i = -2; i < 3; i++)
        {
            if (Physics2D.Raycast(transform.position + (Vector3.right * 0.25f * i), Vector2.up, 1.5f, _floorLayer))
            {
                if (i == 2)
                {
                    transform.position = new Vector3(transform.position.x - 0.26f, transform.position.y);
                }
                return true;
            }
        }
        return false;
    }

    //왼쪽 머리 충돌체크
    private bool IsRightHead()
    {
        for (int j = -2; j < 3; j++)
        {
            if (Physics2D.Raycast(transform.position + (Vector3.right * -0.25f * j), Vector2.up, 1.5f, _floorLayer))
            {
                if (j == 2)
                {
                    transform.position = new Vector3(transform.position.x + 0.26f, transform.position.y);
                }
                return true;
            }
        }
        return false;
    }
    
    private void MoveAnimation()
    {
        //이동에 따라 애니메이션 제어
        _animator.SetBool(IsMoving, _horizontal != 0 && IsFloor());
        _animator.SetBool(IsJumping, _horizontal != 0 && _isJumping || _horizontal != 0 && !IsFloor());
        _animator.SetBool(IsStayJumping, _horizontal == 0 && _isJumping || _horizontal == 0 && !IsFloor());
        // CharPivot 오브젝트의 로테이션을 사용하여 플립
        if (_horizontal < 0)
        {
            _charPivot.rotation = Quaternion.Euler(0f, 180f, 0f); // 왼쪽으로 이동할 때 캐릭터를 뒤집음
        }
        else if (_horizontal > 0)
        {
            _charPivot.rotation = Quaternion.Euler(0f, 0f, 0f); // 오른쪽으로 이동할 때 캐릭터를 되돌림
        }
    }

    //기즈모
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        //for (int i = -2; i < 3; i++)
        //{
        //    Gizmos.DrawRay(transform.position + (Vector3.right * -0.25f * i), Vector2.up * 1.5f);
        //}
        for (int i = -1; i < 2; i++)
        {
            Gizmos.DrawRay(transform.position + (Vector3.right * 0.5f * i), Vector2.down * 0.5f);
        }
    }
}
