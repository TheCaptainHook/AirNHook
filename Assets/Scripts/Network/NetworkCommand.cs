using System;
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

    #region TryGrabItem
    public Action<NetworkIdentity, bool> itemGrabCallback;
    public Action<NetworkIdentity> itemReleaseCallback;
    
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
        
        interactable.Fixed(true);
        if (!NetworkServer.localConnection.Equals(conn) || !item.isOwned)
        {
            item.RemoveClientAuthority();
            item.AssignClientAuthority(conn);
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
    public void TryReleaseItem(NetworkConnectionToClient conn, NetworkIdentity item)
    {
        if (!item.isOwned)
        {
            item.RemoveClientAuthority();
            item.AssignClientAuthority(NetworkServer.localConnection);
        }
        
        if(!item.TryGetComponent<IInteractable>(out var interactable)) return;
        
        interactable.Fixed(false);
        ReleaseItem(conn, item);
    }

    [TargetRpc]
    private void ReleaseItem(NetworkConnectionToClient conn, NetworkIdentity item)
    {
        itemReleaseCallback?.Invoke(item);
    }
    #endregion
}
