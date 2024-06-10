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
    [SerializeField] GameObject particle;

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
        particle.SetActive(true);
    }
    public void Deactivation()
    {
        particle.SetActive(false);
    }

}
