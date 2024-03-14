using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using System.IO;
using static ButtonActivated;


[System.Serializable]
public class Map
{
    public Vector2 mapSize;
    public string mapID;
    public Vector2 playerSpawnPosition;
    public Vector2 playerExitPosition;
    public List<TileData> mapTileDataList = new List<TileData>();
    public float cellSize;

    //public List<T> buttonActivatedDoorList = new List<T>();
    public List<ButtonActivatedBtn> buttonActivatedList = new List<ButtonActivatedBtn>();




    public Map(Vector2 mapSize, string id, Vector2 playerSpawnPosition, 
        Vector2 playerExitPosition, List<TileData> list, float cellSize)
    {
        mapID = id;
        mapTileDataList = list;
        this.playerSpawnPosition = playerSpawnPosition;
        this.playerExitPosition = playerExitPosition;
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
