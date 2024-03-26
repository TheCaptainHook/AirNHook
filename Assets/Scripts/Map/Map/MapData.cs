using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UGS;
using UnityEngine;

public class MapData
{

    public Dictionary<int, MapDataStruct> mapTileDataDictionary = new Dictionary<int, MapDataStruct>();

    public Dictionary<int, MapDataStruct> mapObjectDataDictionary = new Dictionary<int, MapDataStruct>();

    public Dictionary<string, Map> mapDictionary = new Dictionary<string, Map>();

    public void SetUp()
    {

        UGS_MapDataLoad();

        MapJsonLoad();        

    }


    void UGS_MapDataLoad()
    {
        //Tile Data
        UnityGoogleSheet.Load<MapObjectData.TileData>();
        foreach (var value in MapObjectData.TileData.TileDataList)
        {
            mapTileDataDictionary.Add(value.id, new MapDataStruct(value.type, value.path));
        }
        //Object Data
        UnityGoogleSheet.Load<MapObjectData.ObjectData>();
        foreach (var value in MapObjectData.ObjectData.ObjectDataList)
        {
            mapObjectDataDictionary.Add(value.id, new MapDataStruct(value.type, value.path));
        }
    }

    void MapJsonLoad()
    {
        foreach (TextAsset json in Resources.LoadAll<TextAsset>("MapDat"))
        {
            Map map = JsonUtility.FromJson<Map>(json.text);
            mapDictionary.Add(map.mapID, map);
            Debug.Log(map.mapID);
        }
    }

}


public struct MapDataStruct
{
    public TileType tileType;
    public string path;

    public MapDataStruct(TileType tileType,string path)
    {
        this.tileType = tileType;
        this.path = path;
    }
}
