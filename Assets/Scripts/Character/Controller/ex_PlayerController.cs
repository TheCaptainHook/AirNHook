using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ex_PlayerController : MonoBehaviour
{
    public enum eState
    {
        IDLE,MOVE,DIE
    }

    private eState _state = eState.IDLE;//플레이어의 현재상태
    private bool _isDie; //사망여부확인
    private Vector2 _moveDir;

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
    private void OnMove(InputValue value)
    {
        Vector2 dir = value.Get<Vector2>();
        this._moveDir = dir;
    }
}
