using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class ExitPointObj : BuildObj
{
    [Header("State")]
    [SerializeField] bool stageClear;
    public string nextMapId;
    public int curPlayerInDoor;


    [Header("Info")]
    [ReadOnly]
    public int condition_KeyAmount;
    private int current_KeyAmount;
    //public int Current_KeyAmount {
    //    get { return current_KeyAmount; }
    //    set { current_KeyAmount -= value; //TODO 0729
    //        current_KeyAmount = Math.Clamp(current_KeyAmount,0, condition_KeyAmount);//TODO 0729
    //        keyBubble.MinusConditionKeyAmount(current_KeyAmount);//TODO 0802 Need Network
    //        if (current_KeyAmount == 0 && !stageClear) //TODO 0729
    //        {
    //            stageClear = true;
    //            MapEditor.Instance.stageClear = true;
    //            doorOpeningAnim.CallOnUnlockAnimation();
    //            //absencePanel.OnAbsencePanel(); //TOdo 0802 Need Network
    //            ExitPoint_Net.OnAbsencePanel();


    //        }
    //        } }


    private int curKeyAmount = 0;
    public void SetKey()
    {
        //curKeyAmount--;
        //keyBubble.MinusConditionKeyAmount(curKeyAmount);
        //if (current_KeyAmount <= 0 && !stageClear) //TODO 0729
        //{
        //    stageClear = true;
        //    MapEditor.Instance.stageClear = true;
        //    doorOpeningAnim.CallOnUnlockAnimation();
        //    //absencePanel.OnAbsencePanel(); //TOdo 0802 Need Network
        //    //ExitPoint_Net.OnAbsencePanel();


        //}
        //ExitPoint_Net.Server_SetCurrent_KeyAmount(1);
        ExitPoint_Net.Cmd_SetCurrent_KeyAmount(1);
    }


    [Header("Componenets")]
    DoorOpeningAnim doorOpeningAnim;
    Collider2D _col;
    UI_Dialogue dialogue; //TODO 0805

    [SerializeField] KeyBubble keyBubble;//TOdo 0802 Need Network
    [SerializeField] AbsencePanel absencePanel;//TOdo 0802 Need Network

    ExitPoint_Net exitPoint_Net;
    ExitPoint_Net ExitPoint_Net
    {
        get
        {
            if (exitPoint_Net == null) exitPoint_Net = GetComponent<ExitPoint_Net>();
            return exitPoint_Net;
        }
    }

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
    //--------------------------------------------------------------------------------------------NetWork
    public void AddKeyAmount()
    {
        //keyBubble.AddKeyAmount();
        //condition_KeyAmount++;
        //current_KeyAmount++;
        ExitPoint_Net.Server_AddKeyAmount();
    }
    //--------------------------------------------------------------------------------------------NetWork
    public override void SetData<T>(T data)
    {
        if(typeof(T)==typeof(ExitObjStruct)){
            ExitObjStruct eData = (ExitObjStruct)(object)data;
            condition_KeyAmount = eData.condition_KeyAmount;
            current_KeyAmount = condition_KeyAmount;

            //keyBubble.SetData(current_KeyAmount);
            if(Application.isPlaying)
            {
                ExitPoint_Net.Server_SetCurMapId(MapEditor.Instance.mapID);
                ExitPoint_Net.Server_SetInit(condition_KeyAmount);
                ExitPoint_Net.Server_SetNextMapId(eData.nextMapId);
            }

            nextMapId = eData.nextMapId;
            // SetTileData(eData.position);

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
        //Current_KeyAmount = 1;
        SetKey();
    }

    //void GetKey(GameObject obj)
    //{
    //    if(Managers.Game.CurrentState != GameState.Editor)
    //    {
    //        //obj.GetComponent<Key>().CallOnInterableObjectRelease();
    //        //obj.GetComponent<SpriteRenderer>().enabled = false;
    //        //obj.GetComponent<IInteractable>().Interacting(true);
    //        //obj.transform.position = new Vector3(-1000, -1000);
    //        //Destroy(obj, 1f);
    //        Managers.Command.DestroyKey(obj);
    //        Current_KeyAmount = 1;
    //    }
    //}



    //TOdo 0729
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // if (collision.gameObject.layer == LayerMask.NameToLayer("Key") && !turnOff)
        //     ClientGetKey(collision.gameObject);

        if(collision.TryGetComponent(out Key component) && !turnOff){
            ClientGetKey(collision.gameObject);
        }
        
        if (Managers.Game.CurrentState == GameState.Editor || !Managers.Game.Player.GetComponent<PlayerSM>().isServer) return;
        
        //if (collision.gameObject.layer == LayerMask.NameToLayer("Key") && !turnOff)
        //    GetKey(collision.gameObject);
        
        if(collision.gameObject.layer == LayerMask.NameToLayer("Player") && ExitPoint_Net.stageClear)
        {
            //absencePanel.Enter(collision.gameObject);//TODO 0802 Need Networking
            // doorOpeningAnim.Enter(collision.gameObject);

            ExitPoint_Net.Enter(collision.gameObject);
            ExitPoint_Net.Server_SetInDoor(1);
            
            //curPlayerInDoor++;
            //if(stageClear && curPlayerInDoor >= 2)
            //{
#if !UNITY_EDITOR
                var playerCharacter = Managers.Game.Player.GetComponent<Player>().characterType;
                var otherPlayerCharacter = Managers.Game.OtherPlayer.GetComponent<Player>().characterType;

                if (playerCharacter != otherPlayerCharacter && playerCharacter != CharacterType.Default && otherPlayerCharacter != CharacterType.Default)
#endif
                    //MoveNextStage();
            //}
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(ExitPoint_Net.OnMoveNextStage) return;
        if (Managers.Game.CurrentState != GameState.Editor && !Managers.Game.Player.GetComponent<PlayerSM>().isServer) return;
        
        if(collision.gameObject.layer == LayerMask.NameToLayer("Player") && ExitPoint_Net.stageClear)
        {
            //absencePanel.Exit(collision.gameObject);//TODO 0802 Need Networking
            //doorOpeningAnim.Exit(collision.gameObject);

            ExitPoint_Net.Exit(collision.gameObject);
            ExitPoint_Net.Server_SetInDoor(-1);
            
            //curPlayerInDoor--;
            //if(curPlayerInDoor < 0) { curPlayerInDoor = 0; }
            
        }
    }

    //public void MoveNextStage()
    //{
    //    doorOpeningAnim.CmdMoveNextStage(nextMapId);
    //}

    //TODO 0802
    public void Net_SetNextMapId(string nextMapId)
    {
        ExitPoint_Net.Cmd_SetNextMapId(nextMapId);
    }

    //public void MoveNextStage() 
    //{
    //    //absencePanel.NextMoveAnimation(); //TODO 0802 Need Networking
    //    //TODO 0804
    //    if (MapEditor.Instance.CurMap.mapID == "Tutorial_3"&& !Managers.Data.saveData._SaveFileData._PlayerSaveData._IstutorialClear)
    //    {
    //        Managers.Data.saveData._SaveFileData._PlayerSaveData._IstutorialClear = true;
    //        StartCoroutine(ExecuteAfterDelay(dialogue.TutorialClearDialogue(), () =>
    //        {
    //            doorOpeningAnim.CmdMoveNextStage(ExitPoint_Net.nextMapId);
    //            //doorOpeningAnim.CmdMoveNextStage(nextMapId);
    //        }));

    //        return;
    //    }

       
    //        StartCoroutine(ExecuteAfterDelay(1f, () => //TODO 0802
    //        {
    //            //doorOpeningAnim.CmdMoveNextStage(nextMapId);
    //            doorOpeningAnim.CmdMoveNextStage(ExitPoint_Net.nextMapId);
    //        }));
        
    //}


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

      #region  AbsenecePanel
    public void Enter(GameObject obj)
    {
        absencePanel.Enter(obj);
    }
    public void Exit(GameObject obj)
    {
        absencePanel.Exit(obj);
    }
    public void OnAbsence()
    {
        absencePanel.OnAbsencePanel();
    }
    #endregion
}
