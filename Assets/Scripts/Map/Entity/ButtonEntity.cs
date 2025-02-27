using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonEntity : BuildObj
{

    [CustomHeader("ButtonEntity, Target Object")]
     [Header(@"
    -------------ButtonEntity Field
     * ↓ can added this field.
        - ActivatableObjectEntity was inherited 
    ")]
    public List<GameObject> targetObjects;

    private ButtonObjectStruct buttonObjectData;
    public ButtonObjectStruct ButtonObjectData {
        get { return buttonObjectData; }
        set { buttonObjectData = value;
            ObjectData = new ObjectData(value.id, value.position, value.scale);
            transform.position = value.position;
            transform.rotation = value.quaternion;
            transform.localScale = value.scale;
            targetPosition = value.targetPositions;
            lightPosition = value.lightPositions;
        } }

    protected List<Vector2> targetPosition; //TODO 0829
    protected List<Vector2> lightPosition;

    #region  Debug
    private Transform debugModeTransform;
    private List<LineRenderer> lineRendererList;
    #endregion
    

    #region Main Logic
    protected bool onPrograss;
    protected bool onActive;
    protected virtual IEnumerator Co_Activation() {yield break; }
    protected virtual IEnumerator Co_Deactivated() {yield break; }
    protected virtual void Activation() {}
    protected virtual void Deactivated(){ }
    protected virtual void PrograssButtonActivatedObject(bool onActivate)
    {

        if(targetObjects == null) return;
        foreach(GameObject obj in targetObjects){
           if(obj.TryGetComponent(out ActivatableObjectEntity component)){
            component.ApplyActive(onActivate ? 1 :-1);
           }
        }
        foreach(GameObject obj in lightObjects)
        {
            if(obj.TryGetComponent(out IPowerConsumer component))
            {
                if(onActivate) component.hasPower = true;
                else component.hasPower = false;
            }
            
        }

    }

    #endregion

    #region IPowerConsumer, Light Object
    [Header(@"
    * ↓ Can use this field.
       - only Light Object
    ")]
    public List<GameObject> lightObjects;
    
    #endregion


    #region  GET,SET

    public override void SetData<T>(T data)
    {
        try{
            if (typeof(T) == typeof(ButtonObjectStruct))
            {
                 ButtonObjectStruct buttonData = (ButtonObjectStruct)(object)data;
                 ButtonObjectData = buttonData;
                 FindTargetObject();
            }
                
        }catch(Exception ex){
                Debug.Log($"name : {gameObject.name},{ex}");
        }
        
    }
    public override T GetData<T>()
    {
        if(typeof(T) == typeof(ButtonObjectStruct)){
            return (T)(object)new ButtonObjectStruct(id,GetTargetPositions(),transform.position,transform.rotation,transform.localScale);
        }

        return default(T);
    }
#endregion

    #region Util

    protected virtual List<Vector2> GetTargetPositions(){
        List<Vector2> list = new();

        foreach(GameObject obj in targetObjects){
            if (obj == null) continue;
            list.Add(ConvertPosition(obj.transform.position));
        }

        return list;
    }
    protected virtual List<Vector2> GetLightPositions()
    {
        List<Vector2> list = new();
        foreach(GameObject obj in lightObjects)
        {
            if (obj == null) continue;
            if(obj.TryGetComponent(out IPowerConsumer component))
            {
                list.Add(component.GetTransformPosition());
            }
            
        }
        return list;
    }

    private Vector3 ConvertPosition(Vector3 vec)
    {
        return new Vector3(
            Mathf.Round(vec.x * 100) / 100, 
            Mathf.Round(vec.y * 100) / 100, 
            Mathf.Round(vec.z * 100) / 100
        );

    }

    public virtual void FindTargetObject(){

        if(!Application.isPlaying) return;
        List<GameObject> objList = new();

        foreach(Vector2 vec in targetPosition)
        {
           foreach(Transform obj in MapEditor.Instance.buttonActivatableObjectTransform){
                if(obj.TryGetComponent(out ActivatableObjectEntity component))
                {
                            if(CompareVec(component.ButtonActivatedObjectStruct.position,vec)){
                                objList.Add(obj.gameObject);
                            }
                }
            }    
        }
        targetObjects = objList;
    }
    public virtual void FindLightObject()
    {
        if(!Application.isPlaying) return;
        List<GameObject> list = new();

        OtherContainer otherContainer = MapEditor.Instance.otherContainer.GetComponent<OtherContainer>();

        foreach(Vector2 vec in ButtonObjectData.lightPositions)
        {
            otherContainer.GetCompareVec(vec,ref list);
            //otherObject vec 전달 -> group transform 순회 같은거 있는지 확인 -> 있으면 해당 IPowerConsumer 반환
        }
        lightObjects = list;
    }
    protected bool CompareVec(Vector3 p1,Vector3 p2){
        bool x = Mathf.Approximately(p1.x,p2.x);
        bool y = Mathf.Approximately(p1.y,p2.y);

        return x&&y;
    }
    // public override void Editor_Setting(Transform transform)
    // {
    //      List<GameObject> objList = new();

    //     foreach(Vector2 vec in targetPosition){
    //        foreach(Transform obj in transform){
    //         if(obj.TryGetComponent(out ActivatableObjectEntity component)){
    //             if(component.ButtonActivatedObjectStruct.position == vec){
    //                 objList.Add(obj.gameObject);
    //             }
    //         }
    //        }   
    //     }
    //     targetObjects = objList;
    // }

    public override void Editor_Setting(MapEditor mapEditor)
    {
         List<GameObject> objList = new();

        foreach(Vector2 vec in targetPosition){
           foreach(Transform obj in mapEditor.buttonActivatableObjectTransform){
            if(obj.TryGetComponent(out ActivatableObjectEntity component)){
                // if(component.ButtonActivatedObjectStruct.position == vec){
                //     objList.Add(obj.gameObject);
                // }
                if(CompareVec(component.ButtonActivatedObjectStruct.position,vec)){
                    objList.Add(obj.gameObject);
                }
            }
           }   
        }
        targetObjects = objList;

         List<GameObject> list = new();

        OtherContainer otherContainer = mapEditor.otherContainer.GetComponent<OtherContainer>();

        foreach(Vector2 vec in ButtonObjectData.lightPositions)
        {
            otherContainer.GetCompareVec(vec,ref list);
            //otherObject vec 전달 -> group transform 순회 같은거 있는지 확인 -> 있으면 해당 IPowerConsumer 반환
        }
        lightObjects = list;
    }

    #endregion

}


