
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;
using System.Threading.Tasks;
using System;
using System.Text;
using System.Threading;

public class Util
{

    #region  Text

    // Create Text in the World
    public  TextMesh CreateWorldText(string text, Transform parent = null, Vector3 localPosition = default(Vector3), int fontSize = 40, Color? color = null, TextAnchor textAnchor = TextAnchor.UpperLeft, TextAlignment textAlignment = TextAlignment.Left, int sortingOrder = 500)
    {
        if (color == null) color = Color.white;
        return CreateWorldText(parent, text, localPosition, fontSize, (Color)color, textAnchor, textAlignment, sortingOrder);
    }

    public  TextMesh CreateWorldText(Transform parent,string text,Vector3 localPosition,int fontSize,Color fontColor,TextAnchor textAnchor,TextAlignment textAlignment, int sortingOrder)
    {
        GameObject gameObject = new GameObject("World_Text", typeof(TextMesh));
        Transform transform = gameObject.transform;
        transform.SetParent(parent, false);
        transform.localPosition = localPosition;
        TextMesh textMesh = gameObject.GetComponent<TextMesh>();
        textMesh.anchor = textAnchor;
        textMesh.alignment = textAlignment;
        textMesh.text = text;
        textMesh.fontSize = fontSize;
        textMesh.color = fontColor;
        textMesh.GetComponent<MeshRenderer>().sortingOrder = sortingOrder;
        
        return textMesh;
    }





    public async Task TypingEffectTask(TextMeshProUGUI text, string sentence, Color color, float fontSize,float delayTime, CancellationTokenSource token = null)
    {
        if (text == null)
        {
            Debug.LogError("TextMeshProUGUI is null!");
            return;
        }


        CancellationToken _token = (CancellationToken)token?.Token; // Simplified the initialization


        int time = Mathf.FloorToInt(delayTime * 1000);
        text.text = "";

        StringBuilder typedSentence = new StringBuilder();
        for (int i = 0; i < sentence.Length; i++)
        {
            //if (_token != null && _token.IsCancellationRequested)
            //{
            //    text.text = sentence;
            //    return;
            //}
            Managers.Sound.PlaySound(AudioType.Dialogue_Click, AudioMixerGroupType.Effects, false, 0.35f, 0f);
            typedSentence.Append(sentence[i]);
            text.color = color;
            text.text = typedSentence.ToString(); // Update text with typed characters
            text.fontSize = fontSize;
            // Await Task.Delay asynchronously
            try
            {
                await Task.Delay(time,_token);
            }
            catch (TaskCanceledException)
            {
                text.text = sentence;
                return;
            }
            catch (Exception ex)
            {
                Debug.LogError("Error during typing effect task: " + ex.Message);
            }
        }

    }


    public async Task EraserEffectTask(TextMeshProUGUI text, float delayTime = 0.005f)
    {
        if (text == null)
        {
            Debug.LogError("TextMeshProUGUI is null! or string.Empty");
            return;
        }

        int time = Mathf.FloorToInt(delayTime * 1000);
        string st = text.text;

        for (int i = st.Length-1; i >=0; i--)
        {
            try
            {
                text.text = st.Substring(0, i);
                await Task.Delay(time);
            }
            catch (TaskCanceledException ex)
            {
                Debug.LogWarning("Typing effect task was canceled: " + ex.Message);
                text.text = "";
                return;

            }
            catch (Exception ex)
            {
                Debug.LogError("Error during typing effect task: " + ex.Message);
                text.text = "";
            }
            
        }

    }


    public List<string> SplitText(string text, int length, char[] delimiters)
    {
        List<string> result = new List<string>();

        // 먼저 구두점으로 텍스트를 분할합니다.
        string[] parts = text.Split(delimiters, StringSplitOptions.None);

        foreach (string part in parts)
        {
            // 분할된 각 부분을 다시 length 크기로 분할합니다.
            for (int i = 0; i < part.Length; i += length)
            {
                if (i + length <= part.Length)
                {
                    result.Add(part.Substring(i, length));
                }
                else
                {
                    result.Add(part.Substring(i));
                }
            }
        }

        return result;
    }

    #endregion

    #region  Mouse

    public Vector3 GetMouseWorldPosition(Vector3 screenPosition, Camera camera)
    {
        Vector3 worldPosition = camera.ScreenToWorldPoint(screenPosition);
        worldPosition.z = 0;
        return worldPosition;
    }

    #endregion


    #region Transform
    public Transform CreateChildTransform(Transform parent, string name)
    {
        if (parent.Find(name) != null)
        {
           UnityEngine.Object.Destroy(parent.Find(name).gameObject);
        }

        GameObject childObject = new GameObject(name);
        Transform childTransform = childObject.transform;
        childTransform.SetParent(parent);
        return childTransform;
    }
    public Transform CreateChildTransform( string name)
    {
        GameObject childObject = new GameObject(name);
        Transform childTransform = childObject.transform;
        return childTransform;
    }


    #endregion


    #region Date

    #endregion

}
