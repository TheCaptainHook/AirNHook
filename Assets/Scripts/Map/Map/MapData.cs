
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UGS;
using UnityEngine;

using MapType = ANH_MapEditor.MapType;

public class MapData
{
    public Dictionary<int, MapDataStruct> mapObjectDataDictionary = new Dictionary<int, MapDataStruct>();

    public Dictionary<string, Map> mapSceneDictionary = new Dictionary<string, Map>();
    public Dictionary<string, Map> mapMainDictionary = new Dictionary<string, Map>();
    public Dictionary<string, Map> mapAllDictionary = new Dictionary<string, Map>(); // todo 0423


    public Dictionary<int, Map[]> mapMainStageDictionary = new Dictionary<int, Map[]>();

    //Get.Keys, check other user have map
    public Dictionary<int, UserMapData> mapUserDictionary = new Dictionary<int, UserMapData>(); // todo 0423

    public void SetUp()
    {
        UGS_MapDataLoad();
        MapJsonLoad();
    }

    void UGS_MapDataLoad()
    {
        //Tile Data
        UnityGoogleSheet.LoadAllData();
        
        foreach (var value in MapObjectData.TileData.TileDataList)
        {
            mapObjectDataDictionary.Add(value.id, new MapDataStruct(value.id,value.name,value.type, value.path));
        }
        //Object Data
        foreach (var value in MapObjectData.ObjectData.ObjectDataList)
        {
            mapObjectDataDictionary.Add(value.id, new MapDataStruct(value.id,value.name, value.type, value.path));
        }
        
        foreach (var value in MapObjectData.SceneData.SceneDataList)
        {
            mapObjectDataDictionary.Add(value.id, new MapDataStruct(value.id,value.name, value.type, value.path));
        }
        
        foreach (var value in MapObjectData.OtherData.OtherDataList)
        {
            var type = GetObjectType(value.type);
            mapObjectDataDictionary.Add(value.id, new MapDataStruct(value.id,value.name, type.type, value.path,type.subType));
        }

        foreach (var value in MapObjectData.BackGroundData.BackGroundDataList)
        {
            mapObjectDataDictionary.Add(value.id, new MapDataStruct(value.id,value.name, value.type, value.path));
        }

    }

    void MapJsonLoad()
    {
        SceneMapDataLoad();

        MainMapDataLoad();

        //User Map Data Load
        // UserMapDataLoad();
        //User Map Data Load

    }
    private void SceneMapDataLoad()
    {
        foreach (TextAsset json in Resources.LoadAll<TextAsset>("MapDat/Scene"))
        {
            Map map = JsonUtility.FromJson<Map>(json.text);
            mapSceneDictionary.Add(map.mapID, map);
            mapAllDictionary.Add(map.mapID, map);
        }
    }


    private void MainMapDataLoad()
    {
        // int index = GetMainStageLevelIndex();
        for (int i = 0; i <= 1; i++)
        {
            GetMainStageMapData(i);
        }
    }

    #region Main Map Load
    public void GetMainStageMapData(int level)
    {
        TextAsset[] jsons = Resources.LoadAll<TextAsset>($"MapDat/Main/{level}");
        if (jsons.Length != 0)
        {
            Map[] maps = new Map[jsons.Length];

            for (int i = 0; i < maps.Length; i++)
            {
                Map map = JsonUtility.FromJson<Map>(jsons[i].text);
                maps[i] = map;
                mapMainDictionary.Add(map.mapID, map);
                mapAllDictionary.Add(map.mapID, map);
            }
            mapMainStageDictionary.Add(level, maps);
        }


    }

    int GetMainStageLevelIndex()
    {
        string path = Path.Combine(Application.dataPath, "Resources/MapDat/Main");
        int index = 0;
        while (true)
        {
            if (Directory.Exists(Path.Combine(path, index.ToString())))
            {
                index++;

            }
            else { break; }

        }
        return index - 1;
    }

    #endregion

    #region User Map Load
    public void RefreshUserMapData()
    {
        string path = Path.Combine(Application.dataPath, "UserMapData");
        string[] filePaths = Directory.GetFiles(path, "*.json");

        foreach (string filePath in filePaths)
        {
            string jsonString = File.ReadAllText(filePath);
            UserMapData data = JsonUtility.FromJson<UserMapData>(jsonString);
            if (!mapUserDictionary.ContainsKey(data.hashValue))
            {
                mapUserDictionary.Add(data.hashValue, data);

                mapAllDictionary.Add(data.hashValue.ToString(), data.LoadMap());
            }

        }
    }
    #endregion


    public Dictionary<string,Map> GetDictionary(MapType mapType)
    {
        switch (mapType)
        {
            case MapType.Scene:
                return mapSceneDictionary;
            case MapType.Main:
                return mapMainDictionary;
        }
        return null;
    }

    public Map[] GetMainMapStageArray(int level){
        return mapMainStageDictionary[level];
    }

 

private (ObjectType type,string[] subType) GetObjectType(string objectType){
    string[] arr = objectType.Split("/");
    if(arr.Length>1){
        return(GetType(arr[0]),arr.Skip(1).ToArray());
        
    }else{
        return (GetType(arr[0]),null);
    }
}

private ObjectType GetType(string type){
    switch(type){
        case "Tile":
        return ObjectType.Tile;
         case "Object":
        return ObjectType.Object;
         case "N_Object":
        return ObjectType.N_Object;
         case "Background":
        return ObjectType.Background;
         case "Other":
        default :
        return ObjectType.Other;
        
        
    }
}

}


public struct MapDataStruct
{
    public int id;
    public ObjectType objectType;
    public string[] subObjectType;
    public string name;
    public string path;

    public MapDataStruct(int id,string name ,ObjectType objectType,string path,string[] subObjectType = null)
    {
        this.id = id;
        this.name = name;
        this.objectType = objectType;
        this.subObjectType = subObjectType;
        this.path = path;
    }
}
