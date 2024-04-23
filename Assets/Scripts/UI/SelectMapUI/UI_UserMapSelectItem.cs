using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
public class UI_UserMapSelectItem : MonoBehaviour
{
    UserMapData data;
    string dateTitleText = "Upload Date:";

    [Header("Components")]
    [SerializeField] Button selectBtn;
    [SerializeField] TextMeshProUGUI mapIdText;
    [SerializeField] TextMeshProUGUI dateText;
    [SerializeField] Outline outline;

    [Header("Info")]
    private string mapId;

    ExitPointObj exitdoor;

    public event Action OnSelectEvent;

    private void Awake()
    {
        exitdoor = MapEditor.Instance.exitDoorObjectTransform.GetChild(0).GetComponent<ExitPointObj>();

    }

    public void SetData(UserMapData data, Action<string> action)
    {
        this.data = data;
        mapId = data.GetMapId();
        mapIdText.text = mapId;
        DateTimeData date = data.LoadDateTimeData();
        dateText.text = $"{dateTitleText} {date.year}/{date.month}/{date.day}  {date.hour}:{date.minute}:{date.second}";

        selectBtn.onClick.AddListener(() => { action?.Invoke(mapId); SelectItem(); });
   
        
    }



    public void SelectItem()
    {
        outline.effectColor = Color.red;
        exitdoor.nextMapId = mapId;

    }

    public void Reset()
    {
        outline.effectColor = Color.white;
    }

}
