using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public enum TypingType
{
    Write,
    Read,
}

public class TextLine : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("Info")]
    public int index;
    [HideInInspector] public string mainSentence;
    UnityEngine.Color orgColor;
    UnityEngine.Color selectColor = new UnityEngine.Color(51f / 255f, 118f / 255f, 182f / 255f);
    [ReadOnly]
    public TypingType type;
    public bool onSelectable;

    [Header("Component")]
    public TextMeshProUGUI text;
    public Image image;

    public UI_StageSelect_var3 _UI_StageSelect_var3;
    
    //todo 250103
    [SerializeField] TypingEffect typingEffect;

    string[] ranString = new string[]{"#","!","@","$","%","^","&","*","(",")","-","_","+","=","1","2","3","4","5","6","7","8","9"};

    public float maxWidth = 870;

    #region Mouse Pointer
    public bool isPointerInside = false;
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (mainSentence == string.Empty || !onSelectable) return;
        isPointerInside = true;

        _UI_StageSelect_var3.Net.Server_GetMousePointer(index);

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (mainSentence == string.Empty || !onSelectable) return;
        isPointerInside = false;
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left && isPointerInside)
        {
            _UI_StageSelect_var3.Net.Server_MouseClick();
            Debug.Log("왼쪽 클릭됨!");
        }
    }
    #endregion

    #region Write
    /// <summary>
    /// 
    /// </summary>
    /// <param name="sentence"></param>
    /// <param name="color"></param>
    /// <param name="writeAndRead">true : Write, false : Read </param>
    /// <param name="fontSize"></param>
    /// <param name="delayTime"></param>
    /// <param name="onSelectable"></param>
    /// <returns></returns>
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

        // 💡 미리 최종 텍스트에 대해 넓이를 계산
        PreferredSizeWidth(sentence);


        yield return typingEffect.NormalTyping(text, sentence, color, 4, fontSize);
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

    public void WriteText(string sentence, UnityEngine.Color color,bool onSelectable = false)
    {
        text.color = color;
        text.text = sentence;
        mainSentence = text.text;
        this.onSelectable = onSelectable;
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
        onSelectable = false;
        yield return typingEffect.NormalEraser(text,5);
    }
   

 #endregion

    public void SelectSentence()
    {
        text.color = selectColor;
        text.fontStyle = FontStyles.Bold;
        string term = $" > {text.text}";
        PreferredSizeWidth(term);
        text.text = term;
    }

    public void UnSelectSentence()
    {
        text.color = orgColor;
        text.fontStyle = FontStyles.Normal;
        PreferredSizeWidth(mainSentence);
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
        onSelectable = false;
    }

    private void PreferredSizeWidth(string sentence)
    {
        Vector2 preferredSize = text.GetPreferredValues(sentence);
        float paddedWidth = preferredSize.x;

        RectTransform rect = transform as RectTransform;
        RectTransform textRect = text.rectTransform;

        rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, paddedWidth);
        textRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, paddedWidth);

        rect.anchorMin = rect.anchorMax = new Vector2(0f, 0.5f);
        rect.pivot = new Vector2(0f, 0.5f);
        rect.anchoredPosition = Vector2.zero;

        textRect.anchorMin = textRect.anchorMax = new Vector2(0f, 0.5f);
        textRect.pivot = new Vector2(0f, 0.5f);
        textRect.anchoredPosition = Vector2.zero;
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
