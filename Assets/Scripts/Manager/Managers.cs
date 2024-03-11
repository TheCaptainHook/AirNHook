using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class Managers : MonoBehaviour
{
    private static Managers _instance;
    public static Managers Instance { get { Initialize(); CheckNetworkManager(); return _instance; } }

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
    }

    /// <summary>
    /// NetworkManager의 구조상 서버가 닫히면 NetworkManager가 Destroy되므로
    /// StartScene에서 null체크를 해줘 새로 생성해줌.
    /// 해당 코드는 StartScene에서만 불리고 StartScene에는 Object가 별로 없어
    /// FindObjectOfType으로 확인 가능.
    /// </summary>
    private static void CheckNetworkManager()
    {
        if (_instance._network != null) return;
        
        var networkManager = FindObjectOfType<CustomNetworkManager>();
        
        if (networkManager == null)
        {
            var go = Instantiate(Resources.Load("Prefabs/Manager/NetworkManager")) as GameObject;
            _instance._network = go.GetComponent<CustomNetworkManager>();
        }
        else
        {
            _instance._network = networkManager.GetComponent<CustomNetworkManager>();
        }
    }
}
