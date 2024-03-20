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
    BoxCollider2D _collider;

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
                buttonActivatedBtnList = value.buttonActivatePositionList;
                transform.position = value.position;
                transform.rotation = value.quaternion;
            } }
    }
    public bool onOpen;

   
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        _collider = GetComponent<BoxCollider2D>();
        orgColor = spriteRenderer.material.color;
    }
    void Activation()
    {
        onOpen = true;
        Color color = orgColor;
        color.a = 0;
        spriteRenderer.color = color;
        _collider.enabled = false;
    }
    void Deactivated()
    {
        onOpen = false;
        spriteRenderer.color = orgColor;
        _collider.enabled = true;
    }


}
