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
        _networkManager = GetComponent<NetworkManager>();
        
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

    public void HostLobby()
    {
        // 테스트를 위해 친구 전용으로 로비 생성
        //SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, _networkManager.maxConnections);
        SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypePublic, _networkManager.maxConnections);
    }

    private void OnLobbyCreated(LobbyCreated_t callback)
    {
        if (callback.m_eResult != EResult.k_EResultOK)
        {
            return;
        }

        _networkManager.StartHost();
        currentLobbyID = new CSteamID(callback.m_ulSteamIDLobby);
        SteamMatchmaking.SetLobbyData(currentLobbyID,
            HostAddressKey,
            SteamUser.GetSteamID().ToString());
    }

    private void OnGameLobbyJoinRequested(GameLobbyJoinRequested_t callback)
    {
        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
    }

    private void OnLobbyEntered(LobbyEnter_t callback)
    {
        if(NetworkServer.active) return;

        var hostAddress = SteamMatchmaking.GetLobbyData(
            new CSteamID(callback.m_ulSteamIDLobby),
            HostAddressKey);

        _networkManager.networkAddress = hostAddress;
        _networkManager.StartClient();
    }

    public void GetLobbyList()
    {
        if(lobbyIDDict.Count > 0)
            lobbyIDDict.Clear();
        
        SteamMatchmaking.RequestLobbyList();
    }
    
    private void OnGetLobbyList(LobbyMatchList_t result)
    {
        for (var i = 0; i < result.m_nLobbiesMatching; i++)
        {
            var lobbyID = SteamMatchmaking.GetLobbyByIndex(i);
            
            lobbyIDDict.Add(lobbyID.m_SteamID, lobbyID);
        }
        joinLobbyCallback?.Invoke();
    }
    
    public bool JoinLobby(string steamID)
    {
        var id = Base62Converter.FromBase62(steamID);
        if (!lobbyIDDict.ContainsKey(id)) return false;
        
        SteamMatchmaking.JoinLobby(lobbyIDDict[id]);
        return true;
    }

    private void EncryptRoomCode()
    {
        
    }

    private void DecryptRoomCode()
    {
        
    }
}

