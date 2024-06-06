using System.Collections;
using System.Collections.Generic;
using Org.BouncyCastle.Pqc.Crypto.Lms;
using UnityEngine;
using System;
using Unity.VisualScripting;

public class MainMapSelectContainer : MonoBehaviour
{

    [SerializeField] int allStage;
    [SerializeField] Transform layout;
    [SerializeField] GameObject mapSelectItem;


    MainMapStageSelectItem[] mainStageSelectUIs;


    private MainMapStageSelectItem curMainMapStageSelectItem;
    public MainMapStageSelectItem CurMainMapStageSelectItem
    {
        get
        {
            return curMainMapStageSelectItem;
        }
        set
        {
            if (curMainMapStageSelectItem != null && curMainMapStageSelectItem != value)
                curMainMapStageSelectItem.Reset();
            curMainMapStageSelectItem = value;
        }
    }


    [Header("Screen")]
    GameObject computer;
    GameObject computerScreen;
    public string curMapId;


    /// <summary>
    /// Deligates to initialize screen data when map is selected.
    /// </summary>
    event Action<string> OnSelectMapEvent;


    public GameObject ui_stageSelect_var_2;
    public Transform container;


    private void OnEnable()
    {
        if (mainStageSelectUIs == null)
        {
            CreateMapSelectItems();
        }

        if (computerScreen != null && !computerScreen.activeSelf) computerScreen.SetActive(true);

    }

    private void Awake()
    {
        OnSelectMapEvent += SelectMap;
        MapEditor.Instance.OnStageMove += Reset;

        if (computer == null) CreateComputerScreen();
        ui_stageSelect_var_2 = Managers.UI.GetUI<UI_StageSelect_Var2>().gameObject;

        container = Managers.UI.GetUI<UI_StageSelect_Var2>().GetComponent<UI_StageSelect_Var2>().container;

    }

   


    private void CreateMapSelectItems()
    {
        mainStageSelectUIs = new MainMapStageSelectItem[allStage+1];
        for(int i = 0; i<= allStage; i++)
        {
            MainMapStageSelectItem item = Instantiate(mapSelectItem, layout).GetComponent<MainMapStageSelectItem>();
            mainStageSelectUIs[i] = item;
            item.SetData(i.ToString(), Managers.Data.mapData.mapMainStageDictionary[i],container,this,OnSelectMapEvent);


        }

    }



    public void Reset()
    {
        if (CurMainMapStageSelectItem) CurMainMapStageSelectItem.Reset();
    }



    #region Computer, Screen

    private void CreateComputerScreen()
    {
        computer = MapEditor.Instance.FindObj(MapEditor.Instance.objectTransform, 1000);
        computerScreen = ResourceManager.Instantiate("Prefabs/UI/UI_ComputerScreen");
        computerScreen.transform.position = computer.transform.position + new Vector3(4, 4.5f);
    }


    private void SetScreen(string mapId)
    {
        computerScreen.GetComponent<UI_ComputerScreen>().SetData(mapId);
    }


    //todo 0423 클라이언트 맵데이터 확인해야함.
    private void SelectMap(string mapId)
    {
#if UNITY_EDITOR
        if (mapId == curMapId) return;
        MapSelected(mapId, true);
#else
        if (mapId == curMapId || Managers.Game.OtherPlayer is null) return;
        
        Managers.Command.stageCheckCallback += MapSelected;
        Managers.Command.StageDataCheck(mapId);
#endif
    }

    public void MapSelected(string mapId, bool value)
    {
        var player = Managers.Game.Player.GetComponent<Player>();
        if (player.isServer)
            Managers.Command.stageCheckCallback -= MapSelected;

        // TODO popup ui로 client가 해당 맵이 없다고 뜨게 표시 필요.
        if (!value) return;

        SetScreen(mapId); //스크린에 맵 데이터 표시 

    }


    #endregion
}
