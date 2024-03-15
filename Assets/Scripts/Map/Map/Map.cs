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
    public ExitObjStruct exitObjStruct; //클리어 조건 포함
    public List<TileData> mapTileDataList = new List<TileData>();
    public float cellSize;

    //public List<T> buttonActivatedDoorList = new List<T>();
    public List<TileData> objectDataList = new List<TileData>();
    public List<TileData> buttonActivatedList = new List<TileData>();



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
        CreateObj(transform, mapTileDataList);
    }




    void CreateObj(Transform transform,List<TileData> list)
    {
        foreach (TileData data in list)
        {
            GameObject obj = Object.Instantiate(Resources.Load<GameObject>(data.path));
            obj.GetComponent<BuildObj>().tileData = data;
            obj.transform.position = data.position;
            obj.transform.SetParent(transform);

        }
    }
 

    // 타일 생성 -> 오브젝트 생성 -> 상호작용 오브젝트 생성 -> 스폰 탈출위치 생성


 
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

[System.Serializable]
public struct TileData
{
    public TileType tileType;
    public Vector2 position;
    public string path;

    public TileData(TileType tileType, Vector2 position, string path)
    {
        this.tileType = tileType;
        this.position = position;
        this.path = path;
    }
}