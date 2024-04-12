using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadData : MonoBehaviour
{
    //생성할 위치
    public Transform stageButton;

    public Dictionary<string, PlayerData> playerData = new Dictionary<string, PlayerData>();

    private MapData _mapData;

    List<PlayerData> data = new List<PlayerData>();

    private void Awake()
    {
        _mapData = Managers.Data.mapData;
    }

    private void Start()
    {
        //TODO 아래부분을 Start가 아닌 다른부분에 넣어서 사용하면됩니다.
        //플레이어 데이터에 정보 넣는 코드 28~60
        var fliePath = Path.Combine(Application.streamingAssetsPath, "PlayerData/MapDatas.json");

        foreach (var key in _mapData.mapMainDictionary.Keys)
        {
            if (!playerData.ContainsKey(key))
            {
                PlayerData data = new PlayerData()
                {
                    StageID = key,
                    StageClear = false,
                };
                playerData.Add(key, data);
            }
        }
        foreach (var key in _mapData.mapUserDictionary.Keys)
        {
            if (!playerData.ContainsKey(key))
            {
                PlayerData data = new PlayerData()
                {
                    StageID = key,
                    StageClear = false,
                };
                playerData.Add(key, data);
            }
        }

        //해당위치에 해당파일이 있는지 체크하고 없으면 생성
        if (!File.Exists(fliePath))
        {
            File.WriteAllText(fliePath, JsonConvert.SerializeObject(playerData.Values, Formatting.Indented));
        }

        //Json파일 읽어오는 코드
        var path = Path.Combine(Application.streamingAssetsPath, "PlayerData/MapDatas.json");
        var list = Managers.Data.ReadJson<PlayerData>(path);
        
        //Json에 제대로 저장이 되었는지 확인하기위한 버튼설정
        foreach (var key in list)
        {
            //이곳을 통해서 스테이지번호를가진 버튼이 생성됨
            var slot = ResourceManager.Instantiate("Prefabs/Button/StageButton", stageButton);
        
            var button = slot.GetComponent<StageButton>();
        
            button.StageSelect(key);
        }
    }

    public void WriteAlltext()
    {
        //var fliePath = Path.Combine(Application.dataPath, "Resources/PlayerData/PlayerDatas.json");
        //
        //foreach(var key in playerData.Keys)
        //{
        //    data.Add(playerData[key]);
        //}
        //File.WriteAllText(fliePath, JsonConvert.SerializeObject(data, Formatting.Indented));
    }
}