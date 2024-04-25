using System;
using System.Collections.Generic;
using Mirror;
using Steamworks;
using UnityEngine;

public class SteamLobby : MonoBehaviour
{
    private NetworkManager _networkManager;
    
    protected Callback<LobbyCreated_t> lobbyCreated;
    protected Callback<GameLobbyJoinRequested_t> gameLobbyJoinRequested;
    protected Callback<LobbyEnter_t> lobbyEntered;

    protected Callback<LobbyMatchList_t> lobbyList;
    public Action joinLobbyCallback;
    public CSteamID currentLobbyID;
    
    private Dictionary<ulong, CSteamID> lobbyIDDict = new();

    private const string HostAddressKey = "AirNHookKey";
    
    private void Start()
    {
        _networkManager = Managers.Network;
        
        // SteamManager가 활성이 안됐으면 return
        if(!SteamManager.Initialized) return;

        // Lobby가 만들어졌을때의 콜백
        lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        // Lobby 입장 시도 콜백
        gameLobbyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnGameLobbyJoinRequested);
        // Lobby 입장 콜백
        lobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);

        lobbyList = Callback<LobbyMatchList_t>.Create(OnGetLobbyList);
    }

    private void OnDisable()
    {
        lobbyCreated.Unregister();
        gameLobbyJoinRequested.Unregister();
        lobbyEntered.Unregister();
        lobbyList.Unregister();
    }

    public void HostLobby()
    {
        //SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, _networkManager.maxConnections);
        SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypePublic, _networkManager.maxConnections);
    }

    /// <summary> 로비 생성 </summary>
    private void OnLobbyCreated(LobbyCreated_t callback)
    {
        if (callback.m_eResult != EResult.k_EResultOK)
        {
            return;
        }

        _networkManager.StartHost();
        // Lobby Steam ID
        currentLobbyID = new CSteamID(callback.m_ulSteamIDLobby);
        SteamMatchmaking.SetLobbyData(currentLobbyID,
            HostAddressKey,
            SteamUser.GetSteamID().ToString());
    }

    /// <summary> 친구 초대를 받았을 때, 불리는 Callback </summary>
    private void OnGameLobbyJoinRequested(GameLobbyJoinRequested_t callback)
    {
        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
    }

    /// <summary> 로비 입장시의 Callback </summary>
    private void OnLobbyEntered(LobbyEnter_t callback)
    {
        if(NetworkServer.active) return;

        var hostAddress = SteamMatchmaking.GetLobbyData(
            new CSteamID(callback.m_ulSteamIDLobby),
            HostAddressKey);

        _networkManager.networkAddress = hostAddress;
        _networkManager.StartClient();
    }

    /// <summary> 해당 게임의 스팀 로비 리스트 받아오는 Method </summary>
    public void GetLobbyList()
    {
        if(lobbyIDDict.Count > 0)
            lobbyIDDict.Clear();
        
        SteamMatchmaking.RequestLobbyList();
    }
    
    /// <summary> 로비 리스트를 다 받아왔을 때의 Callback </summary>
    private void OnGetLobbyList(LobbyMatchList_t result)
    {
        for (var i = 0; i < result.m_nLobbiesMatching; i++)
        {
            var lobbyID = SteamMatchmaking.GetLobbyByIndex(i);
            
            lobbyIDDict.Add(lobbyID.m_SteamID, lobbyID);
        }
        joinLobbyCallback?.Invoke();
    }
    
    /// <summary> 62진수로 변환된 Lobby Steam ID를 통해 방 입장 </summary>
    public bool JoinLobby(string steamID)
    {
        var id = Base62Converter.FromBase62(steamID);
        if (!lobbyIDDict.ContainsKey(id)) return false;
        
        SteamMatchmaking.JoinLobby(lobbyIDDict[id]);
        return true;
    }
}
