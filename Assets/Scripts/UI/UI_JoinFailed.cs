using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UI_JoinFailed : UI_Base
{
    [SerializeField] private GameObject _mainFrame;

    [SerializeField] private Button _retryBtn;
    [SerializeField] private Button _exitBtn;
    public override void OnEnable()
    {
        OpenUI();
        Show();
    }
    
    private void Start()
    {
        _retryBtn.onClick.AddListener(OnRetryBtn);
        _exitBtn.onClick.AddListener(OnExitBtn);
    }
    
    private void Show()
    {
        var seq = DOTween.Sequence();

        seq.Append(_mainFrame.transform.DOScale(1.1f, 0.2f));
        seq.Append(_mainFrame.transform.DOScale(1f, 0.1f));
    }
    
    private void OnRetryBtn()
    {
        Managers.UI.ShowUI<UI_Join>();
        _mainFrame.transform.localScale = Vector3.one * 0.1f;
        CloseUI();
    }
    
    private void OnExitBtn()
    {
        //켜질 때 다시 커지는 애니메이션이 나오도록
        _mainFrame.transform.localScale = Vector3.one * 0.1f;
        CloseUI();
    }
    
    
}
