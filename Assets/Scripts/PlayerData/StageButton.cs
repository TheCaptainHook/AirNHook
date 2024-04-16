using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StageButton : MonoBehaviour
{
    public TextMeshProUGUI stageText;

    private LoadData _loadData;

    public void StageSelect(StageData playData)
    {
        stageText.text = playData.stageID.ToString();
    }

    //TODO 메소드만들어서 클리어판정나오게
    public void Click()
    {
        var key = stageText.text;
        _loadData.playerData[key].stageClear = true;
    }

    public void LoadData(LoadData loadData)
    {
        _loadData = loadData;
    }
}