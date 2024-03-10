using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildObj : MonoBehaviour
{
    public TileType tileType;
    public Vector2 position;
    public string path;


    public TileData tileData;

    public TileData SetTileData()
    {
        tileData = new TileData(tileType, position, path);
        return tileData;
    }
}
