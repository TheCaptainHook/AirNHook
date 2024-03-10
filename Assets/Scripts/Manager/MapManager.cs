using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UGS;

public class MapManager: MonoBehaviour
{
    public static MapManager Instance;

    Util Util = new Util();
    public Dictionary<string, Map> mapDictionary = new Dictionary<string, Map>();

    //Data Load
    private void Awake()
    {
        if (Instance != null) Destroy(gameObject);
        else { Instance = this; }

    }


    private void Start()
    {

        //UnityGoogleSheet.Load<MapData.Data>();
        //foreach (var value in MapData.Data.DataList)
        //{
        //    Debug.Log(value.ID);
        //}
        //DataManager.Instance.MapDataLoad();
        DataManager.Instance.Load<MapData.Data>();


        foreach (var value in MapData.Data.DataList)
        {
            List<TileData> list = Util.FromJsonData<TileData>(value.TileData);
            Map map = new Map(value.ID, list, value.PlayerSpawnPot, value.PlayerExitPot, value.MapSize);
            mapDictionary.Add(value.ID, map);
            Debug.Log(map.mapID);
        }
        //StartCoroutine(Co_SetMapData());

    }



    IEnumerator Co_SetMapData()
    {
        while (!DataManager.Instance.mapDataReceiveComplete)
        {
            yield return null;
        }

        foreach (var value in MapData.Data.DataList)
        {
            List<TileData> list = Util.FromJsonData<TileData>(value.TileData);
            Map map = new Map(value.ID, list, value.PlayerSpawnPot, value.PlayerExitPot,value.MapSize);            
            mapDictionary.Add(value.ID, map);
        }

    }




}
