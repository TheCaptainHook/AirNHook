using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildObj : MonoBehaviour
{
    public TileType tileType;
    public int id;
    public Vector2 position;
    public string path;

    public TileData SetTileData()
    {
        return new TileData(tileType,id,position,path);
    }
}
