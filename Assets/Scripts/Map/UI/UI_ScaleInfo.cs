using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_ScaleInfo : UI_Base
{
    GameObject curObject;
    Vector3 orgScale;

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
        //float x = Mathf.Clamp( curObject.transform.localScale.x + sliderX.value, 0.5f, 2f);
        curObject.transform.localScale = new Vector3(sliderX.value, curObject.transform.localScale.y, curObject.transform.localScale.z);
       
    }

    public void ChangeSliderValueY()
    {
        //float y = Mathf.Clamp(curObject.transform.localScale.y + sliderX.value, 0.5f, 2f);
        curObject.transform.localScale = new Vector3(curObject.transform.localScale.x, sliderY.value, curObject.transform.localScale.z);
    }

    public override void SetCurObject(GameObject obj)
    {
        base.SetCurObject(obj);
        curObject = obj;
        orgScale = curObject.transform.localScale;
        Debug.Log(curObject.transform.localScale);

        sliderX.value = curObject.transform.localScale.x;
        sliderX.minValue = .5f;
        sliderX.maxValue = 3;

        sliderY.value = curObject.transform.localScale.y;
        sliderY.minValue = .5f;
        sliderY.maxValue = 3;
        
    }



    private void Confirm()
    {
        MapEditor.Instance.placeMentSystem.onInteraction = true;
        Destroy(gameObject);
    }


    protected override void CloseUI()
    {
        MapEditor.Instance.placeMentSystem.onInteraction = true;
        curObject.transform.localScale = orgScale;
        Destroy(gameObject);
    }
}
