using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ButtonActivated 가 1개 이상 존재해야함
public class ButtonActivatedDoor : BuildBase
{
    public int linkId;

    [Header("Components")]
    SpriteRenderer spriteRenderer;
  
    public int curLinkBtn;
    public int curActiveBtn;
    public int CurActiveBtn { set { curActiveBtn += value;
            if (curActiveBtn == curLinkBtn) { Activation(); }
            else { Deactivated(); }
        } }
    public List<Vector2> buttonActivatedBtnList = new List<Vector2>();

    private ButtonActivatedDoorStruct _buttonActivatedDoorStruct;
    public ButtonActivatedDoorStruct ButtonActivatedDoorStruct { 
        get { return _buttonActivatedDoorStruct; }
        set { {  _buttonActivatedDoorStruct = value; 
                ObjectData = new ObjectData(_buttonActivatedDoorStruct.id, _buttonActivatedDoorStruct.position, _buttonActivatedDoorStruct.quaternion);
                linkId = _buttonActivatedDoorStruct.linkId;
                buttonActivatedBtnList = _buttonActivatedDoorStruct.buttonActivatePositionList;
            } }
    }

   
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        orgColor = spriteRenderer.material.color;
    }
    void Activation()
    {
        spriteRenderer.material.color = Color.green;
    }
    void Deactivated()
    {
        spriteRenderer.material.color = orgColor;
    }
}
