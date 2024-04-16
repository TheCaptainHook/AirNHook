using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UI_StageSelect : UI_Base
{
    public Transform layout;
    [SerializeField] Button closeBtn;
    private TMP_Text _startText;

    [SerializeField] Button curSelectBtn; //todo 0415

    int curCreatedStage = 0;
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
        var maps = Managers.Data.mapData.mapMainStageDictionary.Keys;

        foreach (var key in maps)
        {
            CreateStage(key);
        }

        //TEST 맵 시작 테스트 코드
        //var endButton = ResourceManager.Instantiate("Prefabs/UI/Button", layout).GetComponent<Button>();
        ////endButton.GetComponentInChildren<TMP_Text>().text = _startText.text;
        //endButton.onClick.AddListener(StartGame);
    }

    protected override void OpenUI() // Update select menu when clear stage
    {
        //Check player stage Clear level. if curCreatedStage is different from the player stage clear level then Update Ui.
        base.OpenUI();
    }

    private void UpdateUI()// Update select menu when clear stage
    {

    }

    public void CreateStage(int level)
    {
        var button = ResourceManager.Instantiate("Prefabs/UI/Button", layout).GetComponent<Button>();
        curCreatedStage++;
        Debug.Log(curCreatedStage);
        button.GetComponentInChildren<TMP_Text>().text = level.ToString();
        string mapName = Managers.Data.mapData.mapMainStageDictionary[level][0].mapID;
        button.onClick.AddListener(() => { SelectStage(button); StageSet(mapName); });
    }

    private void StageSet(string mapName)
    {
        SetNextStage(mapName);
        //Managers.Game.Player.GetComponent<Player>().CmdChangeStage(mapName);
    }

    private void SetNextStage(string mapName) 
    {
        // if(playerData.stageClearDataDictionary.ContainKey(level){ string mapId = playerData.stageClearDataDictionary[level][lastIdx] ;}
        //else{string mapId = Managers.Data.mapData.mapMainStageDictionary[level][0].mapID; }
        GameObject ExitObj = MapEditor.Instance.exitDoorObjectTransform.GetChild(0).gameObject;
        ExitObj.GetComponent<ExitPointObj>().nextMapId = mapName;
    }

    private void SelectStage(Button button)
    {
        if(curSelectBtn != null)
        {
            curSelectBtn.GetComponent<Outline>().effectColor = Color.white;
        }
        curSelectBtn = button;
        curSelectBtn.GetComponent<Outline>().effectColor = Color.red;
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
