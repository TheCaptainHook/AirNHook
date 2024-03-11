using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UGS;
using Mirror;

public class MapManager: MonoBehaviour
{
    public static MapManager Instance;

    public Dictionary<string, Map> mapDictionary = new Dictionary<string, Map>();

    private void Awake()
    {
        if (Instance != null) Destroy(gameObject);
        else { Instance = this; }

    }


    private void Start()
    {
        LoadJsonFile();
        //UnityGoogleSheet.Load<MapData.Data>();
        //foreach (var value in MapData.Data.DataList)
        //{
        //    Debug.Log(value.ID);
        //}
        //DataManager.Instance.MapDataLoad();
        //DataManager.Instance.Load<MapData.Data>();


        //foreach (var value in MapData.Data.DataList)
        //{
        //    List<TileData> list = Util.FromJsonData<TileData>(value.TileData);
        //    Map map = new Map(value.ID, list, value.PlayerSpawnPot, value.PlayerExitPot, value.MapSize);
        //    mapDictionary.Add(value.ID, map);
        //    Debug.Log(map.mapID);
        //}
        //StartCoroutine(Co_SetMapData());

    }



    //IEnumerator Co_SetMapData()
    //{
    //    while (!DataManager.Instance.mapDataReceiveComplete)
    //    {
    //        yield return null;
    //    }

    //    foreach (var value in MapData.Data.DataList)
    //    {
    //        List<TileData> list = Util.FromJsonData<TileData>(value.TileData);
    //        Map map = new Map(value.ID, list, value.PlayerSpawnPot, value.PlayerExitPot,value.MapSize);            
    //        mapDictionary.Add(value.ID, map);
    //    }

    //}


    public void LoadJsonFile()
    {

        foreach(TextAsset json in Resources.LoadAll<TextAsset>("MapDat"))
        {
            Map map = JsonUtility.FromJson<Map>(json.text);
            mapDictionary.Add(map.mapID, map);
            Debug.Log(map.mapID);
        }
    }

}
