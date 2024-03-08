using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    // UI 오브젝트를 저장하는 딕셔너리
    public Dictionary<string, GameObject> _uIDict = new Dictionary<string, GameObject>();
    // UI 오브젝트를 관리하는 부모 Transform
    public Transform _uiManager;

    // 현재 Scene 이름
    [HideInInspector] public string sceneName;
    
    // UI를 보여주기, 받아올 UI가 딕셔너리에 없으면 생성
    public T ShowUI<T>(Transform parent = null) where T : Component
    {
        if (UIListCheck<T>())
        {
            _uIDict[typeof(T).Name].SetActive(true);
            return _uIDict[typeof(T).Name].GetComponent<T>();
        }
        else
            return CreateUI<T>(parent);
    }
    
    // UI 숨기기
    public void HideUI<T>()
    {
        if (UIListCheck<T>())
            _uIDict[typeof(T).Name].SetActive(false);
    }
    
    // UI를 제거합니다.
    public void RemoveUI<T>()
    {
        if (UIListCheck<T>())
            _uIDict.Remove(typeof(T).Name);
    }
    
    // UI 생성 메서드
    public T CreateUI<T>(Transform parent = null)
    {
        try
        {
            if (UIListCheck<T>())
                _uIDict.Remove(typeof(T).Name);

            // 리소스 폴더 UI 프리팹 로드하여 생성
            GameObject go = Instantiate(Resources.Load<GameObject>(GetPath<T>()), parent);

            T temp = go.GetComponent<T>();
            AddUI<T>(go);

            return temp;
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
        }

        return default;
    }
    
    // UI를 딕셔너리에 추가
    public void AddUI<T>(GameObject go)
    {
        if (_uIDict.ContainsKey(typeof(T).Name) == false)
            _uIDict.Add(typeof(T).Name, go);
    }
    
    // LoadingUI를 표시
    public UI_Loading ShowLoadingUI(string loadSceneName)
    {
        sceneName = loadSceneName;
    
        if (_uIDict.ContainsKey(typeof(UI_Loading).Name) && _uIDict[typeof(UI_Loading).Name] != null)
        {
            _uIDict[typeof(UI_Loading).Name].SetActive(true);
            return _uIDict[typeof(UI_Loading).Name].GetComponent<UI_Loading>();
        }
        else
            return CreateUI<UI_Loading>();
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
        if (_uIDict.ContainsKey(className))
        {
            Destroy(_uIDict[className]?.gameObject);
            _uIDict.Remove(className);
        }
    }
    
    // 해당 UI 활성화 여부 확인
    public bool IsAcitve<T>()
    {
        if(_uIDict.ContainsKey(typeof(T).Name) && _uIDict[typeof(T).Name] == null)
        {
            RemoveUI<T>();
            return false;
        }

        if (_uIDict.ContainsKey(typeof(T).Name) && _uIDict[typeof(T).Name].activeSelf)
        {
            return _uIDict[typeof(T).Name].GetComponent<UI_Base<T>>().IsEnabled;
        }
        else
            return false;
    }
    
    // UI 프리팹 경로 반환
    private string GetPath<T>()
    {
        string className = typeof(T).Name;  
        return "Prefabs/UI/" + className;
    }
    
    // 해당 UI가 딕셔너리에 있는지 확인
    private bool UIListCheck<T>()
    {
        return _uIDict.ContainsKey(typeof(T).Name) && _uIDict[typeof(T).Name] != null;
    }
}
