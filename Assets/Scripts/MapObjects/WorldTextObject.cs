using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WorldTextObject : BuildObj
{
    public Vector2 size;
    public string mainText;
    public float fontSize;

    [SerializeField] RectTransform _CanvasRT;
    [SerializeField] TextMeshProUGUI textMesh;



    public void SetSize(Vector2 size)
    {
        this.size = size;
        float x = size.x;
        float y = size.y;

        _CanvasRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, x);
        _CanvasRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, y);

    }

    public void SetText(string text)
    {
        mainText = text;
        textMesh.text = text;
    }
    public void SetFontSize(float fontSize){
        this.fontSize = fontSize;
        textMesh.fontSize = fontSize;
    }

    public override void SetData(ObjectData data)
    {
        SetSize(data.size);
        SetText(data.text);
        SetFontSize(data.fontSize);
    }

    public override T GetData<T>()
    {
        if (typeof(T) == typeof(ObjectData))
        {
            return (T)(object)new ObjectData(id, transform.position,transform.rotation,transform.localScale ,size, mainText,fontSize);
        }
        
        return default(T);
    }
    public override void SetData<T>(T data)
    {
        if (typeof(T) == typeof(ObjectData))
        {
            ObjectData objData = (ObjectData)(object)data;
            ObjectData = objData;

            transform.position = objData.position;
            SetData(objData);
        }
    }

}
