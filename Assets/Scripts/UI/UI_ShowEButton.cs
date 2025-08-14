using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_ShowEButton : UI_Base
{
    [field: SerializeField] private Sprite _defaultSprite;
    [field: SerializeField] private Sprite _pressedSprite;
    [field: SerializeField] private SpriteRenderer _spriteRenderer;


    public override void OnEnable()
    {
        OpenUI();
    }

    private void OnDisable()
    {
        ChangeSprite(false);
        CloseUI();
    }

    public void ChangeSprite(bool isPressed)
    {
        var sprite = isPressed ? _pressedSprite : _defaultSprite;

        _spriteRenderer.sprite = sprite;
    }
}

