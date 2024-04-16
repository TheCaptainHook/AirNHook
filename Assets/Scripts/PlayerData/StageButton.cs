using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class StageButton : MonoBehaviour
{
    public TextMeshProUGUI stageName;
    public TextMeshProUGUI clearTime;
    public TextMeshProUGUI clearDeath;

    private LoadData _loadData;


    //TODO 스테이지 진입했을때 실시간으로 변동되고, 스테이지를 클리어하면 그순간 변동을 멈추고 저장되도록해야함.
    public void StageSelect(PlayData playData)
    {
        stageName.text = playData.stageID;
        clearTime.text = playData.clearTime.ToString();
        clearDeath.text = playData.deathCount.ToString();
    }

    //TODO 메소드만들어서 클리어판정나오게
    public void Click()
    {
        var key = stageName.text;
        _loadData.stageData[key].stageClear = true;
    }

    public void LoadData(LoadData loadData)
    {
        _loadData = loadData;
    }
}