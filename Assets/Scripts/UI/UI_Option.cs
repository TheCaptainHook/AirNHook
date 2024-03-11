using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UI_Option : UI_Base
{
    #region SerializeFields
    [Header("Frames")]
        [SerializeField] private GameObject _mainFrame;
        [SerializeField] private GameObject _gameOption;
        [SerializeField] private GameObject _graphicsOption;
        [SerializeField] private GameObject _volumeOption;
        [SerializeField] private GameObject _languageOption;
        [SerializeField] private Button _optionExitBtn;
    
        [Header("OptionBar")]
        [SerializeField] private Button _gameOptionBtn;
        [SerializeField] private Button _graphicsOptionBtn;
        [SerializeField] private Button _volumeOptionBtn;
        [SerializeField] private Button _languageOptionBtn;

        [Header("GameOptionGroups")]
        [SerializeField] private GameObject _inGameBtnGroups;
        [SerializeField] private GameObject _inLobbyBtnGroups;
        [SerializeField] private GameObject _inExitBtnGroups;
        
        [Header("GameOption")]
        [SerializeField] private Button _stageRestartBtn;
        [SerializeField] private Button _toTitleBtn;
        [SerializeField] private Button _toLobbyBtn;
        
        [SerializeField] private Button _exitGameBtn;
        
        [SerializeField] private TMP_Text _infoTxt;
        [SerializeField] private GameObject _menuInfo;
        
        [Header("GraphicsOption")]

        
        
        [Header("GameData")]
        //임시 불린 체크
        //게임 매니저로부터 게임스테이트 받아야 할 내용들 + 맵 데이터 구현에 따라 달라질 내용
        public bool isInGame; //인게임용 버튼 (스테이지 재시작, 로비로, 타이틀 띄우기 용)
        public bool isInLobby;
        public bool isNotInMenu;
        [SerializeField] private string _currStageLevel; //스테이지 재시작을 위한 정보 받기

        [Header("Strings")]
        //Test용 언어팩 때 바뀔 곳
        public string menuGameOptionInfo = "This option is not available in menu";
        
        //TODO: 언어설정 바꿀 때 스트링 값들 리프래쉬 해주는 메소드
        //TODO: 그래픽 설정 저장과 불러오기
        
    #endregion
    
    public override void OnEnable()
    {
        OpenUI();
        Show();
        _inGameBtnGroups.SetActive(isInGame);
        _inLobbyBtnGroups.SetActive(isInLobby);
        _inExitBtnGroups.SetActive(isNotInMenu);
        _menuInfo.SetActive(!isNotInMenu);
    }

    private void Start()
    {
        //Frames
        _optionExitBtn.onClick.AddListener(OnOptionExitBtn);
        
        //OptionBar
        _gameOptionBtn.onClick.AddListener(OnGameOptionBtn);
        _graphicsOptionBtn.onClick.AddListener(OnGraphicsOptionBtn);
        _volumeOptionBtn.onClick.AddListener(OnVolumeOptionBtn);
        _languageOptionBtn.onClick.AddListener(OnLanguageOptionBtn);
        
        //GameOption
        _stageRestartBtn.onClick.AddListener(OnStageRestartBtn);
        _toTitleBtn.onClick.AddListener(OnTitleBtn);
        _toLobbyBtn.onClick.AddListener(OnLobbyBtn);
        _exitGameBtn.onClick.AddListener(OnExitBtn);

        _infoTxt.text = menuGameOptionInfo;
        
        //GraphicsOption

 
        _mainFrame.transform.localScale = Vector3.one * 0.1f;
    }
    
    private void Show()
    {
        //등장 애니메이션
        var seq = DOTween.Sequence();

        seq.Append(_mainFrame.transform.DOScale(1.1f, 0.2f));
        seq.Append(_mainFrame.transform.DOScale(1f, 0.1f));
    }

    //==================옵션 바 버튼==================
    private void OnGameOptionBtn()
    {
        _gameOption.SetActive(true);
        _graphicsOption.SetActive(false);
        _volumeOption.SetActive(false);
        _languageOption.SetActive(false);
        _inGameBtnGroups.SetActive(isInGame);
        _inLobbyBtnGroups.SetActive(isInLobby);
        _inExitBtnGroups.SetActive(isNotInMenu);
        _menuInfo.SetActive(!isNotInMenu);
    }
    
    private void OnGraphicsOptionBtn()
    {
        _gameOption.SetActive(false);
        _graphicsOption.SetActive(true);
        _volumeOption.SetActive(false);
        _languageOption.SetActive(false);
    }
    
    private void OnVolumeOptionBtn()
    {
        _gameOption.SetActive(false);
        _graphicsOption.SetActive(false);
        _volumeOption.SetActive(true);
        _languageOption.SetActive(false);
    }
    
    private void OnLanguageOptionBtn()
        {
            _gameOption.SetActive(false);
            _graphicsOption.SetActive(false);
            _volumeOption.SetActive(false);
            _languageOption.SetActive(true);
        }
    
    //==================게임 옵션===========================
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
    
    private void OnExitBtn()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif

    }
    
    //====================그래픽 옵션=====================

    private void OnWindowModeBtn()
    {
        if (Screen.fullScreen)
        {

            Screen.fullScreen = false;
        }
        else
        {
           Screen.fullScreen = true;
        }
    }
    
    //==================나가기 옵션들=====================

    private void OnOptionExit()
    {
        //처음 켜질 때 게임옵션이 보이도록 설정
        CloseUI();
        _mainFrame.SetActive(true);
        _gameOption.SetActive(true);
        _graphicsOption.SetActive(false);
        _volumeOption.SetActive(false);
        _languageOption.SetActive(false);
        
        //켜질 때 다시 커지는 애니메이션이 나오도록
        _mainFrame.transform.localScale = Vector3.one * 0.1f;
    }
    
    private void OnDisable()
    {
        OnOptionExit();
    }
    private void OnOptionExitBtn()
    {
        OnOptionExit();
    }
}