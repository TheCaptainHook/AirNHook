using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public void ApplyActive(int num) //Maybe only Server
    {
        curActiveBtn += num;
        ApplyActive_Sync(curActiveBtn);

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
            return (T)(object)new ButtonActivatableObjectStruct(id, activeRequirAmount, transform.position, transform.rotation, transform.localScale);
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
                Create_Indicator_var_1();
                //Create Indicator
            }
        }
        catch
        {
            Debug.Log($"ERROR,{typeof(T)}");
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
    protected virtual void ApplyActive_Sync(int curActiveAmount)
    {
        //TEST
        indicator_var1.SetApplyActive(curActiveAmount);
    }
    
    private ActivatableObject_Indicator_var1 indicator_var1;
    public void Create_Indicator_var_1()
    {
        var indicator = Resources.Load<GameObject>(GlobalText.ACTIVATABLE_OBJECT_INDICATOR_VAR_1_Path);
        indicator_var1 = Instantiate(indicator).GetComponent<ActivatableObject_Indicator_var1>();
        indicator_var1.Setting(this);

    }
    #endregion
   
}
