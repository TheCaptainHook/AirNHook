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
    public List<ObjectData> mapTileDataList = new List<ObjectData>();
    public List<ObjectData> mapObjectDataList = new List<ObjectData>();
    public List<ButtonActivatedDoorStruct> mapButtonActivatedDoorDataList = new List<ButtonActivatedDoorStruct>();

    public float cellSize;


    public Map(Vector2 mapSize, string id, Vector2 playerSpawnPosition,
        ExitObjStruct exitObjStruct, 
        List<ObjectData> tileList, 
        List<ObjectData> objectList,
        List<ButtonActivatedDoorStruct> mapButtonActivatedDoorDataList,
        float cellSize)
    {
        mapID = id;
        mapTileDataList = tileList;
        mapObjectDataList = objectList;
        this.playerSpawnPosition = playerSpawnPosition;
        this.exitObjStruct = exitObjStruct;
        this.mapSize = mapSize;
        this.mapButtonActivatedDoorDataList = mapButtonActivatedDoorDataList;
        this.cellSize = cellSize;
    }


}


[System.Serializable]
public struct ButtonActivatedDoorStruct
{
    public int id;
    public Vector2 position;
    public Vector2[] buttonActivatePosition;//Vector2의 개수만큼 버튼 생성

    public ButtonActivatedDoorStruct(int id, Vector2 position, Vector2[] buttonActivatePosition)
    {
        this.id= id;
        this.position = position;
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
public struct ObjectData
{
    public int id;
    public Vector2 position;
    public Quaternion quaternion;

    public ObjectData(int id,Vector2 position)
    {
        this.id = id;
        this.position = position;
        quaternion = Quaternion.identity;
    }
    public ObjectData(int id, Vector2 position,Quaternion quaternion)
    {
        this.id = id;
        this.position = position;
        this.quaternion = quaternion;
    }

}
