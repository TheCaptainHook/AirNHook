using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Mirror;

public class UI_StageSelect : UI_Base
{

    [Header("Icon")]
    public Transform layout;
    [SerializeField] Button closeBtn;
    [SerializeField] Button curSelectBtn; //todo 0415
    [SerializeField] Button spawnKey;
    private GameObject key;
    public GameObject Key { get { return key; }
        set { if (key != null) { Destroy(key);} key = value; }
    }

    public Button CurSelectBtn
    {
        get { return curSelectBtn; }
        set
        {
            if (curSelectBtn != value)
            {
                ResetStageInMapItem();
                curSelectBtn = value;
            }
        }
    }

    private TMP_Text _startText;

    [SerializeField] GameObject ui_StageInMapSelect;

    private List<GameObject> stageInMapSelectList;

    private void Awake()
    {
        closeBtn.onClick.AddListener(CloseUI);
        spawnKey.onClick.AddListener(SpawnKey);
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
            Create(key);
        }

        //TEST 맵 시작 테스트 코드
        //var endButton = ResourceManager.Instantiate("Prefabs/UI/Button", layout).GetComponent<Button>();
        ////endButton.GetComponentInChildren<TMP_Text>().text = _startText.text;
        //endButton.onClick.AddListener(StartGame);
    }

    protected override void OpenUI() // Update select menu when clear stage
    {
        CheckCurStageLevel();
        CheckStageClearAndChangeStageInMapItemTextColor();
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

    #region Create

    public void Create(int level)
    {
        CreateStageInMapUI(level);
        CreateStage(level);
    }

    private void CreateStageInMapUI(int level)
    {
        GameObject ui = Instantiate(ui_StageInMapSelect);
        UI_StageInMapSelect selectMap = ui.GetComponent<UI_StageInMapSelect>();
        selectMap.CreateStageInMap(level);
        ui.transform.SetParent(transform);
        stageInMapSelectList.Add(ui);
        ui.SetActive(false);

    }
    private void CreateStage(int level)
    {
        var button = ResourceManager.Instantiate("Prefabs/UI/Button", layout).GetComponent<Button>();
        button.GetComponentInChildren<TMP_Text>().text = level.ToString();
        string mapName = Managers.Data.mapData.mapMainStageDictionary[level][0].mapID;
        button.onClick.AddListener(() => { SelectStage(button); OpenStageInMapUI(level); });

    }


    public void SpawnKey()
    {
        GameObject key = Managers.Stage.CmdBatchObject("Key");
        Key = key;

        ObjectData data = MapEditor.Instance.curMap.FindObjectData(1000);
        Key.transform.position = data.position;
        Vector2 launchDirection = new Vector2(-1, 1).normalized;

        Key.GetComponent<Rigidbody2D>().AddForce(launchDirection * 5f, ForceMode2D.Impulse);

        CloseUI();
    }
    #endregion

    #region Util
    private void SelectStage(Button button)
    {
        if (CurSelectBtn != null)
        {
            curSelectBtn.GetComponent<Outline>().effectColor = Color.white;
        }
        CurSelectBtn = button;
        CurSelectBtn.GetComponent<Outline>().effectColor = Color.red;
    }

    private void OpenStageInMapUI(int level)
    {
        foreach (GameObject obj in stageInMapSelectList)
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

    private void CheckCurStageLevel() // Used when stage level up
    {
        if (stageInMapSelectList == null) return;
        if(Managers.Data.mapData.stageLevel > stageInMapSelectList.Count - 1)
        {
            Managers.Data.mapData.GetMainStageMapData(Managers.Data.mapData.stageLevel);
            Create(Managers.Data.mapData.stageLevel);
        }

    }

    private void CheckStageClearAndChangeStageInMapItemTextColor()
    {
        if (stageInMapSelectList != null)
        {
            foreach (GameObject obj in stageInMapSelectList)
            {
                UI_StageInMapSelect sis = obj.GetComponent<UI_StageInMapSelect>();
                sis.CheckStageClearItem();
            }

        }
    }
    #endregion


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
