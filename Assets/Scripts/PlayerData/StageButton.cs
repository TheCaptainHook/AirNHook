using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StageButton : MonoBehaviour
{
    public TextMeshProUGUI stageText;

    private LoadData _loadData;
    //
    //private void Awake()
    //{
    //    _loadData = GetComponent<LoadData>();
    //}

    public void StageSelect(StageData playData)
    {
        stageText.text = playData.StageID.ToString();
    }

    //1~5의 버튼을 누르면 누른번호에 해당되는 StageClear값이 true로 
    //TODO 메소드만들어서 클리어판정나오게
    public void Click()
    {
        var key = stageText.text;
        _loadData.playerData[key].StageClear = true;
    }

    public void LoadData(LoadData loadData)
    {
        _loadData = loadData;
    }
}