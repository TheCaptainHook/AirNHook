using System.Collections;
using System.Collections.Generic;
using Org.BouncyCastle.Crypto.Engines;
using TMPro;
using UnityEngine;

public class TextSplitWord : MonoBehaviour
{
    TextMeshProUGUI textMeshPro;
    RectTransform rectTransform;

    Color localColor = new Color(48f / 255f, 172f / 255f, 52f / 255f); 

    private void Awake(){
        rectTransform = transform as RectTransform;
        textMeshPro = GetComponent<TextMeshProUGUI>();
        textMeshPro.color = localColor;
    }

   public void Setting(int fontSize,string word){
    textMeshPro.text = word;
    Debug.Log(textMeshPro.preferredWidth);
    Vector2 sizeDelta = new Vector2(textMeshPro.preferredWidth,rectTransform.sizeDelta.y);
    rectTransform.sizeDelta = sizeDelta;
    textMeshPro.fontSize = fontSize;
    

   }
}
