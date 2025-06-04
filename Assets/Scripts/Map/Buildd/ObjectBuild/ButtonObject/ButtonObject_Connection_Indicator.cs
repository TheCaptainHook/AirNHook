using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ButtonObject_Connection_Indicator : MonoBehaviour
{
    [SerializeField] Transform containerTR;


    string mainPath = "Arts/Sprites/PreviewSprites/Object";
    string bgPath = "Arts/Sprites/Objects/Circle";

    private void CreateBorder(GameObject main)
    {
        var border = new GameObject("Border").AddComponent<RectTransform>();
        border.transform.SetParent(containerTR);
        border.sizeDelta = new Vector2(.5f, .5f);

        var bg = new GameObject("bg").AddComponent<RectTransform>();
        bg.SetParent(border);
        bg.anchorMin = Vector2.zero;
        bg.anchorMax = Vector2.one;
        bg.offsetMin = Vector2.zero;
        bg.offsetMax = Vector2.zero;

        var mainSprite = new GameObject("main").AddComponent<RectTransform>();
        mainSprite.SetParent(border);
        mainSprite.anchorMin = Vector2.zero;
        mainSprite.anchorMax = Vector2.one;
        mainSprite.offsetMin = Vector2.zero;
        mainSprite.offsetMax = Vector2.zero;

        var bgImage = bg.AddComponent<Image>().sprite = Resources.Load<Sprite>(bgPath);
        var mainImage = mainSprite.AddComponent<Image>().sprite = Resources.Load<Sprite>($"{mainPath}/{main.name}");
    
    }
}
