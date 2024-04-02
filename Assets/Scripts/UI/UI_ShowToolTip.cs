using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;

public class UI_ShowToolTip : MousePointerEntity
{
    [SerializeField] string toolTipText;
    public GameObject toolTipObj;


    public override void OnPointerEnter(PointerEventData data)
    {
        if (toolTipObj == null) { CreateTooltip(); }
        else toolTipObj.SetActive(true);


        Debug.Log("Enter");
    }
    public override void OnPointerExit(PointerEventData data)
    {
        //if(toolTipObj != null)
        //{
        //    toolTipObj.SetActive(false);
        //}
        Debug.Log("Exit");
    }



    private void CreateTooltip()
    {
        toolTipObj = new GameObject("ToolTip");

        GameObject backGround = new GameObject("BackGround");
        GameObject textObj = new GameObject("Text");
        toolTipObj.AddComponent<RectTransform>();
        toolTipObj.transform.SetParent(transform);
        toolTipObj.transform.localPosition = new Vector3(0, 0);
        RectTransform rect = toolTipObj.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(250, 30);


        //bg
        backGround.AddComponent<Image>().color = Color.gray;
        //text
        textObj.AddComponent<TextMeshProUGUI>();
        TextMeshProUGUI text = textObj.GetComponent<TextMeshProUGUI>();
        text.fontSize = 15;
        text.alignment = TextAlignmentOptions.Center;
        text.color = Color.black;
        text.text = toolTipText;


        backGround.transform.SetParent(toolTipObj.transform);
        textObj.transform.SetParent(toolTipObj.transform);
    }



}
