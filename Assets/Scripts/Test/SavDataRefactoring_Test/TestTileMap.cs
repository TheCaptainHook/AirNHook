using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;
using Unity.VisualScripting;

public class TestTileMap : MonoBehaviour
{
    [SerializeField] Tilemap tileMap;

    string path = "Prefabs/MapEditor/Tile/";

    public List<TileData> tileList;
    
    public List<CompressedTileData> compressedTileist;

    public List<CompressedTileData> secondCompressedTileList;

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


   #region Draw Test
   public void DrawTile()
   {
    foreach(var data in secondCompressedTileList)
    {
        TileBase tileBase = Resources.Load<TileBase>(Path.Combine(path,data.TileId.ToString()));

        var values = GetMaxMin(data);

        for(int i = values.minX ; i<= values.maxX;i++)
        {
            for(int j = values.minY;j<= values.maxY;j++)
            {
                tileMap.SetTile(new Vector3Int(i,j,0),tileBase);
            }
        }

        
    }
   }
   
   #endregion

    private (int maxX,int minX,int maxY,int minY) GetMaxMin(CompressedTileData data)
    {

        Vector2Int start = data.Start; //0 ,5
        Vector2Int end = data.End; // 5 , 7

        int maxX = Mathf.Max(start.x, end.x);
        int minX = Mathf.Min(start.x, end.x);
        int maxY = Mathf.Max(start.y, end.y);
        int minY = Mathf.Min(start.y, end.y);

        return (maxX,minX,maxY,minY);

    }

    #region Compressed
   

     public List<CompressedTileData> CompressTileData(List<TileData> tileDataList) //first compress
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
                if (currentCompressedData != null) compressedList.Add(currentCompressedData.Value);
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

    public List<CompressedTileData> CompressTileData_Second(List<CompressedTileData> list)
    {
       List<CompressedTileData> compressedList = new List<CompressedTileData>();
       CompressedTileData? curData = null;

       foreach (var tileData in list)
       {
            if(curData == null || 
                tileData.TileId != curData.Value.TileId ||
                !IsAdjacent_2(tileData,curData.Value)
            )
            {
                if(curData != null) compressedList.Add(curData.Value);
                curData = new CompressedTileData(tileData.TileId,tileData.Start,tileData.End);
            }else{
                var updateData = curData.Value;
                updateData.Extend(tileData.End);
                curData = updateData;
            }
            
       }
       if(curData != null)
       {
        compressedList.Add(curData.Value);
       }
       return compressedList;
    }

   

    #endregion

    #region Test Code
   
   

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
    private bool IsAdjacent_2(CompressedTileData cur,CompressedTileData pre)
    {
        return (cur.Start.y == pre.Start.y) &&
                (cur.End.y == pre.End.y) &&
                (Mathf.Abs(cur.Start.x - pre.End.x) ==1);
    }
    #endregion


    #region Editor
     
    public void Save()
    {
        //compressedTileist = GetCompressedTileData();

        //TestPasing test = new TestPasing(GetCompressedTileData());
        //string json = JsonUtility.ToJson(test);
        //string path = GetPath();
        //File.WriteAllText(path, json);
        //Debug.Log($"File saved to: {path}");
    }
     public void Preview()
    {
        tileList = GetTileData(tileMap);
        Sorting();

        compressedTileist = GetCompressedTileData();
        secondCompressedTileList = CompressTileData_Second(compressedTileist); //2
    }
    public void Clear()
    {
        tileList.Clear();
        compressedTileist.Clear();
        secondCompressedTileList.Clear();

    }
    public List<CompressedTileData> GetCompressedTileData()
    {
        return CompressTileData(tileList);
    }


    public void Sorting()
    {
        tileList = tileList
        .OrderBy(td => td.id)
        .ThenBy(td => td.position.x)
        .ThenBy(td => td.position.y)
        .ToList();
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
