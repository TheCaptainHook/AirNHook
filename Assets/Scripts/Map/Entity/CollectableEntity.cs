using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableEntity : BuildObj
{


    public override T GetData<T>()
    {
          if(typeof(T)==typeof(CollectableObjectStruct)){
            return (T)(object)new CollectableObjectStruct(id,transform.position,transform.rotation);
        }

       return default(T);
    }

    public override void SetData<T>(T data)
    {
         if(typeof(T) == typeof(CollectableObjectStruct)){
            CollectableObjectStruct objData = (CollectableObjectStruct)(object)data;
            transform.position = objData.position;
            transform.rotation = objData.quaternion;
            id = objData.id;
        }
    }


    public void AddCollectable(int id){
        Managers.Data.saveData.AddCollectable(id);
    }


}
