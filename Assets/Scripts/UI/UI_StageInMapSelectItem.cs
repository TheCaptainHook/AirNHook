using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using UnityEditor.Search;
public class UI_StageInMapSelectItem : MonoBehaviour
{
    string mapId;
    int stageLevel;
    int index;
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] Button button;
    GameObject exitObj;
    public bool onSelect;
    [SerializeField] Outline outLine;

    bool onActive;

   
    public event Action OnSelectItem;

    public void CallOnSelectItem()
    {
        OnSelectItem?.Invoke();
    }

    public void SetData(string mapId,int stageLevel,int index)
    {
        this.mapId = mapId;
        text.text = mapId;
        this.stageLevel = stageLevel;
        this.index = index;

        if (Managers.Data.loadData.playData[mapId].stageClear)
        {
            text.color = Color.green;
        }

        button.onClick.AddListener(()=> { if (!onActive) { NextStage();} });
    }


    public void CheckStageClear()
    {
        if (Managers.Data.loadData.playData[mapId].stageClear)
        {
            text.color = Color.green;
        }
    }

    private void NextStage()
    {
        if (stageLevel > 0)
        {
            if(index == 0)
            {
                Map[] map = Managers.Data.mapData.mapMainStageDictionary[stageLevel - 1];
                Map lastMap = map[map.Length - 1];
                if (Managers.Data.loadData.playData[lastMap.mapID].stageClear){
                    onSelect = true;
                    exitObj = MapEditor.Instance.exitDoorObjectTransform.GetChild(0).gameObject;
                    exitObj.GetComponent<ExitPointObj>().nextMapId = mapId;
                    MapEditor.Instance.onStageSelect = true;
                }
                else
                {
                    StartCoroutine(Co_CantSelectEffect());  
                    return;
                }
            }else if (index > 0)
            {
                Map list = Managers.Data.mapData.mapMainStageDictionary[stageLevel][index - 1];
                if (Managers.Data.loadData.playData[list.mapID].stageClear)
                {
                    onSelect = true;
                    exitObj = MapEditor.Instance.exitDoorObjectTransform.GetChild(0).gameObject;
                    exitObj.GetComponent<ExitPointObj>().nextMapId = mapId;
                    MapEditor.Instance.onStageSelect = true;
                }
                else
                {
                    StartCoroutine(Co_CantSelectEffect());
                    return;
                }

            }
            else
            {
                onSelect = true;
                exitObj = MapEditor.Instance.exitDoorObjectTransform.GetChild(0).gameObject;
                exitObj.GetComponent<ExitPointObj>().nextMapId = mapId;
                MapEditor.Instance.onStageSelect = true;
            }
        }
        else
        {
            if (index > 0)
            {
                Map list = Managers.Data.mapData.mapMainStageDictionary[stageLevel][index - 1];
                if (Managers.Data.loadData.playData[list.mapID].stageClear)
                {
                    onSelect = true;
                    exitObj = MapEditor.Instance.exitDoorObjectTransform.GetChild(0).gameObject;
                    exitObj.GetComponent<ExitPointObj>().nextMapId = mapId;
                    MapEditor.Instance.onStageSelect = true;
                }
                else
                {
                    StartCoroutine(Co_CantSelectEffect());
                    return;
                }

            }
            else
            {
                onSelect = true;
                exitObj = MapEditor.Instance.exitDoorObjectTransform.GetChild(0).gameObject;
                exitObj.GetComponent<ExitPointObj>().nextMapId = mapId;
                MapEditor.Instance.onStageSelect = true;
            }
        }

        

        CallOnSelectItem();
    }


    public void SelectItem()
    {
        Debug.Log("select");
        outLine.effectColor = Color.red;
    }

    public void Reset()
    {
        onSelect = false;
        outLine.effectColor = Color.white;
    }




    IEnumerator Co_CantSelectEffect()
    {
        onActive = true;
        float percent = 0;
        text.color = Color.red;

        //item Cant Select Animation play

        while(percent < 1)
        {
            percent += Time.deltaTime;
            text.color = Color.Lerp(text.color, Color.white, percent);
            yield return null;
        }
        text.color = Color.white;
        onActive = false;
    }

}
