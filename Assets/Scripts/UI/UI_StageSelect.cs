using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_StageSelect : UI_Base
{
    public Transform layout;
    [SerializeField] Button closeBtn;
    private TMP_Text _startText;

    private void Awake()
    {
        closeBtn.onClick.AddListener(CloseUI);
    }

    public override void OnEnable()
    {
        OpenUI();
    }



    protected override void Start()
    {
        //var tutorialMaps = Managers.Data.mapData.mapTutorialDictionary.Keys;
        //var mainMaps = Managers.Data.mapData.mapMainDictionary.Keys;
        var maps = Managers.Data.mapData.mapMainStageDictionary.Keys;

        foreach (var map in maps)
        {
            var button = ResourceManager.Instantiate("Prefabs/UI/Button", layout).GetComponent<Button>();      
            button.GetComponentInChildren<TMP_Text>().text = map.ToString();
            string mapName = Managers.Data.mapData.mapMainStageDictionary[map][0].mapID;
            button.onClick.AddListener(() => StageSet(mapName));
        }

        //foreach (var map in mainMaps)
        //{
        //    var button = ResourceManager.Instantiate("Prefabs/UI/Button", layout).GetComponent<Button>();
        //    button.GetComponentInChildren<TMP_Text>().text = map;
        //    button.onClick.AddListener(() => StageSet(map));
        //}

        //TEST 맵 시작 테스트 코드
        var endButton = ResourceManager.Instantiate("Prefabs/UI/Button", layout).GetComponent<Button>();
        //endButton.GetComponentInChildren<TMP_Text>().text = _startText.text;
        endButton.onClick.AddListener(StartGame);
    }



    private void StageSet(string mapName)
    {
        SetNextStage(mapName);
        //Managers.Game.Player.GetComponent<Player>().CmdChangeStage(mapName);
    }

    private void SetNextStage(string mapName)
    {
        GameObject ExitObj = MapEditor.Instance.exitDoorObjectTransform.GetChild(0).gameObject;
        ExitObj.GetComponent<ExitPointObj>().nextMapId = mapName;
    }

    
    //TEST 맵 시작 테스트 코드
    private void StartGame()
    {
        Managers.Network.ServerChangeScene("MainScene");
        CloseUI();
    }
    
    public override void SetLanguage()
    {
        SetSentence(_startText, 2101);
    }
}
