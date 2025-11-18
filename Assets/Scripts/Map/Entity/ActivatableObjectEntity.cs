
using System;
using Mirror;
using UnityEngine;
public enum INDICATOR
{
    NONE = 0,
    TEXT = 1,
    MARK = 2,
    BOTH = 3
}
public class ActivatableObjectEntity : BuildObj
{
    [CustomHeader("ActivatableObjectEntity")]
    [Header("Important condition")]
    public int activeRequirAmount;//문 활성화 조건
    [Header("Indicator Offset")]
    public INDICATOR indicator = 0;
    [HideInInspector]
    public Vector2 indicatorOffset_val_1;
    [HideInInspector]
    public Vector2 indicatorOffset_val_2;
    [HideInInspector]
    public bool isHorizontal;
    [HideInInspector]
    public IndicatorStruct indicatorStruct;

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
                indicator = value.indicatorStruct.indicator;
                indicatorOffset_val_1 = value.indicatorStruct.indicator_1_position;
                indicatorOffset_val_2 = value.indicatorStruct.indicator_2_position;
                isHorizontal = value.indicatorStruct.isHorizontal;
                indicatorStruct = value.indicatorStruct;
            }
        }
    }

    protected ActivatableObject_Net_Entity net;
    protected ActivatableObject_Net_Entity Net { get{ net ??= GetComponent<ActivatableObject_Net_Entity>();  return net; }}
    
    protected Animator _animator;
    protected Animator Animator {get{_animator ??= GetComponent<Animator>(); return _animator;}}
    

    protected virtual void Awake()
    {
        
    }

    [ReadOnly]
    public int curActiveBtn;
    //----------------------------------------------------------------Refactoring 250124
    public void ApplyActive(int num, uint id = 9999) //server
    {
        curActiveBtn += num;

        if (ButtonActivatedObjectStruct.indicatorStruct.indicator != INDICATOR.NONE)
        {
            //------------------------------------NET
            if (id != 9999 && ButtonActivatedObjectStruct.indicatorStruct.indicator == INDICATOR.MARK)
            {
                Net.ApplyActive_Sync_var2(id, curActiveBtn, num);
            }
            else if (ButtonActivatedObjectStruct.indicatorStruct.indicator == INDICATOR.TEXT)
            {
                Net.ApplyActive_Sync_var1(curActiveBtn);

                if (curActiveBtn == activeRequirAmount) Activation();
                else Deactivated();
            }
            else if (ButtonActivatedObjectStruct.indicatorStruct.indicator == INDICATOR.BOTH)
            {
                Net.ApplyActive_Sync_var2(id, curActiveBtn, num);
                Net.ApplyActive_Sync_var1(curActiveBtn);
            }
            //------------------------------------NET
            return;
        }


        if (curActiveBtn == activeRequirAmount) Activation();
        else Deactivated();

    }
    //----------------------------------------------------------------Refactoring 250124

    public virtual void Activation() { }
    public virtual void Deactivated() { }

    #region GET,SET
    public override T GetData<T>()
    {
        if (typeof(T) == typeof(ButtonActivatableObjectStruct))
        {
            return (T)(object)new ButtonActivatableObjectStruct(id,transform.position, transform.rotation, transform.localScale,activeRequirAmount,indicatorStruct);
        }

        return default(T);
    }
    public void GetIndicatorStruct()
    {
        indicatorStruct = new IndicatorStruct(indicator, indicatorOffset_val_1, indicatorOffset_val_2, isHorizontal);
    }
    protected Util util;
    public override async void SetData<T>(T data)
    {
        if (typeof(T) == typeof(ButtonActivatableObjectStruct))
        {
            util = new Util();
            ButtonActivatableObjectStruct objData = (ButtonActivatableObjectStruct)(object)data;
            ButtonActivatedObjectStruct = objData;

            AdditionalInspectorConfig();

            if (Application.isPlaying)
            {
                Net.Server_InitSync();
                await util.Delay(() => { CheckActiveRequirAmount(); });
            }
        }
    }




    protected virtual void AdditionalInspectorConfig()
    {

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

    #region Indicator
    public void PathChacking(GameObject target) //Server, call ButtonEntity
    {
        var netId = target.TryGetComponent(out NetworkIdentity identity) ? identity.netId : 9999;
        if (netId == 9999) return;

        Debug.Log("[2] Start Path Chaking -> RPC ");
        Net.Server_Indicator_var_2_PathChacking(netId);
    }

    #endregion

    #region  Clean
    public override void Clean()
    {
        if(Animator != null)
        {
            Animator.Rebind();
            Animator.Update(0);
        }
        
        curActiveBtn = 0;
        activeRequirAmount = 0;
        
        Clean_Value();
        Net.Clean();
    }


        
    protected virtual void Clean_Value()
    {
        
    }
    #endregion

}
