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

    private void ClientGetKey(GameObject obj)
    {
        Current_KeyAmount = 1;
    }
    
    void GetKey(GameObject obj)
    {
        if(Managers.Game.CurrentState != GameState.Editor)
        {
            //obj.GetComponent<Key>().CallOnInterableObjectRelease();
            //obj.GetComponent<SpriteRenderer>().enabled = false;
            //obj.GetComponent<IInteractable>().Interacting(true);
            //obj.transform.position = new Vector3(-1000, -1000);
            //Destroy(obj, 1f);
            Managers.Command.DestroyKey(obj);
            Current_KeyAmount = 1;
        }
    }




    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Key") && !turnOff)
            ClientGetKey(collision.gameObject);
        
        if (Managers.Game.CurrentState == GameState.Editor || !Managers.Game.Player.GetComponent<Player>().isServer) return;
        
        if (collision.gameObject.layer == LayerMask.NameToLayer("Key") && !turnOff)
            GetKey(collision.gameObject);
        
        if(collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            curPlayerInDoor++;
            if(stageClear && curPlayerInDoor >= 2)
            {
#if !UNITY_EDITOR
                var playerCharacter = Managers.Game.Player.GetComponent<Player>().characterType;
                var otherPlayerCharacter = Managers.Game.OtherPlayer.GetComponent<Player>().characterType;

                if (playerCharacter != otherPlayerCharacter && playerCharacter != CharacterType.Default && otherPlayerCharacter != CharacterType.Default)
#endif
                    MoveNextStage();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (Managers.Game.CurrentState != GameState.Editor && !Managers.Game.Player.GetComponent<Player>().isServer) return;
        
        if(collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            curPlayerInDoor--;
            if(curPlayerInDoor < 0) { curPlayerInDoor = 0; }
        }
    }

    public void MoveNextStage()
    {
        doorOpeningAnim.CmdMoveNextStage(nextMapId);
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
