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
    public Vector2 startPosition;
    public List<TileData> mapTileDataList = new();
    public List<ObjectData> mapObjectDataList = new List<ObjectData>();
    public List<ButtonActivatedDoorStruct> mapButtonActivatedDoorDataList = new List<ButtonActivatedDoorStruct>();

    public List<ExitObjStruct> mapExitObjectDataList = new();

    public float cellSize;


    public Map(Vector2 mapSize, string id, Vector2 startPosition,
        List<ExitObjStruct> mapExitObjectDataList,
        List<TileData> tileList, 
        List<ObjectData> objectList,
        List<ButtonActivatedDoorStruct> mapButtonActivatedDoorDataList,
        float cellSize)
    {
        mapID = id;
        mapTileDataList = tileList;
        mapObjectDataList = objectList;
        this.startPosition = startPosition;
        this.mapExitObjectDataList = mapExitObjectDataList;
        this.mapSize = mapSize;
        this.mapButtonActivatedDoorDataList = mapButtonActivatedDoorDataList;
        this.cellSize = cellSize;
    }

    public Map()
    {

    }
}


[System.Serializable]
public struct ButtonActivatedDoorStruct
{
    public int id;
    public int linkId;
    public int activeRequirAmount;
    public Vector2 position;
    public List<Vector2> buttonActivatePositionList;//Vector2의 개수만큼 버튼 생성
    public Quaternion quaternion;
    public Vector3 scale;
    public ButtonActivatedDoorStruct(int id,int linkId,int activeRequirAmount, Vector2 position,
        List<Vector2> buttonActivatePositionList,
        Quaternion quaternion,
        Vector3 scale)
    {
        this.id= id;
        this.linkId= linkId;
        this.activeRequirAmount = activeRequirAmount;
        this.position = position;
        this.buttonActivatePositionList = buttonActivatePositionList;
        this .quaternion = quaternion;
        this.scale = scale;
    }
}

[System.Serializable]
public struct ExitObjStruct
{
    public int id;
    public Vector2 position;
    public string path;
    public int condition_KeyAmount;
    public Vector2 nextPosition;

    public ExitObjStruct(int id,Vector2 position,int condition_KeyAmount,Vector2 nextPosition)
    {
        this.id = id;
        this.position = position;
        this.condition_KeyAmount = condition_KeyAmount;
        path = "/Prefabs/Map/Object/ExitPoint";
        this.nextPosition = nextPosition;
    }

}

[System.Serializable]
public struct ObjectData
{
    public int id;
    public Vector2 position;
    public Quaternion quaternion;
    public Vector3 scale;
    public ObjectData(int id,Vector2 position,Vector3 scale)
    {
        this.id = id;
        this.position = position;
        quaternion = Quaternion.identity;
        this.scale = scale;
       
    }
    public ObjectData(int id, Vector2 position,Quaternion quaternion,Vector3 scale)
    {
        this.id = id;
        this.position = position;
        this.quaternion = quaternion;
        this.scale = scale;
    }

}
[System.Serializable]
public struct TileData
{
    public int id;
    public Vector3Int position;

    public TileData(Vector3Int position)
    {
        this.id = 1;
        this.position = position;
    }
}
