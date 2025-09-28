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
    private TextMeshProUGUI TMP { get { _tmp ??= GetComponent<TextMeshProUGUI>();  return _tmp; } }
    // private string _text;
    public int _dialogue_ID;

    private WaitForSeconds _waitZeroDot;
    private bool _onSkip;
    void Awake()
    {
        _root = transform.root.GetComponent<UI_CutSceneController>();
        _waitZeroDot = new WaitForSeconds(0.01f);
    }


    void OnEnable()
    {
        Write();
    }
    void OnDisable()
    {
        Clean();
    }


    private string GetDialogue(int id)
    {
        return Managers.Data.language.GetSentence(id);
    }

    #region Write
    private void Write()
    {
        if (_onSkip) return;
        StartCoroutine(WriteCo(GetDialogue(_dialogue_ID)));
    }
    private IEnumerator WriteCo(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) yield break;

        _root._isWriteTmp = true;
        StringBuilder sb = new();
        for (int i = 0; i < text.Length; i++)
        {
            sb.Append(text[i]);
            TMP.text = sb.ToString();
            yield return _waitZeroDot;
        }
        _root._isWriteTmp = false;
    }

    public void Skip()
    {
        StopAllCoroutines();
        _onSkip = true;
        TMP.text = GetDialogue(_dialogue_ID);
    }

    private void Clean()
    {
        StopAllCoroutines();
        _onSkip = false;
        TMP.text = "";
    }
    #endregion

}
