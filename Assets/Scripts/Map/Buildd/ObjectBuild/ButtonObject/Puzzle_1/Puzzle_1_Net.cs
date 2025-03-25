using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Random = UnityEngine.Random;
using System;
using UnityEngine.Animations;
using UnityEngine.InputSystem;



public class Puzzle_1_Net : NetworkBehaviour
{
    [SerializeField] Transform partsContainer;
    [SerializeField] Transform itemContainer;
    [SerializeField] Puzzle_1_HintScreen hintScreen;
    [Space(20)]
    [SerializeField] Puzzle_1_LeftTrigger leftTrigger;
    [SerializeField] Puzzle_1_RightTrigger rightTrigger;

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

    #region -------------------------------------------Animation Sync

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
        if(newVal == 0){
            button.Net_Reset();
            button.SetAnimation(newVal);

        }else{
            button.SetAnimation(newVal);
        }
        
    }
    [Command(requiresAuthority = false)]
    public void CmdCorrect()
    {
        RpcCorrect();
    }
    [ClientRpc]
    public void RpcCorrect()
    {
        button.Net_Correct();
    }

    #endregion

    #region -------------------------------------------Init Sync

    private int previousNumber;
    // [SyncVar] private string answer;
    private string answer;

    private List<Part> partsList;
    private List<Item> itemsList;
    private List<Item> dummyItemList;
    public Hint hint;
    
    public bool onSync; //-------------------------------------------------250307

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
        GameObject obj = Managers.Stage.CmdBatchObject(puzzle_1_Items[previousNumber - 1]);//Poozing

        obj.transform.SetParent(Puzzle.transform.GetChild(1));
        obj.transform.position = itemPot;
        obj.GetComponent<Puzzle_1_Item>().Server_SetOrgPosition(itemPot);

        Server_SetItems(GetNetId(obj), itemPot); //Server Data Save

        //parts
        Puzzle_1_Parts parts = Managers.Stage.CmdBatchObject("Puzzle_1_Parts").GetComponent<Puzzle_1_Parts>();//Pooling
        parts.transform.SetParent(Puzzle.transform.GetChild(0));
         parts.transform.position = partPot;
         parts.Settting(Puzzle, previousNumber, index);
        Puzzle.SetPart(parts);

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
    private void Server_SetDummyItem(uint itemNetId,Vector2 position)
    {
        if(dummyItemList == null) dummyItemList = new();
        dummyItemList.Add(new Item(Puzzle_netId,itemNetId,position));
    }

    [Server]
    public void Server_SetHintSetting()
    {
        var hint = Puzzle.GetHintData();
        this.hint = new Hint(hint.isHint, answer, hint.position);

        if(this.hint.isHint)
        {
            hintScreen.gameObject.SetActive(true);
            hintScreen.transform.position = hint.position;
            hintScreen.SetHint(this.hint.answer);
        }


        onSync  = true;
        
        Rpc_SetPuzzleSetting(Puzzle.ButtonObjectData.position, partsList, itemsList, this.hint);

        if(dummyItemList.Count > 0)
        Rpc_SetDummyItem(dummyItemList);
    }

    [Server]
    public void Server_Create_DummyItem(Vector2 dummyItemPot)
    {
        int randomNum  =Random.Range(1,7);
        GameObject obj = Managers.Stage.CmdBatchObject(puzzle_1_Items[randomNum-1]);//Poozing

        obj.transform.SetParent(Puzzle.transform.GetChild(1));
        obj.transform.position = dummyItemPot;
        obj.GetComponent<Puzzle_1_Item>().Server_SetOrgPosition(dummyItemPot);

        Server_SetDummyItem(GetNetId(obj),dummyItemPot);
    }

    #endregion





    #endregion


    #region Init_Cmd

    //[Server]
    //private void Server_Sync()
    //{
    //    Rpc_SetPuzzleSetting(Puzzle.ButtonObjectData.position,partsList,itemsList,hint);
        
    //}
    //[Command]
    //public void Cmd_Sync()
    //{
    //    Server_Sync();
    //}
    #endregion

    #region Init_Rpc
    [ClientRpc]
    private void Rpc_SetPuzzleSetting(Vector2 mainPosition,List<Part> parts, List<Item> items,Hint hint)
    {
        if (onSync) return;
        //data sync
        transform.position = mainPosition;
        partsList = parts;
        itemsList = items;
        this.hint = hint;
        //data sync

        NetworkIdentity puzzle = Client_GetNetworkIdentity(Puzzle_netId);
        Puzzle_1 puzzle_1 = puzzle.gameObject.GetComponent<Puzzle_1>();
        Puzzle_1_Net puzzle_net = puzzle.gameObject.GetComponent<Puzzle_1_Net>();

        foreach (var part in parts)
        {
            NetworkIdentity netPart = Client_GetNetworkIdentity(part.netId);

            Transform partTr = netPart.gameObject.transform;

            partTr.SetParent(puzzle_net.partsContainer);
            partTr.position = part.position;

            netPart.GetComponent<Puzzle_1_Parts>().Settting(puzzle.GetComponent<Puzzle_1>(), part.answer, part.index);

            puzzle_1.SetPart(partTr.GetComponent<Puzzle_1_Parts>());
        }

        foreach (var item in items)
        {
            NetworkIdentity netitem = Client_GetNetworkIdentity(item.netId);

            Transform itemTr = netitem.gameObject.transform;

            itemTr.SetParent(puzzle_net.itemContainer);
            itemTr.position = item.position;

        }


        if (hint.isHint)
        {

            hintScreen.gameObject.SetActive(true); ;
  
            hintScreen.transform.position = hint.position;
       
            hintScreen.SetHint(hint.answer);
        }
        else
        {
            hintScreen.gameObject.SetActive(false);
        }

        onSync = true;

    }

    [ClientRpc]
    private void Rpc_SetDummyItem(List<Item> list)
    {
        NetworkIdentity puzzle = Client_GetNetworkIdentity(Puzzle_netId);
        Puzzle_1 puzzle_1 = puzzle.gameObject.GetComponent<Puzzle_1>();
        Puzzle_1_Net puzzle_net = puzzle.gameObject.GetComponent<Puzzle_1_Net>();

        dummyItemList = list;
        foreach(var item in list)
        {
            NetworkIdentity netitem = Client_GetNetworkIdentity(item.netId);
            Transform itemTr = netitem.gameObject.transform;

            itemTr.SetParent(puzzle_net.itemContainer);
            itemTr.position = item.position;
        }
    }

    #endregion



    //IEnumerator Delay()
    //{
    //    while (!NetworkClient.ready) 
    //    {
    //        Debug.Log("Wait ");
    //        yield return null;
    //    }
       
    //    Cmd_Sync();
    //}


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
        button.Net_Wrong();
    }
 


    [Command(requiresAuthority = false)]
    public void CmdReset()
    {
        //RpcReset();
        Sever_Reset();
    }
    //[ClientRpc]
    //public void RpcReset()
    //{
    //    Sever_Reset();
    //}


    #region -------------------------------------------Hint Screen
    [Command(requiresAuthority = false)]
    public void Cmd_HintScreen_Correct(){
        if(hintScreen.gameObject.activeSelf)
        Rpc_HintScreen_Correct();
    }
    [ClientRpc]
    public void Rpc_HintScreen_Correct(){
        if(hintScreen.gameObject.activeSelf)
        Puzzle.Net_HintScreen_Correct();
    }
    [Command(requiresAuthority = false)]
    public void Cmd_HintScreen_False(){
        if(hintScreen.gameObject.activeSelf)
        Rpc_HintScreen_False();
    }
    [ClientRpc]
    public void Rpc_HintScreen_False(){
        if(hintScreen.gameObject.activeSelf)
        Puzzle.Net_HintScreen_False();
    }
    #endregion


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




    #region --------------------------------Trigger

    public bool onActive;
    [ReadOnly]
    public GameObject airObject;

    [Command(requiresAuthority = false)] //Left : true, Right : false
    public void Cmd_Interact(uint playerNetworkId,bool leftOrRight,bool onOff)
    {
            if(NetworkClient.spawned.TryGetValue(playerNetworkId,out NetworkIdentity identity))
            {
                TRpc_Interact(identity.connectionToClient,identity.gameObject,leftOrRight,onOff);
            }
    }
    [TargetRpc]
    private void TRpc_Interact(NetworkConnection _,GameObject player,bool leftOrRight,bool onOff)
    {
       HoldAndRecover(player,leftOrRight,onOff);
    }

    private void HoldAndRecover(GameObject player,bool leftOrRight,bool onOff)
    {
        if(onOff)
        {
            Hold(player,leftOrRight);

        }else
        {
           Recover(player);
        }
        
    }
    private void Hold(GameObject player,bool leftOrRight)
    {
        onActive = true;
        airObject = player;
        
        var sm = player.GetComponent<PlayerSM>();
        sm.canMovable = false;
        Fix_AirGun_Direct(player,leftOrRight);
        
        //Input
        var input = Managers.Game.playerInput;
        input.playerActions.Action.started += OnHoldAirGun;
        input.playerActions.Action.canceled += OnRecoverAirGun;
        //Input

        //Show UI

        //Show UI

        //Air ready for blow animation
        
        //Air ready for blow animation

        Transform hold_Pivot = leftOrRight ? leftTrigger.Hold_Pivot : rightTrigger.Hold_Pivot; //right
        Connection(player,hold_Pivot);

        sm.deathEvent += Event_Recover;

    }
   
    private void Fix_AirGun_Direct(GameObject player,bool leftOrRight)
    {
        if(leftOrRight)
        {
            //Left
            Debug.Log("Set Direction to Air [Left]");
        }else
        {
            //Right
            Debug.Log("Set Direction to Air [Right]");
        }
        
    }
    private void Recover(GameObject player)
    {
        onActive = false;
        
        var sm = player.GetComponent<PlayerSM>();
        sm.canMovable = true;

        //Input
        var input = Managers.Game.playerInput;
        input.playerActions.Action.started -= OnHoldAirGun;
        input.playerActions.Action.canceled -= OnRecoverAirGun;
        //Input

        //Hide UI

        //Hide UI

        //Recover Animation

        //Recover Animation

        Disconnection(player);
        sm.deathEvent -= Event_Recover;
        
        airObject = null;

    }
    private void Event_Recover()
    {
        if(airObject)
        {
            HoldAndRecover(airObject,false,false);
        }
    }
    private void OnRecoverAirGun(InputAction.CallbackContext context)
    {
        if(airObject)
        {
            var air = airObject.GetComponent<AirSM>();
            air.canControl =true;

            //Stop Air Blow Animation -> ready to blow Animation

            //Stop Air Blow Animation -> ready to blow Animation
        }
    }
    private void OnHoldAirGun(InputAction.CallbackContext context)
    {
        if(airObject)
        {
            var air = airObject.GetComponent<AirSM>();
            air.canControl =false;

            //ready to blow Animation -> Air Blow Animation

            //ready to blow Animation -> Air Blow Animation
        }
    }

    #region UI
    [Command(requiresAuthority = false)]
    public void Cmd_ShowE(GameObject player, bool leftOrRight,bool onOff) //left : true, right : false
    {
        if (player.TryGetComponent(out NetworkIdentity identity))
        {
            TRpc_ShowE(identity.connectionToClient, leftOrRight,onOff);
        }
    }
    [TargetRpc]
    private void TRpc_ShowE(NetworkConnection conn,bool leftOrRight, bool onOff)
    {
        if (leftOrRight)
        {
            //left
            leftTrigger.ShowE(onOff);
        }
        else
        {
            //right
            rightTrigger.ShowE(onOff);
        }
    }
    #endregion
   
    private void Connection(GameObject player,Transform hold_Pivot)
    {
        if(player.GetComponent<ParentConstraint>()) return;

        ParentConstraint constraint = player.AddComponent<ParentConstraint>();
        SetParentConstraint(constraint,hold_Pivot);
    }
    private void Disconnection(GameObject player)
    {
        if(player.TryGetComponent(out ParentConstraint component))
        {
            Destroy(component);
        }
    }
    private void SetParentConstraint(ParentConstraint constraint,Transform parent)
    {
        ConstraintSource source = new ConstraintSource
        {
            sourceTransform = parent,
            weight = 1
        };
        constraint.AddSource(source);

        constraint.translationAtRest = transform.localPosition;
        constraint.translationOffsets = new Vector3[constraint.sourceCount];
        constraint.constraintActive = true;

        constraint.locked = true;
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
[Serializable]
public struct Hint
{
    public bool isHint;
    public string answer;
    public Vector2 position;
    public Hint(bool isHint,string answer,Vector2 position)
    {
        this.isHint = isHint;
        this.answer = answer;
        this.position = position;
    }
}
