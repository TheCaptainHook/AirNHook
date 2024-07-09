using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class KeyBubble : MonoBehaviour
{
    [SerializeField] private TMP_Text _text;
    private ExitPointObj _exitPointObj;

    private void Awake()
    {
        _exitPointObj = FindObjectOfType<ExitPointObj>();
        _text.text = _exitPointObj.condition_KeyAmount.ToString();
    }
}
