using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UGS;
using UnityEngine;

public class MapData
{

    public Dictionary<int, MapDataStruct> mapTileDataDictionary = new Dictionary<int, MapDataStruct>();
    public Dictionary<int, MapDataStruct> mapObjectDataDictionary = new Dictionary<int, MapDataStruct>();
    public Dictionary<int, MapDataStruct> mapSceneDataDictionary = new Dictionary<int, MapDataStruct>();

    public Dictionary<string, Map> mapTutorialDictionary = new Dictionary<string, Map>();
    public Dictionary<string, Map> mapMainDictionary = new Dictionary<string, Map>();
    public Dictionary<string, Map> mapUserDictionary = new Dictionary<string, Map>();

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
        UnityGoogleSheet.Load<MapObjectData.SceneData>();
        foreach (var value in MapObjectData.SceneData.SceneDataList)
        {
            mapSceneDataDictionary.Add(value.id, new MapDataStruct(value.type, value.path));
        }
    }

    void MapJsonLoad()
    {
        foreach (TextAsset json in Resources.LoadAll<TextAsset>("MapDat/Tutorial"))
        {
            Map map = JsonUtility.FromJson<Map>(json.text);
            mapTutorialDictionary.Add(map.mapID, map);
        }
        foreach (TextAsset json in Resources.LoadAll<TextAsset>("MapDat/Main"))
        {
            Map map = JsonUtility.FromJson<Map>(json.text);
            mapMainDictionary.Add(map.mapID, map);
        }
        foreach (TextAsset json in Resources.LoadAll<TextAsset>("MapDat/User"))
        {
            Map map = JsonUtility.FromJson<Map>(json.text);
            mapUserDictionary.Add(map.mapID, map);
        }
    }
    public Dictionary<string,Map> GetDictionary(MapType mapType)
    {
        switch (mapType)
        {
            case MapType.Tutorial:
                return mapTutorialDictionary;
            case MapType.Main:
                return mapMainDictionary;
            case MapType.User:
                return mapUserDictionary;
        }
        return null;
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
