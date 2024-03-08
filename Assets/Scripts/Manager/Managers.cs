using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class Managers : MonoBehaviour
{
    private static Managers _instance;
    public static Managers Instance { get { Initialize(); return _instance; } }

    private UIManager _uiManager;
    private GameManager _game;
    private SceneLoader _loader;
    private StageManager _stage;
    private CustomNetworkManager _network;

    public static GameManager Game => Instance._game;
    public static UIManager UI => Instance._uiManager;
    public static SceneLoader Loader => Instance._loader;
    public static StageManager Stage => Instance._stage;
    public static CustomNetworkManager Network => Instance._network;

    /// <summary> 게임 시작시 자동으로 호출 - Scene에 넣을 필요 X </summary>
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Execute()
    {
        Initialize();
    }

    /// <summary> Manager들을 초기화 해주는 곳. </summary>
    private static void Initialize()
    {
        if (_instance != null) return;
        
        var go = GameObject.Find("@Managers");

        if (go == null)
        {
            go = new GameObject("@Managers");
            go.AddComponent<Managers>();
            go.AddComponent<NetworkIdentity>();
        }

        DontDestroyOnLoad(go);
        _instance = go.GetComponent<Managers>();

        if (!go.TryGetComponent(out _instance._game))
        {
            _instance._game = go.AddComponent<GameManager>();
        }
        
        if (!go.TryGetComponent(out _instance._uiManager))
        {
            _instance._uiManager = go.AddComponent<UIManager>();
        }

        if (!go.TryGetComponent(out _instance._loader))
        {
            _instance._loader = go.AddComponent<SceneLoader>();
        }
        
        if (!go.TryGetComponent(out _instance._stage))
        {
            _instance._stage = go.AddComponent<StageManager>();
        }
        
        // 네트워크 매니저의 특정상 gameObject로 child에 추가.
        var networkManager = go.GetComponentInChildren<CustomNetworkManager>();
        if (networkManager == null)
        {
            var child = Instantiate(Resources.Load("Prefabs/Manager/NetworkManager")) as GameObject;
            _instance._network = child.GetComponent<CustomNetworkManager>();
        }
    }
}
