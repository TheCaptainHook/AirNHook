using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
public class UI_UserMapSelect : UI_Base
{
    [SerializeField] Transform contents;


    [Header("Prefabs")]
    [SerializeField] GameObject uI_UserMapSelectItem;
    List<UI_UserMapSelectItem> userMapSelectItemList;


    public override void OnEnable()
    {
    }


    public void Create(Action<string> action)
    {
        if(userMapSelectItemList == null)
        {
            userMapSelectItemList = new List<UI_UserMapSelectItem>();
        }

        foreach (int key in Managers.Data.mapData.mapUserDictionary.Keys)
        {
            UserMapData data = Managers.Data.mapData.mapUserDictionary[key];
            GameObject umsi = Instantiate(uI_UserMapSelectItem, contents);
            UI_UserMapSelectItem item = umsi.GetComponent<UI_UserMapSelectItem>();

            item.SetData(data, action); 

            userMapSelectItemList.Add(item);

        }
        Debug.Log(userMapSelectItemList.Count);
    }
    public void ResetItem()
    {
        foreach(UI_UserMapSelectItem item in userMapSelectItemList)
        {
            item.Reset();
        }
    }




   

}
