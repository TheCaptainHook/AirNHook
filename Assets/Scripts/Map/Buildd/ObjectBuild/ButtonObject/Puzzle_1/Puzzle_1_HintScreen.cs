
using System.Text;
using TMPro;
using UnityEngine;

public class Puzzle_1_HintScreen : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text;

    public void SetHint(string answer)
    {
        ConvertAnswer(answer);
        //Effect Coroutine
        
    }


    private void ConvertAnswer(string answer)
    {
        StringBuilder sb = new StringBuilder(answer);

        for (int i = 0; i < sb.Length; i++)
        {
            if(i%2 == 0)
            {
                sb[i] = 'X';
            }
        }

        text.text = sb.ToString();

    }


    #region Editor
    public void SetText(string text)
    {
        if (!string.Equals(this.text.text, text)) 
        {
            this.text.text = text;
        }

        
    }
    
    #endregion
}
