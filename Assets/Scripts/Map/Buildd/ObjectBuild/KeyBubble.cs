
using TMPro;
using UnityEngine;

public class KeyBubble : MonoBehaviour
{
    [SerializeField] private TMP_Text _text;
    private int keyAmount;

    public void SatisfiedCondition()
    {
        if(gameObject.activeSelf == false)
        {
            return;
        }

        Debug.Log("Satisfied condition");
        gameObject.SetActive(false);
    }


    public void SetData(int keyAmount)
    {
        Debug.Log("1");
        if (keyAmount == 0) return;
        _text.text = keyAmount.ToString();
        gameObject.SetActive(true);
        Debug.Log("1");
        this.keyAmount = keyAmount;
    }

    public void MinusConditionKeyAmount(int amount)
    {
        if (amount == 0)
        {
            SatisfiedCondition();
            return;
        }
        keyAmount = amount;
        _text.text = amount.ToString();
    }

    public void AddKeyAmount()
    {
        keyAmount++;
        _text.text = keyAmount.ToString();
    }
   
}
