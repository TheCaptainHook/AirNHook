using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map
{
    public string mapID;
    public List<TileData> mapTileDataList = new List<TileData>();
    Vector2 playerSpawnPosition;
    Vector2 playerExitPosition;

    public Map(string id, List<TileData> list,Vector2 playerSpawnPosition,Vector2 playerExitPosition)
    {
        mapID = id;
        mapTileDataList = list;
        this.playerSpawnPosition = playerSpawnPosition;
        this.playerExitPosition = playerExitPosition;
    }

    public void CreateMap()
    {
        foreach (TileData data in mapTileDataList)
        {
            //Transform parent - MapManager
            GameObject obj = Resources.Load(data.path) as GameObject;
            obj.transform.position = (Vector2)data.position;
        }
        Debug.Log($"playerSapwnPosition : {playerSpawnPosition} \n playerExitPosition : {playerExitPosition}");
    }






}
