using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleSheet;
using UGS;


public class DataManager : MonoBehaviour
{
    public static DataManager Instance;


    
    bool mapDataReceived = false;
    public bool mapDataReceiveComplete = false;

    private void Awake()
    {
        if(Instance != null) { Destroy(gameObject); }
        else { Instance = this; }

    }

    //todo
    public void MapDataLoad()
    {
        StartCoroutine(Co_MapDataLoad());
        
    }


    public void Load<T>() where T : ITable
    {
        UnityGoogleSheet.Load<T>();
    }




    IEnumerator Co_MapDataLoad()
    {
        mapDataReceiveComplete = false;

        UnityGoogleSheet.LoadFromGoogle<string, MapData.Data>((list, map) =>
        {
            Debug.Log("Load"); //이부분 수정 데이터를 받아오면 이부분 실행
        }, true);


        while (!mapDataReceived)
        {
            MapReceiveData();
            yield return null;
        }

        mapDataReceived = false;
    }

    void MapReceiveData()
    {

        if (MapData.Data.DataList.Count > 0)
        {
            mapDataReceived = true;
            mapDataReceiveComplete = true;
        }
        else
        {
            Debug.Log("Load Data");
        }
    }
    //todo
}
