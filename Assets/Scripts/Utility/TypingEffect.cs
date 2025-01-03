using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;

public class TypingEffect : MonoBehaviour
{

    private TextMeshProUGUI textComponent;
    private TMP_TextInfo textInfo;

    public float letterDelay = 0.1f; // 한 글자 출력 간격
    public float animationDuration = 0.5f; // 대각선 애니메이션 시간
    public Vector2 startOffset = new Vector2(0, 0); // 출발 위치 오프셋
    public float startRotationAngle = 45f; // 초기 기울기 각도


    private bool onPrograss;

    private bool isSkip;


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && onPrograss)
        {
            isSkip = true;
        }
    }

    public IEnumerator Typing(TextMeshProUGUI textMesh,string sentence,Color color,float fontSize = 42,bool audioActive = false)
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
        
        for (int i = 0; i < totalCharacters; i++)
        {
            if (isSkip)
            {
                break; 
            }

            // 현재 글자 활성화
            sb.Append(sentence[i]);
            textMesh.text = sb.ToString();
            // 애니메이션 코루틴 시작
            if (audioActive)
                Managers.Sound.PlaySound(GlobalText.DIALOGUE_CLICK_SOUND);
            StartCoroutine(AnimationLatter(i));
            yield return new WaitForSeconds(0.05f);
        }

        textMesh.text = sentence;
        onPrograss = false;
    }




    private IEnumerator AnimationLatter(int index)
    {
        textComponent.ForceMeshUpdate();
        TMP_CharacterInfo charInfo = textInfo.characterInfo[index];

        if (!charInfo.isVisible) yield break; // 문자가 보이지 않으면 스킵

        Vector3[] vertices = textInfo.meshInfo[charInfo.materialReferenceIndex].vertices;
        int vertexIndex = charInfo.vertexIndex;

        // 네 개의 버텍스를 시작 위치로 이동
        Vector3[] originalPositions = new Vector3[4];
        Vector3 charMidBaseline = (vertices[vertexIndex] + vertices[vertexIndex + 2]) / 2;
        Quaternion rotation = Quaternion.Euler(0, 0, 45); // Z축 기준으로 회전

        for (int j = 0; j < 4; j++)
        {
            originalPositions[j] = vertices[vertexIndex + j]; // 원래 위치 저장
            vertices[vertexIndex + j] += (Vector3)startOffset; // 대각선 시작 위치로 이동 + rotate
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
    for(int i = 0;i<sentence.Length;i+=batchSize)
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
