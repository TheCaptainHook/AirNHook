using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;

public class UI_ShowToolTip : MousePointerEntity
{
    public bool OnPointer;
    public float timer;

    public string toolTipText;
    public GameObject toolTipObj;
    private TextMeshProUGUI text;

    private void Awake()
    {
        CreateTooltip();
    }



    public override void OnPointerClick(PointerEventData data)
    {
        if (toolTipObj) { toolTipObj.SetActive(false); }

        OnPointer = false;
        timer = 0;
    }
    public override void OnPointerEnter(PointerEventData data)
    {
        OnPointer = true;
        StartCoroutine(Co_Timer());
    }
    public override void OnPointerExit(PointerEventData data)
    {
        if (toolTipObj != null)
        {
            OnPointer = false;
            timer = 0;
            toolTipObj.SetActive(false);
        }
    }



    private void CreateTooltip()
    {
        toolTipObj = new GameObject("ToolTip");

        GameObject backGround = new GameObject("BackGround");
        GameObject textObj = new GameObject("Text");
        toolTipObj.AddComponent<RectTransform>();
        toolTipObj.transform.SetParent(transform);
        toolTipObj.transform.localPosition = new Vector3(10, -35);
        RectTransform rect = toolTipObj.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(130, 10);

        //bg
        backGround.AddComponent<Image>().color = Color.gray;
        //text
        textObj.AddComponent<TextMeshProUGUI>();
        text = textObj.GetComponent<TextMeshProUGUI>();
        text.fontSize = 15;
        //text.alignment = TextAlignmentOptions.Center;
        text.alignment = TextAlignmentOptions.Midline;
        text.color = Color.white;
        text.text = toolTipText;


        backGround.transform.SetParent(toolTipObj.transform);
        backGround.GetComponent<RectTransform>().sizeDelta = new Vector2(250, 30);
        
        backGround.transform.localPosition = new Vector3(0, 0);
        textObj.transform.SetParent(toolTipObj.transform);
        textObj.GetComponent<RectTransform>().sizeDelta = new Vector2(250, 30);
        textObj.transform.localPosition = new Vector3(0, 0);

        rect.localScale = new Vector3(1, 1, 1);

        toolTipObj.SetActive(false);
    }

    public void SetText(string text)
    {
        //if (toolTipObj == null) return;
        //toolTipObj.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);

        toolTipText = text;
        this.text.text = text;
    }

   IEnumerator Co_Timer()
    {
        while (OnPointer)
        {
            if(timer >= 1)
            {
                toolTipObj.SetActive(true);
                break;

            }
            timer += Time.deltaTime;
            yield return null;

        }

        //if(timer == 0 && toolTipObj.activeSelf)
        //{
        //    toolTipObj.SetActive(false);
        //}
    }


}
