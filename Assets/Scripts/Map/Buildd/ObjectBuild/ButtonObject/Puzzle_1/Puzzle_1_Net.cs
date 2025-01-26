using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Runtime.CompilerServices;

public class Puzzle_1_Net : NetworkBehaviour
{
    Puzzle_1 puzzle;
    Puzzle_1 Puzzle
    {
        get
        {
            if (puzzle == null) puzzle = GetComponent<Puzzle_1>();
            return puzzle;
        }
    }
    [SerializeField] Puzzle_1_Button button;

    #region Animation Sync

    [SyncVar(hook = nameof(OnRateChanged))]
    public float chargingRate;

    [Server]
    public void SetRate(float rate)
    {
        chargingRate += rate;
        chargingRate = Mathf.Clamp01(chargingRate);
    }
    [Server]
    public void Sever_Reset()
    {
        chargingRate = 0;
    }

    [Command(requiresAuthority = false)]
    private void CmdSetRate(float rate)
    {
        SetRate(rate);
    }

    public void HandleSetRate(float rate)
    {
        if (isServer)
        {
            SetRate(rate);
        }
        else
        {
            CmdSetRate(rate);
        }
    }
    public void OnRateChanged(float old, float newVal)
    {
        button.SetAnimation(newVal);
    }

    [Command(requiresAuthority = false)]
    public void CmdReset()
    {
        RpcReset();
    }
    [ClientRpc]
    public void RpcReset()
    {
        Sever_Reset();
    }
    #endregion

    #region Init Sync
    /**
    [syncvar] int random Number (1~7)
    [syncvar] string answer

    Create puzzle item 


    command [random number] -> ClinetRpc [(int randomnum)] ->Create item;

    Sync list
    1. Puzzle item
        - sync random number item, set sync answer
    2. puzzle parts setting
        - create parts, and position setting
    3. hint screen
        - check onHint, hint postiion, answer setting
    **/
 
 //Server -> get random number -> ClinetRpc -> Create Item

    // [Command(requiresAuthority = false)]
    // public GameObject CmdCreatePuzzle_Item()
    // {
    //     int num = GetItemNumber();
    //     return RpcCreatePuzzle_Item(num);

    // }
    // [ClientRpc]
    // public GameObject RpcCreatePuzzle_Item(int num)
    // {
    //     return puzzle.Net_CreatePuzzleItem(num);
    // }
    [SyncVar] private int previousNumber;
    [SyncVar] private string answer;

    [Server]
    public void Server_CreatePuzzle_Item(
        int index,
        Vector2 itemPot,
        Vector2 partPot
        )
        {
            if(!isServer) return;
            int num = Random.Range(1, 7);
            while(previousNumber == num) num = Random.Range(1, 7);
            previousNumber = num;
            answer += previousNumber.ToString();
            RpcCreatePuzzle_Item(num,index,itemPot,partPot);
        }

    [ClientRpc]
    public void RpcCreatePuzzle_Item(
        int randomNumber,
        int index,
        Vector2 itemPot,
        Vector2 partPot)
    {

    }

    #endregion




    [Command(requiresAuthority = false)]
    public void CmdCharging()
    {
        RpcCharging();
    }
    [ClientRpc]
    private void RpcCharging()
    {
        Puzzle.Charging();
    }

    [Command(requiresAuthority = false)]
    public void CmdWrong()
    {
        RpcWrong();
    }
    [ClientRpc]
    public void RpcWrong()
    {
        button.AniWrong();
    }
}
