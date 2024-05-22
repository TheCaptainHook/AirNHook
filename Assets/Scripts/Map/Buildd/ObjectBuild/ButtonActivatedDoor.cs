using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

//ButtonActivated 가 1개 이상 존재해야함
public class ButtonActivatedDoor : BuildBase
{
    public int linkId;

    [Header("Components")]
    [SerializeField] private BoxCollider2D _collider;
    private NetworkAnimator _animator;
    
    #region StringCache
    private static readonly int UnlockTrigger = Animator.StringToHash("UnlockTrigger");
    private static readonly int LockTrigger = Animator.StringToHash("LockTrigger");
    #endregion

    public int curLinkBtn;//현재 링크된 버튼 
    public int curActiveBtn;//현재 활성화된 버튼 //todo 0426 
    public int activeRequirAmount;//문 활성화 조건
    /// <summary>
    /// 수정
    /// </summary>
    public int CurActiveBtn
    {
        set { curActiveBtn += value; Debug.Log($"{curActiveBtn}");
            curActiveBtn = Mathf.Clamp(curActiveBtn, 0, curLinkBtn);
            if (curActiveBtn == activeRequirAmount) { Activation(); }
            else { Deactivated(); }
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
                transform.position = value.position;
                transform.rotation = value.quaternion;
                transform.localScale = value.scale;
                curLinkBtn = buttonActivatedBtnList.Count;
            } }
    }
    public bool onOpen;


    public bool onPrograss;

    public ButtonActivatedDoorStruct GetButtonActivatedDoorStruct()
    {
        return new ButtonActivatedDoorStruct(id, linkId, activeRequirAmount,transform.position, transform.rotation, transform.localScale);
    }

    private void Awake()
    {
        _animator = GetComponent<NetworkAnimator>();
    }



    void Activation()
    {
        //if (onPrograss) return;
        if (onOpen) return;
        onOpen = true;
        _collider.enabled = false;
        _animator.SetTrigger(UnlockTrigger);
        Debug.Log("OpenOpen");
        //StartCoroutine(Co_Activation());
    }
    void Deactivated()
    {
        //if (onPrograss) return;
        if (!onOpen) return;

        onOpen = false;
        _collider.enabled = true;
        _animator.SetTrigger(LockTrigger);

        Debug.Log("Deactivate");
        //StartCoroutine(Co_Deactivated());
    }

    //public void CheckActiveRequirAmount() todo 0522
    //{
    //    //if (onPrograss || onOpen) return;
    //    if (activeRequirAmount == curActiveBtn) { Activation(); Debug.Log("CheckActiveRequirAmount"); _animator.SetTrigger(UnlockTrigger);
    //    }
    //    else
    //    {
    //        Deactivated();
    //        _animator.SetTrigger(LockTrigger);
    //    }
    //}


    IEnumerator Co_Activation()
    {
        onPrograss = true;

        onOpen = true;
        _collider.enabled = false;
        _animator.SetTrigger(UnlockTrigger);
        
        Debug.Log("OpenOpen");
        yield return new WaitForSeconds(0.5f);
        _animator.SetTrigger(UnlockTrigger);
        onPrograss = false;
    }


    IEnumerator Co_Deactivated()
    {
        onPrograss = true;

        onOpen = false;
        _collider.enabled = true;
        _animator.SetTrigger(LockTrigger);
        yield return new WaitForSeconds(0.5f);
        _animator.SetTrigger(LockTrigger);
        onPrograss = false;
    }

}
