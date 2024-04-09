using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Additional_Indicator : Indicator
{

    GameObject ui;
    private void Update()
    {
        if(curLinkObj != null)
        {
            transform.position = curLinkObj.transform.position;
        }
    }

    public override void OnPointerClick(PointerEventData data)
    {
        Debug.Log("CClick");
    }

    public override void SetLinkObj(GameObject obj)
    {
        base.SetLinkObj(obj);
    }
}
