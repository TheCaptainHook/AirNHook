using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Additional_Indicator : Indicator
{
    Transform mainT;

    GameObject ui;



    private void Awake()
    {
        mainT = transform.GetChild(0);
    }
    private void Update()
    {
        if(linked && curLinkObj == null)
        {
            Destroy(gameObject);
        }
        else
        {
            transform.position = curLinkObj.transform.position;
        }
  
    }

    public override void OnPointerClick(PointerEventData data)
    {
        ui.GetComponent<UI_Base>().SetCurObject(curLinkObj);
        ui.GetComponent<UI_Base>().OnEnable();
    }

    public override void SetLinkObj(GameObject obj)
    {
        base.SetLinkObj(obj);
        if(obj.GetComponent<BuildObj>().id == 305)
        {
            ui = ResourceManager.Instantiate("Prefabs/UI/UI_InteractionDoorInfo", mainT);
            ui.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            ui.transform.rotation = Quaternion.identity;
            ui.SetActive(false);
        }else if(obj.GetComponent<BuildObj>().id == 306)
        {
            ui = ResourceManager.Instantiate("Prefabs/UI/UI_InteractionInfo", mainT);
            ui.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            ui.transform.rotation = Quaternion.identity;
            ui.SetActive(false);
        }else if(obj.GetComponent<BuildObj>().id == 312)
        {
            ui = ResourceManager.Instantiate("Prefabs/UI/UI_InteractionLeverInfo", mainT);
            ui.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            ui.transform.rotation = Quaternion.identity;
            ui.SetActive(false);
        }
    }
    public override void OnPointerEnter(PointerEventData data)
    {
     
    }
    public override void OnPointerExit(PointerEventData data)
    {
        
    }

}
