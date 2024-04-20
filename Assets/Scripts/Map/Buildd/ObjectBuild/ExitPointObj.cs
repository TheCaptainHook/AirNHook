using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class ExitPointObj : BuildBase
{
    [Header("State")]
    [SerializeField] bool stageClear;
    public string nextMapId;
    public int curPlayerInDoor;


    [Header("Info")]
    public int condition_KeyAmount;
    private int current_KeyAmount;
    public int Current_KeyAmount {
        get { return current_KeyAmount; }
        set { current_KeyAmount++;
            if (current_KeyAmount >= condition_KeyAmount)
            {
                stageClear = true;
                MapEditor.Instance.stageClear = true;
                doorOpeningAnim.CallOnUnlockAnimation();
            }
            } }

    [Header("Componenets")]
    DoorOpeningAnim doorOpeningAnim;
    Collider2D _col;
    private void Awake()
    {
        doorOpeningAnim = GetComponent<DoorOpeningAnim>();
        _col = GetComponent<Collider2D>();
    }

    //event Action OnCheckKey;
    bool isClear;


    public ExitObjStruct GetExitObjectStruct()
    {
      
        return new ExitObjStruct(id,transform.position, condition_KeyAmount, nextMapId);
    }
    
    
    public void SetData(ExitObjStruct data)
    {
        condition_KeyAmount = data.condition_KeyAmount;
        nextMapId = data.nextMapId;
        SetTileData(data.position);
    }

    public void Init(int condition_keyAmount)
    {
        this.condition_KeyAmount = condition_keyAmount;
    }

    void GetKey(GameObject gameObject)
    {
        gameObject.GetComponent<Key>().CallOnInterableObjectRelease();
        Managers.Stage.CmdDestroyObject(gameObject);
        Current_KeyAmount = 1;
    }




    private void OnTriggerEnter2D(Collider2D collision)
    {
        if( Managers.Game.CurrentState != GameState.Editor && !Managers.Game.Player.GetComponent<Player>().isServer) return;
        
        if (collision.gameObject.layer == LayerMask.NameToLayer("Key") && !turnOff)
        {
            GetKey(collision.gameObject);
            Debug.Log(current_KeyAmount);
        }

        if(collision.gameObject.layer == LayerMask.NameToLayer("Player") && stageClear)
        {
            curPlayerInDoor++;
            if(curPlayerInDoor >= 2)
            {
                MoveNextStage();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (Managers.Game.CurrentState != GameState.Editor && !Managers.Game.Player.GetComponent<Player>().isServer) return;
        
        if(collision.gameObject.layer == LayerMask.NameToLayer("Player") && stageClear)
        {
            
            curPlayerInDoor--;
            if(curPlayerInDoor < 0) { curPlayerInDoor = 0; }
        }
    }

    public void MoveNextStage()
    {
        UpdateStageClearData(MapEditor.Instance.curMap.mapID);
        doorOpeningAnim.CmdMoveNextStage(nextMapId);
    }


    private void UpdateStageClearData(string mapId)
    {
        if (mapId == "Lobby") return;
        if (!Managers.Data.loadData.stageData[mapId].stageClear)
        {
            Managers.Data.loadData.stageData[mapId].stageClear = true;
        }

        
    }


    public override void TurnOff()
    {
        base.TurnOff();
        turnOff = true;
    }
    public override void TurnOn()
    {
        base.TurnOn();
        turnOff = false;
    }
}
