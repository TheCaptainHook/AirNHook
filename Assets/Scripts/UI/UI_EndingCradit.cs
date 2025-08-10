using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class UI_EndingCradit : UI_Base
{
    [Header("Refs")]
    [SerializeField] RectTransform viewport;
    private float _defViewportY = -1080f;
    private float _curviewPortY = 0;
    [SerializeField] RectTransform content;
    [SerializeField] TMP_Text creditText;
    [SerializeField] TMP_FontAsset _font;
    private float _defaultFontSize = 30;
    private float _boldFontSize = 35;
    private bool _setText = false;

    #region  Scroll
    private bool _onScroll = false;
    [Header("Scroll")]
    [SerializeField, Tooltip("기본 스크롤 속도 (px/sec)")]
    float baseSpeed = 60f;
    [SerializeField, Tooltip("시작 대기 (초)")]
    float startDelay = 1.0f;
    [SerializeField, Tooltip("끝에서 대기 (초)")]
    float endHold = 1.0f;
    [SerializeField, Tooltip("끝자리 여백 (px)")]
    float bottomPadding = 100f;
    [SerializeField, Tooltip("Time.timeScale의 영향을 받지 않게 하려면 체크")]
    bool useUnscaledTime = true;
    #endregion
    string testText =
    @"
/bGAME TITLE
My Awesome Game

/bDIRECTOR
John Doe

/bPROGRAMMING
Jane Smith
Mike Johnson

/bART & DESIGN
Emily Davis
Alex Kim

/bMUSIC
Kevin Brown

/bSPECIAL THANKS
All Our Players!

/bCOPYRIGHT
© 2025 Your Studio. All rights reserved.
";

    public override void OnEnable()
    {
        OpenUI();
    }
    protected override void OpenUI()
    {
        base.OpenUI();
        if (!_setText) SplitText(testText);

        _onScroll = true;
    }
    protected override void CloseUI()
    {
        base.CloseUI();
        Reset();
    }

    private void Reset()
    {
        _onScroll = false;
        _curviewPortY = _defViewportY;
        viewport.anchoredPosition = new Vector2(viewport.anchoredPosition.x, _curviewPortY);
    }

    protected override void Start()
    {
        SplitText(testText);
    }
    private void SplitText(string text)
    {
        string[] lines = text.Split(new[] { '\n' }, System.StringSplitOptions.None);
        foreach (string line in lines)
        {
            TextMeshProUGUI textMesh = new GameObject("CreditLine", typeof(TextMeshProUGUI)).GetComponent<TextMeshProUGUI>();
            textMesh.transform.SetParent(content, false);
            textMesh.font = _font;
            if (line.StartsWith("/b"))
            {
                textMesh.fontStyle = FontStyles.Bold;
                textMesh.fontSize = _boldFontSize;
                textMesh.text = line.Substring(2);
            }
            else
            {
                textMesh.fontStyle = FontStyles.Normal;
                textMesh.fontSize = _defaultFontSize;
                textMesh.text = line;
            }
        }

        _setText = true;
    }

    
}
