using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Mirror;
using System;

public class UI_StageSelect : UI_Base
{

    [Header("Main Map Container")]
    public Transform mainMapcontainer;
    public Transform mainMaplayout;
    private List<GameObject> stageInMapSelectList;

    [Header("User Map Container")]
    public Transform userMapcontainer;
    private GameObject userMapSelect;

    [Header("Icon")]
    [SerializeField] Button closeBtn;
    [SerializeField] Button curSelectBtn; //todo 0415
    [SerializeField] Button spawnKey;
    private GameObject key;
    public GameObject Key { get { return key; }
        set { if (key != null) { Destroy(key); } key = value; }
    }
    int mapType; //0 : Main ,1: User
    List<Button> stageBtnList;

    [SerializeField] Button mainMapBtn;
    [SerializeField] Button userMapBtn;

    [Header("Btn Color")]
    Color activeColor = new Color(0.686f, 0.913f, 0.713f);

    Action<string> OnSelectItemEvent;

    public string curMapId;
    public bool onSelect;

    [Header("Screen")]
    GameObject computer;
    GameObject computerScreen;

    public Button CurSelectBtn
    {
        get { return curSelectBtn; }
        set
        {
            if (curSelectBtn != value)
            {
                //ResetStageInMapItem();
                curSelectBtn = value;
            }
        }
    }

    private TMP_Text _startText;

    [Header("Prefabs")]
    [SerializeField] GameObject ui_StageInMapSelectPrefab;
    [SerializeField] GameObject ui_UserMapSelectPrefab;


    //todo 0423 클라이언트 맵데이터 확인해야함.
    private void SelectMap(string mapId)
    {
#if UNITY_EDITOR
        if (mapId == curMapId) return;
        
        MapSelected(mapId, true);
#else
        if (mapId == curMapId || Managers.Game.OtherPlayer is null) return;

        var player = Managers.Game.Player.GetComponent<Player>();
        player.stageCheckCallback += MapSelected;
        player.CmdStageDataCheck(mapId);
#endif
    }

    public void MapSelected(string mapId, bool value)
    {
        var player = Managers.Game.Player.GetComponent<Player>();
        if (player.isServer)
            player.stageCheckCallback -= MapSelected;
        
        // TODO popup ui로 client가 해당 맵이 없다고 뜨게 표시 필요.
        if(!value) return;
        
        SetScreen(mapId); //스크린에 맵 데이터 표시 
        
        ResetSelect(); // 선택 버튼들 리셋
        curMapId = mapId;
        onSelect = true;

        if(mapType == 1)
        {
            StageBtnReset();
            ResetStageInMapItem();
        }
        else { ResetUserMapItem(); }
    }
    //todo 0423


    private void Awake()
    {
        stageBtnList = new();
        closeBtn.onClick.AddListener(CloseUI);
        spawnKey.onClick.AddListener(SpawnKey);
        mainMapBtn.onClick.AddListener(() => { OpenMainMapSelectUI();mapType = 0; });
        userMapBtn.onClick.AddListener(() => { OpenUserMapSelectUI(); mapType = 1; });

        OnSelectItemEvent += SelectMap;
        MapEditor.Instance.OnStageMove += ResetSelect;
        MapEditor.Instance.OnStageMove += StageBtnReset;
    }

    public override void OnEnable()
    {
        OpenUI();
    }

    protected override void Start()
    {
        foreach(var key in Managers.Data.mapData.mapMainStageDictionary.Keys)
        {
            Debug.Log(key);
        }


        var maps = Managers.Data.mapData.mapMainStageDictionary.Keys;
        stageInMapSelectList = new();


        for (int i = 0; i <= 1; i++)
        {
            Create(i);
        }

        CreateUserMap();
        //foreach (var key in maps)
        //{
        //    Create(key);
        //}

        //TEST 맵 시작 테스트 코드
        //var endButton = ResourceManager.Instantiate("Prefabs/UI/Button", layout).GetComponent<Button>();
        ////endButton.GetComponentInChildren<TMP_Text>().text = _startText.text;
        //endButton.onClick.AddListener(StartGame);
    }

    


    protected override void OpenUI() // Update select menu when clear stage
    {
        if (computerScreen == null) CreateComputerScreen();
        computerScreen.SetActive(true);
        Key = null;


        CheckCurStageLevel();
        CheckUserMapData();
        CheckClearItem();
        base.OpenUI();
    }

    protected override void CloseUI()
    {
        foreach(GameObject obj in stageInMapSelectList)
        {
            obj.SetActive(false);
        }

        ResetMapTypeBtn();
        base.CloseUI();
    }

    #region Create

    public void Create(int level)
    {
        if (level>1) return;
        CreateStageInMapUI(level);
        CreateStage(level);
    }

    private void CreateStageInMapUI(int level)
    {
        GameObject ui = Instantiate(ui_StageInMapSelectPrefab);
        RectTransform rt = ui.transform as RectTransform;
        UI_StageInMapSelect selectMap = ui.GetComponent<UI_StageInMapSelect>();
        selectMap.CreateStageInMap(level, OnSelectItemEvent); // todo 0423
        ui.transform.SetParent(mainMapcontainer);
        rt.anchoredPosition = Vector2.zero;
        stageInMapSelectList.Add(ui);
        ui.SetActive(false); 

    }
    private void CreateStage(int level)
    {
        var button = ResourceManager.Instantiate("Prefabs/UI/Button", mainMaplayout).GetComponent<Button>();
        button.GetComponentInChildren<TMP_Text>().text = level.ToString();
        string mapName = Managers.Data.mapData.mapMainStageDictionary[level][0].mapID;
        button.onClick.AddListener(() => { SelectStage(button); OpenStageInMapUI(level); });
        stageBtnList.Add(button);
    }


    public void SpawnKey()
    {
       if (curMapId != string.Empty)
        {
            GameObject key = Managers.Stage.CmdBatchObject("Key");
            Key = key;

            ObjectData data = MapEditor.Instance.curMap.FindObjectData(1000);
            Key.transform.position = data.position;
            Vector2 launchDirection = new Vector2(-1, 1).normalized;

            Key.GetComponent<Rigidbody2D>().AddForce(launchDirection * 5f, ForceMode2D.Impulse);

            CloseUI();
        }
           

      
    }



    private void CreateUserMap()
    {
        if (userMapSelect != null) Destroy(userMapSelect);
        userMapSelect = Instantiate(ui_UserMapSelectPrefab, userMapcontainer);
        UI_UserMapSelect ums = userMapSelect.GetComponent<UI_UserMapSelect>();
        ums.Create(OnSelectItemEvent);
        
    }

    private void CheckUserMapData()
    {
        if(userMapSelect != null)
        {
            if (userMapSelect.GetComponent<UI_UserMapSelect>().CheckUserMapData())
            {
                CreateUserMap();
            }
        }
    }

    private void CreateComputerScreen()
    {
        computer = MapEditor.Instance.FindObj(MapEditor.Instance.objectTransform, 1000);
        computerScreen = ResourceManager.Instantiate("Prefabs/UI/UI_ComputerScreen");
        computerScreen.transform.position = computer.transform.position + new Vector3(4, 4.5f);
        computerScreen.GetComponent<UI_ComputerScreen>().FadeIn();
    }


    private void SetScreen(string mapId)
    {
        computerScreen.GetComponent<UI_ComputerScreen>().SetData(mapId);
    }

    #endregion

    #region Button
  

    private void OpenMainMapSelectUI()
    {
        ResetMapTypeBtn();
        mainMapcontainer.gameObject.SetActive(true);
        Active_BtnChangeColor(mainMapBtn);
    }
    private void OpenUserMapSelectUI()
    {
        ResetMapTypeBtn();
        userMapcontainer.gameObject.SetActive(true);
        Active_BtnChangeColor(userMapBtn);
    }

    private void Active_BtnChangeColor(Button btn)
    {
        ColorBlock colorBlock = btn.colors;
        colorBlock.normalColor = activeColor;
        btn.colors = colorBlock;
    }
    private void Deactive_BtnChangeColor(Button btn)
    {
        if (!btn.interactable) btn.interactable = true;
        ColorBlock colorBlock = btn.colors;
        colorBlock.normalColor = Color.white;
        btn.colors = colorBlock;
    }



    #endregion

    #region Util

    #region MainMap
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

  

    #endregion

    private void CheckCurStageLevel() // Used when stage level up
    {
        if (Managers.Game.stageLevel > 1) return;
        if (stageInMapSelectList == null) return;
        if (Managers.Game.stageLevel > stageInMapSelectList.Count - 1)
        {
            //Managers.Data.mapData.GetMainStageMapData(Managers.Game.stageLevel);
            Create(Managers.Game.stageLevel);
        }

    }

    private void CheckClearItem()
    {
        if (stageInMapSelectList == null) return;
        foreach(GameObject obj in stageInMapSelectList)
        {
            UI_StageInMapSelect usims = obj.GetComponent<UI_StageInMapSelect>();
            usims.CheckStageClearItem();
        }
    }

    #endregion

    #region Reset
    private void StageBtnReset()
    {
        foreach(var button in stageBtnList)
        {
            button.GetComponent<Outline>().effectColor = Color.white;
        }
    }
    private void ResetMapTypeBtn()
    {
        Deactive_BtnChangeColor(mainMapBtn);
        Deactive_BtnChangeColor(userMapBtn);

        mainMapcontainer.gameObject.SetActive(false);
        userMapcontainer.gameObject.SetActive(false);
    }
    private void ResetSelect()
    {
        ResetStageInMapItem();
        ResetUserMapItem();
    }

    private void ResetStageInMapItem()
    {
        if (stageInMapSelectList == null) return;

        foreach (GameObject obj in stageInMapSelectList)
        {
            UI_StageInMapSelect selectMap = obj.GetComponent<UI_StageInMapSelect>();
            selectMap.ResetBtn();
        }
    }
    private void ResetUserMapItem()
    {
        if (userMapSelect != null)
            userMapSelect.GetComponent<UI_UserMapSelect>().ResetItem();
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
