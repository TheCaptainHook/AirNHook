using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonEntity : BuildObj
{
    
    [SerializeField] List<GameObject> targetObjects;//TODO 0829

    private ButtonObjectStruct buttonObjectData;
    public ButtonObjectStruct ButtonObjectData {
        get { return buttonObjectData; }
        set { buttonObjectData = value;
            ObjectData = new ObjectData(value.id, value.position, value.scale);
            transform.position = value.position;
            transform.localScale = value.scale;
            targetPosition = value.targetPositions;
        } }

    private List<Vector2> targetPosition; //TODO 0829


    protected virtual void Activation(){}
    protected virtual void Deactivated(){}
    protected virtual void PrograssButtonActivatedObject(bool onActivate){}



    #region  GET,SET

    public override void SetData<T>(T data)
    {
        try{
            if (typeof(T) == typeof(ButtonObjectStruct))
        {
         ButtonObjectStruct buttonData = (ButtonObjectStruct)(object)data;
         ButtonObjectData = buttonData;

        //Set Target

        FindTargetObject(ButtonObjectData.targetPositions);

        }
        }catch{
                Debug.Log($"ERROR\n{typeof(T)}");
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

    private void FindTargetObject(List<Vector2> list){

        List<GameObject> objList = new();

        foreach(Vector2 vec in list){
            foreach(Transform tr in MapEditor.Instance.buttonActivatedObjectTransform){
             BuildObj buildObj = tr.GetComponent<BuildObj>();
              if(buildObj != null){
                if(buildObj.position == vec){
                    objList.Add(tr.gameObject);
                }
             }
        }

        targetObjects = objList;
        }

       
    }

    #endregion
}
