using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadData : MonoBehaviour
{
    [SerializeField] GameObject _stageSelect;
    //생성할 위치
    public Transform stageButton;

    public Dictionary<string, StageData> playerData = new Dictionary<string, StageData>();
    private MapData _mapData;

    private bool _stageSelectShow;

    private void Awake()
    {
        _mapData = Managers.Data.mapData;
    }

    private void Start()
    {
        //TODO 아래부분을 Start가 아닌 다른부분에 넣어서 사용하면됩니다.
        var fliePath = Path.Combine(Application.streamingAssetsPath, "PlayerData/StageDatas.json");
        if (File.Exists(fliePath))
        {
            var list = Managers.Data.ReadJson<StageData>(fliePath);
            foreach (var data in list)
            {
                playerData.Add(data.stageID, data);
            }
        }

        foreach (var key in _mapData.mapMainDictionary.Keys)
        {
            if (!playerData.ContainsKey(key))
            {
                StageData data = new StageData()
                {
                    stageID = key,
                    stageClear = false,
                };
                playerData.Add(key, data);
            }
        }

        foreach (var key in _mapData.mapUserDictionary.Keys)
        {
            if (!playerData.ContainsKey(key))
            {
                StageData data = new StageData()
                {
                    stageID = key,
                    stageClear = false,
                };
                playerData.Add(key, data);
            }
        }
        //해당위치에 해당파일이 있는지 체크하고 없으면 생성
        File.WriteAllText(fliePath, JsonConvert.SerializeObject(playerData.Values, Formatting.Indented));
    }

    public void CreateButton()
    {
        _stageSelect.SetActive(true);
        if (!_stageSelectShow)
        {
            //Json파일 읽어오는 코드
            var path = Path.Combine(Application.streamingAssetsPath, "PlayerData/StageDatas.json");
            var list = Managers.Data.ReadJson<StageData>(path);

            //Json에 제대로 저장이 되었는지 확인하기위한 버튼설정
            foreach (var key in list)
            {
                //list 개수만큼 버튼이 생성
                var slot = ResourceManager.Instantiate("Prefabs/Button/StageButton", stageButton);

                var button = slot.GetComponent<StageButton>();

                button.StageSelect(key);
                button.LoadData(GetComponent<LoadData>());
            }
        }
        _stageSelectShow = true;
    }

    public void Save()
    {
        var fliePath = Path.Combine(Application.streamingAssetsPath, "PlayerData/StageDatas.json");
        foreach (var key in playerData.Keys)
        {
            Debug.Log(playerData[key].stageID + " = " + playerData[key].stageClear);
        }

        File.WriteAllText(fliePath, JsonConvert.SerializeObject(playerData.Values, Formatting.Indented));
    }
}