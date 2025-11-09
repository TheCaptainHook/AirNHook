using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Random = UnityEngine.Random;
using System;
using UnityEngine.Animations;
using UnityEngine.InputSystem;
using System.Collections;




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
            if(Puzzle.TryGetComponent(out NetworkIdentity identity))
            {
                return identity.netId;
            }
            return 99999;
            
        }
    }


    [SerializeField] Puzzle_1_Button button;

    private WaitForSeconds waitForSeconds;

    private void Awake()
    {
        waitForSeconds = new WaitForSeconds(1);
    }
    void OnDisable()
    {
        StopAllCoroutines();

        if (audioSourceController != null)
        {
            Managers.Sound.StopSound(audioSourceController);
        }

    }

    #region -------------------------------------------Init Sync

    private int previousNumber;
    // [SyncVar] private string answer;
    private string answer;

    private List<Part> partsList;
    private List<Item> itemsList;
    private List<Item> dummyItemList;
    public Hint hint;

    public bool onSync; //-------------------------------------------------250307


    #region  Clean
    
    public void Clean()
    {
        answer = "";
        // hintScreen.Clean();
        hintScreen.gameObject.SetActive(false);


        foreach (var part in partsList)
        {
            var go = Client_GetNetworkIdentity(part.netId).gameObject;
            Managers.Pooling.N_ReleaseToPool(go);
            go.GetComponent<Puzzle_1_Parts>().Clean();
        }
        
        foreach (var item in itemsList)
        {
            var go = Client_GetNetworkIdentity(item.netId).gameObject;
            go.GetComponent<Puzzle_1_Item>().Clean();
            
            Managers.Pooling.N_ReleaseToPool(go);
        }

        if (dummyItemList?.Count > 0)
        {
            foreach (var item in dummyItemList)
            {
                var go = Client_GetNetworkIdentity(item.netId).gameObject;
                Managers.Pooling.N_ReleaseToPool(go);
            }

        }
        chargingRate = 0;
        onCheckAnswerTrue = false;
        onWrongPrograss = false;
        onCorrect = false;
    }
    
    #endregion
    
    #region Server

    //==========Refactoring 1105
    /**
    1. asnwer Clean
    2. itemList,partList,dummy item list Clean
    3. hint Clean
    **/

    //==========Refactoring 1105
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

        //====Item
        // GameObject obj = Managers.Stage.CmdBatchObject(puzzle_1_Items[previousNumber - 1]);//Poozing
        GameObject obj = Managers.Stage.ServerBatchObejct(puzzle_1_Items[previousNumber - 1]);
        obj.SetActive(true);
        
        obj.transform.SetParent(Puzzle.transform.GetChild(1));
        obj.transform.position = itemPot;

        var item = obj.GetComponent<Puzzle_1_Item>();
        item.Set_Item();

        item.Server_SetOrgPosition(itemPot);

        Server_SetItems(GetNetId(obj), itemPot); //Server Data Save
        //====Item

        //====Parts
        // Puzzle_1_Parts parts = Managers.Stage.CmdBatchObject("Puzzle_1_Parts").GetComponent<Puzzle_1_Parts>();//Pooling
        Puzzle_1_Parts parts = Managers.Stage.ServerBatchObejct("Puzzle_1_Parts").GetComponent<Puzzle_1_Parts>();//Pooling
        parts.gameObject.SetActive(true);

        parts.transform.SetParent(Puzzle.transform.GetChild(0));
        parts.transform.position = partPot;
        parts.Settting(Puzzle, previousNumber, index);
        Puzzle.SetPart(parts);

        Server_SetParts(GetNetId(parts.gameObject), previousNumber, index, partPot);  //Server Data Save
        //====Parts
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

        if(dummyItemList?.Count > 0)
        Rpc_SetDummyItem(dummyItemList);
    }

    [Server]
    public void Server_Create_DummyItem(Vector2 dummyItemPot)
    {
        int randomNum = Random.Range(1, 7);
        // GameObject obj = Managers.Stage.CmdBatchObject(puzzle_1_Items[randomNum-1]);//Poozing
        GameObject obj = Managers.Stage.ServerBatchObejct(puzzle_1_Items[randomNum-1]);//Poozing

        obj.transform.SetParent(Puzzle.transform.GetChild(1));
        obj.transform.position = dummyItemPot;
        obj.SetActive(true);

        var item = obj.GetComponent<Puzzle_1_Item>();

        item.Server_SetOrgPosition(dummyItemPot);
        item.GetComponent<Puzzle_1_Item>().Set_Item();

        Server_SetDummyItem(GetNetId(obj),dummyItemPot);
    }

    #endregion





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
            netPart.gameObject.SetActive(true);

            Transform partTr = netPart.gameObject.transform;

            partTr.SetParent(puzzle_net.partsContainer);
            partTr.position = part.position;

            netPart.GetComponent<Puzzle_1_Parts>().Settting(puzzle.GetComponent<Puzzle_1>(), part.answer, part.index);
            puzzle_1.SetPart(partTr.GetComponent<Puzzle_1_Parts>());
        }

        foreach (var item in items)
        {
            NetworkIdentity netitem = Client_GetNetworkIdentity(item.netId);
            netitem.gameObject.SetActive(true);
            netitem.GetComponent<Puzzle_1_Item>().Set_Item();

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
        // Puzzle_1 puzzle_1 = puzzle.gameObject.GetComponent<Puzzle_1>();
        Puzzle_1_Net puzzle_net = puzzle.gameObject.GetComponent<Puzzle_1_Net>();

        dummyItemList = list;
        foreach(var item in list)
        {
            NetworkIdentity netitem = Client_GetNetworkIdentity(item.netId);
            netitem.gameObject.SetActive(true);
            
            netitem.GetComponent<Puzzle_1_Item>().Set_Item();

            Transform itemTr = netitem.gameObject.transform;

            itemTr.SetParent(puzzle_net.itemContainer);
            itemTr.position = item.position;
        }
    }

    #endregion


    public bool onCorrect;
    public bool onWrongPrograss;
    private float defaultChargingRate = 0.01f;
    [SyncVar] public float chargingRate;

    private Coroutine bullonRecoverCoroutine;
   
    IEnumerator BullonRecoverCo()
    {
        yield return waitForSeconds;

        while(0 < chargingRate && chargingRate < 1)
        {
            chargingRate -= Time.fixedDeltaTime;
            Rpc_Ballon_Animation_Charging(chargingRate);
            yield return null;
        }
       
    }
    bool onCheckAnswerTrue;

    [Server]
    private void Server_Puzzle_ChargingControl()
    {
        if(onCorrect) return;
        if(onWrongPrograss) return;

        //Recover Coroutine
        if(bullonRecoverCoroutine != null) StopCoroutine(bullonRecoverCoroutine);
        bullonRecoverCoroutine = StartCoroutine(BullonRecoverCo());

        chargingRate += defaultChargingRate;
        
        //Ballon Animation Rpc
        if(chargingRate <1) Rpc_Ballon_Animation_Charging(chargingRate);
        
        if(chargingRate >=1)
        {
            //Check Answer
            if(Puzzle.CheckAnswer())
            {
                if (onCheckAnswerTrue) return;
                onCheckAnswerTrue = true;
                //Activation (Only Server)
                Puzzle.Net_Activation();
                //Correct
                Rpc_Correct();

            }
            else
            {
                Rpc_Wrong();
                StartCoroutine(WrongPrograssCo());
            }
        }
    }
    [Command(requiresAuthority = false)]
    public void CmdCharging()
    {
        Server_Puzzle_ChargingControl();
    }
    #region  Sound
    private AudioSourceController audioSourceController;
    private AudioSource audioSource
    {
        get
        {
            if (audioSourceController == null) return null;
            else return audioSourceController.GetAudioSource();
        }
    }
    [Command(requiresAuthority = false)]
    public void Cmd_SoundStop()
    {
        Server_SoundStop();
    }
    [Server]
    private void Server_SoundStop()
    {
        Rpc_SoundStop();
    }
    [ClientRpc]
    private void Rpc_SoundStop()
    {
        if (audioSource != null) audioSource.volume = 0;
    }
    #endregion

    [ClientRpc]
    private void Rpc_Ballon_Animation_Charging(float rate)
    {
        //Sound
        if (audioSourceController == null)
        {
            audioSourceController = Managers.Sound.PlaySound3D(GlobalText.PUZZLE_BALLON_INFLATE, transform.position, 1, true);
        }

        audioSource.volume = rate;

        //Sound

        button.SetAnimation(rate);
            
    }

    [ClientRpc]
    private void Rpc_Ballon_Animation_Explode()
    {
        button.SetAnimation_Explode();
    }
    public void ExplodeSound()
    {
        if (audioSourceController != null)
        {
            var clip = Managers.Sound.GetAudioClip(GlobalText.PUZZLE_BALLON_EXPLODE);
            audioSourceController.ClipChange(clip, false);
            audioSourceController = null;
        }
    }
   
    #region Correct 
    [ClientRpc]
    private void Rpc_Correct()
    {
        if (audioSourceController != null)
        {
            Managers.Sound.StopSound(audioSourceController);
            audioSourceController = null;
        }
        onCorrect = true;
        HintScreen_Correct();
    }
    #endregion

    #region  Wrong
    [ClientRpc]
    private void Rpc_Wrong()
    {
        Managers.AcManager.CallPlayer_Puzzle_Wrong();
        Puzzle.Boom();
        HintScreen_False();
    }
    
    private IEnumerator WrongPrograssCo()
    {
        onWrongPrograss = true;
        Rpc_Ballon_Animation_Explode();
        //Bullon Explode
        yield return waitForSeconds;
        chargingRate = 0;
        Rpc_Ballon_Animation_Charging(chargingRate);
        onWrongPrograss = false;
    }
    #endregion

    

    #region -------------------------------------------Hint Screen
 
    public void HintScreen_Correct(){
        if(hintScreen.gameObject.activeSelf)
        Puzzle.Net_HintScreen_Correct();
    }
 
    
    public void HintScreen_False(){
        if(hintScreen.gameObject.activeSelf)
        Puzzle.Net_HintScreen_False();
    }
    #endregion


    #region  Util
 
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

    public void HoldAndRecover(GameObject player,bool leftOrRight,bool onOff)
    {
        if(onOff)
        {
            Hold(player,leftOrRight);
            onActive = true;

        }else
        {
           Recover(player);
            onActive = false;
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
        input.playerActions.SubAction.Disable();
        input.playerActions.Action.started += OnHoldAirGun;
        input.playerActions.Action.canceled += OnRecoverAirGun;
        //Input

        //Show UI

        //Show UI

        //Air ready for blow animation
        var air = player.TryGetComponent(out AirSM airSm);
        if(air) airSm.animator.SetBool(GlobalText.AIR_BALLON_USINGBTN_STRING,true);
        //Air ready for blow animation

        Transform hold_Pivot = leftOrRight ? leftTrigger.Hold_Pivot : rightTrigger.Hold_Pivot; //right
        Connection(player,hold_Pivot);

        sm.deathEvent += Event_Recover;

    }

    private void Fix_AirGun_Direct(GameObject player,bool leftOrRight)
    {
        var air = player.TryGetComponent(out AirSM airSm);
        if(!air) return;

        var charPivot = airSm.transform.GetChild(2);
        var weaponPivot = airSm.transform.GetChild(3);
        
        if(leftOrRight)
        {
            //Left
            charPivot.rotation = Quaternion.Euler(0,0,0);
            weaponPivot.rotation = Quaternion.Euler(0,0,0);
        }else
        {
            //Right
            charPivot.rotation = Quaternion.Euler(0,-180,0);
            weaponPivot.rotation = Quaternion.Euler(0,180,0);
        }
        
    }
    private void Recover(GameObject player)
    {
        onActive = false;
        
        var sm = player.GetComponent<PlayerSM>();
        sm.canMovable = true;

        //Input 
        var input = Managers.Game.playerInput;
        input.playerActions.SubAction.Enable();
        input.playerActions.Action.started -= OnHoldAirGun;
        input.playerActions.Action.canceled -= OnRecoverAirGun;
        //Input

        //Hide UI

        //Hide UI

        //Recover Animation
        var air = player.TryGetComponent(out AirSM airSm);
        if(air) airSm.animator.SetBool(GlobalText.AIR_BALLON_USINGBTN_STRING,false);
        //Recover Animation

        Disconnection(player);
        sm.deathEvent -= Event_Recover;


        airObject.GetComponent<AirSM>()._airGunMountObj = null;
        
        airObject = null;

    }
    private void Event_Recover(DamageType damageType = DamageType.Default)
    {
        if(airObject)
        {
            leftTrigger.Clean();
            rightTrigger.Clean();

            HoldAndRecover(airObject, false, false);
            
        }
    }
    private void OnRecoverAirGun(InputAction.CallbackContext context)
    {
        if(airObject)
        {
            var air = airObject.GetComponent<AirSM>();
            air.canControl =true;

            //Stop Air Blow Animation -> ready to blow Animation
            air.animator.SetFloat(GlobalText.AIR_BALLON_EXHAILING_STRING,0);
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
            air.animator.SetFloat(GlobalText.AIR_BALLON_EXHAILING_STRING,1);
            //ready to blow Animation -> Air Blow Animation
        }
    }

   
    private void Connection(GameObject player,Transform hold_Pivot)
    {
        if (player.TryGetComponent(out ParentConstraint parentConstraint))
        {
            if (parentConstraint.sourceCount > 0)
            {
                parentConstraint.RemoveSource(0);
            }
            SetParentConstraint(parentConstraint, hold_Pivot);
        }

    }
    private void Disconnection(GameObject player)
    {
        if(player.TryGetComponent(out ParentConstraint component))
        {
            if (component.sourceCount > 0)
            {
                component.RemoveSource(0);
            }
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
