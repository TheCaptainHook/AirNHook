using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UI_EmoteWheel : UI_Base
{
    
    #region SerializeFields

    [Header("Frames")] 
    [SerializeField] private GameObject _mainFrame;
    [SerializeField] private GameObject _mainEmoteWheel;
    [SerializeField] private GameObject _arrowEmoteWheel;
        
    [Header("EmoteWheel")] 
    [SerializeField] private Button _emotePanel1;
    [SerializeField] private Button _emotePanel2;
    [SerializeField] private Button _emotePanel3;
    [SerializeField] private Button _emotePanel4;
    [SerializeField] private Button _emotePanel5;
    [SerializeField] private Button _emotePanel6;
    
    [Header("ArrowWheel")]
    [SerializeField] private Button _arrowPanel1;
    [SerializeField] private Button _arrowPanel2;
    [SerializeField] private Button _arrowPanel3;
    [SerializeField] private Button _arrowPanel4;

    #endregion
    //TODO: 인게임에서만 사용 가능하도록 게임 스테이트 체크
    
    public override void OnEnable()
    {
        OpenUI();
        Show();
    }

    protected override void Start()
    {
        //이모트 패널
        _emotePanel1.onClick.AddListener(OnPanel1);
        _emotePanel2.onClick.AddListener(OnPanel2);
        _emotePanel3.onClick.AddListener(OnPanel3);
        _emotePanel4.onClick.AddListener(OnPanel4);
        _emotePanel5.onClick.AddListener(OnPanel5);
        _emotePanel6.onClick.AddListener(OnPanel6);
        
        //방향키 패널
        _arrowPanel1.onClick.AddListener(OnArrow1);
        _arrowPanel2.onClick.AddListener(OnArrow2);
        _arrowPanel3.onClick.AddListener(OnArrow3);
        _arrowPanel4.onClick.AddListener(OnArrow4);
    }                                           
    
    private void Show()                         
    {
        var seq = DOTween.Sequence();

        seq.Append(_mainFrame.transform.DOScale(1.3f, 0.15f));
        seq.Append(_mainFrame.transform.DOScale(1f, 0.05f));
    }
    
    private void Disapper()                         
    {
        var seq = DOTween.Sequence();
        
        seq.Append(_mainFrame.transform.DOScale(1.3f, 0.05f));
        seq.Append(_mainFrame.transform.DOScale(0f, 0.05f));
    }

    private void ShowEmote(string emoteName)
    {
        //Managers.Resource.NetworkInstantiate($"UI/Emotes/{emoteName}", Managers.Game.Player.transform);
        //Instantiate(Resources.Load<GameObject>(), Managers.Game.Player.transform, worldPositionStays:false);
        Managers.Game.Player.GetComponent<Player>().CmdEmote($"{emoteName}");
        Test_UI_Input.Instance.UsingEmote();
        OnExit();
    }
    
    #region ListenerEvents

    //========이모트==========
    private void OnPanel1()
    {
        ShowEmote("Panel1");
    }
    private void OnPanel2()
    {
        ShowEmote("Panel2");
    }
    private void OnPanel3()
    {
        ShowEmote("Panel3");
    }
    private void OnPanel4()
    {
        _mainEmoteWheel.SetActive(false);
        _arrowEmoteWheel.SetActive(true);
    }
    private void OnPanel5()
    {
        ShowEmote("Panel5");
    }
    private void OnPanel6()
    {
        ShowEmote("Panel6");
    }
    
    //========방향키==========

    private void OnArrow1()
    {
        ShowEmote("Arrow1");
    }
    private void OnArrow2()
    {
        ShowEmote("Arrow2");
    }
    private void OnArrow3()
    {
        ShowEmote("Arrow3");
    }
    private void OnArrow4()
    {
        ShowEmote("Arrow4");
    }
    #endregion
    
    private void OnDisable()
    {
        OnExit();
    }

    private void OnExit()
    {
        Disapper();
        CloseUI();
        _mainEmoteWheel.SetActive(true);
        _arrowEmoteWheel.SetActive(false);
    }
}
