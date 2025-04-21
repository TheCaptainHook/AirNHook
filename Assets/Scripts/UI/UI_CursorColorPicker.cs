using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_CursorColorPicker : UI_Base
{
    [SerializeField] private GameObject _mainFrame;
    [SerializeField] private ColorPickerControl _colorPicker;

    private Color _lastConfirmedColor;

    [Header("Text")]
    [SerializeField] private TMP_Text _titleText;
    [SerializeField] private TMP_Text _cancelText;
    [SerializeField] private TMP_Text _applyText;
    public override void OnEnable()
    {
        OpenUI();
        AppendAnim(_mainFrame, 1.1f, 0.2f, 1f, 0.1f);
        // 저장된 HSV값 불러오기
        LoadPlayerprefs();
    }

    protected override void Start()
    {
        base.Start();
    }
    private void Awake()
    {
        _colorPicker.Init();
        StartCoroutine(DelayApplyHSV());
    }

    private IEnumerator DelayApplyHSV()
    {
        yield return null; // 한 프레임 대기
        LoadPlayerprefs();
    }
    public void LoadPlayerprefs()
    {
        if (PlayerPrefs.HasKey("CursorColor_HSV"))
        {
            string[] hsv = PlayerPrefs.GetString("CursorColor_HSV").Split(',');
            if (hsv.Length == 3 &&
                float.TryParse(hsv[0], out float h) &&
                float.TryParse(hsv[1], out float s) &&
                float.TryParse(hsv[2], out float v))
            {
                _lastConfirmedColor = Color.HSVToRGB(h, s, v);
                _colorPicker.SetHSV(h, s, v); // 복원 및 Picker 위치 설정
            }
        }
    }

    public void OnClickApply()
    {
        OnClick();
        float h = _colorPicker.currentHue;
        float s = _colorPicker.currentSat;
        float v = _colorPicker.currentVal;

        _lastConfirmedColor = Color.HSVToRGB(h, s, v);
        _lastConfirmedColor.a = 1f;

        Managers.CursorManager.SaveColor(h, s, v);
        Managers.CursorManager.UpdateInGameCursorColor(_lastConfirmedColor);
        OnExit();
    }

    public void OnClickCancel()
    {
        OnClick();
        OnExit();
    }

    private void OnDisable()
    {
        OnExit();
    }

    public void OnExit()
    {
        _mainFrame.transform.localScale = Vector3.one * 0.1f;
        CloseUI();
    }

    public override void SetLanguage()
    {

        SetSentence(_cancelText, 1021);
        SetSentence(_applyText, 1022);
        SetSentence(_titleText, 1023);
    }
}