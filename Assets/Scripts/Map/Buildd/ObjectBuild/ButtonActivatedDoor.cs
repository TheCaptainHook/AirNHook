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
    [SerializeField] private BoxCollider2D _collider;
    private Animator _animator;
    
    #region StringCache
    private static readonly int UnlockTrigger = Animator.StringToHash("UnlockTrigger");
    private static readonly int LockTrigger = Animator.StringToHash("LockTrigger");
    #endregion

    [HideInInspector] public int curLinkBtn;//현재 링크된 버튼 
    [HideInInspector] private int curActiveBtn;//현재 활성화된 버튼
    public int activeRequirAmount;//문 활성화 조건
    public int CurActiveBtn
    {
        set { curActiveBtn += value; Debug.Log(curLinkBtn) ;
            curActiveBtn = Mathf.Clamp(curActiveBtn, 0, curLinkBtn);
            if (curActiveBtn == activeRequirAmount) { if(!onOpen)Activation(); }
            else { if(onOpen)Deactivated(); }
        } }
    public List<Vector2> buttonActivatedBtnList = new List<Vector2>();
    //todo 0416
    public List<Vector2> leverBodyPotiionList;
    //todo 0416
    private ButtonActivatedDoorStruct _buttonActivatedDoorStruct;
    public ButtonActivatedDoorStruct ButtonActivatedDoorStruct { 
        get { return _buttonActivatedDoorStruct; }
        set { {  _buttonActivatedDoorStruct = value; 
                ObjectData = new ObjectData(_buttonActivatedDoorStruct.id, _buttonActivatedDoorStruct.position, _buttonActivatedDoorStruct.quaternion,_buttonActivatedDoorStruct.scale);
                linkId = _buttonActivatedDoorStruct.linkId;
                activeRequirAmount = value.activeRequirAmount;
                buttonActivatedBtnList = value.buttonActivatePositionList;
                leverBodyPotiionList = value.leverPositionList;
                transform.position = value.position;
                transform.rotation = value.quaternion;
                transform.localScale = value.scale;
                curLinkBtn = buttonActivatedBtnList.Count;
            } }
    }
    public bool onOpen;


    public ButtonActivatedDoorStruct GetButtonActivatedDoorStruct()
    {
        return new ButtonActivatedDoorStruct(id, linkId, activeRequirAmount,transform.position, buttonActivatedBtnList, leverBodyPotiionList, transform.rotation, transform.localScale);
    }

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void LateUpdate()
    {
        CheckActiveRequirAmount();
    }


    void Activation()
    {
        if (onOpen) return;
        onOpen = true;
        _collider.enabled = false;
        _animator.SetTrigger(UnlockTrigger);
    }
    void Deactivated()
    {
        if (!onOpen) return;
        onOpen = false;
        _collider.enabled = true;
        _animator.SetTrigger(LockTrigger);
    }

    public void CheckActiveRequirAmount()
    {
        if (onOpen) return;
        if (activeRequirAmount == curActiveBtn) Activation();
    }


}
