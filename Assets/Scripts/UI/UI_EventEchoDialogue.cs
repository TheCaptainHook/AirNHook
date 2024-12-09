using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using UnityEngine.TextCore;
using System;
using Unity.VisualScripting;
using System.Text;


public enum Mark
{
    Default,
    Mark_1,
    Mark_2,
    Mark_3
}

public class UI_EventEchoDialogue : UI_Base
{
    [SerializeField] Image mainSprite;
    [SerializeField] TextMeshProUGUI main_Text;//Main_Text
    [Space(20)]
    [SerializeField] TMP_FontAsset font;
    private float fontSize;
    private float spacing; // 빈 공백

    [SerializeField] RectTransform splitTextBox;


    #region  Components

    #endregion

    #region Dialogue
    float startX;
    float currentX;
    Vector3 center;
    int index;

    //Mark
    Mark mark;
    public List<EventEchoText> eventEchoTextList;
    public List<EventEchoText> mark_1List;

    #endregion


    string testSentence = "Lorem [Ipsum] is simply dummy /1";


    #region Pattern
    private string stringToIntPattern_PlayerDeath = "/1"; //player death
    #endregion


    protected override void Start()
    {
        Init();

        StartDialogue(testSentence);

    }

    private void Init()
    {
        center = splitTextBox.TransformPoint(splitTextBox.rect.center);
        mark_1List = new();
        eventEchoTextList = new();
        fontSize = main_Text.fontSize;
        spacing = GetSpaceWidth();
    }

    private void Reset()
    {
        startX = 0;
        currentX = 0;
        index = 0;
        eventEchoTextList.Clear();
        mark_1List.Clear();
        mark = Mark.Default;
    }




    #region Main
    private void StartDialogue(string sentence) //0
    {
        //Replace
        Replace(ref sentence);

        //main text.text
        main_Text.text = sentence;
        Vector2 preferredValues = main_Text.GetPreferredValues(main_Text.text);
        startX = center.x - preferredValues.x / 2f;
        currentX = startX;

        CreateTextMesh(sentence.ToCharArray());

        //CreateTextMesh("aaaaaa".ToCharArray());
        //CreateTextMesh("bbbbbb".ToCharArray());
        //CreateTextMesh("cccccc".ToCharArray());

    }




    private void CreateTextMesh(char[] characters)
    {
        foreach (char c in characters)
        {
            CheckMark(c, ref mark);
            if (c == '[' || c == ']') continue;

            if (c == ' ') // 공백 처리 (간격만 추가)
            {
                currentX += spacing; // 공백은 적당히 조정
                continue;
            }

           
            // 프리팹 생성
            GameObject charObject = new GameObject("Split");
            TextMeshProUGUI text = charObject.AddComponent<TextMeshProUGUI>();

            text.text = c.ToString();
            text.font = font;
            text.fontSize = fontSize;
            text.color = main_Text.color;
            // 위치 지정
            RectTransform charRect = charObject.GetComponent<RectTransform>();
            charRect.anchoredPosition = new Vector2(currentX, center.y);

            // 현재 X 위치 업데이트
            Vector2 charSize = text.GetPreferredValues(text.text);
            currentX += charSize.x + 0.25f;

            charObject.transform.SetParent(splitTextBox);
            EventEchoText echotext = new EventEchoText(index, text, c);

            index++;

            switch (mark)
            {
                case Mark.Default:
                    break;
                case Mark.Mark_1:
                    mark_1List.Add(echotext);
                    break;
                case Mark.Mark_2:
                    break;
                case Mark.Mark_3:
                    break;
            }

            eventEchoTextList.Add(echotext);

        }
    }
    #endregion



    #region UI
    public override void OnEnable()
    {
    }

    protected override void OpenUI()
    {
        base.OpenUI();
    }
    protected override void CloseUI()
    {
        base.CloseUI();
    }
    #endregion

    #region  Util
    private float GetSpaceWidth()
    {
        return 0.25f * fontSize;
    }

    private string GetDialogue(int id) {
        return Managers.Data.language.dict[id];
    }


   
    private void Replace(ref string sentence)
    {
        StringBuilder sb = new StringBuilder(sentence);
        sb.Replace(stringToIntPattern_PlayerDeath, 5.ToString());
        
        sentence = sb.ToString();
    }

    //onMark_1 : '[]'
    //onMark_2 : '{}'
    private void CheckMark(char c, ref Mark mark) //2
    {
        if (c == '[')
        {
            mark = Mark.Mark_1;

        }
        if (c == ']')
        {
            mark = Mark.Default;

        }
    }
    #endregion



    [Serializable]
    public struct EventEchoText
    {
        public int index;
        public TextMeshProUGUI text;
        public char c;

        public EventEchoText(int index, TextMeshProUGUI text,char c)
        {
            this.index= index;
            this.text = text;
            this.c = c;
        }

    }
}
