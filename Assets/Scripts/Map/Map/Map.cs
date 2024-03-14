using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using System.IO;


[System.Serializable]
public class Map
{
    public string mapID;
    public List<TileData> mapTileDataList = new List<TileData>();
    public Vector2 playerSpawnPosition;
    public Vector2 playerExitPosition;
    public Vector2 mapSize;
    public float cellSize;


    public Map(string id, List<TileData> list, Vector2 playerSpawnPosition, Vector2 playerExitPosition, Vector2 mapSize, float cellSize)
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
