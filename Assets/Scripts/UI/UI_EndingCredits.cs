using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.UI;


public class UI_EndingCredits : UI_Base
{
    [Header("Refs")]
    [SerializeField] private Button _exitBtn;
    [SerializeField] RectTransform viewport;
    private float _defViewportY = -1080f;
    private float _maxViewportY = 1600;
    [SerializeField] RectTransform content;

    [SerializeField] TMP_FontAsset _font;
    private float _defaultFontSize = 30;
    private float _boldFontSize = 35;
    private bool _setText = false;

    #region  Scroll
    Coroutine _scrollingCoroutine = null;
    [Header("Scroll")]
    [SerializeField, Tooltip("기본 스크롤 속도 (px/sec)")]
    float baseSpeed = 60f;

    #endregion
    string testText =
    @"
/bAir&Hook

/bDIRECTOR
Junha Kim

/bPROGRAMMING
Jeonghyeon Moon
Jaesung Choi

/bART&DESIGN
Junha Kim

/bSOUND
Junha Kim

/bMUSIC By
Danya Vodovoz 
(www.youtube.com/@DanyaVodovoz)
- High NRG

Karl Casey @ White Bat Audio
- Last Stop
- Hackers
- Legend
- Race Against Sunset

Lukrembo 
- Onion

/bCOPYRIGHT
© 2025 Team CaptainHook. All rights reserved.
";

    protected override void Start()
    {
        _exitBtn.onClick.AddListener(OnExitBtn);
    }

    public override void OnEnable()
    {
        OpenUI();
    }
    protected override void OpenUI()
    {
        base.OpenUI();
        _option.Disable();
        _scrollingCoroutine = StartCoroutine(ScrollingCo());
       
    }
    protected override void CloseUI()
    {
        StopCoroutine(_scrollingCoroutine);
        _scrollingCoroutine = null;
        content.gameObject.SetActive(false);
        content.anchoredPosition = new Vector2(0, _defViewportY);

        base.CloseUI();
        
    }

    private void SplitText(string text)
    {
        string[] lines = text.Split(new[] { '\n' }, System.StringSplitOptions.None);
        foreach (string line in lines)
        {
            TextMeshProUGUI textMesh = new GameObject("CreditLine", typeof(TextMeshProUGUI)).GetComponent<TextMeshProUGUI>();
            textMesh.transform.SetParent(content, false);
            textMesh.font = _font;
            textMesh.color = Color.white;
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

    InputAction _option => Managers.Game.playerInput.uiActions.Option;

    private float fastMultiplier = 4f;
    private IEnumerator ScrollingCo()
    {
        if (!_setText) SplitText(testText);
        yield return new WaitForSeconds(0.1f);
        content.anchoredPosition = new Vector2(0, _defViewportY);
        content.gameObject.SetActive(true);

        while (content.anchoredPosition.y < _maxViewportY)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                break;
            }
        
            float mult = Input.GetKey(KeyCode.Space) ? fastMultiplier : 1f;
            float speed = baseSpeed * Time.deltaTime * mult;

            content.anchoredPosition = new Vector2(0, content.anchoredPosition.y + speed);
            yield return null;
        }
        yield return new WaitForSeconds(0.2f);

        _option.Enable();
        CloseUI();
       
    }

    private void OnExitBtn()
    {
        CloseUI();
    }
}
