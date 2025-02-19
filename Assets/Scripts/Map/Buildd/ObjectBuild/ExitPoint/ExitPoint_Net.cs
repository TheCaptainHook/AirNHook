using Edgegap.Editor.Api.Models.Results;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitPoint_Net : NetworkBehaviour
{
    [SerializeField] ExitPointObj exit;

    [SerializeField] KeyBubble keyBubble;
    [SerializeField] DoorOpeningAnim doorOpeningAnim;

    #region Data sync
    [SyncVar] public bool stageClear;
    [SyncVar] public int condition_KeyAmount;
    [SyncVar(hook =nameof(OnChangeCurrent_KeyAmount))] public int current_KeyAmount;
    [SyncVar] public int curPlayerInDoor;
    #endregion

    #region Server_Init Sync
    [Server]
    public void Server_SetInit(int condition)
    {
        condition_KeyAmount = condition;
        current_KeyAmount = condition;

        Rpc_KeySet();
    }
    #endregion

    #region Server_Data Sync
    [Server]
    public void Server_SetCurrent_KeyAmount(int amount)
    {
        current_KeyAmount -= amount;
        if(current_KeyAmount <= 0 && !stageClear)
        {
            stageClear = true;
            MapEditor.Instance.stageClear = true;
            doorOpeningAnim.CallOnUnlockAnimation();
        }
    }

    private void OnChangeCurrent_KeyAmount(int old,int newVal)
    {
        keyBubble.MinusConditionKeyAmount(newVal);
    }

    [Server]
    public void Server_SetInDoor(int num)
    {
        curPlayerInDoor += num;
        if (curPlayerInDoor < 0) curPlayerInDoor = 0;

        if(stageClear && curPlayerInDoor >=2)
        {
            exit.MoveNextStage();
        }
    }



    #region Key
    [Server]
    public void Server_AddKeyAmount()
    {
        condition_KeyAmount++;
        current_KeyAmount++;

        Rpc_KeyBubble_Add();
    }
    [ClientRpc]
    private void Rpc_KeySet()
    {
        keyBubble.SetData(current_KeyAmount);
    }
    [ClientRpc]
    private void Rpc_KeyBubble_Add()
    {
        keyBubble.AddKeyAmount();

    }

    #endregion

    #endregion


    //[Command(requiresAuthority = false)]
    //public void OnAbsencePanel()
    //{
    //    RpcOnAbsencePanel();
    //}



    #region Absence Panel
    [ClientRpc]
    public void RpcOnAbsencePanel()
    {
        exit.OnAbsence();
    }
    [Command(requiresAuthority = false)]
    public void Enter(GameObject obj)
    {
        RpcEnter(obj);
    }
    [ClientRpc]
    public void RpcEnter(GameObject obj)
    {
        exit.Enter(obj);
    }
    [Command(requiresAuthority = false)]
    public void Exit(GameObject obj)
    {
        RpcExit(obj);
    }
    [ClientRpc]
    public void RpcExit(GameObject obj)
    {
        exit.Exit(obj);
    }
    #endregion




}
