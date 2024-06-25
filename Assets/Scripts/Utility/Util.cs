
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;
using System.Threading.Tasks;

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




    
    public async Task TypingEffectTesk(TextMeshProUGUI text,string sentence, float delayTime)
    {
        int time = Mathf.FloorToInt(delayTime * 1000);
        Debug.Log(time);
        text.text = "";
        for(int i = 0; i < sentence.Length; i++)
        {
            if (Input.anyKey) { Debug.Log("anykey"); }
            text.text += sentence[i];
            await Task.Delay(time);
        }
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
            Object.Destroy(parent.Find(name).gameObject);
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
