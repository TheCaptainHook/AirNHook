using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

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
    [SerializeField] private Button _mapEditorBtn;
    [SerializeField] private Button _optionBtn;
    [SerializeField] private Button _exitGameBtn;
    [SerializeField] private Button _endingCradit;
    
    [Header("Texts")]
    [SerializeField] private TMP_Text _joinText;
    [SerializeField] private TMP_Text _createRoomText;
    [SerializeField] private TMP_Text _optionText;
    [SerializeField] private TMP_Text _exitGameText;
    [SerializeField] private TMP_Text _mapEditorText;
    #endregion
    
    public override void OnEnable()
    {
        OpenUI();
        Show();
    }

    private void Show()
    {
        StartCoroutine(Fade(true, _canvasGroup));
        StartCoroutine(BounceRoutine(_titleImg,Vector3.one * 0.5f, Vector3.one * 0.47f, _curve));
    }

    protected override void Start()
    {
        base.Start();
        _joinBtn.onClick.AddListener(OnJoinBtn);
        _createRoomBtn.onClick.AddListener(OnCreateRoomBtn);
        // _mapEditorBtn.onClick.AddListener(OnMapEditorBtn);
        _optionBtn.onClick.AddListener(OnOptionBtn);
        _exitGameBtn.onClick.AddListener(OnExitBtn);
        _endingCradit.onClick.AddListener(OnEndingCreditsBtn);
    }

    private void OnJoinBtn()
    {
        OnClick();
        if (Managers.UI.IsActive<UI_Join>())
            Managers.UI.HideUI<UI_Join>();
        else
            Managers.UI.ShowUI<UI_Join>();
    }

    private void OnCreateRoomBtn()
    {
        OnClick();
        //CloseUI();
        Managers.Game.CurrentState = GameState.Lobby;
        //Managers.Game.CurrentState = GameState.Editor;//Editor TEST
        //Managers.UI.ShowLoadingUI("TestScene_MapEditor");//Editor TEST

//#if UNITY_EDITOR
        //Managers.Network.StartHost(); // todo 0425
        //#else
        Managers.Network.steamLobby.HostLobby();
        // Managers.Command.ChangeStage("Lobby");
        //#endif
    }

    private void OnMapEditorBtn()
    {
        // OnClick();
        // Managers.UI.sceneName = "EditorScene";
        // SceneManager.LoadScene("EditorScene");
        // Managers.UI.ShowUI<UI_Loading>();
        //Comming Soon...
    }
    private void OnOptionBtn()
    {
        OnClick();
        if (Managers.UI.IsActive<UI_Option>())
            Managers.UI.HideUI<UI_Option>();
        else
            Managers.UI.ShowUI<UI_Option>();
    }
    
    private void OnExitBtn()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif

    }
    
    private void OnEndingCreditsBtn()
    {
               OnClick();
       Managers.UI.ShowUI<UI_EndingCredits>();  
    }
    public override void SetLanguage()
    {
        SetSentence(_joinText, 2001);
        SetSentence(_createRoomText, 2002);
        SetSentence(_optionText, 2003);
        SetSentence(_exitGameText, 2004);
        SetSentence(_mapEditorText, 2017);
    }
}
