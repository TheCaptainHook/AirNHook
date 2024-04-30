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
    }
    #endregion

    #region StageChange
    [Command(requiresAuthority = false)]
    public void ChangeStage(string value)
    {
        RpcChangeStage(value);
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

    [Command(requiresAuthority = false)]
    public void StageDataCheck(string value)
    {
        RpcStageDataCheck(value);
    }

    [ClientRpc(includeOwner = false)]
    private void RpcStageDataCheck(string value)
    {
        var isMapExist = Managers.Data.mapData.mapAllDictionary.ContainsKey(value);
        CmdStageDataChecked(value, isMapExist);
        
        if (!isMapExist) return;
        var stageUI = (UI_StageSelect)Managers.UI.GetUI<UI_StageSelect>();
        stageUI.MapSelected(value, true);
    }

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
        
        if(_assignAuthorityCoroutine.TryGetValue(itemNetId, out var coroutine))
            StopCoroutine(coroutine);
        
        interactable.Interacting(true);
        if (!NetworkServer.localConnection.Equals(conn) || !item.isOwned)
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

    [Command(requiresAuthority = false)]
    public void TryReleaseItem(GameObject target, uint itemNetId)
    {
        if (!NetworkClient.spawned.TryGetValue(itemNetId, out var item)) return;
        
        if(!item.TryGetComponent<IInteractable>(out var interactable)) return;
        
        item.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        interactable.Interacting(false);
        ReleaseItem(target.GetComponent<NetworkIdentity>().connectionToClient, itemNetId);
    }

    [TargetRpc]
    private void ReleaseItem(NetworkConnectionToClient conn, uint itemNetId)
    {
        itemReleaseCallback?.Invoke(itemNetId);
    }
    #endregion

    #region InhaleItem
    private Dictionary<uint, Coroutine> _assignAuthorityCoroutine = new();
    private readonly WaitForSeconds _waitForSeconds = new(3f);
    public Action<NetworkIdentity, bool> itemInhaleCallback;
    public Action<bool> fixItemCallback;
    
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
        
        if(_assignAuthorityCoroutine.TryGetValue(itemNetId, out var coroutine))
            StopCoroutine(coroutine);
        
        if ((!ReferenceEquals(Managers.Game.Player, item.gameObject) && !ReferenceEquals(Managers.Game.OtherPlayer, item.gameObject)) || !item.isOwned)
            AssignAuthority(item, conn);
        
        InhaleItem(conn, itemNetId, true);
    }

    [TargetRpc]
    private void InhaleItem(NetworkConnectionToClient conn, uint itemNetId, bool value)
    {
        if (!NetworkClient.spawned.TryGetValue(itemNetId, out var item)) return;
        
        itemInhaleCallback?.Invoke(item, value);
    }

    [Command(requiresAuthority = false)]
    public void TryInhalePlayer(GameObject player, GameObject accessor)
    {
        var conn = player.GetComponent<NetworkIdentity>().connectionToClient;
        
        InhalePlayer(conn, accessor);
    }

    [TargetRpc]
    private void InhalePlayer(NetworkConnectionToClient conn, GameObject accessor)
    {
        if(!conn.identity.TryGetComponent<IInhalable>(out var inhalable)) return;

        if (!accessor.TryGetComponent<AirGunNet>(out var air)) return;
        
        inhalable.Inhalation(air.weaponPoint);
    }

    [Command(requiresAuthority = false)]
    public void StopInhaleItem(uint itemNetId)
    {
        if (!NetworkClient.spawned.TryGetValue(itemNetId, out var item)) return;
        
        if(_assignAuthorityCoroutine.TryGetValue(itemNetId, out var coroutine))
            StopCoroutine(coroutine);
        
        if (!item.isOwned)
        {
            var delayAssignAuthorityCoroutine = StartCoroutine(DelayAssignAuthority(item));
            _assignAuthorityCoroutine.Add(itemNetId, delayAssignAuthorityCoroutine);
        }
        
        if(!item.TryGetComponent<IInhalable>(out var inhalable)) return;
        
        inhalable.Inhaling(false);
        inhalable.Fixed(false);
    }

    [Command(requiresAuthority = false)]
    public void TryFixInhaleItem(GameObject target, uint itemNetId)
    {
        var conn = target.GetComponent<NetworkIdentity>().connectionToClient;

        if (!NetworkClient.spawned.TryGetValue(itemNetId, out var item))
        {
            FixInhaleItem(conn, itemNetId, false);
            return;
        }

        if (!item.TryGetComponent<IInhalable>(out var inhalable) || (inhalable is not null && !inhalable.CanInhale()))
        {
            FixInhaleItem(conn, itemNetId, false);
            return;
        }
        
        inhalable.Fixed(true);
        FixInhaleItem(conn, itemNetId, true);
    }

    [TargetRpc]
    private void FixInhaleItem(NetworkConnectionToClient conn, uint itemNetId, bool value)
    {
        if (!NetworkClient.spawned.TryGetValue(itemNetId, out var item)) return;

        fixItemCallback?.Invoke(value);
    }

    [Command(requiresAuthority = false)]
    public void ShootObject(GameObject obj, Vector2 power)
    {
        if(!obj.TryGetComponent<IInhalable>(out var inhalable)) return;

        var item = obj.GetComponent<NetworkIdentity>();
        
        if (!item.isOwned)
            AssignAuthority(item);
        
        inhalable.Shooting(power);
    }

    private IEnumerator DelayAssignAuthority(NetworkIdentity item)
    {
        yield return _waitForSeconds;
        AssignAuthority(item);
        _assignAuthorityCoroutine.Remove(item.netId);
    }
    #endregion

    #region AuthorityToServer
    [Command(requiresAuthority = false)]
    public void AuthorityToServer(uint itemNetId, Vector2 power)
    {
        if (!NetworkClient.spawned.TryGetValue(itemNetId, out var item)) return;
        
        if(_assignAuthorityCoroutine.TryGetValue(itemNetId, out var coroutine))
            StopCoroutine(coroutine);
        
        if (!item.isOwned)
            AssignAuthority(item);

        if (item.TryGetComponent<Rigidbody2D>(out var rb))
            rb.velocity = power;
    }
    
    [Command(requiresAuthority = false)]
    public void AuthorityToServer(uint itemNetId)
    {
        if (!NetworkClient.spawned.TryGetValue(itemNetId, out var item)) return;

        if(_assignAuthorityCoroutine.TryGetValue(itemNetId, out var coroutine))
            StopCoroutine(coroutine);
        
        if (item.isOwned) return;

        AssignAuthority(item);
    }

    private void AssignAuthority(NetworkIdentity item, NetworkConnectionToClient conn = null)
    {
        item.RemoveClientAuthority();
        item.AssignClientAuthority(conn ?? NetworkServer.localConnection);
    }
    #endregion
}
