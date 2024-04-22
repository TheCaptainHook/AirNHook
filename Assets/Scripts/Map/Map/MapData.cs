
using System.Collections.Generic;
using System.IO;
using UGS;
using UnityEngine;

public class MapData
{
    public Dictionary<int, MapDataStruct> mapTileDataDictionary = new Dictionary<int, MapDataStruct>();
    public Dictionary<int, MapDataStruct> mapObjectDataDictionary = new Dictionary<int, MapDataStruct>();
    public Dictionary<int, MapDataStruct> mapSceneDataDictionary = new Dictionary<int, MapDataStruct>();
    public Dictionary<int, MapDataStruct> mapOtherDataDictionary = new Dictionary<int, MapDataStruct>();

    public Dictionary<string, Map> mapSceneDictionary = new Dictionary<string, Map>();
    public Dictionary<string, Map> mapMainDictionary = new Dictionary<string, Map>();
    public Dictionary<string, Map> mapUserDictionary = new Dictionary<string, Map>();

    public Dictionary<int, Map[]> mapMainStageDictionary = new Dictionary<int, Map[]>();
    public void SetUp()
    {

        UGS_MapDataLoad();
        MapJsonLoad();
        
    }

    void UGS_MapDataLoad()
    {
        //Tile Data
        UnityGoogleSheet.LoadAllData();
        //UnityGoogleSheet.Load<MapObjectData.TileData>();
        foreach (var value in MapObjectData.TileData.TileDataList)
        {
            mapTileDataDictionary.Add(value.id, new MapDataStruct(value.name,value.type, value.path));
        }
        //Object Data
        //UnityGoogleSheet.Load<MapObjectData.ObjectData>();
        foreach (var value in MapObjectData.ObjectData.ObjectDataList)
        {
            mapObjectDataDictionary.Add(value.id, new MapDataStruct(value.name, value.type, value.path));
        }
        //UnityGoogleSheet.Load<MapObjectData.SceneData>();
        foreach (var value in MapObjectData.SceneData.SceneDataList)
        {
            mapSceneDataDictionary.Add(value.id, new MapDataStruct(value.name, value.type, value.path));
        }
        //UnityGoogleSheet.Load<MapObjectData.OtherData>();
        foreach (var value in MapObjectData.OtherData.OtherDataList)
        {
            mapOtherDataDictionary.Add(value.id, new MapDataStruct(value.name, value.type, value.path));
        }

    }

    void MapJsonLoad()
    {
        foreach (TextAsset json in Resources.LoadAll<TextAsset>("MapDat/Scene"))
        {
            Map map = JsonUtility.FromJson<Map>(json.text);
            mapSceneDictionary.Add(map.mapID, map);
        }

        //todo
        for(int i = 0; i<= 3; i++)
        {
            GetMainStageMapData(i);
            
        }
        foreach (TextAsset json in Resources.LoadAll<TextAsset>("MapDat/User"))
        {
            Map map = JsonUtility.FromJson<Map>(json.text);
            mapUserDictionary.Add(map.mapID, map);
        }
    }

    public void GetMainStageMapData(int level)
    {
        string path = Path.Combine(Application.dataPath, $"Resources/MapDat/Main/{level}");
 
        if (Directory.Exists(path)){
            TextAsset[] jsons = Resources.LoadAll<TextAsset>($"MapDat/Main/{level}");
            if (jsons.Length != 0)
            {
                Map[] maps = new Map[jsons.Length];
                for (int j = 0; j < maps.Length; j++)
                {
                    maps[j] = JsonUtility.FromJson<Map>(jsons[j].text);
                }
                mapMainStageDictionary.Add(level, maps);
                for (int j = 0; j < jsons.Length; j++)
                {
                    Map map = JsonUtility.FromJson<Map>(jsons[j].text);
                    mapMainDictionary.Add(map.mapID, map);
                }
                Managers.Data.loadData.Setup();
            }


       
        }
        //mapMainStageDictionary.Add(level, maps);
        //for (int j = 0; j < jsons.Length; j++)
        //{
        //    Map map = JsonUtility.FromJson<Map>(jsons[j].text);
        //    mapMainDictionary.Add(map.mapID, map);
        //}
        //Managers.Data.loadData.Setup();

    }

    public Dictionary<string,Map> GetDictionary(MapType mapType)
    {
        switch (mapType)
        {
            case MapType.Scene:
                return mapSceneDictionary;
            case MapType.Main:
                return mapMainDictionary;
            case MapType.User:
                return mapUserDictionary;
        }
        return null;
    }

    public  Map[] GetDictionary(int i)
    {
        return mapMainStageDictionary[i];

    }




}


public struct MapDataStruct
{
    public TileType tileType;
    public string name;
    public string path;

    public MapDataStruct(string name ,TileType tileType,string path)
    {
        this.name = name;
        this.tileType = tileType;
        this.path = path;
    }
}
