
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;
using System.Threading.Tasks;
using System;

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





    public async Task TypingEffectTesk(TextMeshProUGUI text, string sentence, float delayTime = 0.01f)
    {
        int time = Mathf.FloorToInt(delayTime * 1000);
        Debug.Log(time);
        text.text = "";
        for(int i = 0; i < sentence.Length; i++)
        {
            text.text += sentence[i];
            await Task.Delay(time);
        }
    }

    public List<string> SplitText(string text, int length, char[] delimiters)
    {
        List<string> result = new List<string>();

        // 먼저 구두점으로 텍스트를 분할합니다.
        string[] parts = text.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);

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


}
