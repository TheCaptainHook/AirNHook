using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader
{
    public SceneLoader()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    
    //TODO 각 씬에 필요한 초기화 코드 호출 - 초기화 코드만 GameManager에서 다루고 여기서 불러도 됨.
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Managers.UI.ClearUI();
        switch (scene.buildIndex)
        {
            // StartScene
            case 0:
                Debug.Log("Scene Loaded 0");
                Managers.Game.CurrentState = GameState.Title;
                Managers.UI.ShowUI<UI_Title>();
                break;
            // MainScene
            case 1:
                Debug.Log("Scene Loaded 1");
                break;
        }
    }
}
