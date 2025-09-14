using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowPosterScreen : MonoBehaviour
{
    [field: SerializeField] private int _textID = int.MinValue;
    [field: SerializeField] private SpriteRenderer _spriteRenderer;
    [field: SerializeField] private float _offset = 0.1f;
    private Vector3 _topOfObj;

    private void OnMouseOver()
    {
        var ui = (UI_PosterScreen)Managers.UI.ShowUI<UI_PosterScreen>();

        _topOfObj = new Vector2(_spriteRenderer.bounds.center.x, _spriteRenderer.bounds.max.y + _offset);
        ui.transform.position = _topOfObj;
        ui.SetTextID(_textID);
    }

    private void OnMouseExit()
    {
        Managers.UI.HideUI<UI_PosterScreen>();
    }
}
