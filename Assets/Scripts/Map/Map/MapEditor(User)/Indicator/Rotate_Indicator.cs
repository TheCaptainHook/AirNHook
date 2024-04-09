using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Rotate_Indicator : Indicator
{
    float height = 0;
    Vector3 target;
    Vector3 startPoint;

    private void Update()
    {
        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        target = new Vector3(mousePosition.x, mousePosition.y, 1);
        transform.position = curLinkObj.transform.position + new Vector3(0, height, 0);

        if (isClicking)
        {
            Vector3 mouseDelta = target - startPoint;
            curLinkObj.transform.rotation = Quaternion.Euler(0, 0, curLinkObj.transform.rotation.eulerAngles.z + mouseDelta.x * 0.1f);
        }
    }

    public override void OnPointerDown(PointerEventData data)
    {
        startPoint = target;
        isClicking = true;
        MapEditor.Instance.placeMentSystem.objectModeClient.Rotaion();
        StartCoroutine(Co_CheckClicking());
    }

    private void Tracking()
    {
        Collider2D[] cols = curLinkObj.GetComponentsInChildren<Collider2D>();

        foreach (Collider2D co in cols)
        {
            height = Mathf.Max(height, co.bounds.size.y);
        }
    }

    public override void SetLinkObj(GameObject obj)
    {
        base.SetLinkObj(obj);
        Tracking();
    }
}
