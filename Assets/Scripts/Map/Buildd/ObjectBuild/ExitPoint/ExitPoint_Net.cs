using Mirror;
using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class ExitPoint_Net : NetworkBehaviour
{
    [SerializeField] ExitPointObj main;

    [SerializeField] KeyBubble keyBubble;
    [SerializeField] AbsencePanel panel;
    [SerializeField] DoorOpeningAnim doorOpeningAnim;

    #region Data sync
    [ReadOnly]
    private Vector2 mainPosition;
    [ReadOnly]
    public string curMapId;
    [ReadOnly]
    public string nextMapId;
    // private bool StageClear => MapEditor.Instance.stageClear;
    // [SyncVar] public int condition_KeyAmount;
    [SyncVar(hook =nameof(Hook_OnChangeCurrent_KeyAmount))] public int current_KeyAmount;
    // public int curPlayerInDoor;
    // [SyncVar] public bool OnMoveNextStage; // 다음 맵 이동 조건 충족 
    #endregion

    private Collider2D col;
    private Collider2D Col {get{if(col == null)col = GetComponent<Collider2D>(); return col;}}

    #region Server_Init Sync
    public bool onSync;

    [Server]
    public void Server_InitSync()
    {
         var data = main.data;

        mainPosition = data.position;
        curMapId = MapEditor.Instance.mapID;
        nextMapId = data.nextMapId;
        transform.position = data.position;
        current_KeyAmount = data.condition_KeyAmount;

        onSync = true;

        Rpc_InitSync(data);
    }
     [ClientRpc]
    private void Rpc_InitSync(ExitObjStruct data)
    {
        if(onSync) return;
        mainPosition = data.position;
        curMapId = MapEditor.Instance.mapID;
        nextMapId = data.nextMapId;
        transform.position = data.position;

        onSync = true;
    }

    [Command(requiresAuthority = false)]
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

    #region Use Stage Select_var3
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

    #endregion

   
    private void Hook_OnChangeCurrent_KeyAmount(int old,int newVal)
    {
        keyBubble.MinusConditionKeyAmount(newVal);
    }

    //[Command(requiresAuthority = false)]
    //public void Cmd_GetKey(uint id)
    //{
    //    if (isServer)
    //    {
    //        var item = NetworkClient.spawned.TryGetValue(id, out NetworkIdentity identity) ? identity : null;
    //        if (item == null) return;

    //        Managers.Command.DestroyKey(identity.gameObject);
    //        Debug.Log("Get Key");
    //        current_KeyAmount -= 1;
    //        if (current_KeyAmount <= 0 && !MapEditor.Instance.stageClear)
    //        {
    //            Rpc_StageClear();
    //        }

    //    }

    //}

    [Server]
    public void Server_GetKey(uint id)
    {
        var item = NetworkClient.spawned.TryGetValue(id, out NetworkIdentity identity) ? identity : null;
        if (item == null) return;

        Managers.Command.DestroyKey(identity.gameObject);
        Debug.Log("Get Key");
        current_KeyAmount -= 1;
        if (current_KeyAmount <= 0 && !MapEditor.Instance.stageClear)
        {
            Rpc_StageClear();
        }
    }

   [ClientRpc]
    private void Rpc_StageClear()
    {
       if(doorUnlockAnimationCoroutin == null)
       {
            doorUnlockAnimationCoroutin = StartCoroutine(StageClearDelayCoroutine());
       }
    }

    private Coroutine doorUnlockAnimationCoroutin;
    IEnumerator StageClearDelayCoroutine()
    {
        doorOpeningAnim.CallOnUnlockAnimation();
        Col.enabled = false;    

        yield return new WaitForSeconds(2);
        panel.OnAbsencePanel();
        MapEditor.Instance.stageClear = true;
        Col.enabled = true;
    }


    #region Key
    [Server]
    public void Server_AddKeyAmount()
    {
        // condition_KeyAmount++;
        current_KeyAmount++;
    }

    #endregion


    #region In Out Player
    [ReadOnly] public AirSM innerDoor_Air;
    [ReadOnly] public HookSM innerDoor_Hook;
    
    [ReadOnly] public bool onReadyToMoveNextMap;

    [Server]
    public void Server_InPlayer(uint id)
    {
        var item = NetworkClient.spawned.TryGetValue(id, out NetworkIdentity identity) ? identity : null;
        if (item == null) return;

        var air = item.GetComponent<AirSM>();
        if (air && !innerDoor_Air)
        {
            innerDoor_Air = air;
            //Air panel Open
            Rpc_AirPanelOpenAndClose(true);
            //Air panel Open
        }

        var hook = item.GetComponent<HookSM>();
        if (hook && !innerDoor_Hook)
        {
            innerDoor_Hook = hook;
            //Hook panel Open
            Rpc_HookPanelOpenAndClose(true);
            //Hook panel Open
        }
        

        if (innerDoor_Air && innerDoor_Hook)
        {
            // move Next map
            //Managers.Command.Server_ChangeStage_Use_ExitDoor(); //UI_Option Lock
            Rpc_OnReadyToMoveMap();
            // move Next map

        }

    }
    [ClientRpc]
    private void Rpc_OnReadyToMoveMap()
    {
        onReadyToMoveNextMap = true;
        MoveNextStageCo();
    }

    [Server]
    public void Server_OutPlayer(uint id)
    {
        var item = NetworkClient.spawned.TryGetValue(id, out NetworkIdentity identity) ? identity : null;
        if (item == null) return;

        var air = item.GetComponent<AirSM>();
        if (air == innerDoor_Air)
        {
            innerDoor_Air = null;
            //Air panel Close
            Rpc_AirPanelOpenAndClose(false);
            //Air panel Close
        }

        var hook = item.GetComponent<HookSM>();
        if (hook == innerDoor_Hook)
        {
            innerDoor_Hook = null;
            //Hook panel Close
            Rpc_HookPanelOpenAndClose(false);
            //Hook panel Close
        }
    }

    #endregion

    #region Move Next Stage
    Coroutine moveNextStageCoroutine;

    private void MoveNextStageCo()
    {
        if (moveNextStageCoroutine == null) moveNextStageCoroutine = StartCoroutine(MoveNextStageCoroutine());

    }
    
    private IEnumerator MoveNextStageCoroutine()
    {
        panel.StopAllCoroutines();
        yield return new WaitForSeconds(1);
        MoveNextStage();
        moveNextStageCoroutine = null;

        
    }
    
    private void MoveNextStage()
    {
        //NextMapId 가 null 이면 스테이지 클리어. -> 로비로 이동
            if(string.IsNullOrEmpty(nextMapId) && curMapId != "Lobby")
            {
                Managers.Game.CurrentState = GameState.Lobby;
                Managers.Game.StageClear(curMapId,true);
                MapEditor.Instance.MoveNextStage("Lobby");
                return;
                
            }else //단순 맵 클리어
            {
                Managers.Game.CurrentState = GameState.Game;
                Managers.Game.StageClear(curMapId);
                MapEditor.Instance.MoveNextStage(nextMapId);
             return;
            }
    }
    #endregion

    #region Absence Panel
    [ClientRpc]
    private void Rpc_HookPanelOpenAndClose(bool openAndClose)
    {
        if(openAndClose) panel.HookPanelOpen();
        else panel.HookPanelClose();
    }
    [ClientRpc]
    private  void Rpc_AirPanelOpenAndClose(bool openAndClose)
    {
        if(openAndClose) panel.AirPanelOpen();
        else panel.AirPanelClose();
    }
    #endregion



    // private bool AllClientsReady()
    // {
    //     foreach (var conn in NetworkServer.connections.Values)
    //     {
    //         if (!conn.isReady)
    //         {
    //             return false;
    //         }
    //     }
    //     return true;
    // }
    
}
