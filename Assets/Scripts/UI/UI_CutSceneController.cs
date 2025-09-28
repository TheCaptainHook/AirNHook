using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UI_CutSceneController : UI_Base
{
    private Animator _animator;
    private Animator Animator { get { _animator ??= GetComponent<Animator>(); return _animator; } }

    private PlayerInput PlayerInput => Managers.Game.playerInput;

    #region  Skip Loding Bar
    public bool _onSkip;
    [SerializeField] Image _skipLodingBarImg;
    #endregion

    #region  Event
    public bool _isWriteTmp;
    public event Action _skipEvent;

    #region Popup
    [SerializeField] GameObject _arrow;
    #endregion

    private void CallSkipEvent()
    {
        if (_skipEvent == null) return;
        _skipEvent?.Invoke();

    }
    #endregion

    public override void OnEnable()
    {
        Player_Pause();
    }
    protected override void CloseUI()
    {
        _onSkip = false;
        _isWriteTmp = false;

        base.CloseUI();
    }


    //test//test
    //esc : 
    private float _maxEscKeyDownRate = 1.5f;
    private float _curEscKeyDownRate = 0;


    private bool _getKeyEscape;
    void Update()
    {
        //TEST
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Animator.SetTrigger("page_1");
        }
        //TEST

        //Skip loading bar
        if (_getKeyEscape)
        {
            _curEscKeyDownRate += Time.deltaTime;
            _skipLodingBarImg.fillAmount = _curEscKeyDownRate / _maxEscKeyDownRate;

            if (_curEscKeyDownRate >= _maxEscKeyDownRate)
            {
                _curEscKeyDownRate = 0;
                _skipLodingBarImg.fillAmount = 0;

                _onSkip = true;
                CurAnimation_Skip();
                _getKeyEscape = false;
            }
        }
        if (!_getKeyEscape && _curEscKeyDownRate > 0)
        {
            _curEscKeyDownRate -= Time.deltaTime;
            _curEscKeyDownRate = Mathf.Clamp(_curEscKeyDownRate, 0, _maxEscKeyDownRate);
            _skipLodingBarImg.fillAmount = _curEscKeyDownRate / _maxEscKeyDownRate;
        }
        //Skip loading bar
    }
    

    #region Input
    private void OnSkipStarted(InputAction.CallbackContext context)
    {
        if(_skipLodingBarImg.gameObject.activeSelf) _getKeyEscape = true;
    }
    private void OnSkipCanceled(InputAction.CallbackContext context)
    {
        _getKeyEscape = false;
    }
    private void OnSpacebarPerformed(InputAction.CallbackContext context)
    {

    }
    #endregion

    #region Control
    private void CurAnimation_Skip()
    {
        if (_arrow.activeSelf) _arrow.SetActive(false);

        AnimatorStateInfo stateInfo = Animator.GetCurrentAnimatorStateInfo(0);
        int currentHash = stateInfo.fullPathHash;

        RuntimeAnimatorController ac = Animator.runtimeAnimatorController;
        AnimationClip currentClip = null;

        CallSkipEvent();

        foreach (var clip in ac.animationClips)
        {
            if (stateInfo.IsName(clip.name))
            {
                currentClip = clip;
                break;
            }
        }

        if (currentClip != null)
        {
            Animator.Play(currentHash, 0, 0.95f);
        }
        

        if (_isWriteTmp) _isWriteTmp = false;
    }


    
    private void Player_Resume()
    {
        PlayerInput.playerActions.Enable();
        PlayerInput.uiActions.Enable();

        PlayerInput.cutSceneActions.Disable();
        PlayerInput.cutSceneActions.Skip.started -= OnSkipStarted;
        PlayerInput.cutSceneActions.Skip.canceled -= OnSkipCanceled;

        Managers.Game.Player.GetComponent<PlayerSM>().FreezePlayerState(false);
    }
    private void Player_Pause()
    {
        PlayerInput.playerActions.Disable();
        PlayerInput.uiActions.Disable();

        PlayerInput.cutSceneActions.Enable();
        PlayerInput.cutSceneActions.Skip.started += OnSkipStarted;
        PlayerInput.cutSceneActions.Skip.canceled += OnSkipCanceled;

        Managers.Game.Player.GetComponent<PlayerSM>().FreezePlayerState(true);

    }

    #region Animation Event Trigger
    private void Animation_Pause_PageEnd()
    {
        Animator.speed = 0;
        StartCoroutine(Animation_Pause_PageEndCo());
    }
    private IEnumerator Animation_Pause_PageEndCo()
    {
        yield return new WaitUntil(() => !_isWriteTmp);
        if (!_arrow.activeSelf) _arrow.SetActive(true);

        while (!PlayerInput.cutSceneActions.Next.triggered)
        {
            yield return null;
        }

        _onSkip = false;
        _arrow.SetActive(false);
        Animator.speed = 1;
    }
    private void Animation_Pause()
    {
        Animator.speed = 0;
        StartCoroutine(Animation_PauseCo());
    }
    private float _maxDelay=5;
    private float _curDealy = 0;
    private IEnumerator Animation_PauseCo()
    {
        yield return new WaitUntil(() => !_isWriteTmp);
        if (!_arrow.activeSelf) _arrow.SetActive(true);

        while (!PlayerInput.cutSceneActions.Next.triggered)
        {
            _curDealy += Time.deltaTime;
            if (_curDealy >= _maxDelay)
            {
                break;
            }
            yield return null;
        }

        _arrow.SetActive(false);

        _curDealy = 0;
        Animator.speed = 1;
    }
    #endregion

    private void Animator_CutSceneEnd()
    {

    }
   #endregion
}
