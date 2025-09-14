using TMPro;
using UnityEngine;

public class UI_PosterScreen : UI_Base
{
    [field: SerializeField] private TMP_Text _text;
    private int _textID = int.MinValue;

    public override void OnEnable()
    {
        OpenUI();
    }

    private void OnDisable()
    {
        _textID = int.MinValue;
        _text.text = string.Empty;
        CloseUI();
    }

    public void SetTextID(int textID)
    {
        _textID = textID;
        SetLanguage();
    }

    public override void SetLanguage()
    {
        if (_textID == int.MinValue)
        {
            _text.text = string.Empty;
            return;
        }

        SetSentence(_text, _textID);
        _text.ForceMeshUpdate();

        var textSize = _text.GetRenderedValues(false);
        var padding = new Vector2(10, 5);


    }
}
