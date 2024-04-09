using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Move_Indicator : Indicator
{
 
    private void Update()
    {
        //mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //Vector3 target = new Vector3(mousePosition.x, mousePosition.y, 1);
        if (isClicking)
        {

            curLinkObj.transform.position = MapEditor.Instance.placeMentSystem.gridPosition;
        }
    }


    public override void OnPointerDown(PointerEventData data)
    {
        isClicking = true;
        MapEditor.Instance.placeMentSystem.objectModeClient.Move();
        StartCoroutine(Co_CheckClicking());
    }

}
