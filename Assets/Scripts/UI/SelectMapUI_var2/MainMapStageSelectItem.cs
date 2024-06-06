using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
public class MainMapStageSelectItem : MonoBehaviour
{
    MainMapSelectContainer mainMapSelectContainer;

    [SerializeField] GameObject stageSelectScrollView;
    [SerializeField] TextMeshProUGUI text;


    private StageSelectScrollView curStageSelectScrollView;


    Map[] maps;
    Button btn;



    private void Awake()
    {
        btn = GetComponent<Button>();
    }




    public void SetData(string stageLevel, Map[] maps,Transform transform,MainMapSelectContainer mainMapSelectContainer, Action<string> action)
    {

        text.text = stageLevel;
        this.maps = maps;
        this.mainMapSelectContainer = mainMapSelectContainer;

        CreateStageSelectScrollView(transform,action);

        btn.onClick.AddListener(() =>
        {
            mainMapSelectContainer.CurMainMapStageSelectItem = this;
            Activation();
            curStageSelectScrollView.gameObject.SetActive(true);


        }); //todo 0605

    }



    private void CreateStageSelectScrollView(Transform transfrom,Action<string> action)
    {

        curStageSelectScrollView = Instantiate(stageSelectScrollView,transfrom).GetComponent<StageSelectScrollView>();
        curStageSelectScrollView.SetData(maps,action);
        curStageSelectScrollView.gameObject.SetActive(false);

    }



    public void Reset()
    {
        Deactivation();
        curStageSelectScrollView.Reset();
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
