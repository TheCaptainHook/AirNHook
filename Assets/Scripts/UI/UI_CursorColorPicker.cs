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
    [SerializeField] private TMP_Text _cancelText;
    [SerializeField] private TMP_Text _applyText;
    public override void OnEnable()
    {
        OpenUI();
        AppendAnim(_mainFrame, 1.1f, 0.2f, 1f, 0.1f);

        _colorPicker.Init();

        // 저장된 HSV값 불러오기
        float h = PlayerPrefs.GetFloat("CursorColor_H", 0f);
        float s = PlayerPrefs.GetFloat("CursorColor_S", 0f);
        float v = PlayerPrefs.GetFloat("CursorColor_V", 1f);

        _colorPicker.SetHSV(h, s, v); // 복원 및 Picker 위치 설정
        _lastConfirmedColor = Color.HSVToRGB(h, s, v);
    }

    public void OnClickApply()
    {
        float h = _colorPicker.currenHue;
        float s = _colorPicker.currentSat;
        float v = _colorPicker.currentVal;

        _lastConfirmedColor = Color.HSVToRGB(h, s, v);
        _lastConfirmedColor.a = 1f;

        Managers.CursorManager.UpdateInGameCursorColor(_lastConfirmedColor);
        Managers.CursorManager.SaveColor(h, s, v);

        OnExit();
    }

    public void OnClickCancel()
    {
        Managers.CursorManager.UpdateInGameCursorColor(_lastConfirmedColor);
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
    }
}