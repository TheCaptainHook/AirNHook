using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_Title : UI_Base
{
    #region SerializeFields
    [Header("Animations")]
    [SerializeField] private AnimationCurve _curve;
    
    [Header("Frames")]
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private GameObject _titleImg;
    
    [Header("MenuBtns")]
    [SerializeField] private Button _joinBtn;
    [SerializeField] private Button _createRoomBtn;
    [SerializeField] private Button _optionBtn;
    [SerializeField] private Button _exitGameBtn;
    
    [Header("Texts")]
    [SerializeField] private TMP_Text _joinText;
    [SerializeField] private TMP_Text _createRoomText;
    [SerializeField] private TMP_Text _optionText;
    [SerializeField] private TMP_Text _exitGameText;
    #endregion
    
    public override void OnEnable()
    {
        OpenUI();
        Show();
    }
    
    private void Show()
    {
        StartCoroutine(Fade(true));
        StartCoroutine(BounceRoutine(Vector3.one, Vector3.one * 0.9f));
    }
    
    protected override void Start()
    {
        base.Start();
        
        _joinBtn.onClick.AddListener(OnJoinBtn);
        _createRoomBtn.onClick.AddListener(OnCreateRoomBtn);
        _optionBtn.onClick.AddListener(OnOptionBtn);
        _exitGameBtn.onClick.AddListener(OnExitBtn);
    }

    private void OnJoinBtn()
    {
        if (Managers.UI.IsActive<UI_Join>())
            Managers.UI.HideUI<UI_Join>();
        else
            Managers.UI.ShowUI<UI_Join>();
    }

    private void OnCreateRoomBtn()
    {
        CloseUI();
        Managers.Game.CurrentState = GameState.Lobby;
        Managers.Network.StartHost();
    }

    private void OnOptionBtn()
    {
        if (Managers.UI.IsActive<UI_Option>())
            Managers.UI.HideUI<UI_Option>();
        else
            Managers.UI.ShowUI<UI_Option>();
    }
    
    private IEnumerator Fade(bool isFadein) 
    {
        float timer = 0f;
        while(timer <=1f)
        {
            yield return null;
            timer += Time.unscaledDeltaTime;
            _canvasGroup.alpha = isFadein ? Mathf.Lerp(0f,1f,timer) : Mathf.Lerp(1f,0f,timer);
        }

        if(!isFadein)
        {
            CloseUI();
        }
    }
    
    private IEnumerator BounceRoutine(Vector3 startSize, Vector3 endSize)
    {
        float current = 0;
        float percent = 0;
        
        while(percent < 1)
        {
            current += Time.deltaTime;
            percent = current / 1;

            _titleImg.transform.localScale = Vector3.Lerp(startSize, endSize, _curve.Evaluate(percent));

            yield return null;
        }

        StartCoroutine(BounceRoutine(endSize, startSize));
    }
    
    private void OnExitBtn()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif

    }

    public override void SetLanguage()
    {
        SetSentence(_joinText, 2001);
        SetSentence(_createRoomText, 2002);
        SetSentence(_optionText, 2003);
        SetSentence(_exitGameText, 2004);
    }
}
