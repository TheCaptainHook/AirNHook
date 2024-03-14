using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    //TODO 점프 버퍼
    //[SerializeField] private float _jumpBufferTime = 1f;
    //private float _jumpBufferCount;

    //코요테시간
    [SerializeField] private float _coyoteTime = 0.2f;
    private float _coyoteTimeCount;

    //플레이어 이동 및 점프
    [SerializeField] private Rigidbody2D _rigidbd;
    [SerializeField] private Transform _floorCheck;
    [SerializeField] private LayerMask _floorLayer;
    public PlayerInput playerInput { get; private set; }

    private float _horizontal;
    private float _speed = 4f;
    private float _jumpingPower = 20f;

    private void Update()
    {
        if (IsFloor())
        {
            _coyoteTimeCount = _coyoteTime;
        }
        else
        {
            _coyoteTimeCount -= Time.deltaTime;
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
    }
    public void Move(InputAction.CallbackContext context)
    {
        _horizontal = context.ReadValue<Vector2>().x;
    }
    public void JumpStarted(InputAction.CallbackContext context)
    {
        if (context.started && _coyoteTimeCount > 0f)
        {
            _rigidbd.velocity = new Vector2(_rigidbd.velocity.x, _jumpingPower);
        }
    }
    public void JumpCanceled(InputAction.CallbackContext context)
    {
        if (context.canceled && _rigidbd.velocity.y > 0f)
        {
            _rigidbd.velocity = new Vector2(_rigidbd.velocity.x, _rigidbd.velocity.y * 0.6f);

            _coyoteTimeCount = 0f;
        }
    }
    private bool IsFloor()
    {
        // OverlapCircle <- 매개변수로 전달할 위치를 기준으로 반지름만큼 원 생성
        //그 영역 내에 충돌체를 가진 게임오브젝트가 있는지 검사
        return Physics2D.OverlapCircle(_floorCheck.position, 0.7f, _floorLayer);
    }
    
}
