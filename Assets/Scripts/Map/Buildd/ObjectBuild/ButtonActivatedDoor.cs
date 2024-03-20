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

    [HideInInspector] public int curLinkBtn;//현재 링크된 버튼 
    [HideInInspector] public int curActiveBtn;//현재 활성화된 버튼
    public int activeRequirAmount;//문 활성화 조건
    public int CurActiveBtn { set { curActiveBtn += value;
            if (curActiveBtn >= activeRequirAmount) { Activation(); }
            else { Deactivated(); }
        } }
    public List<Vector2> buttonActivatedBtnList = new List<Vector2>();

    private ButtonActivatedDoorStruct _buttonActivatedDoorStruct;
    public ButtonActivatedDoorStruct ButtonActivatedDoorStruct { 
        get { return _buttonActivatedDoorStruct; }
        set { {  _buttonActivatedDoorStruct = value; 
                ObjectData = new ObjectData(_buttonActivatedDoorStruct.id, _buttonActivatedDoorStruct.position, _buttonActivatedDoorStruct.quaternion,_buttonActivatedDoorStruct.scale);
                linkId = _buttonActivatedDoorStruct.linkId;
                activeRequirAmount = value.activeRequirAmount;
                buttonActivatedBtnList = value.buttonActivatePositionList;
                transform.position = value.position;
                transform.rotation = value.quaternion;
                transform.localScale = value.scale;
            } }
    }
    public bool onOpen;


    public ButtonActivatedDoorStruct GetButtonActivatedDoorStruct()
    {
        return new ButtonActivatedDoorStruct(id, linkId, activeRequirAmount,transform.position, buttonActivatedBtnList, transform.rotation, transform.localScale);
    }

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
