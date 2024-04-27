using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Rotate_Indicator : Indicator
{
    Vector3 target;
    Vector3 startPoint;

    CanvasGroup canvasGroup;
    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    private void Update()
    {
        if (linked && curLinkObj == null)
        {
            Destroy(gameObject);
        }
        else
        {
            if (curLinkObj.transform.position != transform.position)
            {
                transform.position = curLinkObj.transform.position;
            }
           
            mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            target = new Vector3(mousePosition.x, mousePosition.y, 1);

            if (isClicking)
            {
                Vector3 mouseDelta = target - startPoint;
                curLinkObj.transform.rotation = Quaternion.Euler(0, 0, curLinkObj.transform.rotation.eulerAngles.z + mouseDelta.x * 0.1f);
            }
        }

      
    }

    public override void OnPointerDown(PointerEventData data)
    {
        startPoint = target;
        isClicking = true;
        canvasGroup.alpha = 0.5f;
        MapEditor.Instance.placeMentSystem.objectModeClient.Rotaion();
    }

    public override void OnPointerUp(PointerEventData data)
    {
        isClicking = false;
        MapEditor.Instance.placeMentSystem.CreateIndicator(ModeState.Obj_Rotation);
    }



    public override void SetLinkObj(GameObject obj)
    {
        base.SetLinkObj(obj);
    }
}
