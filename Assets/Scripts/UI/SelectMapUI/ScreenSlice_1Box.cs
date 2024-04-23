using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class ScreenSlice_1Box : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI mapIdText;
    [SerializeField] Image mapIdImage;

    public void SetData(string text,Sprite sprite)
    {
        mapIdText.text = text;
        mapIdImage.sprite = sprite;
    }
    public void Reset()
    {
        mapIdText.text = "";
        mapIdImage.sprite = null;
    }
}
