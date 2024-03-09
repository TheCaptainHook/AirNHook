using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        DataManager.Instance.MapDataLoad();

        StartCoroutine(Co_SetMap());
    }



    IEnumerator Co_SetMap()
    {
        while (!DataManager.Instance.mapDataReceiveComplete)
        {
            yield return null;
        }

        foreach (var value in MapData.Data.DataList)
        {
            List<TileData> list = Util.FromJsonData<TileData>(value.TileData);
            Map map = new Map(value.ID, list, value.PlayerSpawnPot, value.PlayerExitPot);            
            mapDictionary.Add(value.ID, map);
        }

        MapEditor.Instance.mapTileDataList = mapDictionary["TestMap"].mapTileDataList;

    }




}
