using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
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

    private bool _isPointerOverPanel4 = false;
    private float _hoverTime = 0f;
    private float _requiredHoverTime = 1f;

    public override void OnEnable()
    {
        OpenUI();
        AppendAnim(_mainFrame, 1.3f, 0.15f, 1f, 0.05f);
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

        AddEventForArrowEmote();
    }

    private void Update()
    {
        if (_isPointerOverPanel4)
        {
            _hoverTime += Time.deltaTime;

            if (_hoverTime >= _requiredHoverTime)
            {
                OnPanel4();
                _isPointerOverPanel4 = false;
            }
        }
    }

    private void ShowEmote(string emoteName)
    {
        //Managers.Resource.NetworkInstantiate($"UI/Emotes/{emoteName}", Managers.Game.Player.transform);
        //Instantiate(Resources.Load<GameObject>(), Managers.Game.Player.transform, worldPositionStays:false);
        Managers.Game.Player.GetComponent<PlayerSM>().CmdEmote($"{emoteName}");
        Managers.Game.Player.GetComponent<PlayerSM>().UsingEmote();
        OnExit();
    }

    private void AddEventForArrowEmote()
    {
        EventTrigger eventTrigger = _emotePanel4.GetComponent<EventTrigger>();
        if (eventTrigger == null)
        {
            eventTrigger = _emotePanel4.gameObject.AddComponent<EventTrigger>();
        }

        EventTrigger.Entry pointerEnterEntry = new EventTrigger.Entry();
        pointerEnterEntry.eventID = EventTriggerType.PointerEnter;
        pointerEnterEntry.callback.AddListener((eventData) => OnPointerEnterPanel4());

        EventTrigger.Entry pointerExitEntry = new EventTrigger.Entry();
        pointerExitEntry.eventID = EventTriggerType.PointerExit;
        pointerExitEntry.callback.AddListener((eventData) => OnPointerExitPanel4());

        eventTrigger.triggers.Add(pointerEnterEntry);
        eventTrigger.triggers.Add(pointerExitEntry);
    }

    private void OnPointerEnterPanel4()
    {
        _isPointerOverPanel4 = true;
        _hoverTime = 0f;
    }

    private void OnPointerExitPanel4()
    {
        _isPointerOverPanel4 = false;
        _hoverTime = 0f;
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
        AppendAnim(_mainFrame, 1.3f, 0.05f, 0f, 0.05f);
        CloseUI();
        _mainEmoteWheel.SetActive(true);
        _arrowEmoteWheel.SetActive(false);
    }
    
    public void TryShowHoveredEmote()
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        List<RaycastResult> raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, raycastResults);

        foreach (RaycastResult result in raycastResults)
        {
            Button hoveredButton = result.gameObject.GetComponent<Button>();
            
            if (hoveredButton != null)
            {
                hoveredButton.onClick.Invoke();
                break;
            }
        }
    }
}
