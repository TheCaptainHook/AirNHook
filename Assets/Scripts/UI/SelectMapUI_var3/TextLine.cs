using System.Collections;

using UnityEngine;
using TMPro;
using System.Threading.Tasks;
using System.Drawing;

public enum TypingType
{
    Write,
    Read,
}

public class TextLine : MonoBehaviour
{
    [Header("Info")]
    [HideInInspector] public int index;
    [HideInInspector] public string mainSentence;
    UnityEngine.Color orgColor;
    UnityEngine.Color selectColor = new UnityEngine.Color(51f / 255f, 118f / 255f, 182f / 255f);
    [HideInInspector]public TypingType type;
    [HideInInspector] public bool onSelectable;

    [Header("Component")]
    [SerializeField] TextMeshProUGUI text;
    Util util = new Util();
    
    #region Write



    /// <summary>
    /// 
    /// </summary>
    /// <param name="sentence"></param>
    /// <param name="color"></param>
    /// <param name="WriteAndRead"> Write -> true, Read -> false</param>
    public void WriteText(string sentence, UnityEngine.Color color, bool WriteAndRead, float fontSize, float delayTime, bool onSelectable)
    {
        StartCoroutine(Task_WriteTyping(sentence, color, WriteAndRead, fontSize, delayTime, onSelectable));
    }

   
    public IEnumerator Task_WriteTyping(string sentence, UnityEngine.Color color, bool WriteAndRead, float fontSize, float delayTime, bool onSelectable)
    {
        this.onSelectable = onSelectable;
        if (WriteAndRead)
        {
            type = TypingType.Write;
        }
        else
        {
            type = TypingType.Read;
        }
        orgColor = color;
        mainSentence = sentence;

        Task task = util.TypingEffectTask(text, sentence, color, fontSize, delayTime);

        yield return new WaitUntil(() => task.IsCompleted);

        Debug.Log($"Finish{sentence}");
    }


    public void WriteText(string sentence, UnityEngine.Color color)
    {
        text.color = color;
        text.text = sentence;
        mainSentence = text.text;
    }

    public void WriteText(string sentence)
    {
        text.text += sentence;
        mainSentence = text.text;
    }

    #endregion

    #region Eraser

    public IEnumerator Task_EraserText() {
        mainSentence = "";
        Task task = util.EraserEffectTask(text);
        yield return new WaitUntil(() => task.IsCompleted);
    }

    public async void EraserText()
    {
        mainSentence = "";
        await util.EraserEffectTask(text);
    }

    #endregion

    public void SelectSentence()
    {
        text.color = selectColor;
        text.fontStyle = FontStyles.Bold;
        string term = $" > {text.text}";
        text.text = term;
    }

    public void UnSelectSentence()
    {
        text.color = orgColor;
        text.fontStyle = FontStyles.Normal;
        text.text = mainSentence;
    }

    #region Util

    public void Reset()
    {
        text.color = orgColor;
        text.fontStyle = FontStyles.Normal;
    }

    public bool CheckCompareString(string sentence)
    {
        if (mainSentence == sentence) return true;
        return false;
    }

    public bool CheckEmpty()
    {
        if (text.text == string.Empty) return true;
        return false;
        
    }
    public void Clear()
    {
        text.text = "";
        mainSentence = "";
    }

    public void ErrorProsses()
    {
        
    }

    #endregion
}
