using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_StageInMapSelect : UI_Base
{
    [SerializeField] Transform contents;
    [SerializeField] GameObject ui_StageInMapSelectItem;
    List<UI_StageInMapSelectItem> itemList;

    UI_StageInMapSelectItem curItem;

    public override void OnEnable()
    {
        OpenUI();
    }

    public void CreateStageInMap(int stageLevel)
    {
        itemList = new();
        Map[] maps = Managers.Data.mapData.mapMainStageDictionary[stageLevel];
        foreach(Map map in maps)
        {
            GameObject selectItem = Instantiate(ui_StageInMapSelectItem);
            selectItem.transform.SetParent(contents);
            UI_StageInMapSelectItem item = selectItem.GetComponent<UI_StageInMapSelectItem>();
            item.SetData(map.mapID);
            item.OnSelectItem += CheckSelectItem;
            item.OnSelectItem += CloseUI;
            itemList.Add(item);
        }
    }


    public void CheckSelectItem()
    {
        if(curItem != null)
        {
            curItem.Reset();
        }

        foreach (UI_StageInMapSelectItem item in itemList)
        {
            if (item.onSelect)
            {
                curItem = item;
                item.SelectItem();
            }
        }


    }


    protected override void CloseUI()
    {
 
        base.CloseUI();
    }


    public void ResetBtn()
    {
        foreach(UI_StageInMapSelectItem item in itemList)
        {
            item.Reset();
        }
    }






}
