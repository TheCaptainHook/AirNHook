using System.Collections;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;

public class NpcEntity : BuildObj
{
    private AnimatorTriggerController animatorTriggerController;
    private AnimatorTriggerController ATC 
    {
        get
        {
            if(animatorTriggerController == null) animatorTriggerController = GetComponent<AnimatorTriggerController>();
            return animatorTriggerController;
        }
    }


    [SerializeField] private AnimationTriggerType animationType = AnimationTriggerType.Idle;


   public override T GetData<T>()  
    {
        if(typeof(T)==typeof(ObjectData)){
            return (T)(object)new ObjectData(id,transform.position,animationType);
        }

       return default(T);
    }
    public override void SetData<T>(T data)  
    {
        if(typeof(T) == typeof(ObjectData)){
            ObjectData objData = (ObjectData)(object)data;
            SetData(objData);

            if(Application.isPlaying)
            {
                ATC.PlayTrigger(animationType);
            }
        }

    }
    public override void SetData(ObjectData data)
    {
        transform.position = data.position;
        animationType = data.animationTriggerType;
    }


    //transform,AnimationTrigger



}
