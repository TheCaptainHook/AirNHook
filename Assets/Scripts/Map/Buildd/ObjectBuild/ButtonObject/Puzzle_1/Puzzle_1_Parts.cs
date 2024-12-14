using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Puzzle_1_Parts : MonoBehaviour,IInteractable
{
    public Puzzle_1_Item onSocketItem;
    private bool onSocket;
    
    private UI_Base _E_Btn;
    [SerializeField] float _BtnOffset;

    #region Components
    private Collider2D col;
    #endregion
    [field: SerializeField] protected ObjectTypeEnum _objectType = ObjectTypeEnum.Grab;
    private void Awake(){
        col = GetComponent<Collider2D>();
    }

    
    // private void RemoveSocket(Puzzle_1_Item item){
    //     if(onSocketItem != null){
    //         onSocketItem.RemoveSocket();
    //         onSocketItem = item;
    //         item.InsertSocket();
    //     }else{
    //         onSocketItem = item;
    //         item.InsertSocket();
    //     }
    // }
    public void InsertSocket(Puzzle_1_Item item){
        if(onSocketItem){
            //기존 소켓 지워버리고, 소켓 교체
            Debug.Log("이미 소켓 있음");
            return;
        }else{
            onSocketItem = item;
            item.transform.position = transform.position;
            HideEButton();
        }
        
    }



    private void OnTriggerEnter2D(Collider2D collider){
         if(collider.TryGetComponent(out HookSM component)){
            Transform grabItem = component.GetGrabbedItem();
            if(grabItem != null){
                if(grabItem.TryGetComponent(out Puzzle_1_Item component1)){
                    ShowEButton();
                    component1.PossibleInsertSocket(this);
                }
            }
        }
    }

    
    private void OnTriggerExit2D(Collider2D collider){
        if(collider.TryGetComponent(out HookSM component)){
            
            Transform grabItem = component.GetGrabbedItem();
            if(grabItem != null){
                if(grabItem.TryGetComponent(out Puzzle_1_Item component1)){
                    HideEButton();
                    component1.UnPossibleInsertSocket();
                }
                
            }
        }
        
    }

    // private void Interaction(){
    //      if(onSocket){
    //             onSocket = false;
    //             RemoveSocket(item);
    //     }else{
    //             onSocket = true;
    //             InsertSocket(item);
    //     }   
    // }


    public void ShowEButton(){
        _E_Btn = Managers.UI.ShowUI<UI_ShowEButton>();
        _E_Btn.transform.position =  transform.position + (transform.up * _BtnOffset);
    }
    public void HideEButton(){
        _E_Btn = null;
        Managers.UI.HideUI<UI_ShowEButton>();
    }

     public void Interaction(Transform accessor = null)
     {
        if(onSocketItem != null){
            onSocketItem.GetComponent<Rigidbody2D>().simulated = true;
            HideEButton();
        }
     }

    public bool CanInteract(){
        return true;
    }

    public void Interacting(bool value){}
    
    public ObjectTypeEnum GetObjectType(){
        return _objectType;
    }

}
