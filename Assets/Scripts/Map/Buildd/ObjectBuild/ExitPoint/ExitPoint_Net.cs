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
    [ReadOnly]
    private Vector2 mainPosition;
    [ReadOnly]
    public string curMapId;
    [ReadOnly]
    public string nextMapId;
    // [SyncVar] public bool stageClear;
    private bool StageClear => MapEditor.Instance.stageClear;
    [SyncVar] public int condition_KeyAmount;
    [SyncVar(hook =nameof(OnChangeCurrent_KeyAmount))] public int current_KeyAmount;
    [SyncVar] public int curPlayerInDoor;
    [SyncVar] public bool OnMoveNextStage; // 다음 맵 이동 조건 충족 
    #endregion

    private Collider2D col;
    private Collider2D Col {get{if(col == null)col = GetComponent<Collider2D>(); return col;}}

    #region Server_Init Sync
    public bool onSync;

    [Server]
    public void Server_SetInit(int condition)
    {
        condition_KeyAmount = condition;
        current_KeyAmount = condition;
    }
    [Server]
    public void Server_InitSync(Vector2 mainPosition,string curMapId,string nextMapId)
    {
        this.mainPosition = mainPosition;
        this.curMapId = curMapId;
        this.nextMapId = nextMapId;
        transform.position = mainPosition;
    }
    [Server]
    public void Server_InitSync()
    {
        Rpc_InitSync(mainPosition,curMapId,nextMapId);
    }
    [ClientRpc]
    private void Rpc_InitSync(Vector2 mainPosition,string curMapId,string nextMapId)
    {
        if(onSync) return;
        this.mainPosition = mainPosition;
        this.curMapId = curMapId;
        this.nextMapId = nextMapId;
        transform.position = mainPosition;
        onSync = true;
    }
    [Command]
    public void Cmd_InitSync()
    {
        Server_InitSync();
    }
    public override void OnStartClient()
    {
        base.OnStartClient();
        if(!onSync)Cmd_InitSync();
    }

    #endregion

    #region Server_Data Sync
    // [Server]
    // public void Server_SetNextMapId(string nextMapId)
    // {
    //     this.nextMapId = nextMapId;
    // }

    [Server] //Use Stage Select UI
    public void Server_SetNextMapId(string nextMapId) 
    {
        Rpc_SetNextMapId(nextMapId);
    }
    [ClientRpc] //Use Stage Select UI
    private void Rpc_SetNextMapId(string nextMapId)
    {
        this.nextMapId = nextMapId;
    }


    //[Server]
    //public void Server_SetCurrent_KeyAmount(int amount)
    //{
    //    current_KeyAmount -= amount;
    //    if(current_KeyAmount <= 0 && !StageClear)
    //    {
    //        doorOpeningAnim.CallOnUnlockAnimation();
    //        // Rpc_StageClear();
    //        StartCoroutine(Delay());
    //    }
    //}
    IEnumerator Delay()
    {
        yield return new WaitForSeconds(2f);
        Rpc_StageClear();
    }
    [ClientRpc]
    private void Rpc_StageClear()
    {
        Col.enabled = false;    
        MapEditor.Instance.stageClear = true;
        
        Col.enabled = true;
    }

    //[Command(requiresAuthority = false)]
    //public void Cmd_SetCurrent_KeyAmount(int amount)
    //{
    //    Server_SetCurrent_KeyAmount(amount);
    //}

    private void OnChangeCurrent_KeyAmount(int old,int newVal)
    {
        keyBubble.MinusConditionKeyAmount(newVal);
    }

    //------------REfec 0423
    [Command(requiresAuthority = false)]
    public void Cmd_GetKey(uint id)
    {
        if (isServer)
        {
            var item = NetworkClient.spawned.TryGetValue(id, out NetworkIdentity identity) ? identity : null;
            if (item == null) return;

            Managers.Command.DestroyKey(identity.gameObject);

            current_KeyAmount -= 1;
            if (current_KeyAmount <= 0 && !StageClear)
            {
                doorOpeningAnim.CallOnUnlockAnimation();
                // Rpc_StageClear();
                StartCoroutine(Delay());
            }

        }

    }
  
    //------------REfec

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
    [Command(requiresAuthority = false)]
    public void Cmd_SetInDoor(int num)
    {
        if (isServer)
        {
            curPlayerInDoor += num;
            if (curPlayerInDoor < 0) curPlayerInDoor = 0;

            if (StageClear && curPlayerInDoor >= 2)
            {
                //exit.MoveNextStage();
                OnMoveNextStage = true;

                //StartCoroutine(_Delay(1,()=>{MoveNextStage();}));
                MoveNextStage();
                //MapEditor.Instance.MoveNextStage(nextMapId);

            }
        }
    }
    [Server]
    public void Server_SetInDoor(int num)
    {
        curPlayerInDoor += num;
        if (curPlayerInDoor < 0) curPlayerInDoor = 0;

        if (StageClear && curPlayerInDoor >= 2)
        {
            //exit.MoveNextStage();
            OnMoveNextStage = true;

            //StartCoroutine(_Delay(1,()=>{MoveNextStage();}));
            MoveNextStage();
            //MapEditor.Instance.MoveNextStage(nextMapId);

        }
    }

    IEnumerator _Delay(float delayTime,Action action)
    {
        yield return new WaitForSeconds(delayTime);
        action?.Invoke();
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

    //private IEnumerator WaitUntilAllClientsReady(Action action)
    //{
    //    while (!AllClientsReady()) // 모든 클라이언트가 준비될 때까지 대기
    //    {
    //        Debug.Log("클라이언트가 아직 준비되지 않음. 기다리는 중...");
    //        yield return null;
    //    }

    //    Debug.Log("모든 클라이언트가 준비됨! ClientRpc 호출");
    //    action?.Invoke();
    //}

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
        if (obj == null) return;
        RpcEnter(obj.GetComponent<NetworkIdentity>().netId);

    }
    //[ClientRpc]
    //public void RpcEnter(GameObject obj)
    //{
    //    exit.Enter(obj);
    //}
    [ClientRpc]
    public void RpcEnter(uint id)
    {
        var item = NetworkClient.spawned.TryGetValue(id, out NetworkIdentity identity) ? identity : null;
        if (item != null) { exit.Enter(identity.gameObject); }


        
    }
    [Command(requiresAuthority = false)]
    public void Exit(GameObject obj)
    {
        if (obj == null) return;
        RpcExit(obj.GetComponent<NetworkIdentity>().netId);
        //try
        //{
        //    RpcExit(obj);
        //}
        //catch (Exception e)
        //{
        //    Debug.Log(e);
        //}
    }
    //[ClientRpc]
    //public void RpcExit(GameObject obj)
    //{
    //    exit.Exit(obj);
    //}
    [ClientRpc]
    public void RpcExit(uint id)
    {
        var item = NetworkClient.spawned.TryGetValue(id, out NetworkIdentity identity) ? identity : null;
        if (item != null) { exit.Exit(identity.gameObject); }
      
    }
    #endregion




}
