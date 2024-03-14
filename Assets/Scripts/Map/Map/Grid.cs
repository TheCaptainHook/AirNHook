using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleSheet.Type;
using GoogleSheet.Core.Type;


public class Grid
{
    private float cellSize;
    private Vector3 originPosition;

    public Grid(float cellSize)
    {
        this.cellSize = cellSize;
    }

 
    //Get
    public Vector2Int GetXY(Vector3 wordPosition) //Get Index
    {
        //0,1
        int x = (int)Mathf.Round((wordPosition).x) / (int)cellSize;
        int y = (int)Mathf.Round((wordPosition).y) / (int)cellSize;


        return new Vector2Int(x,y);
    }


    public Vector2 GetWorldPosition(int x,int y) 
    {
        return new Vector2(x, y) * cellSize;
    }


   
}
