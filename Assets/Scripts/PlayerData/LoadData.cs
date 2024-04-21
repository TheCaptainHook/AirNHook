using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LoadData
{
    public Dictionary<string, PlayData> playData = new Dictionary<string, PlayData>();
    //스테이지 클리어 레벨을 딕셔너리로쓰는 stageClearLevelData
    public Dictionary<int, List<string>> stageClearLevelData = new Dictionary<int, List<string>>();

    private int _stagelevel;

    public void Setup()
    {
        //TODO 아래부분을 Start가 아닌 다른부분에 넣어서 사용하면됩니다.
        var playDataPath = Application.dataPath + "/PlayData.json";

        //playData있는지 없는지 체크
        if (File.Exists(playDataPath))
        {
            var playDataList = Managers.Data.ReadJson<PlayData>(playDataPath);
            if (playDataList.Length < Managers.Data.mapData.mapMainDictionary.Count)
            {
                DataAdd();
            }
            foreach (var play in playDataList)
            {
                playData.Add(play.stageID, play);
                //_stagelevel = play.stageLevel;
            }
            //Managers.Game.stageLevel = _stagelevel;
            //이곳에서 현재 스테이지레벨을 알려줘야함
        }
        else
        {
            DataAdd();
        }
    }

    public void DataAdd()
    {
        var playDataPath = Application.dataPath + "/PlayData.json";
        //playData,stageData에 데이터넣기
        foreach (var key in Managers.Data.mapData.mapMainDictionary.Keys)
        {
            if (!playData.ContainsKey(key))
            {
                PlayData play = new PlayData()
                {
                    stageID = key,
                    stageLevel = Managers.Data.mapData.mapMainDictionary[key].stageLevel,
                };
                playData.Add(key, play);
            }
        }

        foreach (var key in Managers.Data.mapData.mapUserDictionary.Keys)
        {
            if (!playData.ContainsKey(key))
            {
                PlayData play = new PlayData()
                {
                    stageID = key,
                    stageLevel = Managers.Data.mapData.mapUserDictionary[key].stageLevel,
                };
                playData.Add(key, play);
            }
        }
        //해당위치에 해당파일이 있는지 체크하고 없으면 생성
        File.WriteAllText(playDataPath, JsonConvert.SerializeObject(playData.Values, Formatting.Indented));
    }

    //TODO 스테이지 클리어가되면 동시에 작업이 이루어져야함
    public void Save()
    {
        var playDataPath = Application.dataPath + "/PlayData.json";

        File.WriteAllText(playDataPath, JsonConvert.SerializeObject(playData.Values, Formatting.Indented));
    }
}