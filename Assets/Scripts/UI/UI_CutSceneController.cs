using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UI_CutSceneController : UI_Base
{


    private Animator _animator;
    private Animator Animator { get { _animator ??= GetComponent<Animator>(); return _animator; } }


    private PlayerInput PlayerInput => Managers.Game.playerInput;


    public override void OnEnable()
    {
        // Player_Pause();
    }


    //test//test
    //esc : 
    private float _maxEscKeyDownRate = 2f;
    private float _curEscKeyDownRate = 0;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Animator.SetTrigger("page_1");
        }

        if (Input.GetKey(KeyCode.Escape))
        {
            _curEscKeyDownRate += Time.deltaTime;
            if (_curEscKeyDownRate >= _maxEscKeyDownRate)
            {
                _curEscKeyDownRate = 0;
                Debug.Log("Skip Cut Scene");
            }
        }
    }
    /**
    1. ESC InputSystem 에 키 바인드 추가
    2. Skip Ui 추가
    3. 애니메이션이 모두 종료됬을때 실행할 애니메이션 트리거
        - 다음 애니메이션을 실행할지,(애니메이션 종료하면 걍 바로 다음 애니메이션 진행)
        - 컷신 종료할지
    **/

    //test//test


    #region Control
    private void Player_Resume()
    {
        PlayerInput.playerActions.Enable();
        PlayerInput.uiActions.Enable();
        Managers.Game.Player.GetComponent<PlayerSM>().FreezePlayerState(false);
    }
    private void Player_Pause()
    {
        PlayerInput.playerActions.Disable();
        PlayerInput.uiActions.Disable();
        Managers.Game.Player.GetComponent<PlayerSM>().FreezePlayerState(true);

    }
    private void Animator_Pause()
    {
        Animator.speed = 0;
        StartCoroutine(Animator_PauseCo());
    }

    private IEnumerator Animator_PauseCo()
    {
        while (!Input.anyKeyDown)
        {
            yield return null;
        }
        Animator.speed = 1;
    }
    private void Animator_CutSceneEnd()
    {
        
    }
   #endregion
}
