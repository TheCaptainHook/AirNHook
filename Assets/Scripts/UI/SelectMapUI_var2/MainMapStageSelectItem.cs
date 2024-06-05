using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainMapStageSelectItem : MonoBehaviour
{
    MainMapSelectContainer mainMapSelectContainer;

    [SerializeField] GameObject stageSelectScrollView;
    [SerializeField] TextMeshProUGUI text;


    private StageSelectScrollView curStageSelectScrollView;
    public StageSelectScrollView CurStageSelectScrollView {
        get
        {
            return curStageSelectScrollView;
        }
        set
        {

        }

    }//todo 0605

    Map[] maps;
    Button btn;



    private void Awake()
    {
        btn = GetComponent<Button>();
    }




    public void SetData(string stageLevel, Map[] maps,Transform transform,MainMapSelectContainer mainMapSelectContainer)
    {

        text.text = stageLevel;
        this.maps = maps;
        this.mainMapSelectContainer = mainMapSelectContainer;

        CreateStageSelectScrollView(transform);

        btn.onClick.AddListener(() => {  }); //todo 0605

    }



    private void CreateStageSelectScrollView(Transform transfrom)
    {

        curStageSelectScrollView = Instantiate(stageSelectScrollView,transfrom).GetComponent<StageSelectScrollView>();
        curStageSelectScrollView.SetData(maps);
        curStageSelectScrollView.gameObject.SetActive(false);

    }



    private void Reset()
    {
        
    }
}
