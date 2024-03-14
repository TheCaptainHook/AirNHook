using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public Rigidbody2D rigidbd;
    public Transform floorCheck;
    public LayerMask floorLayer;
    public PlayerInput playerInput { get; private set; }

    private float _horizontal;
    private float _speed = 4f;
    private float _jumpingPower = 20f;

    private void Start()
    {
        playerInput = GetComponent<PlayerInput>();

        playerInput.playerActions.Move.started += Move;
        playerInput.playerActions.Jump.performed += JumpPerfomed;
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
        rigidbd.velocity = new Vector2(_horizontal * _speed, rigidbd.velocity.y);
    }
    public void Move(InputAction.CallbackContext context)
    {
        _horizontal = context.ReadValue<Vector2>().x;
    }
    public void JumpPerfomed(InputAction.CallbackContext context)
    {
        if (context.performed && IsFloor())
        {
            rigidbd.velocity = new Vector2(rigidbd.velocity.x, _jumpingPower);
        }
    }
    public void JumpCanceled(InputAction.CallbackContext context)
    {
        if (context.canceled && rigidbd.velocity.y > 0f)
        {
            rigidbd.velocity = new Vector2(rigidbd.velocity.x, rigidbd.velocity.y * 0.6f);
        }
    }
    private bool IsFloor()
    {
        // OverlapCircle <- 매개변수로 전달할 위치를 기준으로 반지름만큼 원 생성
        //그 영역 내에 충돌체를 가진 게임오브젝트가 있는지 검사
        return Physics2D.OverlapCircle(floorCheck.position, 0.7f, floorLayer);
    }
    
}
