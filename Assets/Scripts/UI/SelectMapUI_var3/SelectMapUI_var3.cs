using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SelectMapUI_var3 : MonoBehaviour
{
    

    [Header("Info")]
    private Dictionary<int, Map[]> mapMainStageDictionary;

    [Header("Components")]
    Util util = new Util();
    
    [Header("Transform")]
    [SerializeField] Transform content;
    RectTransform contentRectTransform;

    [Header("Text")]
    [SerializeField] GameObject textLine;
    private List<TextLine> textLineList;
    string term = "Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem IpsumLorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem IpsumLorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.";


    [Header("Stats")]
    [SerializeField] int maxTextLine;
    private TextLine curTextLine;
    private int curTextLineIndex;

    private int nextWriteTextLineIndex; //다음에 쓸 Line Index

    private int minSelectTextLineListIndex; // 선택 가능한 라인 인덱스
    private int maxSelectTextLineListIndex; // 선택 가능한 라인 인덱스

    private bool onInteractable;



    private void Awake()
    {
        contentRectTransform = content.transform as RectTransform;

        //TextLine Pooling
        textLineList = new();
        for (int i = 0; i < maxTextLine; i++)
        {
            TextLine newTextLine= Instantiate(textLine, content).GetComponent<TextLine>();
            textLineList.Add(newTextLine);
            
        }
        //TextCode
        StartCoroutine(WriteTextLineCo(term));



    }



    private void LateUpdate()
    {
        if (onInteractable)
        {
            int index = curTextLineIndex;
            
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                index++;
                if (curTextLineIndex >= maxSelectTextLineListIndex) return;
                SelectTextLine(index,1);

            }
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                index--;
                if (index < 0) return;
                SelectTextLine(index,-1);

            }
        }
    }


    


    private void SelectTextLine(int index,int upAndDown)
    {
        curTextLineIndex = index;
        
        Debug.Log(curTextLineIndex);

        if(index > 22)
        {
            Vector2 position = contentRectTransform.localPosition;
            position.y += 30 * upAndDown;
            contentRectTransform.localPosition = position;
        }
        else
        {
            contentRectTransform.localPosition = Vector2.zero;
        }

        if(curTextLine != null)
        {
            curTextLine.UnSelectSentence();
            curTextLine = textLineList[index];
            curTextLine.SelectSentence();
        }
        else
        {
            curTextLine = textLineList[index];
            curTextLine.SelectSentence();
        }
    }


    IEnumerator WriteTextLineCo(string sentence)
    {
        onInteractable = false;

        List<string> sentenceList = util.SplitText(sentence, 60, new char[] { ',', '.' });
        for (int i = 0; i < sentenceList.Count; i++)
        {
            textLineList[i].WriteText(sentenceList[i]);
            yield return new WaitForSeconds(0.1f);
        }
        nextWriteTextLineIndex = sentenceList.Count;

        //todo 0626 TestCode
        maxSelectTextLineListIndex = sentenceList.Count;

        onInteractable = true;
    }
}
