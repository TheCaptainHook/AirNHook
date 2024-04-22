using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
public class UserMapDataBoxItem : MonoBehaviour
{
    [SerializeField] Image mapImage;
    [SerializeField] TextMeshProUGUI mapIdText;
    [SerializeField] TextMeshProUGUI createDate;
    public Map map;

    public event Action OnLoadUserMap;


    [SerializeField] Button loadBtn;

    private void Awake()
    {
        loadBtn.onClick.AddListener(() => { CallOnLoadUserMap(); MapEditor.Instance.editorUIController.LoadUserMapEditorInit(map); });
    }


    private void CallOnLoadUserMap()
    {
        OnLoadUserMap?.Invoke();
    }


    public void SetData(UserMapData userMapData)
    {
        map = userMapData.LoadMap();
        mapIdText.text = map.mapID;
        DateTimeData date = userMapData.LoadDateTimeData();

        createDate.text = $"{date.year}년 {date.month}월 {date.day}일\n{date.hour}시 {date.minute}분 {date.second}초";
    }


}
