using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleSheet;
using UGS;
using GoogleSheet.Core.Type;
using GoogleSheet.Type;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance;


    //Asynchronous data processing
    bool mapDataReceived = false;
    public bool mapDataReceiveComplete = false;

    private void Awake()
    {
        if(Instance != null) { Destroy(gameObject); }
        else { Instance = this; }

    }

    public void MapDataLoad()
    {
        StartCoroutine(Co_MapDataLoad());
    }

    IEnumerator Co_MapDataLoad()
    {
        mapDataReceiveComplete = false;

        UnityGoogleSheet.LoadFromGoogle<string, MapData.Data>((list, map) =>
        {
            Debug.Log("Load");
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

}
