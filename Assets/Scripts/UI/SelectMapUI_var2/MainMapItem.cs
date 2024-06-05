using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainMapItem : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text;
    Map map;
    int index;

    [SerializeField]  Button btn;

    StageSelectScrollView curStageSelectScrollView;


    public void SetData(Map map, int index,StageSelectScrollView stageSelectScrollView)
    {
        this.map = map;
        text.text = map.mapID;

        this.index = index;
        curStageSelectScrollView = stageSelectScrollView;

        btn.onClick.AddListener(() => { }); //todo 0605

    }



    private void NextStage()
    {
        
    }


}
