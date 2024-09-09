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

        for (int i = 0; i < charArr.Length; i++)
        {
            TextSplitWord word = Instantiate(_TextSplitWordPrefab, transform);
            textSplitWordList.Add(word);
            word.Setting(defFontSize, charArr[i].ToString());
        }

        Debug.Log($"Complet,{mainSentence}");
    }

    public void ChangeTexSplitWord(int i,char c){
        float r = Random.Range(0, 255) / 255f;
        float g = Random.Range(0, 255) / 255f;
        float b = Random.Range(0, 255) / 255f;
        textSplitWordList[i].Setting(GetRandomFontSize(),c.ToString(),new Color(r,g,b));
    }


    #region  Util
     private float ConvertWidthFromFontSize(int fontSize){
        return fontSize/5f * 3;
    }

    private int GetRandomFontSize(){
        return Random.Range(15,55);
    }
    #endregion

   public IEnumerator EraserAll(){
    for(int i = textSplitWordList.Count-1; i>=0 ;i--){
        Destroy(textSplitWordList[i].gameObject);
        yield return new WaitForSeconds(0.01f);
    }
   }




    

}
