using UnityEngine.UI;
using UnityEngine;
using TMPro;
using System.Text;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEditor;


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

    #region Mark_1

    #endregion

    #region  Components

    #endregion

    #region Dialogue


    //Mark
    Mark mark;

    #endregion


    public string testSentence = "Lorem [Ipsum] is simply dummy /1";


    #region Pattern
    private string stringToIntPattern_PlayerDeath = "/1"; //player death
    #endregion


    private void Update()
    {
        //test
        if (Input.GetKeyDown(KeyCode.P))
        {

            Test();

        }
    }

    private void Init()
    {
        textMeshEffectStructList = new();
    }

    public void Reset()
    {
        textMeshEffectStructList.Clear();
        StopAllCoroutines();
        mark_1_EffectCoroutine = null;
        mark_2_EffectCoroutine = null;

        main_Text.text = "";
        main_Text.ForceMeshUpdate();
#if UNITY_EDITOR
        EditorUtility.SetDirty(main_Text.gameObject); // 텍스트가 속한 게임 오브젝트를 수정된 상태로 표시
        //UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene()); // 현재 씬을 더티 상태로 표시
#endif
    }


    public Coroutine mark_1_EffectCoroutine;
    public Coroutine mark_2_EffectCoroutine;

    public void Test()
    {
        string text = testSentence;
        //1. Replace
        Replace(ref text);
        //Search mark
        UpdateTextMeshEffectStructList(ref text);
        //main_text init
        main_Text.text = text;

        main_Text.ForceMeshUpdate();
        List<TMP_EffectField> fadeInList = new();
        List<TMP_EffectField> bounceList = new();

        //on Effect
        foreach (TextMeshEffectStruct data in textMeshEffectStructList)
        {
            ChangeColor(data, Color.red);

            if (data.mark == Mark.Mark_1) GetEffectFieldList(data, ref fadeInList);
            if (data.mark == Mark.Mark_2) GetEffectFieldList(data, ref bounceList);
        }

        main_Text.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);

        if (mark_1_EffectCoroutine != null) StopCoroutine(mark_1_EffectCoroutine);
        mark_1_EffectCoroutine = StartCoroutine(ScaleEffectCo(fadeInList));

        if (mark_2_EffectCoroutine != null) StopCoroutine(mark_2_EffectCoroutine);
        mark_2_EffectCoroutine = StartCoroutine(BounceEffectCo(bounceList));

    }


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

    private string GetDialogue(int id)
    {
        return Managers.Data.language.dict[id];
    }



    private void Replace(ref string sentence)
    {
        StringBuilder sb = new StringBuilder(sentence);
        sb.Replace(stringToIntPattern_PlayerDeath, 5.ToString());

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


    public float animationSpeed = 2f;      // 애니메이션 속도
    public float scaleMultiplier = 1.5f;   // 최대 스케일 배수
    IEnumerator ScaleEffectCo(List<TMP_EffectField> list)
    {
        float scale;
        while (true)
        {
            scale = 1 + (Mathf.Sin(Time.time * animationSpeed) * (scaleMultiplier - 1f));

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
    IEnumerator BounceEffectCo(List<TMP_EffectField> list)
    {
        float value;
        while (true)
        {
            value = (Mathf.Sin(Time.time * 3) + 1) / 2f;

            foreach (var f in list)
            {

                for (int i = 0; i < 4; i++)
                {

                    Vector3 pot = f.charCenter * 4f;
                    pot.x = 0;
                    //f.vertices[f.vertexIndex + i] = f.charCenter + (f.vertices[f.vertexIndex + i] - f.charCenter) * scale;
                    //f.vertices[f.vertexIndex + i] = f.charCenter + (f.originalVertices[f.vertexIndex + i] - f.charCenter) * scale;
                    f.vertices[f.vertexIndex + i] = f.originalVertices[f.vertexIndex + i] - new Vector3(0, Mathf.Sin(Time.time * animationSpeed) * amplitude, 0);
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


    private void ChangeColor(TextMeshEffectStruct data, Color targetColor)
    {

        for (int i = data.startIndex; i < data.startIndex + data.textLength && i < main_Text.textInfo.characterCount; i++)
        {
            var charInfo = main_Text.textInfo.characterInfo[i];
            if (!charInfo.isVisible)
                continue;

            int materialIndex = charInfo.materialReferenceIndex;
            int vertexIndex = charInfo.vertexIndex;

            // vertex 색상 참조
            Color32[] colors = main_Text.textInfo.meshInfo[materialIndex].colors32;

            colors[vertexIndex + 0] = targetColor / 0.5f;
            colors[vertexIndex + 1] = targetColor;
            colors[vertexIndex + 2] = targetColor / 1.5f;
            colors[vertexIndex + 3] = targetColor / 2;
        }

    }



    private void UpdateTextMeshEffectStructList(ref string sentence)
    {
        int startIndex = 0; //1
        int textLength = 0;
        int mark = 0;
        StringBuilder sb = new();
        // 0123 45 678 9
        //[abcd]e [asd]s //5
        //01234567891123
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
        TMP_TextInfo tmp;
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
