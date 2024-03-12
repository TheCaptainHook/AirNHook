using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleSheet.Type;
using GoogleSheet.Core.Type;


public class Grid
{
    private float cellSize;
    private Vector3 originPosition;

    public Grid(float cellSize,Vector3 originPosition)
    {
        this.cellSize = cellSize;
        this.originPosition = originPosition;

    }

 
    //Get
    public Vector2Int GetXY(Vector3 wordPosition) //Get Index
    {
        //0,1
        int x = (int)Mathf.Round((wordPosition - originPosition).x) / (int)cellSize;
        int y = (int)Mathf.Round((wordPosition - originPosition).y) / (int)cellSize;


        return new Vector2Int(x,y);
    }

    //public Vector2Int GetXY(Vector2Int coord) //Get Index
    //{
    //    int x = Mathf.FloorToInt((coord - (Vector2)originPosition).x / cellSize);
    //    int y = Mathf.FloorToInt((coord - (Vector2)originPosition).y / cellSize);

    //    return new Vector2Int(x, y);
    //}

    public Vector2 GetWorldPosition(int x,int y) 
    {
        return new Vector2(x,y) * cellSize + (Vector2)originPosition;
    }


   
}
