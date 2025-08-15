using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using TMPro;
using UnityEngine.UI;
using Mirror;

public class UI_Option : UI_Base
{
    #region SerializeFields
    [Header("Frames")]
    [SerializeField] private GameObject _mainFrame;
    [SerializeField] private GameObject _gameOption;
    [SerializeField] private GameObject _graphicsOption;
    [SerializeField] private GameObject _volumeOption;
    [SerializeField] private GameObject _languageOption;
    [SerializeField] private UI_Ping _UI_Ping;
    [SerializeField] private Button _optionExitBtn;

    [Header("OptionBar")]
    [SerializeField] private Button _gameOptionBtn;
    [SerializeField] private Button _graphicsOptionBtn;
    [SerializeField] private Button _volumeOptionBtn;
    [SerializeField] private Button _languageOptionBtn;

    [Header("GameOptionGroups")]
    [SerializeField] private GameObject _inGameBtnGroups;
    // [SerializeField] private GameObject _inLobbyBtnGroups;
    // [SerializeField] private GameObject _inExitBtnGroups;

    [Header("GameOption")]
    [SerializeField] private Button _stageRestartBtn;
    [SerializeField] private Button _toTitleBtn;
    [SerializeField] private Button _toLobbyBtn;

    [SerializeField] private Button _respawnObjectBtn;

    [SerializeField] private Button _exitGameBtn;

    [SerializeField] private TMP_Text _infoTxt;
    [SerializeField] private GameObject _menuInfo;

    [SerializeField] private GameObject _roomCodeBox;
    [SerializeField] private Button _copyCodeBtn;
    [SerializeField] private Button _inviteFriendBtn;
    [SerializeField] private Button _bugReportBtn;
    
    [Header("GraphicsOption")]
    [SerializeField] private Toggle _fullScreenToggle;
    [SerializeField] private Toggle _vsyncToggle;
    [SerializeField] private Button _colorPickerBtn;
    [SerializeField] private Button _applyBtn;

    //[SerializeField] private GameObject _resolutionWarning;

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
    [SerializeField] private TMP_Text _cursorcolorText;
    [SerializeField] private TMP_Text _colorpickertext;
    [SerializeField] private TMP_Text _applyText;
    [SerializeField] private TMP_Text _respawnObjectText;
    //[SerializeField] private TMP_Text _resolutionWarningText;
    [SerializeField] private TMP_Text _roomCodeNumText;
    [SerializeField] private TMP_Text _joinCodeText;

    [Header("GameData")]
    //임시 불린 체크
    //게임 매니저로부터 게임스테이트 받아야 할 내용들 + 맵 데이터 구현에 따라 달라질 내용
    [SerializeField] private string _currStageLevel; //스테이지 재시작을 위한 정보 받기
    private GameState CurrentGameState => Managers.Game.CurrentState;
    private bool IsInGame => CurrentGameState == GameState.Game; //인게임용 버튼 (스테이지 재시작, 로비로, 타이틀 띄우기 용)
    private bool IsInLobby => CurrentGameState == GameState.Lobby;
    private bool IsInTitle => CurrentGameState == GameState.Title;

    private bool IsServer => NetworkServer.active;
    #endregion

    public override void OnEnable()
    {
        OpenUI();
        AppendAnim(_mainFrame, 1.1f, 0.2f, 1f, 0.1f);
        _inGameBtnGroups.SetActive(IsServer); // -> Host Only
        _toTitleBtn.gameObject.SetActive(!IsInTitle); // is Not Title Only
        _exitGameBtn.gameObject.SetActive(true);

        //UI_Ping
        _UI_Ping.gameObject.SetActive(!IsInTitle);
        if(_UI_Ping.gameObject.activeSelf)_UI_Ping.StartPingCheck(IsServer); 
        //UI_Ping

        // _menuInfo.SetActive(IsInTitle);
        _roomCodeBox.SetActive(IsInLobby && IsServer);
        if(IsInLobby)
            GetRoomCode();

        //MouseCursorss
        Managers.CursorManager.SetDefaultCursor();        
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
        _copyCodeBtn.onClick.AddListener(OnCopyBtn);
        _inviteFriendBtn.onClick.AddListener(OnInviteBtn);
        _respawnObjectBtn.onClick.AddListener(OnRespawnObjectBtn);
        _bugReportBtn.onClick.AddListener(OnBugReportBtn);
        // _infoTxt.text = menuGameOptionInfo;

        //GraphicsOption
        FullScreenToggle();
        VsyncToggle();
        _colorPickerBtn.onClick.AddListener(OnColorPickerBtn);
        _applyBtn.onClick.AddListener(OnApplyBtn);
 
        _mainFrame.transform.localScale = Vector3.one * 0.1f;



        // Managser.game.curstage == gameStage.editor // todo 0421
    }

    //==================옵션 바 버튼==================
    private void OnGameOptionBtn()
    {
        OnClick();
        _graphicsOption.SetActive(false);
        _volumeOption.SetActive(false);
        _languageOption.SetActive(false);

        _gameOption.SetActive(true);
        _bugReportBtn.gameObject.SetActive(true);
        _inGameBtnGroups.SetActive(IsServer);
        _toTitleBtn.gameObject.SetActive(!IsInTitle);
        _exitGameBtn.gameObject.SetActive(true);
        // _menuInfo.SetActive(IsInTitle);
        _roomCodeBox.SetActive(IsInLobby && IsServer);
        //_resolutionWarning.SetActive(false);
    }
    
    private void OnGraphicsOptionBtn()
    {
        OnClick();
        _gameOption.SetActive(false);
        _volumeOption.SetActive(false);
        _languageOption.SetActive(false);
        _bugReportBtn.gameObject.SetActive(false);


        _graphicsOption.SetActive(true);
        //_resolutionWarning.SetActive(!IsInTitle);
    }
    
    private void OnVolumeOptionBtn()
    {
        OnClick();
        _gameOption.SetActive(false);
        _graphicsOption.SetActive(false);
        _languageOption.SetActive(false);
        _bugReportBtn.gameObject.SetActive(false);

        _volumeOption.SetActive(true);
        //_resolutionWarning.SetActive(false);
    }

    private void OnLanguageOptionBtn()
    {
        OnClick();
        _gameOption.SetActive(false);
        _graphicsOption.SetActive(false);
        _volumeOption.SetActive(false);
        _bugReportBtn.gameObject.SetActive(false);

        _languageOption.SetActive(true);
        //_resolutionWarning.SetActive(false);
    }

    //==================게임 옵션===========================
    private void OnLobbyBtn() // server
    {
        // if(!Managers.Game.Player.TryGetComponent<PlayerSM>(out var player)) return;
        
        // if (!player.isServer) return;
        OnClick();
        OnOptionExit();
        Managers.Command._Server_ChangeStage(GlobalText.LOBBY);
    }
    public void HoldAndReleaseLobby_StageRestartBtn(bool onOff)
    {
        _toLobbyBtn.interactable = !onOff;
        _stageRestartBtn.interactable = !onOff;
    }

    
    private void OnStageRestartBtn() //server
    {
        // if(!Managers.Game.Player.TryGetComponent<PlayerSM>(out var player)) return;

        // if (!player.isServer) return;
        OnClick();
        OnOptionExit();
        Managers.Command._Server_ChangeStage(Managers.Stage.stageName); 
    }
    
    private void OnTitleBtn()
    {
        OnClick();
        OnOptionExit();
        // Managers.UI.ShowLoadingUI("Test_TitleScene");
        if (Managers.Game.Player.GetComponent<PlayerSM>().isServer)
        {
            Managers.Network.StopHost();
        }
        else
        {
            SteamMatchmaking.LeaveLobby(Managers.Network.steamLobby.currentLobbyID);
            Managers.Network.StopClient();
        }
    }

    private void OnBugReportBtn()
    {
        var ui = Managers.UI.ShowUI<UI_Bug_Report>();

    }
    private void OnExitBtn()
    {
        OnClick();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif

    }

    private void OnRespawnObjectBtn()
    {
        OnClick();
        MapEditor.Instance.ResetInteractableObjectPosition();
    }

    private void GetRoomCode()
    {
        _roomCodeNumText.text = Base62Converter.ToBase62(Managers.Network.steamLobby.currentLobbyID.m_SteamID);
    }

    private void OnCopyBtn()
    {
        OnClick();
        CopyToClipboard(_roomCodeNumText.text);
    }

    private void OnInviteBtn()
    {
        OnClick();
        SteamFriends.ActivateGameOverlayInviteDialog(Managers.Network.steamLobby.currentLobbyID);
    }

    private void CopyToClipboard(string str)
    {
        GUIUtility.systemCopyBuffer = str;
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

    private void OnColorPickerBtn()
    {
        OnClick();
        if (Managers.UI.IsActive<UI_CursorColorPicker>())
            Managers.UI.HideUI<UI_CursorColorPicker>();
        else
            Managers.UI.ShowUI<UI_CursorColorPicker>();
    }
    private void OnApplyBtn()
    {
        OnClick();
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

        //커서 컬리픽커가 켜져있을 경우 체크
        if (Managers.UI.IsActive<UI_CursorColorPicker>())
            Managers.UI.HideUI<UI_CursorColorPicker>();

        //MouseCursor
        Managers.CursorManager.ClearCursor();
        //켜질 때 다시 커지는 애니메이션이 나오도록
        _mainFrame.transform.localScale = Vector3.one * 0.1f;
    }
    
    private void OnDisable()
    {
        //MouseCursor
        Managers.CursorManager.ClearCursor();

        OnOptionExit();
    }
    private void OnOptionExitBtn()
    {
        OnClick();
        OnOptionExit();
    }

    public override void SetLanguage()
    {
        // SetSentence(_infoTxt, 1014);
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
        //SetSentence(_resolutionWarningText, 1016);
        SetSentence(_joinCodeText, 1017);
        SetSentence(_respawnObjectText,1018);
        SetSentence(_cursorcolorText, 1019);
        SetSentence(_colorpickertext, 1020);
    }
}