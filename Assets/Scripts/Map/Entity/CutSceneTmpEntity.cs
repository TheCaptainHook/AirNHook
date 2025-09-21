using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class CutSceneTmpEntity : MonoBehaviour
{
    private UI_CutSceneController _root;
    private TextMeshProUGUI _tmp;
    private string _text;
    public int _dialogue_ID;

    private WaitForSeconds _waitZeroDot;


    private bool onSkip;
    void Awake()
    {
        _root = transform.root.GetComponent<UI_CutSceneController>();
        _tmp = GetComponent<TextMeshProUGUI>();
        _waitZeroDot = new WaitForSeconds(0.01f);
        _root._skipEvent += Skip;

        _text = GetDialogue(_dialogue_ID);
    }

    void OnEnable()
    {
        if (onSkip) return;
        Write();
    }
    void OnDisable()
    {
        _root._skipEvent -= this.Skip;
    }


    private string GetDialogue(int id)
    {
        return Managers.Data.language.GetSentence(id);
    }

    #region Write
    private void Write()
    {
        StartCoroutine(WriteCo());
    }
    private IEnumerator WriteCo()
    {
        if (string.IsNullOrWhiteSpace(_text)) yield break;

        _root._isWriteTmp = true;
        StringBuilder sb = new();
        for (int i = 0; i < _text.Length; i++)
        {
            sb.Append(_text[i]);
            _tmp.text = sb.ToString();
            yield return _waitZeroDot;
        }

        _root._skipEvent -= Skip;
        _root._isWriteTmp = false;
        
    }
    #endregion
    private void Skip()
    {
        StopAllCoroutines();
        onSkip = true;

        _tmp.text = _text;
        _root._skipEvent -= this.Skip;
        if (_root._isWriteTmp) _root._isWriteTmp = false;
    }
}
