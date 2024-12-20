
using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;

public class Puzzle_1_HintScreen : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text;

    CharInfoField[] charInfos;
    //float glitchIntensity = 7f; // 흔들리는 강도

    public void SetHint(string answer)
    {
        ConvertAnswer(answer);
        //Effect Coroutine
        Init();
        StartCoroutine(EffectCo(charInfos));
    }


    private void ConvertAnswer(string answer)
    {
        StringBuilder sb = new StringBuilder(answer);

        for (int i = 0; i < sb.Length; i++)
        {
            if(i%2 == 0)
            {
                sb[i] = 'X';
            }
        }

        text.text = sb.ToString();

    }


    #region Editor
    public void SetText(string text)
    {
        if (!string.Equals(this.text.text, text)) 
        {
            this.text.text = text;
        }

        
    }

    #endregion


    #region Effect
 
    private void Init()
    {
        text.ForceMeshUpdate();

        charInfos = new CharInfoField[text.text.Length];
        for (int i = 0; i < text.text.Length; i++)
        {
            charInfos[i] = new CharInfoField(text.textInfo, text.textInfo.characterInfo[i]);
        }
    }

    IEnumerator EffectCo(CharInfoField[] charInfos)
    {
        while (true)
        {
            int num = Random.Range(0, charInfos.Length);
            yield return CharEffectCo();

            yield return new WaitForSeconds(1f);
        }
    }

    IEnumerator CharEffectCo()
    {
        float percent = 0;

        char[] cached = text.text.ToCharArray();
        float changeCharPercent = 0;

        int[] c = new int[2];
        for (int i = 0; i < 2; i++)
        {
            int ran = Random.Range(0, text.text.Length);
            c[i] = ran;
        }

        float baseGlitchIntensity = 7f; // 기본 강도
        float glitchIntensity = baseGlitchIntensity * text.fontSize / 100f;

        while (percent < 1f)
        {
            percent += Time.deltaTime;
            changeCharPercent += Time.deltaTime;

            for (int i = 0; i < 2; i++)
            {
                CharInfoField charInfoField = charInfos[c[i]];

                if (changeCharPercent > 0.3f)
                {
                    cached[charInfoField.charIndex] = (char)Random.Range(33, 126);
                    text.SetText(cached);
                }

                //float size = 1 - percent;
                for (int j = 0; j < 4; j++)
                {
                    Vector3 offset = new Vector3(
                        Random.Range(-glitchIntensity, glitchIntensity),
                        Random.Range(-glitchIntensity, glitchIntensity),
                        0);
                    charInfoField.vertices[charInfoField.vertexIndex + j] = charInfoField.originalVertices[charInfoField.vertexIndex + j] + offset;

                }

                var meshInfo = text.textInfo.meshInfo[charInfoField.materialIndex];
                meshInfo.mesh.vertices = meshInfo.vertices;
                text.UpdateGeometry(meshInfo.mesh, charInfoField.materialIndex);

            }

            if (changeCharPercent > 0.3f) changeCharPercent = 0;



            yield return null;
        }


        for (int i = 0; i < c.Length; i++)
        {
            CharInfoField charInfoField = charInfos[c[i]];
            cached[charInfoField.charIndex] = charInfoField.orgChar;
            text.SetText(cached);

            for (int j = 0; j < 4; j++)
                charInfoField.vertices[charInfoField.vertexIndex + j] = charInfoField.originalVertices[charInfoField.vertexIndex + j];
        }

    }
    #endregion
}

public class CharInfoField
{

    public TMP_TextInfo tmp;
    public TMP_CharacterInfo charInfo;
    public Vector3[] vertices;
    public Vector3[] originalVertices;
    public int vertexIndex;
    public int materialIndex;
    public Vector3 charCenter;

    public char orgChar;
    public int charIndex;

    public CharInfoField(TMP_TextInfo tmp, TMP_CharacterInfo charInfo)
    {
        this.tmp = tmp;
        this.charInfo = charInfo;
        vertexIndex = charInfo.vertexIndex;
        materialIndex = charInfo.materialReferenceIndex;

        vertices = tmp.meshInfo[materialIndex].vertices;
        originalVertices = (Vector3[])vertices.Clone();
        charCenter = (vertices[vertexIndex] + vertices[vertexIndex + 2]) / 2;

        orgChar = charInfo.character;
        charIndex = charInfo.index;

    }
}