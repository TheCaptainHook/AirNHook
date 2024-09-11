using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Rendering;
using UnityEngine;
using Random = UnityEngine.Random;
public class TextMeshTextController : MonoBehaviour
{
    [SerializeField] TextSplitWord _TextSplitWordPrefab;
    [SerializeField] GameObject pacman;




    TextLine parent;
    TextMeshProUGUI parentTextMeshPro;
    string mainSentence;
    List<TextSplitWord> textSplitWordList;

    public void Setting(TextLine textLine){
        parent = textLine;
        parentTextMeshPro = textLine.text;
        mainSentence = textLine.mainSentence;   
        textSplitWordList = new();
        textLine.text.text = "";
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
        textSplitWordList[i].Setting(30,c.ToString(),new Color(230f/255,170/255f,50/255f));
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

    while(parent.isEncryption){
        Debug.Log("Is ENcryption");
        yield return null;
    }

    GameObject pacmanObj = Instantiate(pacman,transform);

    for(int i = textSplitWordList.Count-1; i>=0 ;i--){
        Destroy(textSplitWordList[i].gameObject);
        yield return new WaitForSeconds(0.15f);
    }
    Destroy(pacmanObj.gameObject);
   }




    

}
