using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class ButtonActivatedDoor : ActivatableObjectEntity
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
            //curActiveBtn = Mathf.Clamp(curActiveBtn, 0, curLinkBtn);
            if (curActiveBtn == activeRequirAmount) { Activation(); }
            else { Deactivated(); }
        } }

    //public List<Vector2> buttonActivatedBtnList = new List<Vector2>();
    ////todo 0416
    //public List<Vector2> leverBodyPotiionList;
    //todo 0416
    private ButtonActivatableObjectStruct _buttonActivatedObjectStruct;
    public ButtonActivatableObjectStruct ButtonActivatedObjectStruct {
        get { return _buttonActivatedObjectStruct; }
        set { { _buttonActivatedObjectStruct = value;
                ObjectData = new ObjectData(_buttonActivatedObjectStruct.id, _buttonActivatedObjectStruct.position, _buttonActivatedObjectStruct.quaternion, _buttonActivatedObjectStruct.scale);
                activeRequirAmount = value.activeRequirAmount;
                transform.position = value.position;
                transform.rotation = value.quaternion;
                transform.localScale = value.scale;

            } }
    }
    public bool onOpen;


    public bool onPrograss;

    private void Awake()
    {
        _animator = GetComponent<NetworkAnimator>();
    }



    #region  GET,SET
    public override T GetData<T>()
    {
         if(typeof(T) == typeof(ButtonActivatableObjectStruct)){
            return (T)(object)new ButtonActivatableObjectStruct(id,activeRequirAmount,transform.position,transform.rotation,transform.localScale);
        }

        return default(T);
    }

    public override void SetData<T>(T data)
    {
        try{
            if (typeof(T) == typeof(ButtonActivatableObjectStruct))
        {
         ButtonActivatableObjectStruct objData = (ButtonActivatableObjectStruct)(object)data;
         ButtonActivatedObjectStruct = objData;

        }
        }catch(Exception ex){
                Debug.Log($"{ex}\n{typeof(T)}");
        }
        
    }
    #endregion

    public void ApplyActive(int num){
        CurActiveBtn = num;
    }

    protected override void Activation()
    {
        if (onPrograss) return;
        if (onOpen) return;
        onOpen = true;
        _collider.enabled = false;
        _animator.SetTrigger(UnlockTrigger);
        Debug.Log("OpenOpen");
        //StartCoroutine(Co_Activation());
    }


    protected override void Deactivated()
    {
          //if (onPrograss) return;
        if (!onOpen) return;

        onOpen = false;
        _collider.enabled = true;
        _animator.SetTrigger(LockTrigger);

        Debug.Log("Deactivate");
        //StartCoroutine(Co_Deactivated());
    }

    protected override void PrograssButtonActivatedObject(int num)
    {
        
    }

    public void CheckActiveRequirAmount()
    {
         if (activeRequirAmount == curActiveBtn) { Activation(); }
    }


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





