using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_PingWheel : UI_Base
{
    #region SerializeFields
    [Header("Frames")]
    [SerializeField] private GameObject _mainFrame;
    [SerializeField] private GameObject _mainEmoteWheel;

    [Header("EmoteWheel")]
    [SerializeField] private Button _emotePanel0;
    [SerializeField] private Button _emotePanel1;
    [SerializeField] private Button _emotePanel2;
    [SerializeField] private Button _emotePanel3;
    [SerializeField] private Button _emotePanel4;
    #endregion

    private Vector2 _mousePosition;

    public override void OnEnable()
    {
        OpenUI();
        _mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        gameObject.transform.position = _mousePosition;
        AppendAnim(_mainFrame, 1.3f, 0.15f, 1f, 0.05f);
    }

    private void OnDisable()
    {
        OnExit();
    }

    protected override void Start()
    {
        _emotePanel0.onClick.AddListener(OnPanel0);
        _emotePanel1.onClick.AddListener(OnPanel1);
        _emotePanel2.onClick.AddListener(OnPanel2);
        _emotePanel3.onClick.AddListener(OnPanel3);
        _emotePanel4.onClick.AddListener(OnPanel4);
        
    }

    private void OnExit()
    {
        AppendAnim(_mainFrame, 1.3f, 0.05f, 0f, 0.05f);
        CloseUI();
        _mainEmoteWheel.SetActive(true);
    }

    private void ShowPing(string pingName)
    {
        Managers.Game.Player.GetComponent<PlayerSM>().CmdPing($"{pingName}", _mousePosition);
        // Managers.Game.Player.GetComponent<PlayerSM>().Rpc_Ping($"{pingName}", _mousePosition);
        Managers.Game.Player.GetComponent<PlayerSM>().UsingPing();
        OnExit();
    }
    private void OnPanel0()
    {
        ShowPing("Ping1");
    }
    private void OnPanel1()
    {
        ShowPing("Ping2");
    }
    private void OnPanel2()
    {
        ShowPing("Ping3");
    }
    private void OnPanel3()
    {
        ShowPing("Ping4");
    }
    private void OnPanel4()
    {
        ShowPing("Ping5");
    }

    public void TryShowHoveredPing()
    {
        Vector2 endPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Vector2 direction = endPos - _mousePosition;
        
        if (direction.magnitude < 1.5f)
            return;

        direction.Normalize();

        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            if (direction.x > 0)
                OnPanel2(); // 오른
            else
                OnPanel4(); // 왼
        }
        else
        {
            if (direction.y > 0)
                OnPanel1(); // 위
            else
                OnPanel3(); // 아래
        }
    }
}