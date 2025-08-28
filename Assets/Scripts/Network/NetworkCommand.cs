using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class NetworkCommand : NetworkBehaviour
{
    #region SetUp
    private void Awake()
    {
        Managers.Command = this;
        // waitSecond = new WaitForSeconds(5);
    }
    #endregion

    #region StageChange
    [Space(20)]
    [Header("Stage Change Field")]
    //--------------Server
    // private WaitForSeconds waitSecond;
    private Queue<Action> changeStageQueue = new();
    
    //--------------Server
    private Coroutine waitChangeStageCoroutine; //Use only Server
    //---------------------------------
    [ReadOnly]
    [SyncVar] public int isCompleteMoveStageCount;
    [ReadOnly]
    [SyncVar] public int currentClientConnectionCount = 0;

    [Server]
    public void Server_UpdateCurClientConnectionCount()
    {
        currentClientConnectionCount = NetworkServer.connections.Count;
        isCompleteMoveStageCount = 0;
    }
    public bool AllReadyClient()
    {
        return isCompleteMoveStageCount == currentClientConnectionCount;
    }

    [Command(requiresAuthority = false)]
    public void Cmd_IsCompleteMoveStage()
    {
        isCompleteMoveStageCount++;
    }
    private int ClientCount => NetworkServer.connections.Count;
    //---------------------------------
    private IEnumerator Wait_ChangeStage() //Server
    {
        var uiOption = Managers.UI.GetUI<UI_Option>().GetComponent<UI_Option>();
        
        uiOption.HoldAndReleaseLobby_StageRestartBtn(true);
        
        while(changeStageQueue.Count>0)
        {
            isCompleteMoveStageCount = 0;
            var action = changeStageQueue.Dequeue();
            action?.Invoke();

            if(ClientCount > 1)
            yield return new WaitUntil(()=> isCompleteMoveStageCount == currentClientConnectionCount);
           
        }

        uiOption.HoldAndReleaseLobby_StageRestartBtn(false);
        waitChangeStageCoroutine = null;

    }
    [Server]
    private void Server_ChangeStage(string value)
    {
        changeStageQueue.Enqueue(()=>RpcChangeStage(value));

        if(waitChangeStageCoroutine == null)
        {
            waitChangeStageCoroutine = StartCoroutine(Wait_ChangeStage());
        }
    }
    private Coroutine server_waitChangeStageCoroutine;
    [Server]
    public void Server_ChangeStage_Use_ExitDoor()
    {
        if (server_waitChangeStageCoroutine == null)
        {
            server_waitChangeStageCoroutine = StartCoroutine(Server_WaitChangeStageCoroutine());
        }
    }
    IEnumerator Server_WaitChangeStageCoroutine()
    {
        var uiOption = Managers.UI.GetUI<UI_Option>().GetComponent<UI_Option>();
        uiOption.HoldAndReleaseLobby_StageRestartBtn(true);
        yield return new WaitForSeconds(10);
        yield return new WaitUntil(() => isCompleteMoveStageCount == 2);
        isCompleteMoveStageCount = 0;
        server_waitChangeStageCoroutine = null;
        uiOption.HoldAndReleaseLobby_StageRestartBtn(false);
    }

    // [Command(requiresAuthority = false)]
    [Server]
    public void _Server_ChangeStage(string value) //chit Option, [UI,Console,Serve Dissconnection]
    {
        Server_ChangeStage(value);
        // RpcChangeStage(value);

    }



    [ClientRpc]
    private void RpcChangeStage(string value)
    {
        Managers.Stage.stageName = value;
        if (value.Equals("Lobby"))
        {
            Managers.Game.CurrentState = GameState.Lobby;
            MapEditor.Instance.MoveNextStage(value);
        }
        else
        {
            Managers.Game.CurrentState = GameState.Game;
            MapEditor.Instance.MoveNextStage(value);
        }
    }
    #endregion

    #region StageDataCheck
    public Action<string, bool> stageCheckCallback;

    //[Command(requiresAuthority = false)]
    //public void StageDataCheck(string value)
    //{
    //    RpcStageDataCheck(value);
    //}

    //[ClientRpc(includeOwner = false)]
    //private void RpcStageDataCheck(string value)
    //{
    //    var isMapExist = Managers.Data.mapData.mapAllDictionary.ContainsKey(value);
    //    CmdStageDataChecked(value, isMapExist);
        
    //    if (!isMapExist) return;
    //    var stageUI = (UI_StageSelect)Managers.UI.GetUI<UI_StageSelect>();
    //    stageUI.MapSelected(value, true);
    //}

    [Command(requiresAuthority = false)]
    private void CmdStageDataChecked(string mapID, bool value)
    {
        stageCheckCallback?.Invoke(mapID, value);
    }
    #endregion

    #region GrabReleaseItem
    public Action<NetworkIdentity, bool> itemGrabCallback;
    public Action<uint> itemReleaseCallback;
    
    [Command(requiresAuthority = false)]
    public void TryGrabItem(GameObject target, uint itemNetId)
    {
        if (!NetworkClient.spawned.TryGetValue(itemNetId, out var item)) return;

        var conn = target.GetComponent<NetworkIdentity>().connectionToClient;
        
        if (!item.TryGetComponent<IInteractable>(out var interactable) || (interactable is not null && !interactable.CanInteract()))
        {
            GrabItem(conn, itemNetId, false);
            return;
        }
        
        if (!interactable.Interacting(true, target))
        {
            GrabItem(conn, itemNetId, false);
            return;
        }

        if (!ReferenceEquals(Managers.Game.Player, target) || !item.isOwned)
        {
            AssignAuthority(item, conn);
        }
        
        GrabItem(conn, itemNetId, true);
    }
    
    [TargetRpc]
    private void GrabItem(NetworkConnectionToClient conn, uint itemNetId, bool value)
    {
        if (!NetworkClient.spawned.TryGetValue(itemNetId, out var item)) return;
        
        itemGrabCallback?.Invoke(item, value);
    }

    //[Command(requiresAuthority = false)]
    //public void TryReleaseItem(GameObject target, uint itemNetId)
    //{
    //    if (!NetworkClient.spawned.TryGetValue(itemNetId, out var item)) return;
    //    
    //    if (!item.TryGetComponent<IInteractable>(out var interactable)) return;
    //    
    //    //item.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
    //    interactable.Interacting(false);
    //    ReleaseItem(target.GetComponent<NetworkIdentity>().connectionToClient, itemNetId);
    //}

    //[TargetRpc]
    //private void ReleaseItem(NetworkConnectionToClient conn, uint itemNetId)
    //{
    //    itemReleaseCallback?.Invoke(itemNetId);
    //}

    [Command(requiresAuthority = false)]
    public void SyncVelocity(GameObject target, Vector2 velocity)
    {
        target.GetComponent<Rigidbody2D>().velocity = velocity;
        target.GetComponent<Rigidbody2D>().angularVelocity = 0f;
    }
    #endregion

    #region InhaleItem
    public Action<GameObject, bool> itemInhaleCallback;

    [Command(requiresAuthority = false)]
    public void TryInhaleItem(GameObject target, uint itemNetId)
    {
        if (!NetworkClient.spawned.TryGetValue(itemNetId, out var item)) return;

        var conn = target.GetComponent<NetworkIdentity>().connectionToClient;

        if (!item.TryGetComponent<IInhalable>(out var inhalable) || (inhalable is not null && !inhalable.CanInhale()))
        {
            InhaleItem(conn, itemNetId, false);
            return;
        }

        if (!inhalable.Inhaling(true, target))
        {
            InhaleItem(conn, itemNetId, false);
            return;
        }

        if (!ReferenceEquals(Managers.Game.Player, item.gameObject) && !ReferenceEquals(Managers.Game.OtherPlayer, item.gameObject)
            && (!ReferenceEquals(Managers.Game.Player, target) || !item.isOwned))
            AssignAuthority(item, conn);
        
        InhaleItem(conn, itemNetId, true);
    }

    [TargetRpc]
    private void InhaleItem(NetworkConnectionToClient conn, uint itemNetId, bool value)
    {
        if (!NetworkClient.spawned.TryGetValue(itemNetId, out var item)) return;
        
        itemInhaleCallback?.Invoke(item.gameObject, value);
    }

    //[Command(requiresAuthority = false)]
    //public void StopInhaleItem(uint itemNetId)
    //{
    //    if (!NetworkClient.spawned.TryGetValue(itemNetId, out var item)) return;
    //
    //    //if (_assignAuthorityCoroutine.TryGetValue(itemNetId, out var coroutine))
    //    //{
    //    //    StopCoroutine(coroutine);
    //    //    _assignAuthorityCoroutine.Remove(itemNetId);
    //    //}
    //
    //    if (!ReferenceEquals(Managers.Game.Player, item.gameObject) && !ReferenceEquals(Managers.Game.OtherPlayer, item.gameObject) && !item.isOwned)
    //    {
    //        var delayAssignAuthorityCoroutine = StartCoroutine(DelayAssignAuthority(item));
    //        //_assignAuthorityCoroutine.TryAdd(itemNetId, delayAssignAuthorityCoroutine);
    //    }
    //    
    //    if (!item.TryGetComponent<IInhalable>(out var inhalable)) return;
    //    
    //    inhalable.Inhaling(false);
    //    inhalable.Fixed(false);
    //}

    //[Command(requiresAuthority = false)]
    //public void TryFixInhaleItem(GameObject target, uint itemNetId)
    //{
    //    var conn = target.GetComponent<NetworkIdentity>().connectionToClient;
    //
    //    if (!NetworkClient.spawned.TryGetValue(itemNetId, out var item))
    //    {
    //        FixInhaleItem(conn, itemNetId, false);
    //        return;
    //    }
    //
    //    if (!item.TryGetComponent<IInhalable>(out var inhalable) || (inhalable is not null && !inhalable.CanInhale()))
    //    {
    //        FixInhaleItem(conn, itemNetId, false);
    //        return;
    //    }
    //    
    //    inhalable.Fixed(true);
    //    FixInhaleItem(conn, itemNetId, true);
    //}

    //[TargetRpc]
    //private void FixInhaleItem(NetworkConnectionToClient conn, uint itemNetId, bool value)
    //{
    //    if (!NetworkClient.spawned.TryGetValue(itemNetId, out var item)) return;
    //    
    //    if(ReferenceEquals(Managers.Game.Player, item.gameObject) || ReferenceEquals(Managers.Game.OtherPlayer, item.gameObject))
    //        fixItemCallback?.Invoke(value, true);
    //    else
    //        fixItemCallback?.Invoke(value, false);
    //}
    #endregion

    #region Object
    private WaitForSeconds _waitForDestroy = new(0.5f);
    
    [Command(requiresAuthority = false)]
    public void DestroyKey(GameObject target)
    {
        var id = target.GetComponent<NetworkIdentity>();
        if (!id.isOwned)
            AssignAuthority(id);
        
        RpcDestroyKey(target);
    }

    [ClientRpc]
    private void RpcDestroyKey(GameObject target)
    {
        //target.GetComponent<SpriteRenderer>().enabled = false;
        //if (target.TryGetComponent(out SpriteRenderer spriteRenderer))
        //{
        //    spriteRenderer.enabled = false;
        //}
        target.transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = false;
       // target.GetComponent<IInteractable>().Interacting(true);
        if (NetworkServer.active)
        {
            target.GetComponent<Key>().CallOnInterableObjectRelease();
            StartCoroutine(WaitForDestroy(target));
        }
       

    }

    private IEnumerator WaitForDestroy(GameObject target)
    {
        yield return _waitForDestroy;
        NetworkServer.Destroy(target);
    }
    #endregion
    
    #region AuthorityAssign
    //[Command(requiresAuthority = false)]
    //public void AuthorityToServer(uint itemNetId, bool isRelease, Vector2 velocity)
    //{
    //    if (!NetworkClient.spawned.TryGetValue(itemNetId, out var item)) return;
    //    
    //    if (item.isOwned) return;
    //
    //    AssignAuthority(item);
    //    
    //    if (isRelease)
    //    {
    //        if (item.TryGetComponent<InteractableObject>(out var interactableObject))
    //        {
    //            interactableObject.Release();
    //            interactableObject.GetComponent<Rigidbody2D>().velocity = velocity;
    //        }
    //    }
    //}
    
    [Command(requiresAuthority = false)]
    public void AuthorityToServer(uint itemNetId)
    {
        if (!NetworkClient.spawned.TryGetValue(itemNetId, out var item)) return;
        
        if (item.isOwned) return;

        AssignAuthority(item);
    }

    [Command(requiresAuthority = false)]
    public void AuthorityToClient(uint itemNetId)
    {
        if (!NetworkClient.spawned.TryGetValue(itemNetId, out var item)) return;

        if (!item.isOwned) return;

        if (Managers.Game.OtherPlayer is null || !Managers.Game.OtherPlayer.TryGetComponent<NetworkIdentity>(out var client)) return;

        AssignAuthority(item, client.connectionToClient);
    }

    private void AssignAuthority(NetworkIdentity item, NetworkConnectionToClient conn = null)
    {
        item.RemoveClientAuthority();
        item.AssignClientAuthority(conn ?? NetworkServer.localConnection);
    }
    #endregion
}
