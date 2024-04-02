using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileClearBox : MonoBehaviour
{
    PlaceMentSystem placeMentSystem;
    Vector3Int startPosition;
    Vector3Int endPosition;
    TileBase tilebase;

    int minX;
    int maxX;
    int minY;
    int maxY;

    List<Vector3Int> list = new List<Vector3Int>();

    public TileClearBox()
    {
        placeMentSystem = MapEditor.Instance.placeMentSystem;
        startPosition = placeMentSystem.startPosition;
        endPosition = placeMentSystem.endPosition;
        tilebase = placeMentSystem.tileBase;
        minAndMax();
    }

    public void ClearTile()
    {
        Debug.Log("Clear Tile");

        for (int i = minX; i <= maxX; i++)
        {
            for (int j = minY; j <= maxY; j++)
            {
                if (placeMentSystem.floorTileMap.GetTile(new Vector3Int(i, j)) != null)
                {
                    list.Add(new Vector3Int(i, j));
                    placeMentSystem.floorTileMap.SetTile(new Vector3Int(i, j), null);
                }

            }
        }
    }

    public void UndoTile()
    {
        Debug.Log("UndoTIle");
        for (int i = 0; i < list.Count; i++)
        {
            placeMentSystem.floorTileMap.SetTile(list[i], tilebase);
        }

    }

    void minAndMax()
    {
        minX = startPosition.x < endPosition.x ? startPosition.x : endPosition.x;
        maxX = startPosition.x > endPosition.x ? startPosition.x : endPosition.x;
        minY = startPosition.y < endPosition.y ? startPosition.y : endPosition.y;
        maxY = startPosition.y > endPosition.y ? startPosition.y : endPosition.y;
    }
}
