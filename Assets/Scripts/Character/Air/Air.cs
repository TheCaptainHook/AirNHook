using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Air : MonoBehaviour
{
    //에어 무기 회전
    [SerializeField] private SpriteRenderer _armRenderer;
    [SerializeField] private Transform _armPivot;
    [SerializeField] private SpriteRenderer _characterRenderer;
    [SerializeField] private SpriteRenderer _weaponSprite;

    //마우스가 움직인 값
    private Vector2 _mouseDelta;
    private Camera _camera;

    private void Awake()
    {
        _camera = Camera.main;
    }

    public PlayerInput playerInput { get; private set; }

    private void Start()
    {
        playerInput = GetComponent<PlayerInput>();

        playerInput.playerActions.Look.performed += Look;
        playerInput.playerActions.Action.started += PlayerAction;
    }
    
    private void FixedUpdate()
    {
        RotateArm();
    }
    //에어 액션
    private void PlayerAction(InputAction.CallbackContext context)
    {

    }

    //에어 시선
    public void Look(InputAction.CallbackContext context)
    {
        _mouseDelta = context.ReadValue<Vector2>();
    }

    //에어 무기회전 및 방향전환
    private void RotateArm()
    {
        Vector2 worldPos = _camera.ScreenToWorldPoint(_mouseDelta);

        Vector2 newAim = worldPos - (Vector2)_armPivot.position;

        float rotZ = Mathf.Atan2(newAim.y, newAim.x) * Mathf.Rad2Deg;


        _armRenderer.flipY = Mathf.Abs(rotZ) > 90f;
        _characterRenderer.flipX = _armRenderer.flipY;
        _weaponSprite.flipX = _armRenderer.flipY;

        _armPivot.rotation = Quaternion.AngleAxis(rotZ, Vector3.forward);
    }
}
