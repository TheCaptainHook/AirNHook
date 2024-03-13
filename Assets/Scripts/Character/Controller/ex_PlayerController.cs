using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

public class ex_PlayerController : MonoBehaviour
{
    public enum eState
    {
        IDLE,MOVE,DIE,JUMP
    }

    private eState _state = eState.IDLE;//플레이어의 현재상태
    private bool _isDie; //사망여부확인
    private Vector2 _moveDir;
    private double _holdingTime;
    private Rigidbody2D _rigidbody;
    private Vector2 jumpForce;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        StartCoroutine(this.ChecksState());
    }

    private IEnumerator ChecksState()
    {
        while (!_isDie)
        {
            yield return null;
            if(this._moveDir != Vector2.zero && this._moveDir.y == 0)
            {//좌우입력상태 = 이동상태
                this._state = eState.MOVE;
                this.transform.localScale = new Vector2(-this._moveDir.x, 1);//좌우 반전

                if(this._moveDir.x > 0)
                {
                    this.transform.Translate(Vector2.right * 4.0f * Time.deltaTime);
                }
                else
                {
                    this.transform.Translate(Vector2.left * 4.0f * Time.deltaTime);
                }
            }
            else
            {//이동입력 없을때
                this._state = eState.IDLE;
            }
        }
    }
    public void OnMove(InputAction.CallbackContext ctx)
    {
        Vector2 dir = ctx.ReadValue<Vector2>();
        this._moveDir = dir;

        if(ctx.phase == InputActionPhase.Canceled)
        {
            this._state = eState.IDLE;
        }
    }

    public void OnJump(InputAction.CallbackContext ctx)
    {
        Debug.Log("Jump");
        if(ctx.interaction is HoldInteraction)
        {//holding 중
            if(ctx.duration > 0.5 && ctx.duration < 1.5)
            {
                this._holdingTime = ctx.duration;
            }
            else if(ctx.duration > 1.5)
            {
                this._holdingTime = 1.5;
            }
            else
            {
                this._holdingTime = 0.8;
            }
        }
        else if(ctx.interaction is PressInteraction)
        {//press했을 경우
            this._rigidbody.AddForce(Vector2.up * this.jumpForce * (float)this._holdingTime, ForceMode2D.Impulse);
            this._state = eState.JUMP;
        }
    }
}
