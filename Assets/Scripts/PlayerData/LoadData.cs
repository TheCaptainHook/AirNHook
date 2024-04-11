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

    public Dictionary<int, PlayerData> playerData = new Dictionary<int, PlayerData>();

    public Dictionary<int, MapData> mapData = new Dictionary<int, MapData>();

    List<PlayerData> data = new List<PlayerData>();

    private void Start()
    {
        //Json파일 읽어오는 코드
        var path = Path.Combine(Application.dataPath, "Resources/PlayerData/PlayerDatas.json");

        var list = Managers.Data.ReadJson<PlayerData>(path);

        foreach(var sentence in list)
        {
            playerData.Add(sentence.StageID, sentence);
        }

        //현재 테스트과정이기때문에 버튼을 누르면 true로 바뀌고 저장이되도록 제작할예정
        foreach (var key in playerData.Keys)
        {
            //이곳을 통해서 스테이지번호를가진 버튼이 생성됨
            var slot = ResourceManager.Instantiate("Prefabs/Button/StageButton", stageButton);

            var button = slot.GetComponent<StageButton>();

            button.StageSelect(playerData[key]);
        }
    }

    public void WriteAlltext()
    {
        var fliePath = Path.Combine(Application.dataPath, "Resources/PlayerData/PlayerDatas.json");

        foreach(var key in playerData.Keys)
        {
            data.Add(playerData[key]);
        }
        File.WriteAllText(fliePath, JsonConvert.SerializeObject(data, Formatting.Indented));
    }
}