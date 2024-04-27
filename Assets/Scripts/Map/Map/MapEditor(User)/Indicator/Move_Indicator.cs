using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Move_Indicator : Indicator
{

    private void Awake()
    {
        spriteRenderers = transform.GetComponentsInChildren<SpriteRenderer>();
        orgColor = spriteRenderers[0].color;
    }

    private void Update()
    {
        //mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //Vector3 target = new Vector3(mousePosition.x, mousePosition.y, 1);
        if (linked && curLinkObj == null)
        {
            Destroy(gameObject);
        }
        else
        {
            if (transform.position != curLinkObj.transform.position)
            {
                transform.position = curLinkObj.transform.position;
            }

            if (isClicking && MapEditor.Instance.placeMentSystem.CheckMousePosition_InGridBoundary())
            {
                curLinkObj.transform.position = MapEditor.Instance.placeMentSystem.mousePosition;
            }
        }

        
    }


    public override void OnPointerDown(PointerEventData data)
    {
        isClicking = true;
        SpriteAlphaChange(0);
        MapEditor.Instance.placeMentSystem.objectModeClient.Move();
    }

    public override void OnPointerUp(PointerEventData data)
    {
        isClicking = false;
        MapEditor.Instance.placeMentSystem.CreateIndicator(ModeState.Obj_Move);
    }



}
