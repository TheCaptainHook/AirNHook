using System;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    #region FeedBackMemo
    /*
    이후 모노비헤이비어 빼고 => 리소스 매니저 만들어지면 여기서 인스턴시에이트랑 디스트로이 처리
    실무 때는 현재 형태의 딕셔너리로 진행하게 되면 찾기는 쉽겠지만
    여러가지 UI가 많이 뜨는 게임의 형태에서는 분명히 쌓여서 렉을 먹을 것
    
    게임오브젝트로 저장되는 방식이 아닌 UI_Base로 저장되는 방식으로
    +로 쓰면 많이 무겁다, Get하는 메소드는 ShowUI에서 겟까지 하기에 큰 필요성이 없을 것 같다.
    */
    #endregion

    #region Static&Const_String
    private const string _uiPath = "Prefabs/UI/";
    #endregion
    
    // UI 오브젝트를 저장하는 딕셔너리
    private Dictionary<string, UI_Base> _uIDict = new Dictionary<string, UI_Base>();
    // UI 오브젝트를 관리하는 부모 Transform
    public Transform uiManager;

    // 현재 Scene 이름
    [HideInInspector] public string sceneName;
    
    // UI를 보여주기, 받아올 UI가 딕셔너리에 없으면 생성
    public UI_Base ShowUI<T>(Transform parent = null) where T : Component
    {
        if (UIListCheck<T>())
        {
            _uIDict[typeof(T).Name].gameObject.SetActive(true);
            return _uIDict[typeof(T).Name].GetComponent<UI_Base>();
        }
        else
            return CreateUI<T>(parent);
    }
    
    // UI 숨기기
    public void HideUI<T>()
    {
        if (UIListCheck<T>())
            _uIDict[typeof(T).Name].gameObject.SetActive(false);
    }
    
    // UI를 제거합니다.
    private void RemoveUI<T>()
    {
        if (UIListCheck<T>())
            _uIDict.Remove(typeof(T).Name);
    }
    
    // UI 생성 메서드
    private UI_Base CreateUI<T>(Transform parent = null)
    {
        try
        {
            if (UIListCheck<T>())
                _uIDict.Remove(typeof(T).Name);

            // 리소스 폴더 UI 프리팹 로드하여 생성
            GameObject go = Instantiate(Resources.Load<GameObject>(GetPath<T>()), parent);

            var temp = go.GetComponent<UI_Base>();
            AddUI<T>(temp);

            return temp;
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
        }

        return default;
    }
    
    // UI를 딕셔너리에 추가
    private void AddUI<T>(UI_Base ui)
    {
        _uIDict.TryAdd(typeof(T).Name, ui);
    }
    
    // LoadingUI를 표시
    public UI_Loading ShowLoadingUI(string loadSceneName)
    {
        sceneName = loadSceneName;
    
        if (_uIDict.ContainsKey(nameof(UI_Loading)) && _uIDict[nameof(UI_Loading)] != null)
        {
            _uIDict[nameof(UI_Loading)].gameObject.SetActive(true);
            return _uIDict[nameof(UI_Loading)].GetComponent<UI_Loading>();
        }
        else
            return (UI_Loading)CreateUI<UI_Loading>();
    }

    #region ToDo<Emote>
    // 이모트 띄우는 방식을 의논해 보다가 걸리는 부분들이 생겨 주석화
        // 우선 기존 딕셔너리에다가 집어 넣을지 <= 그냥 Destroy는 GC가 회수할 때 까지 메모리에 쌓이기에
        // 이모트 휠을 띄우는 ShowUI방식을 차라리 하나 만들어서 LOL의 이모티콘 형식을 구현해 볼 수 없을까 하는 토의
        // 추가기능 구현 부분이기에 이후 구현
        /*
        public void ShowEmote(Transform playerTransform, GameObject emotePrefab, float displayTime = 2f)
        {
            GameObject emoteInstance = Instantiate(emotePrefab, playerTransform.position, Quaternion.identity);
            emoteInstance.transform.SetParent(playerTransform);
            // 지정된 시간 후에 이모티콘을 제거
            StartCoroutine(DestroyEmote(emoteInstance, displayTime));
        }
    
        private IEnumerator DestroyEmote(GameObject emoteInstance, float displayTime)
        {
            yield return new WaitForSeconds(displayTime);
            Destroy(emoteInstance);
        }
        */
    #endregion
    
    // UI 제거
    public void DestroyUI<T>()
    {
        string className = typeof(T).Name;
        if (!_uIDict.ContainsKey(className)) return;
        Destroy(_uIDict[className]?.gameObject);
        _uIDict.Remove(className);
    }
    
    // 해당 UI 활성화 여부 확인
    public bool IsActive<T>()
    {
        if(_uIDict.ContainsKey(typeof(T).Name) && _uIDict[typeof(T).Name] == null)
        {
            RemoveUI<T>();
            return false;
        }

        if (_uIDict.ContainsKey(typeof(T).Name) && _uIDict[typeof(T).Name].gameObject.activeSelf)
        {
            return _uIDict[typeof(T).Name].GetComponent<UI_Base>().IsEnabled;
        }
        else
            return false;
    }
    
    // UI 프리팹 경로 반환
    private static string GetPath<T>()
    {
        string className = typeof(T).Name;  
        return _uiPath + className;
    }
    
    // 해당 UI가 딕셔너리에 있는지 확인
    private bool UIListCheck<T>()
    {
        return _uIDict.ContainsKey(typeof(T).Name) && _uIDict[typeof(T).Name] != null;
    }

    public void SettingLanguage()
    {
        foreach (var ui in _uIDict.Values)
        {
            ui.SetLanguage();
        }
    }
}
