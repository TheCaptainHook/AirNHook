using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleSheet.Type;
using GoogleSheet.Core.Type;


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


   
}
