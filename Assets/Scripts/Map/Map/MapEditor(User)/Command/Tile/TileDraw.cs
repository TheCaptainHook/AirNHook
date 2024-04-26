using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileDraw
{
    PlaceMentSystem placeMentSystem;
    Vector3Int target;

    public TileDraw()
    {
        placeMentSystem = MapEditor.Instance.placeMentSystem;
        target = placeMentSystem.gridPosition;
    }
    public void DrawTile()
    {
        Debug.Log($"DrawTIle,{target}");
        TileBase tilebase = placeMentSystem.tileBase;
        placeMentSystem.floorTileMap.SetTile(target, tilebase);
    }
    public void UndoTile()
    {
        Debug.Log("UndoTIle");
        placeMentSystem.floorTileMap.SetTile(target, null);
    }
  
}
