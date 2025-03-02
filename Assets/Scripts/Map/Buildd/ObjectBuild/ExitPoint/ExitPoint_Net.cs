using Mirror;
using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class ExitPoint_Net : NetworkBehaviour
{
    [SerializeField] ExitPointObj exit;

    [SerializeField] KeyBubble keyBubble;
    [SerializeField] DoorOpeningAnim doorOpeningAnim;

    #region Data sync
    [SyncVar] public string curMapId;
    [SyncVar] public string nextMapId;
    [SyncVar] public bool stageClear;
    [SyncVar] public int condition_KeyAmount;
    [SyncVar(hook =nameof(OnChangeCurrent_KeyAmount))] public int current_KeyAmount;
    [SyncVar] public int curPlayerInDoor;
    [SyncVar] public bool OnMoveNextStage; // 다음 맵 이동 조건 충족 
    #endregion

    #region Server_Init Sync
    [Server]
    public void Server_SetInit(int condition)
    {
        condition_KeyAmount = condition;
        current_KeyAmount = condition;
        Debug.Log($"Condition : {condition}");
    }
    #endregion

    #region Server_Data Sync
    [Server]
    public void Server_SetNextMapId(string nextMapId)
    {
        this.nextMapId = nextMapId;
    }
    [Server]
    public void Cmd_SetNextMapId(string nextMapId)
    {
        Server_SetNextMapId(nextMapId);
    }
    [Server]
    public void Server_SetCurMapId(string curMapId)
    {
        this.curMapId = curMapId;
    }

    [Server]
    public void Server_SetCurrent_KeyAmount(int amount)
    {
        current_KeyAmount -= amount;
        if(current_KeyAmount <= 0 && !stageClear)
        {

            stageClear = true;

            Rpc_StageClear();

            doorOpeningAnim.CallOnUnlockAnimation();
        }
    }
    [ClientRpc]
    private void Rpc_StageClear()
    {
        MapEditor.Instance.stageClear = true;
    }

    [Command(requiresAuthority = false)]
    public void Cmd_SetCurrent_KeyAmount(int amount)
    {
        Server_SetCurrent_KeyAmount(amount);
    }

    private void OnChangeCurrent_KeyAmount(int old,int newVal)
    {
        Debug.Log($"old : {old},new : {newVal}");
        keyBubble.MinusConditionKeyAmount(newVal);
    }



    #region Key
    [Server]
    public void Server_AddKeyAmount()
    {
        condition_KeyAmount++;
        current_KeyAmount++;

        // Rpc_KeyBubble_Add();
    }

    // [ClientRpc]
    // private void Rpc_KeyBubble_Add()
    // {
    //     keyBubble.AddKeyAmount();

    // }

    #endregion

    #endregion

    #region Next Map
    [Server]
    public void Server_SetInDoor(int num)
    {
        curPlayerInDoor += num;
        if (curPlayerInDoor < 0) curPlayerInDoor = 0;

        if (stageClear && curPlayerInDoor >= 2)
        {
            //exit.MoveNextStage();
            OnMoveNextStage = true;

            MoveNextStage();
            //MapEditor.Instance.MoveNextStage(nextMapId);

        }
    }

    

    [ClientRpc]
    private void MoveNextStage()
    {
        try
        {
            Managers.Sound.CollectAmbientSoundSource();
            Managers.Stage.stageName = nextMapId;
            Camera.main.GetComponent<ParallaxCamera>().enabled = false;

            string nextMap = nextMapId;

            //NextMapId 가 null 이면 스테이지 클리어. -> 로비로 이동
            if(string.Empty == nextMap && curMapId != "Lobby")
            {
                Managers.Game.CurrentState = GameState.Lobby;
                Managers.Game.StageClear(curMapId,true);
                nextMap = "Lobby";
                
            }else //단순 맵 클리어
            {
                Managers.Game.CurrentState = GameState.Game;
                Managers.Game.StageClear(curMapId);
            }

            //특정 스테이지 클리어시 나오는 다이어그램
            // -> 실행 후 네트워크에 다음맵으로 이동할 준비가 됐다는 신호 보내기.

            //페이드 아웃, -> 클라이언트 준비 중 UI 

            //모든 클라이언트의 준비 확인. -> 맵 이동

            MapEditor.Instance.MoveNextStage(nextMap);

        }
        catch (Exception e)
        {
            Debug.Log(e);
        }

    }



    #endregion
    //[Command(requiresAuthority = false)]
    //public void OnAbsencePanel()
    //{
    //    RpcOnAbsencePanel();
    //}

    private IEnumerator WaitUntilAllClientsReady(Action action)
    {
        while (!AllClientsReady()) // 모든 클라이언트가 준비될 때까지 대기
        {
            Debug.Log("클라이언트가 아직 준비되지 않음. 기다리는 중...");
            yield return null;
        }

        Debug.Log("모든 클라이언트가 준비됨! ClientRpc 호출");
        action?.Invoke();
    }

    private bool AllClientsReady()
    {
        foreach (var conn in NetworkServer.connections.Values)
        {
            if (!conn.isReady)
            {
                return false;
            }
        }
        return true;
    }
    #region Absence Panel
    [ClientRpc]
    public void RpcOnAbsencePanel()
    {
        exit.OnAbsence();
    }
    [Command(requiresAuthority = false)]
    public void Enter(GameObject obj)
    {
        try
        {
            RpcEnter(obj);
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }

    }
    [ClientRpc]
    public void RpcEnter(GameObject obj)
    {
        exit.Enter(obj);
    }
    [Command(requiresAuthority = false)]
    public void Exit(GameObject obj)
    {
        if (obj == null) return;
        try
        {
            RpcExit(obj);
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }
    [ClientRpc]
    public void RpcExit(GameObject obj)
    {
        exit.Exit(obj);
    }
    #endregion



    // public override void OnStartClient()
    // {
    //     base.OnStartClient();
    //     keyBubble.SetData(current_KeyAmount);
    // }


}
