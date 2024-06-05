using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMapSelectContainer : MonoBehaviour
{

    [SerializeField] int allStage;
    [SerializeField] Transform layout;
    [SerializeField] GameObject mapSelectItem;


    /// <summary>
    /// mainStageSelectUis를 포이치로 돌면서 리셋해버리면 스크롤도 다 리셋.
    /// </summary>

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

        }
    }//todo 0605




    private void OnEnable()
    {
        if (mainStageSelectUIs == null)
        {
            CreateMapSelectItems();
        }
    }

    private void CreateMapSelectItems()
    {
        mainStageSelectUIs = new MainMapStageSelectItem[allStage+1];
        for(int i = 0; i<= allStage; i++)
        {
            MainMapStageSelectItem item = Instantiate(mapSelectItem, layout).GetComponent<MainMapStageSelectItem>();
            mainStageSelectUIs[i] = item;
            item.SetData(i.ToString(), Managers.Data.mapData.mapMainStageDictionary[i],transform,this);


        }

    }

}
