using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    private CharacterController _controller;

    private Vector2 _movementDirection = Vector2.zero;
    private Rigidbody2D _rigidbody;

    private float _speed = 5;
    public int jumpPower;
    private bool _isJumping;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        _controller.OnMoveEvent += Move;
        _controller.OnJumpEvent += Jump;
        _isJumping = false;
    }

    private void FixedUpdate()
    {
        ApplyMovement(_movementDirection);
    }
    private void Jump(Vector2 direction)
    {
        if (!_isJumping)
        {
            _isJumping = true;
            _rigidbody.AddForce(Vector2.up * jumpPower);
        }
        else
        {
            return;
        }
    }
    private void Move(Vector2 direction)
    {
        _movementDirection = direction;
    }
    private void ApplyMovement(Vector2 direction)
    {
        direction = direction * _speed;
        //중력 받기위해서 Rigidbody 속도의y축 방향에도 힘을 가해줌
        //+ Vector2.up * _rigidbody.velocity.y
        _rigidbody.velocity = direction + Vector2.up * _rigidbody.velocity.y;
    }
}
