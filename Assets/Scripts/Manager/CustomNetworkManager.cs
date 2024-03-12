using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CustomNetworkManager : NetworkManager
{
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
        
        //loadingSceneAsync = SceneManager.LoadSceneAsync(newSceneName);
        // 로딩 UI표기. LoadSceneAsync는 로딩 UI에서 progress bar와 동기화를 위해 로딩 UI의 LoadScene에서 구현.
        Managers.UI.ShowLoadingUI(newSceneName);
        Debug.Log("client");
        
        networkSceneName = newSceneName;
    }
}