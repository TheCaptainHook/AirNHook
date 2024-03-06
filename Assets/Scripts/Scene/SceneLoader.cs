using UnityEngine;
using UnityEngine.SceneManagement;

public enum Scenes
{
    Start = 0,
    Main,
}

public class SceneLoader : MonoBehaviour
{
    private void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    
    /// <summary> enum 값을 이용하여 Scene Load. </summary>
    public void LoadScene(Scenes scene)
    {
        SceneManager.LoadSceneAsync((int)scene);
    }
    
    //TODO 각 씬에 필요한 초기화 코드 호출 - 초기화 코드만 GameManager에서 다루고 여기서 불러도 됨.
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        switch (scene.buildIndex)
        {
            // StartScene
            case 0:
                Debug.Log("Scene Loaded 0");
                break;
            // MainScene
            case 1:
                Debug.Log("Scene Loaded 1");
                break;
        }
    }
}
