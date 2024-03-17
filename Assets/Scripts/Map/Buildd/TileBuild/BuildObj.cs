using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BuildObj : MonoBehaviour
{
    private TileType tileType;
    private Vector2 position;
    public string path;

    public TileData tileData;

    public void SetTileData(TileType tileType,Vector2 position)
    {
        this.tileType = tileType;
        this.position = position;

        tileData = new TileData(tileType, position, path);
    }
}
