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

    public float cellSize;


    public Map(Vector2 mapSize, string id, Vector2 playerSpawnPosition, 
        ExitObjStruct exitObjStruct, List<ObjectData> tileList,List<ObjectData> objectList, float cellSize)
    {
        mapID = id;
        mapTileDataList = tileList;
        mapObjectDataList = objectList;
        this.playerSpawnPosition = playerSpawnPosition;
        this.exitObjStruct = exitObjStruct;
        this.mapSize = mapSize;
        this.cellSize = cellSize;
    }





    public void CreateMap_Tile(Transform transform)
    {
        CreateObj(transform, mapTileDataList);
    }

    public void CreateMap_Object(Transform transform)
    {
        CreateObj(transform, mapObjectDataList);
    }

    //데이터 테이블을 사용해 생성하기.
    void CreateObj(Transform transform,List<ObjectData> list)
    {
        foreach (ObjectData data in list)
        {
            MapObjectDataStruct mapObjectDataStruct = Managers.Data.mapData.GetMapObjData(data.id);
            GameObject obj = Object.Instantiate(Resources.Load<GameObject>(mapObjectDataStruct.path));
            obj.GetComponent<BuildObj>().objectData = data;
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
public struct ObjectData
{
    public int id;
    public Vector2 position;
  
    public ObjectData(int id, Vector2 position)
    {
        this.id = id;
        this.position = position;
       
    }
}