
using TMPro;
using UnityEngine;

public class KeyBubble : MonoBehaviour
{
    [SerializeField] private TMP_Text _text;
    private int keyAmount = 0;

    public void SatisfiedCondition()
    {
        if(gameObject.activeSelf == false)
        {
            return;
        }

        gameObject.SetActive(false);
    }


    public void SetData(int keyAmount)
    {
        if (keyAmount == 0) return;
        _text.text = keyAmount.ToString();
        gameObject.SetActive(true);
        this.keyAmount = keyAmount;
    }

    public void MinusConditionKeyAmount(int amount)
    {
        if (amount == 0)
        {
            SatisfiedCondition();
            return;
        }
        if(amount >=1)
        {
            if(!gameObject.activeSelf)
            {
                gameObject.SetActive(true);
            }
        }
        keyAmount = amount;
        _text.text = amount.ToString();
    }

   
}
