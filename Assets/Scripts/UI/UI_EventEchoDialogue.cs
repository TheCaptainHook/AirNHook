using UnityEngine.UI;
using UnityEngine;
using TMPro;
using System.Text;
using System.Collections;
using System;
using System.Collections.Generic;


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
    
    //1210 Text Effect
    public List<TextMeshEffectStruct> textMeshEffectStructList;

    #region Mark_1
    [Header("Mark_1")]
    public AnimationCurve scaleCurve;
    #endregion


    #region  Components

    #endregion

    #region Dialogue
   

    //Mark
    Mark mark;

    #endregion


    string testSentence = "Lorem [Ipsum] is simply dummy /1";


    #region Pattern
    private string stringToIntPattern_PlayerDeath = "/1"; //player death
    #endregion


    private void Update(){
        //test
        if(Input.GetKeyDown(KeyCode.P)){
            UpdateTextMeshEffectStructList(ref testSentence);

            main_Text.text = testSentence;
            

            foreach(TextMeshEffectStruct data in textMeshEffectStructList){
                ChangeColor(data,Color.red);
            }

            StartCoroutine(StartAnimateTextMesh());
        }
    }

    private void Init()
    {
      textMeshEffectStructList = new();
    }

    private void Reset()
    {
     textMeshEffectStructList.Clear();
    }


    //

    #region Main
    private void StartDialogue(string sentence) //0
    {
        //Replace
        Replace(ref sentence);

        //main text.text
        main_Text.text = sentence;


        
        //CreateTextMesh("aaaaaa".ToCharArray());
        //CreateTextMesh("bbbbbb".ToCharArray());
        //CreateTextMesh("cccccc".ToCharArray());

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
    // private float GetSpaceWidth()
    // {
    //     return 0.25f * fontSize;
    // }

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
    bool isAnimating;
    float duration = 1;
    private IEnumerator StartAnimateTextMesh()
    {
        isAnimating = true;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            // AnimationCurve에서 현재 스케일 값 얻기
            float scaleValue = scaleCurve.Evaluate(t); 
            UpdateSubstringScale(scaleValue);

            yield return null;
        }

        // 애니메이션 끝난 뒤에는 스케일을 다시 1로 맞춰준다.
        UpdateSubstringScale(1.0f);

        isAnimating = false;
    }

    // private TMP_TextInfo textInfo;
    private void UpdateSubstringScale(float scaleValue)
    {
        if (string.IsNullOrEmpty(main_Text.text))
            return;

        TMP_TextInfo textInfo = main_Text.textInfo;

        if (textInfo.characterCount == 0)
            return;

        // substringRanges 내 모든 범위에 대해 해당 문자들의 스케일 변경
        for (int r = 0; r < textMeshEffectStructList.Count; r++)
        {
            TextMeshEffectStruct data = textMeshEffectStructList[r];
            int start = data.startIndex;
            int length = data.textLength;

            for (int i = start; i < start + length && i < textInfo.characterCount; i++)
            {
                var charInfo = textInfo.characterInfo[i];
                if (!charInfo.isVisible)
                    continue;

                int materialIndex = charInfo.materialReferenceIndex;
                int vertexIndex = charInfo.vertexIndex;

                Vector3[] vertices = textInfo.meshInfo[materialIndex].vertices;

                // 문자 중앙점 계산
                Vector3 charMidBaseline = (vertices[vertexIndex] + vertices[vertexIndex + 2]) / 2f;



                // 각 정점을 문자 중심 기준으로 스케일링
                for (int j = 0; j < 4; j++)
                {
                    Vector3 offset = vertices[vertexIndex + j] - charMidBaseline;
                    vertices[vertexIndex + j] = charMidBaseline + offset * scaleValue;
                }
            }

            main_Text.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices);
        }
    }
    private void ChangeColor(TextMeshEffectStruct data, Color targetColor){
        main_Text.ForceMeshUpdate();

        for(int i = data.startIndex; i< data.startIndex+data.textLength && i < main_Text.textInfo.characterCount;i++){
            var charInfo = main_Text.textInfo.characterInfo[i];
                if (!charInfo.isVisible)
                    continue;

                int materialIndex = charInfo.materialReferenceIndex;
                int vertexIndex = charInfo.vertexIndex;

                // vertex 색상 참조
                Color32[] colors = main_Text.textInfo.meshInfo[materialIndex].colors32;

                colors[vertexIndex + 0] = targetColor;
                colors[vertexIndex + 1] = targetColor;
                colors[vertexIndex + 2] = targetColor;
                colors[vertexIndex + 3] = targetColor;
        }

        main_Text.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
    }



    private void UpdateTextMeshEffectStructList(ref string sentence){
        int startIndex = 0; //1
        int textLength = 0;

        StringBuilder sb = new();

        //[abcd]e //5
        for(int i =0; i< sentence.Length;i++){
            switch(sentence[i]){
                case '[':
                    startIndex = i;
                break;
                case ']':
                    textLength = i - startIndex-1;
                    textMeshEffectStructList.Add(new TextMeshEffectStruct(startIndex,textLength,Mark.Mark_1));
                break;
                default:
                sb.Append(sentence[i]);
                break;
            }
        }
        sentence = sb.ToString();
    }
    


[Serializable]
public struct TextMeshEffectStruct{
    public int startIndex;
    public int textLength;
    public Mark mark;
    public TextMeshEffectStruct(int startIndex, int textLength,Mark mark){
        this.startIndex = startIndex;
        this.textLength =textLength;
        this.mark = mark;
    }

}

}
