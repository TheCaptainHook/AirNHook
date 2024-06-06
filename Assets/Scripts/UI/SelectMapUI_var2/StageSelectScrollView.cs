using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
public class StageSelectScrollView : MonoBehaviour
{
    
    [SerializeField]Transform content;
    [SerializeField] GameObject stageSelectItem;

    [SerializeField] Button confirmBtn;

    List<MainMapItem> mainMapItems;


    private MainMapItem curMainMapItem;
    public MainMapItem CurMainMapItem
    {
        get { return curMainMapItem; }
        set
        {
            if(curMainMapItem != null && curMainMapItem != value)
            {
                Reset();
            }

            curMainMapItem = value;
        }
    }


    private void Awake()
    {
        confirmBtn.onClick.AddListener(Confirm);
    }


    public void SetData(Map[] maps,Action<string> action)
    {
        mainMapItems = new();
        for (int i = 0; i < maps.Length; i++)
        {
            MainMapItem item = Instantiate(stageSelectItem, content).GetComponent<MainMapItem>();
            mainMapItems.Add(item);
            item.SetData(maps[i],i,this,action);
        }
    }


    public void Reset()
    {
        if(CurMainMapItem)
        CurMainMapItem.Reset();
    }


    private void Confirm()
    {
        gameObject.SetActive(false);
    }
}
