using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Security.Cryptography;

public class Puzzle_1_Net : NetworkBehaviour
{
    [SerializeField] Transform partsContainer;
    [SerializeField] Transform itemContainer;
    [SerializeField] Puzzle_1_HintScreen hintScreen;

    
     private string[] puzzle_1_Items = new string[] { 
        "Puzzle_1_Item (1)", 
        "Puzzle_1_Item (2)", 
        "Puzzle_1_Item (3)",
        "Puzzle_1_Item (4)",
        "Puzzle_1_Item (5)",
        "Puzzle_1_Item (6)"
    };

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
            if (!isServer) return;
            int num = Random.Range(1, 7);
            while (previousNumber == num) num = Random.Range(1, 7);
            previousNumber = num;
            answer += previousNumber.ToString();

        GameObject obj = Managers.Stage.CmdBatchObject(puzzle_1_Items[previousNumber - 1]);

        obj.transform.position = itemPot;
        obj.transform.SetParent(itemContainer);
        //puzzle.Net_CreateParts(Puzzle,partPot, previousNumber, index);
        Puzzle_1_Parts parts = Managers.Stage.CmdBatchObject("Puzzle_1_Parts").GetComponent<Puzzle_1_Parts>();

        parts.transform.position = partPot;
        //parts.transform.SetParent(partsContainer);
        parts.Settting(Puzzle, previousNumber, index);

        uint netId = parts.GetComponent<NetworkIdentity>().netId;
        RpcSetParent(netId);
    }

    [ClientRpc]
    private void RpcSetParent(uint netId)
    {
        // netId를 통해 현재 클라이언트에서 해당 오브젝트를 찾는다
        if (NetworkClient.spawned.TryGetValue(netId, out NetworkIdentity identity))
        {
            // partsContainer가 클라이언트 쪽에서도 동일한 Transform(씬 오브젝트나 싱글톤 매니저 등)
            identity.transform.SetParent(partsContainer);

            Debug.Log($"[ClientRpc] {identity.name} 오브젝트를 {partsContainer.name}의 자식으로 설정.");
        }
        else
        {
            Debug.LogWarning($"[ClientRpc] netId({netId})로 Spawn된 오브젝트를 찾지 못했습니다.");
        }
    }

    //[ClientRpc]
    //public void RpcCreatePuzzle_Item(
    //    int randomNumber,
    //    int index,
    //    Vector2 itemPot,
    //    Vector2 partPot)
    //{
    //    Debug.Log("1");
    //    GameObject obj =Managers.Stage.CmdBatchObject(puzzle_1_Items[randomNumber - 1]);
    //    obj.transform.position= itemPot;
    //    obj.transform.SetParent(itemContainer);
    //    Debug.Log("2");
    //    puzzle.Net_CreateParts(partPot,randomNumber,index);

    //}

    [Server]
    public void Server_SetHintPosition(bool isHint, Vector2 position)
    {
        if(!isServer) return;
        Debug.Log("1");
        //RpcSetHint();
  
    }


    [ClientRpc]
    public void RpcSetHint(bool isHint,Vector2 position)
    {
        Debug.Log("2");
        //if (isHint)
        //{
        //    hintScreen.gameObject.SetActive(true);
        //    hintScreen.transform.position = position;
        //    hintScreen.SetHint(answer);
        //}
        //else
        //{
        //    hintScreen.gameObject.SetActive(false);
        //}
   
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
