using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Random = UnityEngine.Random;
using System;
using UnityEditor.Experimental.GraphView;

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

    private uint Puzzle_netId 
    {
        get
        {
            return Puzzle.GetComponent<NetworkIdentity>().netId;
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

    [SyncVar] private int previousNumber;
    [SyncVar] private string answer;

    private List<Part> partsList;
    private List<Item> itemsList;

    #region Server


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

        //item
        GameObject obj = Managers.Stage.CmdBatchObject(puzzle_1_Items[previousNumber - 1]);

        obj.transform.SetParent(Puzzle.transform.GetChild(1));
        obj.transform.position = itemPot;

        Server_SetItems(GetNetId(obj), itemPot); //Server Data Save

        //parts
        Puzzle_1_Parts parts = Managers.Stage.CmdBatchObject("Puzzle_1_Parts").GetComponent<Puzzle_1_Parts>();
         parts.transform.SetParent(Puzzle.transform.GetChild(0));
         parts.transform.position = partPot;
         parts.Settting(Puzzle, previousNumber, index);

        Server_SetParts(GetNetId(parts.gameObject), previousNumber, index,partPot);  //Server Data Save

    }

    private void Server_SetParts(uint partNetId,int answer,int index,Vector2 position)
    {
        if (partsList == null) partsList = new();
        partsList.Add(new Part(Puzzle_netId, partNetId, answer, index,position));
    }
    private void Server_SetItems(uint partNetId, Vector2 position)
    {
        if (itemsList == null) itemsList = new();
        itemsList.Add(new Item(Puzzle_netId, partNetId, position));
    }

    [Server]
    private void Server_SetPuzzleSetting()
    {
        StartCoroutine(Delay(() => { Rpc_SetPuzzleSetting(partsList,itemsList); }));
       
    }
   

    [Server]
    public void Server_SetHintPosition(bool isHint, Vector2 position)
    {
        if (!isServer) return;
        StartCoroutine(Delay(() => {
            RpcSetHint(isHint, position);
        }));

    }

   
 
    #endregion





    #endregion


    #region Init_Cmd
    [Command(requiresAuthority = false)]
    public void Cmd_SetPuzzleSetting()
    {
        Server_SetPuzzleSetting();
    }

    #endregion

    #region Init_Rpc
    [ClientRpc]
    private void Rpc_SetPuzzleSetting(List<Part> parts, List<Item> items)
    {
        if (isServer) return;
        NetworkIdentity puzzle = Client_GetNetworkIdentity(Puzzle_netId);
        foreach (var part in parts)
        {
            NetworkIdentity netPart = Client_GetNetworkIdentity(part.netId);

            Transform parent = puzzle.gameObject.transform.GetChild(0);
            Transform partTr = netPart.gameObject.transform;

            partTr.SetParent(parent);
            partTr.position = part.position;

            netPart.GetComponent<Puzzle_1_Parts>().Settting(puzzle.GetComponent<Puzzle_1>(), part.answer, part.index);
        }

        foreach(var item in items)
        {
            NetworkIdentity netitem = Client_GetNetworkIdentity(item.netId);

            Transform parent = puzzle.gameObject.transform.GetChild(1);
            Transform itemTr = netitem.gameObject.transform;

            itemTr.SetParent(parent);
            itemTr.position = item.position;
        }

    }
    #endregion


    public override void OnStartClient()
    {
        base.OnStartClient();
        Cmd_SetPuzzleSetting();
    }


    IEnumerator Delay(Action action)
    {
        yield return new WaitForSeconds(0.1f);
            action();
    }


    //[ClientRpc]
    //private void RpcSetParent(GameObject target,int puzzleContainerIndex)
    //{
    //    Debug.Log("Rpc 1");
    //    NetworkIdentity target_Identity = GetNetworkIdentity(target);
    //    if(target_Identity == null){
    //        Debug.Log("Can't found NetworkIdentity");
    //        return;
    //    }
    //    Debug.Log("Rpc 2");

    //    NetworkIdentity puzzle = GetNetworkIdentity(Puzzle_netId);
    //    if(puzzle == null) return;
    //    Debug.Log("Rpc 3");

    //    Transform puzzleTr = puzzle.gameObject.transform;   
    //    Transform targetTr = puzzleTr.GetChild(puzzleContainerIndex);

    //    target_Identity.gameObject.transform.SetParent(targetTr);

    //    Debug.Log("RPC 4");
    //}

    //[ClientRpc]
    //private void RpcPartsSetting(GameObject parts,int answer,int index)
    //{
    //    NetworkIdentity identity = GetNetworkIdentity(parts);
    //    Puzzle_1_Parts target = identity.GetComponent<Puzzle_1_Parts>();

    //    target.Settting(Puzzle,answer,index);
    //}



    [ClientRpc]
    public void RpcSetHint(bool isHint,Vector2 position)
    {
        //NetworkIdentity main =GetNetworkIdentity(Puzzle_netId);

        //Puzzle_1 puzzle = main.GetComponent<Puzzle_1>();
        Puzzle.Net_SetHint(isHint, answer,position);
        //puzzle.Net_SetHint(isHint,answer,position);
      
    }




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




    #region  Util
    private NetworkIdentity GetNetworkIdentity(GameObject obj,Func<uint,NetworkIdentity> action)
    {
        if(obj.TryGetComponent(out NetworkIdentity component))
        {
            return action(component.netId);
        }

        return null;
    }
    private NetworkIdentity Server_GetNetworkIdentity(uint netId)
    {
          if(NetworkServer.spawned.TryGetValue(netId, out NetworkIdentity identity))
          {
            return identity;
          }
        return null;
    }
    private NetworkIdentity Client_GetNetworkIdentity(uint netId)
    {
        if (NetworkClient.spawned.TryGetValue(netId, out NetworkIdentity identity))
        {
            return identity;
        }
        return null;
    }

    private uint GetNetId(GameObject obj)
    {
        return obj.GetComponent<NetworkIdentity>().netId;
    }
    #endregion

}

public struct Part
{
    public uint puzzleNetId;
    public uint netId;
    public int answer;
    public int index;
    public Vector2 position;

    public Part(uint puzzleNetId,uint netId, int answer, int index, Vector2 position )
    {
        this.puzzleNetId = puzzleNetId;
        this.netId = netId;
        this.answer = answer;
        this.index = index;
        this.position = position;
    }

}
public struct Item
{
    public uint puzzleNetId;
    public uint netId;
    public Vector2 position;

    public Item(uint puzzleNetId,uint netId,Vector2 position)
    {
        this.puzzleNetId=puzzleNetId;
        this.netId = netId;
        this.position = position;
    }

}