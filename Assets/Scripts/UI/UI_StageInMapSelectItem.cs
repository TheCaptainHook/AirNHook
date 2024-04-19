using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
public class UI_StageInMapSelectItem : MonoBehaviour
{
    string mapId;
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] Button button;
    GameObject exitObj;
    public bool onSelect;
    [SerializeField] Outline outLine;



    public event Action OnSelectItem;

    public void CallOnSelectItem()
    {
        OnSelectItem?.Invoke();
    }

    public void SetData(string mapId)
    {
        this.mapId = mapId;
        text.text = mapId;

        if (Managers.Data.loadData.stageData[mapId].stageClear)
        {
            text.color = Color.green;
        }

        button.onClick.AddListener(()=> { NextStage(); CallOnSelectItem();  });
    }


    public void CheckStageClear()
    {
        if (Managers.Data.loadData.stageData[mapId].stageClear)
        {
            text.color = Color.green;
        }
    }

    private void NextStage()
    {
        Debug.Log(mapId);
        onSelect = true;
        exitObj = MapEditor.Instance.exitDoorObjectTransform.GetChild(0).gameObject;
        exitObj.GetComponent<ExitPointObj>().nextMapId = mapId;
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


}
