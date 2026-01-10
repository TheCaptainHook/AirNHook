using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum BLINKBOX_COLOR
{
    RED,
    BLUE,
}

public class BlinkingBox : BuildObj
{
    [SerializeField] private GameObject _main;
    [SerializeField] private GameObject _dashedLine;
    [SerializeField] private BLINKBOX_COLOR _boxColor;

    private bool _isActive = false;

    #region  Clean  
    public override void Clean()
    {
        _isActive = false;
        Col.enabled = true;
        _main.SetActive(true);
        _dashedLine.SetActive(false);
    }

    #endregion


    public override void SetData(ObjectData data)
    {
        base.SetData(data);
        if(MapEditor.Instance == null) return;
        
        //MapEditor event subscribe
        switch (_boxColor)
        {
            case BLINKBOX_COLOR.RED:
                MapEditor.Instance.blinkingBoxEvent_Red += BlinkOnOff;
                break;
            case BLINKBOX_COLOR.BLUE:
                MapEditor.Instance.blinkingBoxEvent_Blue += BlinkOnOff;
                break;
        }
        //MapEditor event subscribe
    }



    public void BlinkOnOff()
    {
        _isActive = !_isActive;

        Col.enabled = _isActive;
        _main.SetActive(_isActive);
        _dashedLine.SetActive(!_isActive);
    }
   
}
