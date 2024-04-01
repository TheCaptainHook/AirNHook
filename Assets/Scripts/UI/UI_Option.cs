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
        [SerializeField] private Toggle _fullScreenToggle;
        [SerializeField] private Toggle _vsyncToggle;
        [SerializeField] private Button _applyBtn;

        [Header("Text")]
        [SerializeField] private TMP_Text _escText;
        [SerializeField] private TMP_Text _titleText;
        [SerializeField] private TMP_Text _lobbyText;
        [SerializeField] private TMP_Text _restartText;
        [SerializeField] private TMP_Text _exitText;
        [SerializeField] private TMP_Text _masterVolumeText;
        [SerializeField] private TMP_Text _effectsText;
        [SerializeField] private TMP_Text _bgmText;
        [SerializeField] private TMP_Text _languageText;
        [SerializeField] private TMP_Text _resolutionText;
        [SerializeField] private TMP_Text _fullscreenText;
        [SerializeField] private TMP_Text _vsyncText;
        [SerializeField] private TMP_Text _applyText;

        [Header("GameData")]
        //임시 불린 체크
        //게임 매니저로부터 게임스테이트 받아야 할 내용들 + 맵 데이터 구현에 따라 달라질 내용
        [SerializeField] private string _currStageLevel; //스테이지 재시작을 위한 정보 받기
        private GameState CurrentGameState => Managers.Game.CurrentState;
        private bool IsInGame => CurrentGameState == GameState.Game; //인게임용 버튼 (스테이지 재시작, 로비로, 타이틀 띄우기 용)
        private bool IsInLobby => CurrentGameState == GameState.Lobby;
        private bool IsNotInMenu => CurrentGameState == GameState.Title;
    #endregion
    
    public override void OnEnable()
    {
        OpenUI();
        Show();
        _inGameBtnGroups.SetActive(IsInGame);
        _inLobbyBtnGroups.SetActive(IsInLobby);
        _inExitBtnGroups.SetActive(IsNotInMenu);
        _menuInfo.SetActive(!IsNotInMenu);
    }

    protected override void Start()
    {
        base.Start();
        
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

        // _infoTxt.text = menuGameOptionInfo;
        
        //GraphicsOption
        FullScreenToggle();
        VsyncToggle();
        _applyBtn.onClick.AddListener(OnApplyBtn);
 
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
        _inGameBtnGroups.SetActive(IsInGame);
        _inLobbyBtnGroups.SetActive(IsInLobby);
        _inExitBtnGroups.SetActive(IsNotInMenu);
        _menuInfo.SetActive(!IsNotInMenu);
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
        //Managers.UI.ShowLoadingUI("Test_LobbyScene");
        //TODO 로비로
        var player = Managers.Game.Player.GetComponent<Player>();
        if (player.isServer)
        {
            player.onCallBackAction += LoadLobbyScene;
            player.CmdChangeStage("Lobby");
        }
    }

    private void LoadLobbyScene()
    {
        Managers.Game.Player.GetComponent<Player>().onCallBackAction -= LoadLobbyScene;
        Managers.Network.ServerChangeScene("MainScene");
    }
    
    private void OnStageRestartBtn()
    {
        OnOptionExit();
        Managers.Network.ServerChangeScene("MainScene");
    }
    
    private void OnTitleBtn()
    {
        OnOptionExit();
        // Managers.UI.ShowLoadingUI("Test_TitleScene");
        if (Managers.Game.Player.GetComponent<Player>().isServer)
        {
            Managers.Network.StopHost();
        }
        else
            Managers.Network.StopClient();
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

    private void FullScreenToggle()
    {
        _fullScreenToggle.isOn = Screen.fullScreen;
    }
    
    private void VsyncToggle()
    {
        _vsyncToggle.isOn = QualitySettings.vSyncCount != 0;
    }

    private void OnApplyBtn()
    {
        Screen.fullScreen = _fullScreenToggle.isOn;
        QualitySettings.vSyncCount = _vsyncToggle.isOn ? 1 : 0;
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

    public override void SetLanguage()
    {
        SetSentence(_infoTxt, 1014);
        SetSentence(_escText, 1001);
        SetSentence(_titleText, 1003);
        SetSentence(_lobbyText, 1004);
        SetSentence(_restartText, 1005);
        SetSentence(_exitText, 1002);
        SetSentence(_masterVolumeText, 1007);
        SetSentence(_effectsText, 1008);
        SetSentence(_bgmText, 1009);
        SetSentence(_languageText, 1010);
        SetSentence(_resolutionText, 1011);
        SetSentence(_fullscreenText, 1006);
        SetSentence(_vsyncText, 1012);
        SetSentence(_applyText, 1013);
    }
}