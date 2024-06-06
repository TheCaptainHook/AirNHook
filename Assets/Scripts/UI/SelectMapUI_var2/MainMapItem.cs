using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Mirror;
using Unity.VisualScripting;
using System;

public class MainMapItem : MonoBehaviour
{

    [Header("Info")]
    [SerializeField] Button btn;
    [SerializeField] TextMeshProUGUI text;
    Map curMap;
    int curIndex;
    bool stageClear;


    //Parents ScrollView
    StageSelectScrollView curStageSelectScrollView;



    private void OnEnable()
    {
        if (!stageClear && curMap != null)
            CheckStageClear();
    }


    public void SetData(Map map, int index, StageSelectScrollView stageSelectScrollView,Action<string> action)
    {
        text.text = map.mapID;

        curMap = map;
        curIndex = index;
        curStageSelectScrollView = stageSelectScrollView;

        btn.onClick.AddListener(() =>
        {
            if (CheckNextStageCondition())
            {
                curStageSelectScrollView.CurMainMapItem = this;
                Activation();
                NextStage(action);
            }


        });

    }

    #region Next Stage
    private void NextStage(Action<string> action)
    {
        if (MapEditor.Instance)
        {
            GameObject exitObj = MapEditor.Instance.exitDoorObjectTransform.GetChild(0).gameObject;
            exitObj.GetComponent<ExitPointObj>().nextMapId = curMap.mapID;
            action.Invoke(curMap.mapID);
        }

    }

    private bool CheckNextStageCondition()
    {
        int stageLevel = curMap.stageLevel;

        // Check if it's the first index in the current stage level
        if (stageLevel > 0)
        {
            if (curIndex == 0)
            {
                // Check the last map of the previous stage level
                Map[] previousStageMaps = Managers.Data.mapData.mapMainStageDictionary[stageLevel - 1];
                Map lastMap = previousStageMaps[previousStageMaps.Length - 1];
                return Managers.Data.loadData.playData[lastMap.mapID].stageClear;
            }
        }
        else
        {
            if (curIndex == 0) return true;
        }


        Map beforeMap = Managers.Data.mapData.mapMainStageDictionary[stageLevel][curIndex - 1];
        return Managers.Data.loadData.playData[beforeMap.mapID].stageClear;

    }

    #endregion

    public void CheckStageClear()
    {
        if (Managers.Data.loadData.playData[curMap.mapID].stageClear)
        {
            stageClear = true;

            //
            text.color = Color.green;
            //
        }
    }



    public void Reset()
    {
        Deactivation();
        //탈출 문 정보도 리셋, 열쇠 제거.
        if (MapEditor.Instance)
        {
            GameObject exitObj = MapEditor.Instance.exitDoorObjectTransform.GetChild(0).gameObject;
            exitObj.GetComponent<ExitPointObj>().nextMapId = string.Empty;
        }
       

    }

    public void Activation()
    {
        GetComponent<Image>().color = Color.blue;
    }
    public void Deactivation()
    {
        GetComponent<Image>().color = Color.white;
    }


   

}
