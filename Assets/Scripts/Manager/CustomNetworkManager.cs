using Mirror;
using UnityEngine;

public class CustomNetworkManager : NetworkManager
{
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
        
        networkSceneName = newSceneName;
    }
}
