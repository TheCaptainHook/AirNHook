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
        switch (_boxColor)
        {
            case BLINKBOX_COLOR.RED:
                Off();
                break;
            case BLINKBOX_COLOR.BLUE:
                On();
                break;
        }
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
                Off();
            
                break;
            case BLINKBOX_COLOR.BLUE:
                MapEditor.Instance.blinkingBoxEvent_Blue += BlinkOnOff;
                On();
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



    private void On()
    {
        _isActive = true;
        Col.enabled = true;
        _main.SetActive(true);
        _dashedLine.SetActive(false);
    }
    private void Off()
    {
        _isActive = false;
        Col.enabled = false;
        _main.SetActive(false);
        _dashedLine.SetActive(true);
    }
   
}
