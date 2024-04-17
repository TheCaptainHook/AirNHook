using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class UI_StageSelect : UI_Base
{
    public Transform layout;
    [SerializeField] Button closeBtn;
    private TMP_Text _startText;

    [SerializeField] Button curSelectBtn; //todo 0415
    public Button CurSelectBtn { get { return curSelectBtn; }
        set
        {
            if (curSelectBtn != value)
            {
                ResetStageInMapItem();
                curSelectBtn = value;
            }
        } }
    [SerializeField] GameObject ui_StageInMapSelect;

    private List<GameObject> stageInMapSelectList;

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
        stageInMapSelectList = new();
        foreach (var key in maps)
        {
            CreateStageInMapUI(key);
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

    protected override void CloseUI()
    {
        foreach(GameObject obj in stageInMapSelectList)
        {
            obj.SetActive(false);
        }

        base.CloseUI();
    }
    private void UpdateUI()// Update select menu when clear stage
    {

    }


    public void CreateStageInMapUI(int level)
    {
        GameObject ui = Instantiate(ui_StageInMapSelect);
        UI_StageInMapSelect selectMap = ui.GetComponent<UI_StageInMapSelect>();
        selectMap.CreateStageInMap(level);
        ui.transform.SetParent(transform);
        stageInMapSelectList.Add(ui);
        ui.SetActive(false);

        //CreateStage(level);

    }

    public void CreateStage(int level)
    {
        var button = ResourceManager.Instantiate("Prefabs/UI/Button", layout).GetComponent<Button>();
        curCreatedStage++;
        button.GetComponentInChildren<TMP_Text>().text = level.ToString();
        string mapName = Managers.Data.mapData.mapMainStageDictionary[level][0].mapID;
        button.onClick.AddListener(() => { SelectStage(button); OpenStageInMapUI(level); });


        //stageInMapSelect create,
        
    }


    private void OpenStageInMapUI(int level)
    {
        foreach(GameObject obj in stageInMapSelectList)
        {
            obj.SetActive(false);
        }

        stageInMapSelectList[level].SetActive(true);
    }

    private void ResetStageInMapItem()
    {
        foreach (GameObject obj in stageInMapSelectList)
        {
            UI_StageInMapSelect selectMap = obj.GetComponent<UI_StageInMapSelect>();
            selectMap.ResetBtn();
        }
    }


    private void SelectStage(Button button)
    {
        if(CurSelectBtn != null)
        {
            curSelectBtn.GetComponent<Outline>().effectColor = Color.white;
        }
        CurSelectBtn = button;
        CurSelectBtn.GetComponent<Outline>().effectColor = Color.red;
    }

    //TEST 맵 시작 테스트 코드
    //private void StartGame()
    //{
    //    Managers.Network.ServerChangeScene("MainScene");
    //    CloseUI();
    //}
    
    public override void SetLanguage()
    {
        SetSentence(_startText, 2101);
    }
}
