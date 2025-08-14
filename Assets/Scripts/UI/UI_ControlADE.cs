using UnityEngine;

public class UI_ControlADE : UI_Base
{
    [field: SerializeField] private Sprite _defaultASprite;
    [field: SerializeField] private Sprite _defaultDSprite;
    [field: SerializeField] private Sprite _pressedASprite;
    [field: SerializeField] private Sprite _pressedDSprite;
    [field: SerializeField] private SpriteRenderer _aSpriteRenderer;
    [field: SerializeField] private SpriteRenderer _dSpriteRenderer;

    public override void OnEnable()
    {
        OpenUI();
    }

    private void OnDisable()
    {
        AButtonPress(false);
        DButtonPress(false);
        CloseUI();
    }

    public void AButtonPress(bool isPress)
    {
        _aSpriteRenderer.sprite = isPress ? _pressedASprite : _defaultASprite;
    }

    public void DButtonPress(bool isPress)
    {
        _dSpriteRenderer.sprite = isPress ? _pressedDSprite : _defaultDSprite;
    }
}
