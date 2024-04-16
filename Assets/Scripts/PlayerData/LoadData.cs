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

    public Dictionary<string, StageData> stageData = new Dictionary<string, StageData>();
    public Dictionary<string, PlayData> playData = new Dictionary<string, PlayData>();
    //스테이지 클리어 레벨을 딕셔너리로쓰는 stageClearLevelData
    public Dictionary<int, PlayData> stageClearLevelData = new Dictionary<int, PlayData>();

    private MapData _mapData;
    private List<string> _stageID = new List<string>();

    private bool _stageSelectShow;

    private void Awake()
    {
        _mapData = Managers.Data.mapData;
    }

    private void Start()
    {
        //TODO 아래부분을 Start가 아닌 다른부분에 넣어서 사용하면됩니다.
        var stageDataPath = Path.Combine(Application.streamingAssetsPath, "StageDatas/StageDatas.json");
        var playDataPath = Path.Combine(Application.streamingAssetsPath, "PlayDatas/PlayDatas.json");

        //stageData있는지 없는지 체크
        if (File.Exists(stageDataPath))
        {
            var list = Managers.Data.ReadJson<StageData>(stageDataPath);
            foreach (var data in list)
            {
                stageData.Add(data.stageID, data);
            }
        }

        //playData있는지 없는지 체크
        if (File.Exists(playDataPath))
        {
            var playDataList = Managers.Data.ReadJson<PlayData>(playDataPath);
            foreach (var play in playDataList)
            {
                playData.Add(play.stageID, play);
            }
        }

        //playData,stageData에 데이터넣기
        foreach (var key in _mapData.mapMainDictionary.Keys)
        {
            if (!stageData.ContainsKey(key))
            {
                StageData data = new StageData()
                {
                    stageID = key,
                    stageClear = false,
                };
                stageData.Add(key, data);
            }
            if (!playData.ContainsKey(key))
            {
                PlayData play = new PlayData()
                {
                    stageID = key,
                };
                playData.Add(key, play);
            }
        }

        foreach (var key in _mapData.mapUserDictionary.Keys)
        {
            if (!stageData.ContainsKey(key))
            {
                StageData data = new StageData()
                {
                    stageID = key,
                    stageClear = false,
                };
                stageData.Add(key, data);
            }
            if (!playData.ContainsKey(key))
            {
                PlayData play = new PlayData()
                {
                    stageID = key,
                };
                playData.Add(key, play);
            }
        }
        //해당위치에 해당파일이 있는지 체크하고 없으면 생성
        File.WriteAllText(stageDataPath, JsonConvert.SerializeObject(stageData.Values, Formatting.Indented));
        File.WriteAllText(playDataPath, JsonConvert.SerializeObject(playData.Values, Formatting.Indented));

    }

    //TODO 현재 테스트코드에선 생성할때 1번만불려져서 최신화가 안되고있는상황임
    //실제로 적용할땐 실시간 업데이트가 가능하도록 해야한다.
    public void CreateButton()
    {
        _stageSelect.SetActive(true);
        if (!_stageSelectShow)
        {
            //Json파일 읽어오는 코드
            var playDataPath = Path.Combine(Application.streamingAssetsPath, "PlayDatas/PlayDatas.json");
            var playDataList = Managers.Data.ReadJson<PlayData>(playDataPath);

            foreach(var key in playDataList)
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
        var fliePath = Path.Combine(Application.streamingAssetsPath, "StageDatas/StageDatas.json");
        foreach (var key in stageData.Keys)
        {
            Debug.Log(stageData[key].stageID + " = " + stageData[key].stageClear);
        }

        File.WriteAllText(fliePath, JsonConvert.SerializeObject(stageData.Values, Formatting.Indented));
    }

    public void StageClear()
    {

    }
}