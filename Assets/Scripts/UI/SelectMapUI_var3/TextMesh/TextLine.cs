using System.Collections;
using UnityEngine;
using TMPro;
using Random = UnityEngine.Random;
using System.Text;

public enum TypingType
{
    Write,
    Read,
}

public class TextLine : MonoBehaviour
{
    [Header("Info")]
    public int index;
    [HideInInspector] public string mainSentence;
    UnityEngine.Color orgColor;
    UnityEngine.Color selectColor = new UnityEngine.Color(51f / 255f, 118f / 255f, 182f / 255f);
    [ReadOnly]
    public TypingType type;
    [HideInInspector] public bool onSelectable;

    [Header("Component")]
    public TextMeshProUGUI text;

    //todo 250103
    [SerializeField] TypingEffect typingEffect;

    string[] ranString = new string[]{"#","!","@","$","%","^","&","*","(",")","-","_","+","=","1","2","3","4","5","6","7","8","9"};

#region Write
   
    public IEnumerator Task_WriteTyping(string sentence, UnityEngine.Color color, bool writeAndRead, float fontSize, float delayTime, bool onSelectable)
    {
        this.onSelectable = onSelectable;
        if (writeAndRead)
        {
            type = TypingType.Write;
        }
        else
        {
            type = TypingType.Read;
        }
        orgColor = color;
        mainSentence = sentence;

        yield return typingEffect.NormalTyping(text,sentence,color,4,fontSize);
    }

#region Typing

    // public IEnumerator WriteTyping(string sentece,UnityEngine.Color color, float fontSize, float delayTime,int batchSize = 3){
    //    text.color = color;
    //    text.fontSize = fontSize;
    //     StringBuilder sb = new();
    //     for(int i = 0; i< sentece.Length;i+=batchSize){
    //         int length = Mathf.Min(batchSize,sentece.Length-i);
    //         sb.Append(sentece.Substring(i,length));
    //         text.text  = sb.ToString();
    //         yield return new WaitForSecondsRealtime(delayTime);
    //     }
    // }
#endregion

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
        yield return typingEffect.NormalEraser(text,5);
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
        // if (mainSentence == sentence) return true;
        if(mainSentence.Equals(sentence)) return true;
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

#endregion


    private int[] SuffleIndex(string sentence){
        int[] arr = new int[sentence.Length];
        for(int i = 0;i<arr.Length;i++){
            arr[i] = i;
        }

        for(int i = 0;i<arr.Length;i++){
            int randomNum = Random.Range(i,arr.Length);
            int term = arr[i];
            arr[i] = arr[randomNum];
            arr[randomNum] = term;
        }
        return arr;
    }
    private string GetEncryptionText(string text){
        StringBuilder sb = new();
        for(int i = 0; i< text.Length;i++){
            // encryptionText += ranString[Random.Range(0,ranString.Length)];
            sb.Append(ranString[Random.Range(0,ranString.Length)]);
        }
        return sb.ToString();
    }


}
