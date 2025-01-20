using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TestTileMap : MonoBehaviour
{
    [SerializeField] Tilemap tileMap;


    List<TileData> GetTileData(Tilemap tileMap)
    {
        List<TileData> list = new();
        BoundsInt bounds = tileMap.cellBounds;

        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector2Int tilePos = new Vector2Int(x, y);
                TileBase tile = tileMap.GetTile((Vector3Int)tilePos);
                if (tile != null)
                {
                    TileData tileData = new TileData((Vector3Int)tilePos, int.Parse(tile.name));
                    list.Add(tileData);
                }
            }

        }

        return list;
    }


    public List<CompressedTileData> CompressTileData(List<TileData> tileDataList)
    {
        List<CompressedTileData> compressedList = new List<CompressedTileData>();
        CompressedTileData? currentCompressedData = null;

        foreach (var tileData in tileDataList)
        {
            if (currentCompressedData == null ||
                tileData.id != currentCompressedData.Value.TileId ||
                !IsAdjacent((Vector2Int)tileData.position, currentCompressedData.Value.End))
            {
                // 새로운 범위 시작
                if (currentCompressedData != null)
                {
                    compressedList.Add(currentCompressedData.Value);
                }

                currentCompressedData = new CompressedTileData(tileData.id, (Vector2Int)tileData.position, (Vector2Int)tileData.position);
            }
            else
            {
                // 기존 범위 확장
                var updatedData = currentCompressedData.Value;
                updatedData.Extend((Vector2Int)tileData.position);
                currentCompressedData = updatedData;
            }
        }

        // 마지막 범위 추가
        if (currentCompressedData != null)
        {
            compressedList.Add(currentCompressedData.Value);
        }

        return compressedList;
    }



    #region Test Code
    public void Save()
    {
        TestPasing test = new TestPasing(CompressTileData(GetTileData(tileMap)));
        string json = JsonUtility.ToJson(test);
        string path = GetPath();
        File.WriteAllText(path, json);
        Debug.Log($"File saved to: {path}");
    }

    private string GetPath()
    {
        string downloadFolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

        if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor)
        {
            downloadFolder = Path.Combine(downloadFolder, "Downloads");
        }
        else if (Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.OSXEditor)
        {
            downloadFolder = Path.Combine(downloadFolder, "Downloads");
        }
        else if (Application.platform == RuntimePlatform.LinuxPlayer)
        {
            downloadFolder = Path.Combine(downloadFolder, "Downloads");
        }

        return downloadFolder;
    }
    #endregion



    #region Util
    private bool IsAdjacent(Vector2Int current, Vector2Int previous)
    {
        return (current.x == previous.x && Mathf.Abs(current.y - previous.y) == 1) ||
               (current.y == previous.y && Mathf.Abs(current.x - previous.x) == 1);
    }
    #endregion
}

public class TestPasing
{
    public List<CompressedTileData> testTileData;

    public TestPasing(List<CompressedTileData> testTileData)
    {
        this.testTileData = testTileData;
    }
}
