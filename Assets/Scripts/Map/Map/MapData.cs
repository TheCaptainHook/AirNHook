using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UGS;
using UnityEngine;

public class MapData
{

    public Dictionary<int, MapObjectDataStruct> mapObjectDataDictionary = new Dictionary<int, MapObjectDataStruct>();
    public Dictionary<string, Map> mapDictionary = new Dictionary<string, Map>();

    public void SetUp()
    {
        foreach (TextAsset json in Resources.LoadAll<TextAsset>("MapDat"))
        {
            Map map = JsonUtility.FromJson<Map>(json.text);
            mapDictionary.Add(map.mapID, map);
            Debug.Log(map.mapID);
        }
    }

    public MapObjectDataStruct GetMapObjData(int id)
    {
        return mapObjectDataDictionary[id];
    }

}


public struct MapObjectDataStruct
{
    public int id;
    public TileType tileType;
    public string path;

    public MapObjectDataStruct(int id,TileType tileType,string path)
    {
        this.id = id;
        this.tileType = tileType;
        this.path = path;
    }
}
