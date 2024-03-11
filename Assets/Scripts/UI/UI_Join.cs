using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Join : UI_Base
{
    [SerializeField] private GameObject _mainFrame;

    [SerializeField] private TMP_InputField _inputField;

    [SerializeField] private Button _joinBtn;
    [SerializeField] private Button _exitBtn;

    private void Start()
    {
        _inputField.text = Managers.Network.networkAddress;
        _joinBtn.onClick.AddListener(OnJoinBtn);
        _exitBtn.onClick.AddListener(OnExitBtn);
    }

    public override void OnEnable()
    {
        OpenUI();
        Show();
    }
    
    private void Show()
    {
        var seq = DOTween.Sequence();

        seq.Append(_mainFrame.transform.DOScale(1.1f, 0.2f));
        seq.Append(_mainFrame.transform.DOScale(1f, 0.1f));
    }

    private void OnJoinBtn()
    {
        var ipString = _inputField.text;
        Managers.Network.networkAddress = ipString;
        Managers.Network.StartClient();
        CloseUI();
    }
    
    private void OnExitBtn()
    {
        CloseUI();
    }
}
