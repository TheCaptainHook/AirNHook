using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ButtonActivated 가 1개 이상 존재해야함
public class ButtonActivatedDoor : BuildBase
{
    [Header("Components")]
    SpriteRenderer spriteRenderer;

    public int id;
    public int curLinkBtn;
    public int curActiveBtn;
    public int CurActiveBtn { set { curActiveBtn += value;
            if (curActiveBtn == curLinkBtn) { Activation(); }
            else { Deactivated(); }
        } }

    
    public List<Vector2> buttonActivatedBtnList = new List<Vector2>();


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
