using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Join : UI_Base
{
    [SerializeField] private GameObject _mainFrame;

    [SerializeField] private TMP_InputField _inputField;

    [SerializeField] private Button _joinBtn;
    [SerializeField] private Button _exitBtn;
    
    [Header("Text")]
    [SerializeField] private TMP_Text _joinGameText;
    [SerializeField] private TMP_Text _EnterRoomText;
    [SerializeField] private TMP_Text _EnterCodeText;
    [SerializeField] private TMP_Text _JoinBtnText;

    private string _roomCode;

    protected override void Start()
    {
        base.Start();

#if UNITY_EDITOR
        _inputField.text = Managers.Network.networkAddress;
#else
        _inputField.text = "";
#endif
        _joinBtn.onClick.AddListener(OnJoinBtn);
        _exitBtn.onClick.AddListener(OnExitBtn);
    }

    public override void OnEnable()
    {
        OpenUI();
        AppendAnim(_mainFrame, 1.1f, 0.2f, 1f, 0.1f);
    }

    private void OnJoinBtn()
    {
        _roomCode = _inputField.text;
        if(string.IsNullOrWhiteSpace(_roomCode)) return;

        Debug.Log("a");
        Managers.Network.steamLobby.joinLobbyCallback += Joining;
        Managers.Network.steamLobby.GetLobbyList();
    }

    private void Joining()
    {
        Managers.Network.steamLobby.joinLobbyCallback -= Joining;

        Debug.Log("b");
        if (!Managers.Network.steamLobby.JoinLobby(_roomCode)) return;

        Debug.Log("c");
        _mainFrame.transform.localScale = Vector3.one * 0.1f;
        CloseUI();
    }
    
    private void OnExitBtn()
    {
        //켜질 때 다시 커지는 애니메이션이 나오도록
        _mainFrame.transform.localScale = Vector3.one * 0.1f;
        CloseUI();
    }

    private void OnDisable()
    {
        //켜질 때 다시 커지는 애니메이션이 나오도록
        _mainFrame.transform.localScale = Vector3.one * 0.1f;
        CloseUI();
    }
    
    public override void SetLanguage()
    {
        SetSentence(_joinGameText, 2001);
        SetSentence(_EnterRoomText, 2007);
        SetSentence(_EnterCodeText, 2008);
        SetSentence(_JoinBtnText, 2009);
    }
}
