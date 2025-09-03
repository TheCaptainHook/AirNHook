
using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class Puzzle_1_HintScreen : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text;

    CharInfoField[] charInfos;
    
    [Header("Answer")]
    [SerializeField] Image glowImage;
    [SerializeField] Material redMat;
    [SerializeField] Material greenMat;
    [SerializeField] Material orgMat;

    [SerializeField] GameObject correctObj;
    [SerializeField] GameObject _falseObj;
    private Coroutine answerCoroutine;
    private WaitForSeconds waitSeconds = new WaitForSeconds(1);
    [ReadOnly]
    public string answer;
    [ReadOnly]
    public string orgSentence;
    

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
            sb[i] = '#';
        }

        text.text = sb.ToString();
        this.answer =answer;
        orgSentence = sb.ToString();
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

    #region Answer

    public void Correct()
    {
        StopAllCoroutines();
        text.text = "";
        if (_falseObj.activeSelf) _falseObj.SetActive(false);

        glowImage.material = greenMat;
        correctObj.SetActive(true);

    }
    public void False()
    {
        if (answerCoroutine != null)
        {
            StopCoroutine(answerCoroutine);
            _falseObj.SetActive(false);
        }
        answerCoroutine = StartCoroutine(FalseCo());
    }


    //    bool isAnswerFalse;
    IEnumerator FalseCo()
    {
        //Sound
        Managers.Sound.PlaySound3D(GlobalText.PUZZLE_HINT_WRONG, transform.position);
        //Sound
        // isAnswerFalse = true;
        _falseObj.SetActive(true);
        glowImage.material = redMat;
        yield return waitSeconds;

        glowImage.material = orgMat;
        _falseObj.SetActive(false);
        answerCoroutine = null;
        // isAnswerFalse = false;
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

    // public Color orgColr;
    // 
    private Color correctColor = new Color(30 / 255f, 220 / 255f, 90 / 255f);
    // private Color correctColor = new Color(55/255f,85/255f,235/255f);
    IEnumerator EffectCo(CharInfoField[] charInfos)
    {
        while (true)
        {
            // int num = Random.Range(0, charInfos.Length);

            yield return CharEffectCo();

            yield return new WaitForSeconds(1f);
        }
    }
    float changeCharDuration = 0.2f;
    IEnumerator CharEffectCo()
    {
        float percent = 0;

        char[] cached = text.text.ToCharArray();
        float changeCharPercent = 0;

        int[] c = new int[text.text.Length];
        for (int i = 0; i < c.Length; i++)
        {
            int ran = Random.Range(0, text.text.Length);
            c[i] = ran;
        }

        float baseGlitchIntensity = 9f; // 기본 강도
        float glitchIntensity = baseGlitchIntensity * text.fontSize / 100f;

        float[] correctCharTimers = new float[c.Length]; 

        while (percent < 1f)
        {
            percent += Time.deltaTime;
            changeCharPercent += Time.deltaTime;

            for (int i = 0; i < c.Length; i++)
            {
                CharInfoField charInfoField = charInfos[c[i]];
                
                if (changeCharPercent > changeCharDuration && correctCharTimers[c[i]] <= 0f)
                {
                    if(Random.Range(0,100) <= 20)
                    {
                        cached[charInfoField.charIndex] = answer[c[i]];
                        correctCharTimers[c[i]] = changeCharDuration;
                    }else
                    {
                        char randomChar = (char)Random.Range(33, 126);
                        cached[charInfoField.charIndex] = randomChar; 
                        correctCharTimers[c[i]] = 0;
                    }
                    text.SetText(cached);
                    text.ForceMeshUpdate(); 
                }
                Color32[] colors = charInfoField.tmp.meshInfo[charInfoField.materialIndex].colors32;//color

                for (int j = 0; j < 4; j++) 
                {
                    Vector3 offset = new Vector3(
                        Random.Range(-glitchIntensity, glitchIntensity),
                        Random.Range(-glitchIntensity, glitchIntensity),
                        0);
                        charInfoField.vertices[charInfoField.vertexIndex + j] = charInfoField.originalVertices[charInfoField.vertexIndex + j] + offset;
                        if(correctCharTimers[c[i]] >0f){
                             colors[charInfoField.vertexIndex + j] = correctColor;
                        } 
                }

                var meshInfo = text.textInfo.meshInfo[charInfoField.materialIndex];
                meshInfo.mesh.vertices = meshInfo.vertices;
                meshInfo.mesh.colors32 = meshInfo.colors32;
                text.UpdateGeometry(meshInfo.mesh, charInfoField.materialIndex);

            }
              for (int i = 0; i < correctCharTimers.Length; i++)
                {
                    if (correctCharTimers[i] > 0f)
                    {
                        correctCharTimers[i] -= Time.deltaTime;
                    }
                }

            if (changeCharPercent > 0.5f){
                changeCharPercent = 0;
                // correctChars = new bool[c.Length];
            } 

            yield return new WaitForSeconds(0.05f);
        }
        //Recover Text
        text.SetText(orgSentence);
        text.ForceMeshUpdate(); 


    }

    #endregion
    private void ChangeCharColor(CharInfoField info,Color32 newColor)
    {
        Color32[] colors = info.tmp.meshInfo[info.materialIndex].colors32;
        for(int i = 0; i<4;i++)colors[info.vertexIndex + i] = newColor;
        // info.tmp.textComponent.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
    }
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