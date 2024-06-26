using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class TextLine : MonoBehaviour
{
    [Header("Info")]
    string mainSentence;
    int index;

    [Header("Component")]
    [SerializeField] TextMeshProUGUI text;
    Util util = new Util();
    

    public async void WriteText(string sentence)
    {
        mainSentence = sentence;
        await util.TypingEffectTesk(text, sentence);
    }


    public void SelectSentence()
    {
        string term = $" > {text.text}";
        text.text = term;
    }

    public void UnSelectSentence()
    {
        text.text = mainSentence;
    }
}
