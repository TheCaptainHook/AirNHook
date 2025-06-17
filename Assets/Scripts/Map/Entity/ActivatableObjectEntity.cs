using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public enum INDICATOR
{
    NONE = 0,
    TEXT = 1,
    MARK = 2
}
public class ActivatableObjectEntity : BuildObj
{
    [CustomHeader("ActivatableObjectEntity")]
    [Header("Important condition")]
    public int activeRequirAmount;//문 활성화 조건
    [Header("Indicator Offset")]
    public INDICATOR indicator = 0;
    public Vector2 indicatorOffset_val_1;
    public Vector2 indicatorOffset_val_2;
    [Space(10)]
    private ButtonActivatableObjectStruct _buttonActivatedObjectStruct;
    public ButtonActivatableObjectStruct ButtonActivatedObjectStruct
    {
        get { return _buttonActivatedObjectStruct; }
        set
        {
            {
                _buttonActivatedObjectStruct = value;
                ObjectData = new ObjectData(_buttonActivatedObjectStruct.id, _buttonActivatedObjectStruct.position, _buttonActivatedObjectStruct.quaternion, _buttonActivatedObjectStruct.scale);
                activeRequirAmount = value.activeRequirAmount;
                transform.position = value.position;
                transform.rotation = value.quaternion;
                transform.localScale = value.scale;
                indicator = value.indicator;
            }
        }
    }
    //    [ReadOnly]
    //     public int curActiveBtn;//현재 활성화된 버튼 //todo 0426 
    //     public int CurActiveBtn
    //     {
    //         set { curActiveBtn += value;
    //             if (curActiveBtn == activeRequirAmount) { Activation(); }
    //             else { Deactivated(); }
    //         } }
    [ReadOnly]
    public int curActiveBtn;
    //----------------------------------------------------------------Refactoring 250124
    public void ApplyActive(int num,uint id=9999) //Maybe only Server
    {
        curActiveBtn += num;
        if(id != 9999 && ButtonActivatedObjectStruct.indicator == INDICATOR.MARK)
        {
            ApplyActive_Sync_var2(id,curActiveBtn,num);
        }
        else if (ButtonActivatedObjectStruct.indicator == INDICATOR.TEXT) ApplyActive_Sync_var1(curActiveBtn);


        if (curActiveBtn == activeRequirAmount)
        {
            Activation();
        }
        else
        {
            Deactivated();
        }
    }
    //----------------------------------------------------------------Refactoring 250124

    protected virtual void Activation() { }
    protected virtual void Deactivated() { }
    //    public virtual void ApplyActive(int num)
    //    {
    //     CurActiveBtn = num;
    //    }

    #region GET,SET
    public override T GetData<T>()
    {
        if (typeof(T) == typeof(ButtonActivatableObjectStruct))
        {
            return (T)(object)new ButtonActivatableObjectStruct(id, activeRequirAmount, transform.position, transform.rotation, transform.localScale,indicator);
        }

        return default(T);
    }
    protected Util util; 
    public override void SetData<T>(T data)
    {
        try
        {
            if (typeof(T) == typeof(ButtonActivatableObjectStruct))
            {
                util = new Util();
                ButtonActivatableObjectStruct objData = (ButtonActivatableObjectStruct)(object)data;
                ButtonActivatedObjectStruct = objData;

                //Create Indicator
                //if(ButtonActivatedObjectStruct.indicator == INDICATOR.TEXT) Create_Indicator_var_1();
                //else if(ButtonActivatedObjectStruct.indicator == INDICATOR.MARK)Create_Indicator_var_2();

                Create_Indicator_var_2();
                //Create Indicator
            }
        }
        catch(Exception e)
        {
            Debug.Log($"{e},{typeof(T)}");
        }

        // if (Application.isPlaying)
        // {
        //     await util.Delay(() => { CheckActiveRequirAmount(); });
        // }

    }

    #endregion

    public virtual void CheckActiveRequirAmount()
    {
        if (activeRequirAmount == curActiveBtn)
        {
            Activation();
        }
        else Deactivated();
    }
    public bool Check_Condition_RequirAmount()
    {
        return activeRequirAmount == curActiveBtn;
    }

    #region Indicator
    protected virtual void ApplyActive_Sync_var1(int curActiveAmount) //server
    {
        /// [TEXT]
        /// 1. 각 오브젝트 오버라이딩
        /// 2. server에서 버튼 누름 -> rpc로 적용(curActiveAmount 그대로 걍 전달)
        ///[MARK]
        ///
        //TEST
        indicator_var1.SetApplyActive(curActiveAmount);
    }
    /// </summary>
    /// <param name="id">Network ID</param>
    /// <param name="inc">[-1] : deactive, [1] : active </param>
    protected virtual void ApplyActive_Sync_var2(uint id, int curActiveBtn,int inc) //server
    {
        ///[MARK]
        /// 1. 각 오브젝트 오버라이딩
        /// 2. server에서 버튼 누르면 해당 오브젝트의 id와 현재 활성화된 갯수 전달
        /// 3. item 경로가 이미 생성되있으면 걍 LineOn, 아니면 경로 생성 후 Line On
        indicator_var2.SetApplyActive(id, curActiveBtn ,inc);
    }

    private ActivatableObject_Indicator_var1 indicator_var1;
    public void Create_Indicator_var_1()
    {
        //var indicator = Resources.Load<GameObject>(GlobalText.ACTIVATABLE_OBJECT_INDICATOR_VAR_1_Path);
        var indicator = ResourceManager.Load<GameObject>(GlobalText.ACTIVATABLE_OBJECT_INDICATOR_VAR_1_Path);
        indicator_var1 = Instantiate(indicator).GetComponent<ActivatableObject_Indicator_var1>();
        indicator_var1.Setting(this);

    }
    private ActivatableObject_Indicator_var2 indicator_var2;
    public void Create_Indicator_var_2()
    {
        var indicator = ResourceManager.Load<GameObject>(GlobalText.ACTIVATABLE_OBJECT_INDICATOR_VAR_2_Path);
        //var indicator = Resources.Load<GameObject>(GlobalText.ACTIVATABLE_OBJECT_INDICATOR_VAR_2_Path);
        indicator_var2 = Instantiate(indicator).GetComponent<ActivatableObject_Indicator_var2>();
        indicator_var2.Setting(this);

    }

    #endregion

}
