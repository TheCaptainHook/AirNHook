using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Managers : MonoBehaviour
{
    private static Managers _instance;
    public static Managers Instance { get { Initialize(); return _instance; } }

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
        }

        DontDestroyOnLoad(go);
        _instance = go.GetComponent<Managers>();
        
        //TODO 다른 매니저들 추가.
        // 예시 코드
        // if (!go.TryGetComponent(out _instance._gameManager))
        // {
        //     _instance._gameManager = go.AddComponent<GameManager>();
        // }
    }
}
