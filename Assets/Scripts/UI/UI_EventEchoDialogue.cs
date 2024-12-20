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
    Mark_1, //[] : Scale
    Mark_2, //<> : Bounce
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
    // private List<TMP_EffectField> allTMP_EffectFieldList;

    #region  Components

    #endregion

    #region Dialogue


    //Mark
    Mark mark;

    #endregion


    public string testSentence = "Lorem [Ipsum] is simply dummy /1";


    #region Pattern
    private string stringToIntPattern_PlayerDeath = "/1"; //player death
    private string stringToIntPattern_PlayerUsePortal = "/2"; // use portal
    #endregion


    #region  Color
    private Color transparencyColor = new Color(1,1,1,0);
    #endregion

    private void Update()
    {
        //test
        if (Input.GetKeyDown(KeyCode.P))
        {

            SetDialogue(testSentence);

        }
    }

    private void Init()
    {
        textMeshEffectStructList = new();
        // allTMP_EffectFieldList = new();
    }

    public void Reset()
    {
        textMeshEffectStructList.Clear();
        // allTMP_EffectFieldList.Clear();
        StopAllCoroutines();
        mark_1_EffectCoroutine = null;
        mark_2_EffectCoroutine = null;

        main_Text.text = "";
        main_Text.ForceMeshUpdate();

    }


    public Coroutine mark_1_EffectCoroutine;
    public Coroutine mark_2_EffectCoroutine;

   
    #region Main
    public void SetDialogue(string text)
    {
        // string text = testSentence;
        //1. Replace
        Replace(ref text);
        //Search mark
        UpdateTextMeshEffectStructList(ref text);
        //main_text init
        main_Text.text = text;

        main_Text.ForceMeshUpdate();
        List<TMP_EffectField> scaleList = new();
        List<TMP_EffectField> bounceList = new();

        //start Color
        foreach (TextMeshEffectStruct data in textMeshEffectStructList)
        {
            ChangeColor(data, Color.red);

            if (data.mark == Mark.Mark_1) GetEffectFieldList(data, ref scaleList);
            if (data.mark == Mark.Mark_2) GetEffectFieldList(data, ref bounceList);
        }

        main_Text.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);

        //on effect

        // StartCoroutine(AppearEffect(bounceList));
        // StartCoroutine(AppearEffect(scaleList));

        if (mark_1_EffectCoroutine != null) StopCoroutine(mark_1_EffectCoroutine);
        mark_1_EffectCoroutine = StartCoroutine(ScaleEffectCo(scaleList));

        if (mark_2_EffectCoroutine != null) StopCoroutine(mark_2_EffectCoroutine);
        mark_2_EffectCoroutine = StartCoroutine(BounceEffectCo(bounceList));

        StartCoroutine(ShutDownCo());

    }

    IEnumerator ShutDownCo(){
        yield return new WaitForSeconds(5);
        Reset();
        CloseUI();
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

    private string GetDialogue(int id)
    {
        return Managers.Data.language.dict[id];
    }



    private void Replace(ref string sentence)
    {
        StringBuilder sb = new StringBuilder(sentence);
        // sb.Replace(stringToIntPattern_PlayerDeath, 5.ToString());
        sb.Replace(stringToIntPattern_PlayerUsePortal,Managers.Data.saveData._AchievementData.use_Portal.ToString());

        sentence = sb.ToString();
    }


    // private TMP_TextInfo textInfo;
    private void GetEffectFieldList(TextMeshEffectStruct data, ref List<TMP_EffectField> list)
    {
        for (int i = data.startIndex; i < data.startIndex + data.textLength; i++)
        {
            var charInfo = main_Text.textInfo.characterInfo[i];
            if (!charInfo.isVisible) continue;

            list.Add(new TMP_EffectField(main_Text.textInfo, charInfo, data.mark));

        }
    }

    #endregion
    #region  Effect

    private float scaleAnimationSpeed = 2f;      // 애니메이션 속도
    private float scaleMultiplier = 1.5f;   // 최대 스케일 배수
    IEnumerator ScaleEffectCo(List<TMP_EffectField> list)
    {
        float scale;
        while (true)
        {
            scale = 1 + (Mathf.Sin(Time.time * scaleAnimationSpeed) * (scaleMultiplier - 1f));

            foreach (var f in list)
            {
                for (int i = 0; i < 4; i++)
                {
                    //f.vertices[f.vertexIndex + i] = f.charCenter + (f.vertices[f.vertexIndex + i] - f.charCenter) * scale;
                    f.vertices[f.vertexIndex + i] = f.charCenter + (f.originalVertices[f.vertexIndex + i] - f.charCenter) * scale;
                    
                }
            }

            for (int i = 0; i < main_Text.textInfo.meshInfo.Length; i++)
            {
                var meshInfo = main_Text.textInfo.meshInfo[i];
                meshInfo.mesh.vertices = meshInfo.vertices;
                main_Text.UpdateGeometry(meshInfo.mesh, i);
            }

            yield return null;
        }

    }
    float amplitude = 7;
    float bounceAnimationSpeed = 2;
    IEnumerator BounceEffectCo(List<TMP_EffectField> list)
    {
        while (true)
        {
            foreach (var f in list)
            {
                for (int i = 0; i < 4; i++)
                {

                    Vector3 pot = f.charCenter * 4f;
                    pot.x = 0;
                    //f.vertices[f.vertexIndex + i] = f.charCenter + (f.vertices[f.vertexIndex + i] - f.charCenter) * scale;
                    //f.vertices[f.vertexIndex + i] = f.charCenter + (f.originalVertices[f.vertexIndex + i] - f.charCenter) * scale;
                    f.vertices[f.vertexIndex + i] = f.originalVertices[f.vertexIndex + i] - new Vector3(0, Mathf.Sin(Time.time * bounceAnimationSpeed) * amplitude, 0);
                }
            }

            for (int i = 0; i < main_Text.textInfo.meshInfo.Length; i++)
            {
                var meshInfo = main_Text.textInfo.meshInfo[i];
                meshInfo.mesh.vertices = meshInfo.vertices;
                main_Text.UpdateGeometry(meshInfo.mesh, i);
            }

            yield return null;
        }
    }

    float appearAnimationSpeed =1;
    
    IEnumerator AppearEffect(List<TMP_EffectField> list){
        float percent =0;

        while(percent <1){
            percent += Time.deltaTime * appearAnimationSpeed;
            foreach(var f in list){
                var meshInfo = f.tmp.meshInfo[f.materialIndex];
                Color32[] colors = meshInfo.colors32;
                for(int i = 0; i<4;i++) colors[f.vertexIndex + i] = Color.Lerp(transparencyColor,Color.red,percent);
                meshInfo.mesh.colors32 = colors;
            }
            main_Text.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
            yield return null;
        }
          
    }

    #endregion

    private void ChangeColor(TextMeshEffectStruct data, Color targetColor)
    {

        for (int i = data.startIndex; i < data.startIndex + data.textLength && i < main_Text.textInfo.characterCount; i++)
        {
            var charInfo = main_Text.textInfo.characterInfo[i];
            if (!charInfo.isVisible)
                continue;
            ChangeColor(charInfo,targetColor);
        }

    }

    private void ChangeColor(TMP_CharacterInfo charInfo,Color targetColor){
         int materialIndex = charInfo.materialReferenceIndex;
            int vertexIndex = charInfo.vertexIndex;

            // vertex 색상 참조
            Color32[] colors = main_Text.textInfo.meshInfo[materialIndex].colors32;

            colors[vertexIndex + 0] = targetColor;
            colors[vertexIndex + 1] = targetColor;
            colors[vertexIndex + 2] = targetColor;
            colors[vertexIndex + 3] = targetColor;
    }


    private void UpdateTextMeshEffectStructList(ref string sentence)
    {
        int startIndex = 0; //1
        int textLength = 0;
        int mark = 0;
        StringBuilder sb = new();
       
        for (int i = 0; i < sentence.Length; i++)
        {
            switch (sentence[i])
            {
                case '[':
                    startIndex = i - mark;
                    mark++;
                    break;
                case ']':
                    textLength = i - mark - startIndex;
                    mark++;
                    textMeshEffectStructList.Add(new TextMeshEffectStruct(startIndex, textLength, Mark.Mark_1));
                    break;
                case '<':
                    startIndex = i - mark;
                    mark++;
                    break;
                case '>':
                    textLength = i - mark - startIndex;
                    mark++;
                    textMeshEffectStructList.Add(new TextMeshEffectStruct(startIndex, textLength, Mark.Mark_2));
                    break;
                default:
                    sb.Append(sentence[i]);
                    break;
            }
        }
        sentence = sb.ToString();
    }


    [Serializable]
    public struct TextMeshEffectStruct
    {
        public int startIndex;
        public int textLength;
        public Mark mark;
        public TextMeshEffectStruct(int startIndex, int textLength, Mark mark)
        {
            this.startIndex = startIndex;
            this.textLength = textLength;
            this.mark = mark;
        }

    }


    public class TMP_EffectField
    {
        public TMP_TextInfo tmp;
        public TMP_CharacterInfo charInfo;
        public Vector3[] vertices;
        public Vector3[] originalVertices;
        public int vertexIndex;
        public int materialIndex;
        public Vector3 charCenter;
        public Mark mark;

        public TMP_EffectField(TMP_TextInfo tmp, TMP_CharacterInfo charInfo, Mark mark)
        {
            this.tmp = tmp;
            this.charInfo = charInfo;
            vertexIndex = charInfo.vertexIndex;
            materialIndex = charInfo.materialReferenceIndex;

            vertices = tmp.meshInfo[materialIndex].vertices;
            originalVertices = vertices.Clone() as Vector3[];
            charCenter = (vertices[vertexIndex] + vertices[vertexIndex + 2]) / 2;

            this.mark = mark;
        }

    }


}
