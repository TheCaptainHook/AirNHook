
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

    protected ActivatableObject_Net_Entity Net;
    protected bool Net_Entity(out ActivatableObject_Net_Entity net)
    {
        net = Net ??= GetComponent<ActivatableObject_Net_Entity>();
        return net != null;
    }

    protected virtual void Awake()
    {
        Net = TryGetComponent(out ActivatableObject_Net_Entity entity) ? entity : null;
    }

    [ReadOnly]
    public int curActiveBtn;
    //----------------------------------------------------------------Refactoring 250124
    public void ApplyActive(int num,uint id=9999) //only Server
    {
        curActiveBtn += num;

        if (Net_Entity(out ActivatableObject_Net_Entity net) && ButtonActivatedObjectStruct.indicatorStruct.indicator != INDICATOR.NONE)
        {
            //------------------------------------NET
            if (id != 9999 && ButtonActivatedObjectStruct.indicatorStruct.indicator == INDICATOR.MARK)
            {
                net.ApplyActive_Sync_var2(id, curActiveBtn, num);
            }
            else if (ButtonActivatedObjectStruct.indicatorStruct.indicator == INDICATOR.TEXT) 
            {
                net.ApplyActive_Sync_var1(curActiveBtn);

                if (curActiveBtn == activeRequirAmount) Activation();
                else Deactivated();
            }
            else if (ButtonActivatedObjectStruct.indicatorStruct.indicator == INDICATOR.BOTH)
            {
                net.ApplyActive_Sync_var2(id, curActiveBtn, num);
                net.ApplyActive_Sync_var1(curActiveBtn);
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
            return (T)(object)new ButtonActivatableObjectStruct(id, activeRequirAmount, transform.position, transform.rotation, transform.localScale,indicatorStruct);
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
                if (Net_Entity(out ActivatableObject_Net_Entity net))
                {
                    //Create Indicato
                    // if (ButtonActivatedObjectStruct.indicator == INDICATOR.TEXT) Create_Indicator_var_1();
                    // else if (ButtonActivatedObjectStruct.indicator == INDICATOR.MARK) Create_Indicator_var_2();
                    //Create Indicator
                    net.Server_InitSync();
                }
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
    // protected virtual void ApplyActive_Sync_var1(int curActiveAmount) //server
    // {
    //     /// [TEXT]
    //     /// 1. 각 오브젝트 오버라이딩
    //     /// 2. server에서 버튼 누름 -> rpc로 적용(curActiveAmount 그대로 걍 전달)
    //     ///[MARK]
    //     ///
    //     //TEST
    //     indicator_var1.SetApplyActive(curActiveAmount);
    // }
    // /// </summary>
    // /// <param name="id">Network ID</param>
    // /// <param name="inc">[-1] : deactive, [1] : active </param>
    // protected virtual void ApplyActive_Sync_var2(uint id, int curActiveBtn,int inc) //server
    // {
    //     ///[MARK]
    //     /// 1. 각 오브젝트 오버라이딩
    //     /// 2. server에서 버튼 누르면 해당 오브젝트의 id와 현재 활성화된 갯수 전달
    //     /// 3. item 경로가 이미 생성되있으면 걍 LineOn, 아니면 경로 생성 후 Line On
    //     indicator_var2.SetApplyActive(id, curActiveBtn ,inc);
    // }

    // private ActivatableObject_Indicator_var1 indicator_var1;
    // public void Create_Indicator_var_1()
    // {
    //     //var indicator = Resources.Load<GameObject>(GlobalText.ACTIVATABLE_OBJECT_INDICATOR_VAR_1_Path);
    //     var indicator = ResourceManager.Load<GameObject>(GlobalText.ACTIVATABLE_OBJECT_INDICATOR_VAR_1_Path);
    //     indicator_var1 = Instantiate(indicator).GetComponent<ActivatableObject_Indicator_var1>();
    //     indicator_var1.Setting(this);

    // }
    // private ActivatableObject_Indicator_var2 indicator_var2;
    // public void Create_Indicator_var_2()
    // {
    //     var indicator = ResourceManager.Load<GameObject>(GlobalText.ACTIVATABLE_OBJECT_INDICATOR_VAR_2_Path);
    //     //var indicator = Resources.Load<GameObject>(GlobalText.ACTIVATABLE_OBJECT_INDICATOR_VAR_2_Path);
    //     indicator_var2 = Instantiate(indicator).GetComponent<ActivatableObject_Indicator_var2>();
    //     indicator_var2.Setting(this);

    // }

    #endregion

}
