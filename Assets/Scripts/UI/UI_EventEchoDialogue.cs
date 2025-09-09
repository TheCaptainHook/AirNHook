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
    Mark_2, //<> : Bounce, Currently in development. not ready
    Mark_3
}

/**
    1. Open Effect Animation
    2. Default Animation Start
    3. Set and Convert Text
    4. Typing Sentence
**/
public class UI_EventEchoDialogue : UI_Base
{
    // [Space(20)]
    // [SerializeField] TMP_FontAsset font;

    //1210 Text Effect
    [ReadOnly]
    public List<TextMeshEffectStruct> textMeshEffectStructList;
    // private List<TMP_EffectField> allTMP_EffectFieldList;


    #region Dialogue


    //Mark
    Mark mark;

    #endregion

    #region Components
    [Space(20)]
    [SerializeField] Animator mainAnimator;
    [SerializeField] TextMeshProUGUI main_Text;//Main_Text
    #endregion

    string testSentence = "Lorem [Ipsum] is simply dummy /1";


    #region Pattern
    private string stringToIntPattern_PlayerDeath = "/1"; //player death
    private string stringToIntPattern_PlayerUsePortal = "/2"; // use portal
    #endregion

    #region  Color
    private Color transparencyColor = new Color(1,1,1,0);
    #endregion


    #region Animation
    private readonly int Open = Animator.StringToHash("Open");
    private readonly int Close = Animator.StringToHash("Close");

  
    private  void StartUI()
    {
        mainAnimator.SetTrigger(Open);
    }


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
        //Start UI Animation
        StartUI();
        //Start UI Animation

        //1. Replace, stringToIntPattern
        Replace(ref text);
        //Search mark
        UpdateTextMeshEffectStructList(ref text);
        //main_text init
        main_Text.text = text;
        
         StartCoroutine(DialogueEffect());

    }
    //-----------------------------------------------------------------------250224
    IEnumerator DialogueEffect()
    {
        List<TMP_EffectField> defaultList = new();
        //-----------------------------------------------------------------------250224
        List<TMP_EffectField> scaleList = new();
        List<TMP_EffectField> bounceList = new();
        main_Text.ForceMeshUpdate();
        yield return new WaitForSeconds(0.1f);

        //start Color
        foreach (TextMeshEffectStruct data in textMeshEffectStructList)
        {
            switch(data.mark)
            {
                case Mark.Mark_1:
                ChangeColor(data, transparencyColor);
                GetEffectFieldList(data, ref scaleList);
                break;
                case Mark.Mark_2:
                ChangeColor(data, transparencyColor);
                GetEffectFieldList(data, ref bounceList);
                break;
                case Mark.Default:
                GetEffectFieldList(data,ref defaultList);
                break;
            }
            
            // if(data.mark == Mark.Default) GetEffectFieldList(data,ref defaultList);
            // if (data.mark == Mark.Mark_1) GetEffectFieldList(data, ref scaleList);
            // if (data.mark == Mark.Mark_2) GetEffectFieldList(data, ref bounceList);
        }
        main_Text.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
        yield return new WaitForSeconds(0.3f);

        // StartCoroutine(AppearEffect(bounceList));
        StartCoroutine(AppearEffect(scaleList));

        // main_Text.ForceMeshUpdate();
        // main_Text.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
        yield return new WaitForSeconds(0.2f);

        if (mark_1_EffectCoroutine != null) StopCoroutine(mark_1_EffectCoroutine);
        mark_1_EffectCoroutine = StartCoroutine(ScaleEffectCo(scaleList));

        // if (mark_2_EffectCoroutine != null) StopCoroutine(mark_2_EffectCoroutine);
        // mark_2_EffectCoroutine = StartCoroutine(BounceEffectCo(bounceList));

        StartCoroutine(ShutDownCo());
    }
    //-----------------------------------------------------------------------250224




    IEnumerator ShutDownCo(){
        yield return new WaitForSeconds(5);
        mainAnimator.SetTrigger(Close);
        yield return new WaitForSeconds(0.3f);
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

        sb.Replace(stringToIntPattern_PlayerDeath, Managers.Data.saveData._AchievementData.player_Death.ToString());
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
//a[aab] strat 1, lengh 5 -1-1 = 3
    #endregion
    #region  Effect

    private float scaleAnimationSpeed = 5f;      // 애니메이션 속도
    private float scaleMultiplier = 0.5f;   // 최대 스케일 배수
    IEnumerator ScaleEffectCo(List<TMP_EffectField> list)
    {
        float scale;
        while (true)
        {
            // scale = 1 + (Mathf.Sin(Time.time * scaleAnimationSpeed) * (scaleMultiplier - 1f));
            scale = 1 + (Mathf.Sin(Time.time * scaleAnimationSpeed) * scaleMultiplier);
            foreach (var f in list)
            {
                for (int i = 0; i < 4; i++)
                {
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
        if(list.Count == 0) yield break;

        while (true)
        {
            foreach (var f in list)
            {
                for (int i = 0; i < 4; i++)
                {
                    Vector3 pot = f.charCenter * 4f;
                    pot.x = 0;
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

    float appearAnimationSpeed =1f;

    IEnumerator AppearEffect(List<TMP_EffectField> list)
    {
        if (list.Count == 0) yield break;

        float percent = 0;

        foreach (var f in list)
        {
            // var meshInfo = main_Text.textInfo.meshInfo[f.materialIndex];
            for (int i = 0; i < 4; i++)
            {
                f.vertices[f.vertexIndex + i] = f.charCenter + (f.originalVertices[f.vertexIndex + i] - f.charCenter) * 50;
            }
        }
        yield return null;

        while (percent < 1)
        {
            percent += Time.deltaTime * appearAnimationSpeed;
            foreach (var f in list)
            {
                var meshInfo = f.tmp.meshInfo[f.materialIndex];
                Color32[] colors = meshInfo.colors32;
                for (int i = 0; i < 4; i++)
                {
                    colors[f.vertexIndex + i] = Color.Lerp(transparencyColor, Color.red, percent);
                    f.vertices[f.vertexIndex + i] = Vector3.Lerp(f.vertices[f.vertexIndex + i], f.originalVertices[f.vertexIndex + i], percent);

                }
                meshInfo.mesh.colors32 = colors;

                meshInfo.mesh.vertices = f.vertices;
            }

            main_Text.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32 | TMP_VertexDataUpdateFlags.Vertices);


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

    private void ChangeColor(TMP_CharacterInfo charInfo, Color targetColor)
    {
        int materialIndex = charInfo.materialReferenceIndex;
        int vertexIndex = charInfo.vertexIndex;

        var meshInfo = main_Text.textInfo.meshInfo[materialIndex];

        // vertex 색상 참조

        Color32[] colors = meshInfo.colors32;

        colors[vertexIndex + 0] = targetColor;
        colors[vertexIndex + 1] = targetColor;
        colors[vertexIndex + 2] = targetColor;
        colors[vertexIndex + 3] = targetColor;

        meshInfo.colors32 = colors;
            
    }

    public List<string> testList;
    private void UpdateTextMeshEffectStructList(ref string sentence)
    {
        int startIndex = 0; //1
        int textLength = 0;
        int mark = 0;
        StringBuilder sb = new();

        bool isInsideMarker = false; // 마커 내부 여부 체크 변수
        testList = new();
        for (int i = 0; i < sentence.Length; i++)
        {
            switch (sentence[i])
            {
                case '[':
                    startIndex = i - mark;  
                    mark++;                 
                    isInsideMarker = true;
                    break;
                case ']':  
                    textLength = i - mark - startIndex; 
                    mark++;                             
                    textMeshEffectStructList.Add(new TextMeshEffectStruct(startIndex, textLength, Mark.Mark_1));
                    isInsideMarker = false;
                    break;
                case '<':
                    startIndex = i - mark;
                    mark++;
                    isInsideMarker = true;
                    break;
                case '>':
                    textLength = i - mark - startIndex;
                    mark++;
                    textMeshEffectStructList.Add(new TextMeshEffectStruct(startIndex, textLength, Mark.Mark_2));
                    isInsideMarker = true;
                    break;
                default:
                    if(!isInsideMarker)
                    {
                        textMeshEffectStructList.Add(new TextMeshEffectStruct(i-mark,1,Mark.Default));
                    }
                    
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
