using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Move_Indicator : Indicator
{

    private Vector3 beforePosition;

    [SerializeField] Button btn;
    CanvasGroup canvasGroup;
    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();

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
            //if (onEnterPointer)
            //{
            //    Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //    mousePosition = new Vector3(mousePosition.x, mousePosition.y, 0);
            //    transform.position = mousePosition;
            //}
            if (transform.position != curLinkObj.transform.position)
            {
                transform.position = curLinkObj.transform.position;
            }

            if (isClicking && MapEditor.Instance.placeMentSystem.CheckMousePosition_InGridBoundary())
            {
                //Vector3 addPot = curLinkObj.transform.position + (transform.position - beforePosition);
                //Debug.Log($"{MapEditor.Instance.placeMentSystem.CurbuildObject.transform.position},{MapEditor.Instance.placeMentSystem.CurIndicatior.transform.position}");

                curLinkObj.transform.position = MapEditor.Instance.placeMentSystem.mousePosition;
            }
        }

        
    }

    //public override void Active()
    //{
    //    onEnterPointer = true;
    //}

    //public override void Execute()
    //{
    //    beforePosition = transform.position;
    //    isClicking = true;
    //    SpriteAlphaChange(0);
    //    MapEditor.Instance.placeMentSystem.objectModeClient.Move();
    //}

    //public override void DeActive()
    //{
    //    onEnterPointer = false;
    //    isClicking = false;
    //    MapEditor.Instance.placeMentSystem.CreateIndicator(ModeState.Obj_Move);
    //}
    //public override void Cancel()
    //{
    //    //todo 0427
    //    isClicking = false;
    //    //MapEditor.Instance.placeMentSystem.CreateIndicator(ModeState.Obj_Move);
    //}

    public override void OnPointerDown(PointerEventData data)
    {
        //todo 0427
        beforePosition = transform.position;
        Debug.Log(beforePosition);
        isClicking = true;
        //SpriteAlphaChange(0);
        canvasGroup.alpha = 0.5f;
        MapEditor.Instance.placeMentSystem.objectModeClient.Move();
    }

    public override void OnPointerUp(PointerEventData data)
    {
        isClicking = false;
        MapEditor.Instance.placeMentSystem.CreateIndicator(ModeState.Obj_Move);
    }



}
