using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

//TODO 0729 Develop Code Line(key bubble) : 21,22,23,24,37,58,79,104
//TODO 0801 Develop Code Line(AbsencePanel) :40
//TODO 0802 Develop Code Line(AbsencePanel,Network) : 25,31,40,41,67,120,141,155,
//TODO 0805 Develop Code Line : 
public class ExitPointObj : BuildObj
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
        set { current_KeyAmount -= value; //TODO 0729
            current_KeyAmount = Math.Clamp(current_KeyAmount,0, condition_KeyAmount);//TODO 0729
            keyBubble.MinusConditionKeyAmount(current_KeyAmount);//TODO 0802 Need Network
            if (current_KeyAmount == 0 && !stageClear) //TODO 0729
            {
                stageClear = true;
                MapEditor.Instance.stageClear = true;
                doorOpeningAnim.CallOnUnlockAnimation();
                absencePanel.OnAbsencePanel(); //TOdo 0802 Need Network

                
            }
            } }

    [Header("Componenets")]
    DoorOpeningAnim doorOpeningAnim;
    Collider2D _col;
    UI_Dialogue dialogue; //TODO 0805

    [SerializeField] KeyBubble keyBubble;//TOdo 0802 Need Network
    [SerializeField] AbsencePanel absencePanel;//TOdo 0802 Need Network



    private void Awake()
    {
        doorOpeningAnim = GetComponent<DoorOpeningAnim>();
        _col = GetComponent<Collider2D>();
       
    }

    private void Start()
    {
        dialogue = Managers.UI.GetUI<UI_Dialogue>().gameObject.GetComponent<UI_Dialogue>();//TODO 0805
    }
    //event Action OnCheckKey;
    bool isClear;


    public ExitObjStruct GetExitObjectStruct()
    {
      
        return new ExitObjStruct(id,transform.position, condition_KeyAmount, nextMapId);
    }
    
    
    // public void SetData(ExitObjStruct data)
    // {
    //     condition_KeyAmount = data.condition_KeyAmount;
    //     current_KeyAmount = condition_KeyAmount;//TODO 0729
        
    //     keyBubble.SetData(current_KeyAmount);//TODO 0802 need Networking

    //     nextMapId = data.nextMapId;
        
    //     SetTileData(data.position);

    // }
    public override void SetData<T>(T data)
    {
        if(typeof(T)==typeof(ExitObjStruct)){
            ExitObjStruct eData = (ExitObjStruct)(object)data;
            condition_KeyAmount = eData.condition_KeyAmount;
            current_KeyAmount = condition_KeyAmount;
            keyBubble.SetData(current_KeyAmount);
            nextMapId = eData.nextMapId;
            SetTileData(eData.position);

            transform.position = eData.position;
        }
    }



    public void Init(int condition_keyAmount)
    {
        this.condition_KeyAmount = condition_keyAmount;
    }

    private void ClientGetKey(GameObject obj) //TOdo 0729
    {
        if (Managers.Game.CurrentState != GameState.Editor)
        {
            Managers.Command.DestroyKey(obj);
        }
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



    //TOdo 0729
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Key") && !turnOff)
            ClientGetKey(collision.gameObject);
        
        if (Managers.Game.CurrentState == GameState.Editor || !Managers.Game.Player.GetComponent<Player>().isServer) return;
        
        //if (collision.gameObject.layer == LayerMask.NameToLayer("Key") && !turnOff)
        //    GetKey(collision.gameObject);
        
        if(collision.gameObject.layer == LayerMask.NameToLayer("Player") && stageClear)
        {
            absencePanel.Enter(collision.gameObject);//TODO 0802 Need Networking
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
        
        if(collision.gameObject.layer == LayerMask.NameToLayer("Player") && stageClear)
        {
            absencePanel.Exit(collision.gameObject);//TODO 0802 Need Networking
            curPlayerInDoor--;
            if(curPlayerInDoor < 0) { curPlayerInDoor = 0; }
        }
    }

    //public void MoveNextStage()
    //{
    //    doorOpeningAnim.CmdMoveNextStage(nextMapId);
    //}

    //TODO 0802
    public void MoveNextStage() 
    {
        //absencePanel.NextMoveAnimation(); //TODO 0802 Need Networking
        //TODO 0804
        if (MapEditor.Instance.CurMap.mapID == "Tutorial_3"&& !Managers.Data.saveData._SaveFileData._PlayerSaveData._IstutorialClear)
        {
            StartCoroutine(ExecuteAfterDelay(dialogue.TutorialClearDialogue(), () =>
            {
                doorOpeningAnim.CmdMoveNextStage(nextMapId);
            }));

            return;
        }

       
            StartCoroutine(ExecuteAfterDelay(1f, () => //TODO 0802
            {
                doorOpeningAnim.CmdMoveNextStage(nextMapId);
            }));
        
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


    public override void Reset()
    {
        keyBubble.gameObject.SetActive(false);
        absencePanel.gameObject.SetActive(false);
        stageClear = false;
        //Door Lock
    }

    #region Util

    private IEnumerator ExecuteAfterDelay(float delay, System.Action action)
    {
        yield return new WaitForSeconds(delay);
        action();
    }

    private IEnumerator ExecuteAfterDelay(IEnumerator coroutine, System.Action action)
    {
        yield return coroutine;
        yield return new WaitForSeconds(1f);
        action();
        
    }

    

    #endregion
}
