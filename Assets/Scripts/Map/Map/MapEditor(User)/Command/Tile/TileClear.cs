using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileClear : MonoBehaviour
{
    PlaceMentSystem placeMentSystem;
    Vector3Int target;
    TileBase tilebase;
   
    public TileClear()
    {
        placeMentSystem = MapEditor.Instance.placeMentSystem;
        target = placeMentSystem.gridPosition;
        tilebase = placeMentSystem.tileBase;
    }
    public void ClearTile()
    {
        Debug.Log("ClearTIle");
        Debug.Log(target);
        placeMentSystem.floorTileMap.SetTile(target, null);

    }
    public void UndoTile()
    {
        Debug.Log("UndoTIle");
        placeMentSystem.floorTileMap.SetTile(target, tilebase);


    }
}
