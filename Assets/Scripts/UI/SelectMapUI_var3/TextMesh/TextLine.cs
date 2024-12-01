using System.Collections;

using UnityEngine;
using TMPro;
using System.Threading.Tasks;
using System.Drawing;
using Random = UnityEngine.Random;
using System.Text;
using Unity.VisualScripting;
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
    [HideInInspector]public TypingType type;
    [HideInInspector] public bool onSelectable;

    [Header("Component")]
     [HideInInspector] public TextMeshProUGUI text;

    [Header("Text Mesh Pro Controller")]
    [SerializeField] TextMeshTextController _TextMeshTextController;
    public TextMeshTextController curTMTC;
    [HideInInspector]public bool isEncryption;
    Util util = new Util();
    

    string[] ranString = new string[]{"#","!","@","$","%","^","&","*","(",")","-","_","+","=","1","2","3","4","5","6","7","8","9"};
    #region Write



    /// <summary>
    /// 
    /// </summary>
    /// <param name="sentence"></param>
    /// <param name="color"></param>
    /// <param name="WriteAndRead"> Write -> true, Read -> false</param>
    // public void WriteText(string sentence, UnityEngine.Color color, bool WriteAndRead, float fontSize, float delayTime, bool onSelectable)
    // {
    //     StartCoroutine(Task_WriteTyping(sentence, color, WriteAndRead, fontSize, delayTime, onSelectable));
    // }

   
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

        // Task task = util.TypingEffectTask(text, sentence, color, fontSize, delayTime);

        // yield return new WaitUntil(() => task.IsCompleted);
        yield return WriteTyping(sentence,color,fontSize,delayTime);

    }
    #region Typing
    public IEnumerator WriteTyping(string sentece,UnityEngine.Color color, float fontSize, float delayTime,int batchSize = 3){
       text.color = color;
       text.fontSize = fontSize;
        StringBuilder sb = new();
        for(int i = 0; i< sentece.Length;i+=batchSize){
            int length = Mathf.Min(batchSize,sentece.Length-i);
            sb.Append(sentece.Substring(i,length));
            text.text  = sb.ToString();
            yield return new WaitForSecondsRealtime(delayTime);
        }
    }
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
        // Task task = util.EraserEffectTask(text);
        // yield return new WaitUntil(() => task.IsCompleted);
        yield return EraserEffect();
    }
    public IEnumerator EraserEffect(int batchSize = 3){
       if(string.IsNullOrEmpty(text.text)) yield break;

        mainSentence = "";
        StringBuilder sb = new(text.text);
        while(sb.Length>0){
            int charsToRemove = Mathf.Min(batchSize,sb.Length);
            sb.Remove(sb.Length - charsToRemove,charsToRemove);
            text.text = sb.ToString();
            yield return new WaitForSeconds(0.01f);
        }
        text.text = "";
       
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


    //0909


    public IEnumerator ChangeEncryption(){
        isEncryption = true;

        string encryptionText = GetEncryptionText(text.text);
        int[] indexs = SuffleIndex(encryptionText);

        TextMeshProTextControllerActive();
        

        for(int i = 0; i < encryptionText.Length;i++){
            curTMTC.ChangeTexSplitWord(indexs[i],encryptionText[indexs[i]]);
            yield return new WaitForSeconds(.05f);
        }

        yield return new WaitForSeconds(0.5f);
        isEncryption = false;
    }

    private int[] SuffleIndex(string sentence){
        int[] arr = new int[sentence.Length];
        Debug.Log(sentence.Length);
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

        string encryptionText = "";
        for(int i = 0; i< text.Length;i++){
            encryptionText += ranString[Random.Range(0,ranString.Length)];
        }
        return encryptionText;
    }


    //0909

    public void TextMeshProTextControllerActive(){
        if(curTMTC != null){
            Destroy(curTMTC.gameObject);
        }

        curTMTC = Instantiate(_TextMeshTextController,transform);
        curTMTC.Setting(this);
        curTMTC.CreateTextSplitWord();
    }

}
