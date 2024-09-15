using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonEntity : BuildObj
{

    [CustomHeader("ButtonEntity,Inner Target Object")]
    public List<GameObject> targetObjects;//TODO 0829 

    private ButtonObjectStruct buttonObjectData;
    public ButtonObjectStruct ButtonObjectData {
        get { return buttonObjectData; }
        set { buttonObjectData = value;
            ObjectData = new ObjectData(value.id, value.position, value.scale);
            transform.position = value.position;
            transform.localScale = value.scale;
            targetPosition = value.targetPositions;
            FindTargetObject();
        } }

    private List<Vector2> targetPosition; //TODO 0829


    #region  Debug
    private Transform debugModeTransform;
    private List<LineRenderer> lineRendererList;
    #endregion
    

    #region Main Logic
    protected virtual void Activation(){}
    protected virtual void Deactivated(){}
    protected virtual void PrograssButtonActivatedObject(bool onActivate)
    {
        foreach(GameObject obj in targetObjects){
           if(obj.TryGetComponent(out ActivatableObjectEntity component)){
            component.ApplyActive(onActivate ? 1 :-1);
           }
        }
    }

    #endregion



    #region  GET,SET

    public override void SetData<T>(T data)
    {
        try{
            if (typeof(T) == typeof(ButtonObjectStruct))
            {
                 ButtonObjectStruct buttonData = (ButtonObjectStruct)(object)data;
                 ButtonObjectData = buttonData;
            }
                
        }catch(Exception ex){
                Debug.Log($"{ex}");
        }
        
    }
    public override T GetData<T>()
    {
        if(typeof(T) == typeof(ButtonObjectStruct)){
            return (T)(object)new ButtonObjectStruct(id,GetTargetPositions(),transform.position,transform.localScale);
        }

        return default(T);
    }
#endregion

    #region Util

    private List<Vector2> GetTargetPositions(){
        List<Vector2> list = new();

        foreach(GameObject obj in targetObjects){
            list.Add(obj.transform.position);
        }

        return list;
    }

    public void FindTargetObject(){

        List<GameObject> objList = new();

        foreach(Vector2 vec in targetPosition){
           foreach(Transform obj in MapEditor.Instance.buttonActivatableObjectTransform){
            if(obj.TryGetComponent(out ActivatableObjectEntity component)){
                if(component.ButtonActivatedObjectStruct.position == vec){
                    objList.Add(obj.gameObject);
                }
            }
           }   
        }
        targetObjects = objList;
    }

    public void EditorMode_FindTargetObject(Transform tr){
        List<GameObject> objList = new();

        foreach(Vector2 vec in targetPosition){
           foreach(Transform obj in tr){
            if(obj.TryGetComponent(out ActivatableObjectEntity component)){
                if(component.ButtonActivatedObjectStruct.position == vec){
                    objList.Add(obj.gameObject);
                }
            }
           }   
        }
        targetObjects = objList;
    }

    #endregion

}


