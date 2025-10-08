using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class CustomNetworkManager : NetworkManager
{
    public Dictionary<string, GameObject> spawnPrefabDict;
    public SteamLobby steamLobby;

    public override void Start()
    {
        base.Start();

        spawnPrefabDict = new Dictionary<string, GameObject>();
        foreach (var spawnPrefab in spawnPrefabs)
        {
            spawnPrefabDict.Add(spawnPrefab.name, spawnPrefab);
        }

        steamLobby = GetComponent<SteamLobby>();
    }

    #region Messages
    public struct CreateCustomCharacterMessage : NetworkMessage
    {
        public CharacterType type;
    }
    public double GetPingMessage()
    {
        if(NetworkClient.isConnected)
        {
            return NetworkTime.rtt *1000f;
        }
        
        return 9999;
    }
    #endregion

    #region Character
    public override void OnStartServer()
    {
        base.OnStartServer();
        
        NetworkServer.RegisterHandler<CreateCustomCharacterMessage>(OnCreateCharacter);
        var obj = ResourceManager.Instantiate("Prefabs/NetworkCommand/NetworkCommand");
        NetworkServer.Spawn(obj);
    }

    private void OnCreateCharacter(NetworkConnectionToClient conn, CreateCustomCharacterMessage message)
    {
        GameObject playerObj;
        switch (message.type)
        {
            case CharacterType.Hook:
                playerObj = spawnPrefabDict["PlayerHook"];
                break;
            case CharacterType.Air:
                playerObj = spawnPrefabDict["PlayerAir"];
                break;
            case CharacterType.Default:
            default:
                playerObj = playerPrefab;
                break;
        }

        var player = Instantiate(playerObj, startPos[0].transform.position, Quaternion.identity);
        if (conn.identity != null)
        {
            var oldPlayer = conn.identity.gameObject;
            NetworkServer.ReplacePlayerForConnection(conn, Instantiate(player, player.transform.position, Quaternion.identity), true);
            Destroy(oldPlayer, 0.1f);
        }
        NetworkServer.AddPlayerForConnection(conn, player);
    }

    public void ReplacePlayer(NetworkConnectionToClient conn, CharacterType characterType, Vector2 pos)
    {
        var oldPlayer = conn.identity.gameObject;

        GameObject newPrefab;
        switch (characterType)
        {
            case CharacterType.Hook:
                newPrefab = spawnPrefabDict["PlayerHook"];
                break;
            case CharacterType.Air:
                newPrefab = spawnPrefabDict["PlayerAir"];
                break;
            case CharacterType.Default:
            default:
                newPrefab = playerPrefab;
                break;
        }
        oldPlayer.SetActive(false);
        
        NetworkServer.ReplacePlayerForConnection(conn, Instantiate(newPrefab, pos, Quaternion.identity), true);
        
        Destroy(oldPlayer, 0.1f);
    }
    #endregion
    
    #region Scene
    public override void ServerChangeScene(string newSceneName)
    {
        if (string.IsNullOrWhiteSpace(newSceneName))
        {
            Debug.LogError("ServerChangeScene empty scene name");
            return;
        }

        if (NetworkServer.isLoadingScene && newSceneName == networkSceneName)
        {
            Debug.LogError($"Scene change is already in progress for {newSceneName}");
            return;
        }

        if (!NetworkServer.active && newSceneName != offlineScene)
        {
            Debug.LogError("ServerChangeScene can only be called on an active server.");
            return;
        }

        NetworkServer.SetAllClientsNotReady();
        networkSceneName = newSceneName;

        OnServerChangeScene(newSceneName);

        NetworkServer.isLoadingScene = true;

        // loadingSceneAsync = SceneManager.LoadSceneAsync(newSceneName);
        // Server Host Loading UI
        Managers.UI.ShowLoadingUI(newSceneName);

        if (NetworkServer.active)
        {
            // notify all clients about the new scene
            NetworkServer.SendToAll(new SceneMessage
            {
                sceneName = newSceneName
            });
        }

        startPositionIndex = 0;
        startPositions.Clear();
    }

    public override void OnServerSceneChanged(string sceneName)
    {
        base.OnServerSceneChanged(sceneName);
        
        Managers.Game.CurrentState = GameState.Lobby;
        Instantiate(Resources.Load<GameObject>("Prefabs/MapEditor/MapEditor"));
        Managers.Stage.LoadMap();
        
        Managers.UI.InitializeUI();
        var characterMessage = new CreateCustomCharacterMessage()
        {
            type = Managers.Game.playerCharacterType
        };

        NetworkClient.Send(characterMessage);

        //TEST 250310 Ping Check
        // var ui = Managers.UI.ShowUI<UI_Ping>().gameObject.GetComponent<UI_Ping>();
        // ui.StartPingCheck(PingType.Server);

        //UI_Option,Enable -> PingCheck, disable Stop Check.
        //TEST 250310 Ping Check
    }
    // 로딩 UI 구현을 위한 override
    public override void ClientChangeScene(string newSceneName, SceneOperation sceneOperation = SceneOperation.Normal, bool customHandling = false)
    {
        // 씬 이름 체크
        if (string.IsNullOrWhiteSpace(newSceneName))
        {
            Debug.LogError("ClientChangeScene empty scene name");
            return;
        }

        OnClientChangeScene(newSceneName, sceneOperation, customHandling);

        if (NetworkServer.active)
            return;

        NetworkClient.isLoadingScene = true;

        clientSceneOperation = sceneOperation;

        if (customHandling)
            return;

        // 로딩 UI표기. LoadSceneAsync는 로딩 UI에서 progress bar와 동기화를 위해 로딩 UI의 LoadScene에서 구현.
        Managers.UI.ShowLoadingUI(newSceneName);

        networkSceneName = newSceneName;
    }
    
    public override void OnClientSceneChanged()
    {
        base.OnClientSceneChanged();
        
        if (NetworkServer.active && NetworkClient.isConnected) return;
        
        Managers.Game.CurrentState = GameState.Lobby;
        Instantiate(Resources.Load<GameObject>("Prefabs/MapEditor/MapEditor"));
        Managers.Stage.LoadMap();
        
        Managers.UI.InitializeUI();
        // 캐릭터 생성
        var characterMessage = new CreateCustomCharacterMessage()
        {
            type = Managers.Game.playerCharacterType
        };

        NetworkClient.Send(characterMessage);

        //TEST 250203
        //Managers.Stage.NetworkObject_SetParent();
        //TEST 250310 Ping Check
        // var ui = Managers.UI.ShowUI<UI_Ping>().gameObject.GetComponent<UI_Ping>();
        // ui.StartPingCheck(PingType.Client);
        //TEST 250310 Ping Check
    }
    #endregion


    #region Disconnect
    public override void OnServerDisconnect(NetworkConnectionToClient conn) //server
    {
        if(Managers.Stage.stageName != GlobalText.LOBBY) Managers.Command._Server_ChangeStage(GlobalText.LOBBY);
        
        base.OnServerDisconnect(conn);
    }
    #endregion
}
