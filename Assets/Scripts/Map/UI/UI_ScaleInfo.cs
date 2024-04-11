using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_ScaleInfo : UI_Base
{
    GameObject curObject;
        
    [SerializeField] Button confirmeBtn;
    [SerializeField] Button closeBtn;

    [SerializeField] Slider sliderX;
    [SerializeField] Slider sliderY;

    public override void OnEnable(){}

    private void Awake()
    {
        confirmeBtn.onClick.AddListener(Confirm);
        closeBtn.onClick.AddListener(CloseUI);
    }

    public void ChangeSliderValueX()
    {
        curObject.transform.localScale = new Vector3(sliderX.value, curObject.transform.localScale.y, curObject.transform.localScale.z);
       
    }

    public void ChangeSliderValueY()
    {
        curObject.transform.localScale = new Vector3(curObject.transform.localScale.x, sliderY.value, curObject.transform.localScale.z);
    }

    public override void SetCurObject(GameObject obj)
    {
        base.SetCurObject(obj);
        curObject = obj;

        sliderX.minValue = curObject.transform.localScale.x;
        sliderX.maxValue = curObject.transform.localScale.x + 2;

        sliderY.minValue = curObject.transform.localScale.y;
        sliderY.maxValue = curObject.transform.localScale.y + 2;

        sliderX.value = sliderX.minValue;
    }



    private void Confirm()
    {
        MapEditor.Instance.placeMentSystem.onInteraction = true;

        Destroy(gameObject);
    }


    protected override void CloseUI()
    {
        MapEditor.Instance.placeMentSystem.onInteraction = true;
        Destroy(gameObject);
    }
}
