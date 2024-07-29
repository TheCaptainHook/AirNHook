using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class KeyBubble : MonoBehaviour//TODO 0729
{
    [SerializeField] private TMP_Text _text;
    
    public void SetData(int keyAmount)
    {
        if (keyAmount == 0) return;
        _text.text = keyAmount.ToString();
        gameObject.SetActive(true);
    }

    public void MinusConditionKeyAmount(int amount)
    {
        if (amount == 0)
        {
            SatisfiedCondition();
            return;
        }
            _text.text = amount.ToString();
    }


    public void SatisfiedCondition()
    {
        if(gameObject.activeSelf == false)
        {
            return;
        }

        Debug.Log("Satisfied condition");
        gameObject.SetActive(false);
    }
}
