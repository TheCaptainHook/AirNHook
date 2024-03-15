using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using System.IO;

[System.Serializable]
public class Map
{
    public Vector2 mapSize;
    public string mapID;
    public Vector2 playerSpawnPosition;
    public ExitObjStruct exitObjStruct;
    public List<TileData> mapTileDataList = new List<TileData>();
    public float cellSize;

    [Header("Condition")]
   

    //public List<T> buttonActivatedDoorList = new List<T>();
    public List<ObjectDefaultInfo> buttonActivatedList = new List<ObjectDefaultInfo>();



    public Map(Vector2 mapSize, string id, Vector2 playerSpawnPosition, 
        ExitObjStruct exitObjStruct, List<TileData> list, float cellSize)
    {
        mapID = id;
        mapTileDataList = list;
        this.playerSpawnPosition = playerSpawnPosition;
        this.exitObjStruct = exitObjStruct;
        this.mapSize = mapSize;
        this.cellSize = cellSize;
    }

    public void CreateMap_Tile(Transform transform)
    {

        foreach (TileData data in mapTileDataList)
        {
            GameObject obj = Object.Instantiate(Resources.Load<GameObject>(data.path));
            obj.GetComponent<BuildObj>().tileData = data;
            obj.transform.position = data.position;
            obj.transform.SetParent(transform);

        }


        //리소스에서 스폰위치 오브젝트, 탈출위치오브젝트 생성

    }


 
}

[System.Serializable]
public struct ObjectDefaultInfo
{
    public int id;
    public Vector2 position;
    public string path;

    public ObjectDefaultInfo(int id, Vector2 position, string path)
    {
        this.id = id;
        this.position = position;
        this.path = path;
    }
}

[System.Serializable]
public struct ButtonActivatedDoorStruct
{
    public int id;
    public Vector2 position;
    public string path;
    public Vector2[] buttonActivatePosition;//Vector2의 개수만큼 버튼 생성

    public ButtonActivatedDoorStruct(int id, Vector2 position, string path, Vector2[] buttonActivatePosition)
    {
        this.id= id;
        this.position = position;
        this.path = path;
        this.buttonActivatePosition = buttonActivatePosition;
    }
}

[System.Serializable]
public struct ExitObjStruct
{
    public Vector2 position;
    public string path;
    public int condition_KeyAmount;

    public ExitObjStruct(Vector2 position,int condition_KeyAmount)
    {
        this.position = position;
        this.condition_KeyAmount = condition_KeyAmount;
        path = "/Prefabs/Map/Object/ExitPoint";
    }

}