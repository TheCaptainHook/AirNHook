using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivatableObjectEntity : BuildObj
{
    [CustomHeader("ActivatableObjectEntity")]
   public int activeRequirAmount;//문 활성화 조건
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
    public void ApplyActive(int num)
    {
        curActiveBtn += num;
        if(curActiveBtn == activeRequirAmount)
        {
            Activation();
        }else{
            Deactivated();
        }
    }
    //----------------------------------------------------------------Refactoring 250124

   protected virtual void Activation(){}
   protected virtual void Deactivated(){}
//    public virtual void ApplyActive(int num)
//    {
//     CurActiveBtn = num;
//    }

   #region GET,SET
    public override T GetData<T>()
    {
         if(typeof(T) == typeof(ButtonActivatableObjectStruct)){
            return (T)(object)new ButtonActivatableObjectStruct(id,activeRequirAmount,transform.position,transform.rotation,transform.localScale);
        }

        return default(T);
    }

     public override async void SetData<T>(T data)
    {
        try{
            if (typeof(T) == typeof(ButtonActivatableObjectStruct))
        {
         ButtonActivatableObjectStruct objData = (ButtonActivatableObjectStruct)(object)data;
         ButtonActivatedObjectStruct = objData;

        }
        }catch{
                Debug.Log($"ERROR,{typeof(T)}");
        }
        
        if(Application.isPlaying){
            Util util  = new Util();
            await util.Delay(()=>{CheckActiveRequirAmount();});
        }
    
    }

#endregion

    public virtual void CheckActiveRequirAmount()
    {
        if (activeRequirAmount == curActiveBtn) { Activation(); }
        else Deactivated();
    }
}
