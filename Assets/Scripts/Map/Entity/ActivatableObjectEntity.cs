using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivatableObjectEntity : BuildObj
{
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
   
   protected virtual void Activation(){}
   protected virtual void Deactivated(){}
   public virtual void ApplyActive(int num){}

   #region GET,SET
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
        }catch{
                Debug.Log($"ERROR,{typeof(T)}");
        }
        
    }
    #endregion
}
