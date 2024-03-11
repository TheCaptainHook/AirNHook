using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleSheet.Type;
using GoogleSheet.Core.Type;


[UGS(typeof(TileType))]
public enum TileType
{
    Floor,
    Wall,
    BackGround,
    Object,
    None

}

public class Grid
{
    private int width;
    private int height;
    private float cellSize;
    private Vector3 originPosition;

    public Grid(int width,int height,float cellSize,Vector3 originPosition)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.originPosition = originPosition;

        for (int i = 0; i < width; i++)
        {
            Debug.DrawLine(GetWorldPosition(i, 0) - new Vector2(cellSize, cellSize) * 0.5f, GetWorldPosition(i, height) - new Vector2(cellSize, cellSize) * 0.5f, Color.white, 100f);
        }

        for (int i = 0; i < height; i++)
        {
            Debug.DrawLine(GetWorldPosition(0, i) - new Vector2(cellSize, cellSize) * 0.5f, GetWorldPosition(width, i) - new Vector2(cellSize, cellSize) * 0.5f, Color.white, 100f);
        }

        Debug.DrawLine(GetWorldPosition(0, height) - new Vector2(cellSize, cellSize) * 0.5f, GetWorldPosition(width, height) - new Vector2(cellSize, cellSize) * 0.5f, Color.white, 100f);
        Debug.DrawLine(GetWorldPosition(width, 0) - new Vector2(cellSize, cellSize) * 0.5f, GetWorldPosition(width, height) - new Vector2(cellSize, cellSize) * 0.5f, Color.white, 100f);

    }

 
    //Get
    public Vector2Int GetXY(Vector3 wordPosition) //Get Index
    {
        //0,1
        int x = (int)Mathf.Round((wordPosition - originPosition).x) / (int)cellSize;
        int y = (int)Mathf.Round((wordPosition - originPosition).y) / (int)cellSize;


        return new Vector2Int(x,y);
    }

    public Vector2Int GetXY(Vector2Int coord) //Get Index
    {
        int x = Mathf.FloorToInt((coord - (Vector2)originPosition).x / cellSize);
        int y = Mathf.FloorToInt((coord - (Vector2)originPosition).y / cellSize);

        return new Vector2Int(x, y);
    }

    public Vector2 GetWorldPosition(int x,int y) 
    {
        return new Vector2(x,y) * cellSize + (Vector2)originPosition;
    }

    //public TileData GetValue(int x, int y)
    //{
    //    if (x >= 0 && y >= 0 && x < width && y < height)
    //    {
    //        return tileDataArray[x, y];
    //    }
    //}

    //public int GetValue(Vector3 worldPosition)
    //{
    //    Vector2Int pot = GetXY(worldPosition);
    //    return GetValue(pot.x, pot.y);
    //}


    //Set

    //public void SetValue(int x, int y, TileData tileData)
    //{
    //    if(x>=0 && y>=0 && x < width && y < height){
    //        if (gameObjectArray[x,y] != null)
    //        {
    //            Object.Destroy(gameObjectArray[x, y]);
    //        }
    //        Object obj = Resources.Load("Prefabs/Tile/Tile") as GameObject;
    //        gameObjectArray[x, y] = obj as GameObject;
    //        tileDataArray[x, y] = new TileData();
    //    }
    //}
    //public void SetValue(Vector3 worldPosition, TileData tileData)
    //{
    //    Vector2Int coord = GetXY(worldPosition);
    //    SetValue(coord.x, coord.y, tileData);
    //}

   
}
