using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    //점프 버퍼
    [SerializeField] private float _jumpBufferTime = 0.2f;
    private float _jumpBufferCount;

    //코요테시간
    [SerializeField] private float _coyoteTime = 0.2f;
    private float _coyoteTimeCount;

    //플레이어 이동 및 점프
    [SerializeField] private Rigidbody2D _rigidbd;
    [SerializeField] private Transform _floorCheck;
    //땅 체크
    [SerializeField] private LayerMask _floorLayer;
    public PlayerInput playerInput { get; private set; }

    //플레이어 점프 체크
    private bool _isJumping;

    //점프타임
    private float _jumpingTime;

    private float _horizontal;
    private float _speed = 4f;
    private float _jumpingPower = 20f;


    private void Awake()
    {
        _rigidbd = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        // 땅 체크
        if (IsFloor())
        {
            _coyoteTimeCount = _coyoteTime;
            _jumpingTime = 0.05f;
        }
        else
        {
            _coyoteTimeCount -= Time.deltaTime;
            _jumpingTime -= Time.deltaTime;
        }

        //점프를 눌렀을 때
        if (_isJumping)
        {
            _jumpBufferCount = _jumpBufferTime;
        }
        else
        {
            _jumpBufferCount -= Time.deltaTime;
        }
    }

    private void Start()
    {
        playerInput = GetComponent<PlayerInput>();

        playerInput.playerActions.Move.started += Move;
        playerInput.playerActions.Jump.started += JumpStarted;
        playerInput.playerActions.Jump.canceled += JumpCanceled;
        playerInput.playerActions.Look.started += Look;
        playerInput.playerActions.Action.started += PlayerAction;
    }

    //에어,후크 액션
    private void PlayerAction(InputAction.CallbackContext context)
    {
        
    }


    private void Look(InputAction.CallbackContext context)
    {
        
    }

    private void FixedUpdate()
    {
        _rigidbd.velocity = new Vector2(_horizontal * _speed, _rigidbd.velocity.y);
        IsLeftHead();
        IsRightHead();

        //점프
        if (_coyoteTimeCount > 0f && _jumpBufferCount > 0f && _jumpingTime > 0f)
        {
            _rigidbd.velocity = new Vector2(_rigidbd.velocity.x, _jumpingPower);
            _jumpBufferCount = 0f;
        }
    }

    public void Move(InputAction.CallbackContext context)
    {
        _horizontal = context.ReadValue<Vector2>().x;
    }

    
    public void JumpStarted(InputAction.CallbackContext context)
    {
        _jumpingTime = 0.05f;
        _isJumping = true;
    }

    public void JumpCanceled(InputAction.CallbackContext context)
    {
        if (_rigidbd.velocity.y >= 0f)
        {
            _rigidbd.velocity = new Vector2(_rigidbd.velocity.x, _rigidbd.velocity.y * 0.6f);
            _coyoteTimeCount = 0f;
        }
        _isJumping = false;
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

    //오른쪽 머리 충돌체크
    private bool IsLeftHead()
    {
        //레이 발사 / 정상 작동
        for (int i = -4; i < 5; i++)
        {
            if(Physics2D.Raycast(transform.position + (Vector3.right * 0.125f * i), Vector2.up, 1.1f, _floorLayer))
            {
                if(i == 4)
                {
                    transform.position = new Vector3(transform.position.x - 0.126f, transform.position.y);
                }
                return true;
            }
        }
        return false;
    }

    //왼쪽 머리 충돌체크
    private bool IsRightHead()
    {
        for (int j = -4; j < 5; j++)
        {
            if (Physics2D.Raycast(transform.position + (Vector3.right * -0.125f * j), Vector2.up, 1.1f, _floorLayer))
            {
                if (j == 4)
                {
                    transform.position = new Vector3(transform.position.x + 0.126f, transform.position.y);
                }
                return true;
            }
        }
        return false;
    }
}
