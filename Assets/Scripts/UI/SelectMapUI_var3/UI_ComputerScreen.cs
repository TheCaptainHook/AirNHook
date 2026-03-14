using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UI_ComputerScreen : UI_Base
{
    [SerializeField] Transform contents;


    [SerializeField] Button arrow_Right;
    [SerializeField] Button arrow_Left;

    [Header("Prefabs")]
    [SerializeField] GameObject screenSlice_1Box;
    [SerializeField] GameObject screenSlice_2Box;

    ScreenSlice_1Box screen1;
    ScreenSlice_2Box screen2;

    [Header("Info")]
    bool onInteraction;

    private Map curMap;



    public override void OnEnable()
    {
        transform.localPosition = new Vector3(24, -2f);
    }

    private void Awake()
    {
        arrow_Right.onClick.AddListener(() => { if (!onInteraction) ArrowRight();});
        arrow_Left.onClick.AddListener(()=> { if (!onInteraction) ArrowLeft(); });
        screen1 = Instantiate(screenSlice_1Box, contents).GetComponent<ScreenSlice_1Box>();
        screen2 = Instantiate(screenSlice_2Box, contents).GetComponent<ScreenSlice_2Box>();
        MapEditor.Instance.fadeInOutPanel.preMapLoadEvent += TurnOff;
        //MapEditor.Instance.OnScreen += TurnOn;

    }


    //TestCode 0807
    // private void Update()
    // {
    //     if (Input.GetKeyDown(KeyCode.P))
    //     {
    //         SetData("Tutorial_1");
    //     }
    // }

    public void TurnOff()
    {
        //Reset();
        RectTransform rect = contents as RectTransform;
        rect.anchoredPosition = new Vector2(0, 0);
        Reset();
        transform.gameObject.SetActive(false);
    }

    public void Reset()
    {
        screen1.Reset();
        screen2.Reset();
    }

    public void TurnOn()
    {
        //Todo 0501

        //Refresh();
        //Todo 0501
        transform.gameObject.SetActive(true);
        //Refresh();
    }

    public void SetData(string mapId)
    {
        curMap = Managers.Data.mapData.mapAllDictionary[mapId];
        Texture2D texture = Resources.Load<Texture2D>($"Prefabs/MapScreenShot/{mapId}");

        if(curMap.dataType == 1)// 0 -> Main, 1 -> user map
        {
            UserMapData data = Managers.Data.mapData.mapUserDictionary[int.Parse(mapId)];
            //screen1.SetData(data.GetMapId(),data.mapImage);
            screen1.SetData(data.GetMapId(), texture);
            //screen2.SetData(Managers.Data.loadData.playData[mapId]);
            screen2.SetData(mapId);
        }
        else
        {
            //screen1.SetData(mapId, curMap.bytesImage);
            screen1.SetData(mapId, texture);
            //screen2.SetData(Managers.Data.loadData.playData[mapId]);
            screen2.SetData(mapId);
        }

       
    }

    //public void Refresh() // todo 0501
    //{
    //    if (curMap != null)
    //    {
    //        if (curMap.dataType == 1)
    //        {
    //            UserMapData data = Managers.Data.mapData.mapUserDictionary[int.Parse(curMap.mapID)];
    //            screen1.SetData(data.GetMapId(), data.mapImage);
    //            screen2.SetData(curMap.mapID);
    //        }
    //        else
    //        {
    //            screen1.SetData(curMap.mapID, curMap.bytesImage);
    //            //screen2.SetData(Managers.Data.loadData.playData[curMap.mapID]);
    //            screen2.SetData(curMap.mapID);
    //        }

    //    }


    //}


    private void ArrowRight()
    {
        arrow_Right.gameObject.SetActive(false);
        

        StartCoroutine(Co_Slice(0));

    }

    private void ArrowLeft()
    {
        arrow_Left.gameObject.SetActive(false);
        

        StartCoroutine(Co_Slice(1));
    }



    IEnumerator Co_Slice(int num)
    {
        onInteraction = true;

        float percent = 0;
        Vector2 target;
        if(num == 0)//right
        {
            target = new Vector2(-550, 0);
        }
        else//left
        {
            target = new Vector2(0, 0);
        }

        RectTransform rectContents = contents as RectTransform;

        while (percent < 1)
        {
            percent += Time.deltaTime;
            rectContents.anchoredPosition = Vector2.Lerp(rectContents.anchoredPosition, target, percent);

            yield return null;
        }

        if(num == 0)
        {
            arrow_Left.gameObject.SetActive(true);
        }
        else
        {
            arrow_Right.gameObject.SetActive(true);
        }

        onInteraction = false;
    }

    public void FadeIn()
    {
        StartCoroutine(Co_Fadein());
    }
    IEnumerator Co_Fadein()
    {
        float percent = 0;

        while(percent < 1)
        {
            percent += Time.deltaTime;
            transform.GetComponent<CanvasGroup>().alpha = percent;
            yield return null;
        }

    }

}
