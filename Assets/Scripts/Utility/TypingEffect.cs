using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;

public class TypingEffect : MonoBehaviour
{

    private TextMeshProUGUI textComponent;
    private TMP_TextInfo textInfo;

    public float letterDelay = 0.1f; // 한 글자 출력 간격
    public float animationDuration = 0.1f; // 대각선 애니메이션 시간
    public Vector2 startOffset = new Vector2(0, 0); // 출발 위치 오프셋
    public float startRotationAngle = 45f; // 초기 기울기 각도


    private bool onPrograss;

    private bool isSkip;


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && onPrograss && !isSkip)
        {
            isSkip = true;
        }
    }


    public IEnumerator Typing(TextMeshProUGUI textMesh,string sentence,Color color,float fontSize = 42,bool audioActive = false,string audioName = GlobalText.DIALOGUE_CLICK_SOUND)
    {

        if (textMesh == null || string.Empty == sentence) yield break;
       
        isSkip = false;

        textInfo = textMesh.textInfo;
        textMesh.text = sentence;
        textMesh.ForceMeshUpdate();
        textComponent = textMesh;
        
        int totalCharacters = textMesh.text.Length;

        onPrograss = true;

        //250103
        StringBuilder sb = new();
        int lastLine = 0;
        bool lineChange = false;
        for (int i = 0; i < totalCharacters; i++)
        {
            if (isSkip)
            {
                break; 
            }

            // 현재 글자 활성화
            sb.Append(sentence[i]);
            textMesh.text = sb.ToString();
            textMesh.ForceMeshUpdate();// Add 0512
            textInfo = textMesh.textInfo;// Add 0512
            TMP_CharacterInfo charInfo = textInfo.characterInfo[i];

            
            if (charInfo.lineNumber != lastLine)
            {
                lastLine = charInfo.lineNumber;
                lineChange = true;
                textMesh.enableWordWrapping = false;
            
            }

            if (audioActive && !string.IsNullOrEmpty(audioName) && !char.IsWhiteSpace(sentence[i]))
            {
                Managers.Sound.PlaySound(audioName);
            }
                

            if(lineChange)
            {
                yield return new WaitForSeconds(0.1f);
                textMesh.enableWordWrapping = true;
                yield return new WaitForSeconds(0.1f);
                lineChange = false;
            }

            StartCoroutine(AnimationLatter(i));
            yield return new WaitForSeconds(0.05f);
            
        }

        textMesh.text = sentence;
        onPrograss = false;
    }




    private IEnumerator AnimationLatter(int index)
    {
        textComponent.ForceMeshUpdate();

        textInfo = textComponent.textInfo; // ADD 0512
        if(index >= textInfo.characterCount) yield break;// ADD 0512

        TMP_CharacterInfo charInfo = textInfo.characterInfo[index];
        if (!charInfo.isVisible) yield break; // 문자가 보이지 않으면 스킵

        int vertexIndex = charInfo.vertexIndex;
        Vector3[] vertices = textInfo.meshInfo[charInfo.materialReferenceIndex].vertices;
        if(vertexIndex + 3 >= vertices.Length) yield break;// ADD 0512

        // 네 개의 버텍스를 시작 위치로 이동
        Vector3[] originalPositions = new Vector3[4];
        Vector3 charMidBaseline = (vertices[vertexIndex] + vertices[vertexIndex + 2]) / 2;
        Quaternion rotation = Quaternion.Euler(0, 0, 45); // Z축 기준으로 회전

        for (int j = 0; j < 4; j++)
        {
            originalPositions[j] = vertices[vertexIndex + j];
            vertices[vertexIndex + j] += (Vector3)startOffset;
            vertices[vertexIndex + j] = rotation * (vertices[vertexIndex + j] - charMidBaseline) + charMidBaseline;
        }

        textComponent.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices);

        float elapsedTime = 0f;

        // 대각선 이동 애니메이션
        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / animationDuration);

            for (int j = 0; j < 4; j++)
            {
                vertices[vertexIndex + j] = Vector3.Lerp(vertices[vertexIndex + j], originalPositions[j], t);
            }

            textComponent.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices);
            yield return null;
        }

    }

    public IEnumerator TextDissolveFromLeft(TextMeshProUGUI textmesh, float charFadeDuration = 0.15f, float charDelay = 0.05f)
    {
        var textComponent = textmesh;
        textComponent.ForceMeshUpdate();

        TMP_TextInfo textInfo = textComponent.textInfo;
        int totalCharacters = textInfo.characterCount;

        for (int i = 0; i < totalCharacters; i++)
        {
            if (!textInfo.characterInfo[i].isVisible) continue;

            int matIndex = textInfo.characterInfo[i].materialReferenceIndex;
            int vIndex = textInfo.characterInfo[i].vertexIndex;
            Color32[] vertexColors = textInfo.meshInfo[matIndex].colors32;

            StartCoroutine(FadeOutChar(textComponent,vertexColors, vIndex, charFadeDuration));
            yield return new WaitForSeconds(charDelay);
        }
    }

    private IEnumerator FadeOutChar(TextMeshProUGUI textComponent, Color32[] vertexColors, int vIndex, float duration)
    {
        float elapsed = 0f;
        byte startAlpha = vertexColors[vIndex].a;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            byte alpha = (byte)Mathf.Lerp(startAlpha, 0, t);

            for (int j = 0; j < 4; j++)
                vertexColors[vIndex + j].a = alpha;

            textComponent.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // 마지막 정리
        for (int j = 0; j < 4; j++)
            vertexColors[vIndex + j].a = 0;

        textComponent.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
    }

    #region  Default
    public IEnumerator NormalTyping(
        TextMeshProUGUI textMesh,
        string sentence,
        Color color,
        int batchSize,
        float fontSize = 25,
        float delay = 0.01f,
        bool audioActive = false
        )
    {
        //Dealy : 0.1f
        textMesh.color = color;
        textMesh.fontSize = fontSize;

        StringBuilder sb = new();
        for (int i = 0; i < sentence.Length; i += batchSize)
        {
            int len = Mathf.Min(batchSize, sentence.Length - i);
            for (int j = 0; j < len; j++)
            {
                sb.Append(sentence[i + j]);
                textMesh.text = sb.ToString();
            }
            yield return new WaitForSeconds(delay);
        }

    }
    public IEnumerator NormalEraser(
    TextMeshProUGUI textMesh,
    int batchSize,
    float delay= 0.01f
    )
{
    StringBuilder sb = new(textMesh.text);
    while (sb.Length > 0)
    {
        int len = Mathf.Min(batchSize,sb.Length);
        sb.Remove(sb.Length - len,len);
        textMesh.text = sb.ToString();

        yield return new WaitForSeconds(delay);
    }
}
#endregion



}
