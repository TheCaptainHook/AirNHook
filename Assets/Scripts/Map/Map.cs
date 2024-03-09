using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map
{
    public string mapID;
    public List<TileData> mapTileDataList = new List<TileData>();
    public Vector2 playerSpawnPosition;
    public Vector2 playerExitPosition;

    public Vector2 mapSize;

    public Map(string id, List<TileData> list,Vector2 playerSpawnPosition,Vector2 playerExitPosition,Vector2 mapSize)
    {
        mapID = id;
        mapTileDataList = list;
        this.playerSpawnPosition = playerSpawnPosition;
        this.playerExitPosition = playerExitPosition;
        this.mapSize = mapSize;
    }

    public void CreateMap()
    {
        foreach (TileData data in mapTileDataList)
        {
            //Transform parent - MapManager
            GameObject obj = Object.Instantiate(Resources.Load<GameObject>(data.path));
            obj.transform.position = data.position;
            
        }
        Debug.Log($"playerSapwnPosition : {playerSpawnPosition} \n playerExitPosition : {playerExitPosition}");
    }






}
