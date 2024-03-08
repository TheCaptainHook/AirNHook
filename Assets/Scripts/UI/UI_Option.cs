using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UI_Option : UI_Base<UI_Option>
{
    #region SerializeFields
    [Header("Frames")]
        [SerializeField] private GameObject _mainFrame;
        [SerializeField] private GameObject _gameOption;
        [SerializeField] private GameObject _volumeOption;
        [SerializeField] private GameObject _languageOption;
    
        [Header("OptionBar")]
        [SerializeField] private Button _gameOptionBtn;
        [SerializeField] private Button _volumeOptionBtn;
        [SerializeField] private Button _languageOptionBtn;

        [Header("GameOption")]
        [SerializeField] private GameObject _inGameBtnGroups;
        [SerializeField] private GameObject _inLobbyBtnGroups;
        [SerializeField] private GameObject _inExitBtnGroups;
        
        [SerializeField] private Button _stageRestartBtn;
        [SerializeField] private Button _toTitleBtn;
        [SerializeField] private Button _toLobbyBtn;
        [SerializeField] private Button _windowModeBtn;
        [SerializeField] private TMP_Text _windowModeTxt;
        [SerializeField] private Button _exitGameBtn;
        
        [Header("GameData")]
        //임시 불린 체크
        //게임 매니저로부터 게임스테이트 받아야 할 내용들 + 맵 데이터 구현에 따라 달라질 내용
        public bool isInGame; //인게임용 버튼 (스테이지 재시작, 로비로, 타이틀 띄우기 용)
        public bool isInLobby;
        public bool isNotInMenu;
        [SerializeField] private string _currStageLevel; //스테이지 재시작을 위한 정보 받기

        [Header("Strings")] 
        //Test용 언어팩 때 바뀔 곳
        public string fullScreen = "FullScreen";
        public string windowMode = "WindowMode";
    #endregion
    
    public override void OnEnable()
    {
        OpenUI();
        Show();
        _inGameBtnGroups.SetActive(isInGame);
        _inLobbyBtnGroups.SetActive(isInLobby);
        _inExitBtnGroups.SetActive(isNotInMenu);
    }

    private void Start()
    {
        //OptionBar
        _gameOptionBtn.onClick.AddListener(OnGameOptionBtn);
        _volumeOptionBtn.onClick.AddListener(OnVolumeOptionBtn);
        _languageOptionBtn.onClick.AddListener(OnLanguageOptionBtn);
        
        //GameOption
        _stageRestartBtn.onClick.AddListener(OnStageRestartBtn);
        _toTitleBtn.onClick.AddListener(OnTitleBtn);
        _toLobbyBtn.onClick.AddListener(OnLobbyBtn);
        _windowModeBtn.onClick.AddListener(OnWindowModeBtn);
        _exitGameBtn.onClick.AddListener(OnExitBtn);
 
        _mainFrame.transform.localScale = Vector3.one * 0.1f;
    }

    private void OnDisable()
    {
        Hide();
    }
    private void Show()
    {
        var seq = DOTween.Sequence();

        seq.Append(_mainFrame.transform.DOScale(1.1f, 0.2f));
        seq.Append(_mainFrame.transform.DOScale(1f, 0.1f));
    }

    private void Hide()
    {
        CloseUI();
        _mainFrame.SetActive(true);
        _gameOption.SetActive(true);
        _volumeOption.SetActive(false);
        _languageOption.SetActive(false);
        
        _mainFrame.transform.localScale = Vector3.one * 0.1f;
    }

    private void OnGameOptionBtn()
    {
        _gameOption.SetActive(true);
        _volumeOption.SetActive(false);
        _languageOption.SetActive(false);
        _inGameBtnGroups.SetActive(isInGame);
        _inLobbyBtnGroups.SetActive(isInLobby);
        _inExitBtnGroups.SetActive(isNotInMenu);
    }
    
    private void OnVolumeOptionBtn()
    {
        _gameOption.SetActive(false);
        _volumeOption.SetActive(true);
        _languageOption.SetActive(false);
    }
    
    private void OnLanguageOptionBtn()
        {
            _gameOption.SetActive(false);
            _volumeOption.SetActive(false);
            _languageOption.SetActive(true);
        }
    
    private void OnLobbyBtn()
    {
        OnOptionExit();
        Managers.UI.ShowLoadingUI("Test_LobbyScene");
    }
    
    private void OnStageRestartBtn()
    {
        OnOptionExit();
        //TODO: 현재 스테이지 정보를 받는 게임 매니저 데이터가 필요할듯 + 맵 구현 방식 보고
        //currStageLevel = Managers.Game.currStage
    }
    
    private void OnTitleBtn()
    {
        OnOptionExit();
        Managers.UI.ShowLoadingUI("Test_TitleScene");
    }

    private void OnWindowModeBtn()
    {
        if (Screen.fullScreen)
        {
            _windowModeTxt.text = fullScreen;
            Screen.fullScreen = false;
        }
        else
        {
           _windowModeTxt.text = windowMode; 
           Screen.fullScreen = true;
        }
    }
    
    private void OnExitBtn()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif

    }

    private void OnOptionExit()
    {
        Hide();
    }

}