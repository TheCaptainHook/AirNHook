using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Rendering;
using UnityEngine;
using Random = UnityEngine.Random;
public class TextMeshTextController : MonoBehaviour
{
    [SerializeField] TextSplitWord _TextSplitWordPrefab;



    RectTransform parent;
    TextMeshProUGUI parentTextMeshPro;
    string mainSentence;
    List<TextSplitWord> textSplitWordList;

    public void Setting(TextMeshProUGUI textMeshPro){
        parent = textMeshPro.transform as RectTransform;
        parentTextMeshPro = textMeshPro;
        mainSentence = textMeshPro.text;   
        textSplitWordList = new();
        textMeshPro.text = "";
    }

    public void CreateTextSplitWord(){
          int defFontSize = (int)parentTextMeshPro.fontSize;

        char[] charArr = mainSentence.ToCharArray();

        for(int i =0 ; i<charArr.Length;i++){
            TextSplitWord word = Instantiate(_TextSplitWordPrefab,transform);
            textSplitWordList.Add(word);
            word.Setting(defFontSize,charArr[i].ToString());
        }

        Debug.Log($"Complet,{mainSentence}");
    }

    public void ChangeTexSplitWord(int i,char c){
        int fontSize = GetRandomFontSize();
        textSplitWordList[i].Setting(fontSize,c.ToString());
    }

    // public IEnumerator CreateTextSplitWordCoroutine(){
    //      int defFontSize = (int)parentTextMeshPro.fontSize;

    //     char[] charArr = mainSentence.ToCharArray();

    //     for(int i =0 ; i<charArr.Length;i++){
    //         TextSplitWord word = Instantiate(_TextSplitWordPrefab,transform);
    //         textSplitWordList.Add(word);
    //         var source = GetFontSizeAndWidth(defFontSize);
    //         word.Setting(source.width,source.fontSize,charArr[i].ToString());

    //         yield return new WaitForSeconds(0.5f);
    //     }

    //     Debug.Log($"Complet,{mainSentence}");
    // }


    #region  Util
     private float ConvertWidthFromFontSize(int fontSize){
        return fontSize/5f * 3;
    }
    asdasdasdasdasda

    private int GetRandomFontSize(){
        return Random.Range(15,55);
    }

    // private (float width,int fontSize) GetFontSizeAndWidth(int fontSize){
    //     float width = ConvertWidthFromFontSize(fontSize);
    //     return (width,fontSize);
    // }

    #endregion

   public IEnumerator Eraser(){
    for(int i = textSplitWordList.Count-1; i<=0 ;i--){
        Destroy(textSplitWordList[i].gameObject);
        yield return new WaitForSeconds(0.1f);
    }
   }

    

}
