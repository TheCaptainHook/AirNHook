using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CollectableEntity : BuildObj
{
    private bool isFound;

    #region  Components
    // private Rigidbody2D _rb;
    // private Collider2D _Collider;
    #endregion

    private void Awake(){
        // rb = GetComponent<Rigidbody2D>();
        // _collider = GetComponent<Collider2D>();
    }

    #region  Get,Set
      public override T GetData<T>()
    {
          if(typeof(T)==typeof(CollectableObjectStruct)){
            return (T)(object)new CollectableObjectStruct(id,transform.position,transform.rotation,isFound);
        }
        
       return default(T);
    }

    public override void SetData<T>(T data)
    {
         if(data is CollectableObjectStruct objData){
            transform.position = objData.position;
            transform.rotation = objData.quaternion;
            id = objData.id;
            isFound = objData.isFound;
        }
    }
    #endregion

    private void Start(){
        CheckMapSaveData();
    }

    private void CheckMapSaveData(){
        if(Managers.Data.saveData.dic[MapEditor.Instance.CurMap.mapID].CheckIsFoundCollectableObject(transform.position)){
            gameObject.SetActive(false);
        }
    }




    private void OnTriggerEnter2D(Collider2D collider){
        if(collider.GetComponent<PlayerSM>()){
            AddCollectable();
            Destroy(gameObject);
        }
    }


    #region  Util

    private void AddCollectable(){
        Managers.Data.saveData.AddCollectable(MapEditor.Instance.CurMap.mapID,transform.position);
    }
    #endregion







}
