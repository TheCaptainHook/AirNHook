using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Move_Indicator : MousePointerEntity
{
    [SerializeField] GameObject curLinkObj;
    WaitForSeconds waitForSeconds = new WaitForSeconds(0.1f);
    public bool isClicking;

    public Vector3 mousePosition;

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
        //Vector3 pot = MapEditor.Instance.placeMentSystem.gridPosition;
        StartCoroutine(Co_CheckClicking());
    }


    IEnumerator Co_CheckClicking()
    {
        while (isClicking)
        {
            if (Input.GetMouseButton(0))
            {
                isClicking = true;
            }
            else
            {
                isClicking = false;
                break;
            }
            yield return waitForSeconds;
        }
    }
}
