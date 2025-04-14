using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_CursorColorPicker : UI_Base
{
    [SerializeField] private GameObject _mainFrame;
    [SerializeField] private ColorPickerControl _colorPicker;

    private Color _lastConfirmedColor;
    private bool _isClosing = false;

    public override void OnEnable()
    {
        _isClosing = false;
        OpenUI();
        AppendAnim(_mainFrame, 1.1f, 0.2f, 1f, 0.1f);

        _lastConfirmedColor = Color.HSVToRGB(_colorPicker.currenHue, _colorPicker.currentSat, _colorPicker.currentVal);
    }

    protected override void Start()
    {
        base.Start();
    }

    public void OnClickApply()
    {
        _lastConfirmedColor = Color.HSVToRGB(_colorPicker.currenHue, _colorPicker.currentSat, _colorPicker.currentVal);
        _lastConfirmedColor.a = 1f;

        Debug.Log("[Apply] 적용된 색상: " + _lastConfirmedColor);

        Managers.CursorManager.UpdateInGameCursorColor(_lastConfirmedColor);

        // 적용 이후 실제 커서 텍스처 확인 로그
        Debug.Log("[Apply] 커서가 UpdateInGameCursorColor를 통해 호출됨");

        OnExit();
    }


    private void OnDisable()
    {
        if (!_isClosing)
            OnExit();
    }

    public void OnExit()
    {
        if (_isClosing) return;
        _isClosing = true;
        _mainFrame.transform.localScale = Vector3.one * 0.1f;
        CloseUI();
    }

    public void OnClickCancel()
    {
        // 마지막 확정된 색상으로 복구
        Managers.CursorManager.UpdateInGameCursorColor(_lastConfirmedColor);
        OnExit();
    }
}
