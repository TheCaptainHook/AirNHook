using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;

public class UI_ShowToolTip : MousePointerEntity
{
    private bool OnPointer;
    private float timer;

    [SerializeField] string toolTipText;
    private GameObject toolTipObj;


    public override void OnPointerEnter(PointerEventData data)
    {
        OnPointer = true;
        StartCoroutine(Co_Timer());

        Debug.Log("Enter");
    }
    public override void OnPointerExit(PointerEventData data)
    {
        if (toolTipObj != null)
        {
            OnPointer = false;
            timer = 0;
            toolTipObj.SetActive(false);
        }
        Debug.Log("Exit");
    }



    private void CreateTooltip()
    {
        toolTipObj = new GameObject("ToolTip");

        GameObject backGround = new GameObject("BackGround");
        GameObject textObj = new GameObject("Text");
        toolTipObj.AddComponent<RectTransform>();
        toolTipObj.transform.SetParent(transform);
        toolTipObj.transform.localPosition = new Vector3(30, -40);
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
        backGround.GetComponent<RectTransform>().sizeDelta = new Vector2(250, 30);
        
        backGround.transform.localPosition = new Vector3(0, 0);
        textObj.transform.SetParent(toolTipObj.transform);
        textObj.GetComponent<RectTransform>().sizeDelta = new Vector2(250, 30);
        textObj.transform.localPosition = new Vector3(0, 0);
    }



   IEnumerator Co_Timer()
    {
        while (OnPointer)
        {
            if(timer >= 1)
            {
                if (toolTipObj == null) { CreateTooltip(); }
                else toolTipObj.SetActive(true);
                break;

            }
            timer += Time.deltaTime;
            yield return null;

        }
    }


}
