using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class MapEditor : MonoBehaviour
{
    public static MapEditor Instance;

    public Grid grid;


    [Header("Map Status")]
    public int width;
    public int height;
    public float cellSize;

    public GameObject curBuildObj;

    public TileData[,] tileDataArray;
    public GameObject[,] gameObjectArray;

    private void Awake()
    {
        if (Instance != null)
            Destroy(gameObject);
        else Instance = this;
    }

    private void Start()
    {
        SetMapSize();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            CreateTile();
        }

        if (Input.GetMouseButtonDown(1))
        {
            RemoveTile();
        }
    }

    public void CreateTile()
    {
        Vector2Int pot = grid.GetXY(Util.GetMouseWorldPosition(Input.mousePosition, Camera.main));

        if(pot.x >= 0 && pot.x < width && pot.y >=0 && pot.y < height)
        {
            if (gameObjectArray[pot.x,pot.y] != null)
            {
                Debug.Log("Destory");
                Destroy(gameObjectArray[pot.x, pot.y]);
            }
            
            GameObject obj = Instantiate(curBuildObj, (Vector2)pot * (int)cellSize, Quaternion.identity);
            gameObjectArray[pot.x, pot.y] = obj;
            obj.GetComponent<BuildObj>().position = (Vector2)pot * (int)cellSize;
            tileDataArray[pot.x, pot.y] = obj.GetComponent<BuildObj>().SetTileData();
        }
    }
    public void RemoveTile()
    {
        Vector2Int pot = grid.GetXY(Util.GetMouseWorldPosition(Input.mousePosition, Camera.main));

        if (pot.x >= 0 && pot.x < width && pot.y >= 0 && pot.y < height)
        {
            if (gameObjectArray[pot.x, pot.y] != null)
            {
                Debug.Log("Destory");
                Destroy(gameObjectArray[pot.x, pot.y]);
                gameObjectArray[pot.x, pot.y] = null;
            }
        }
    }
        public void SetMapSize()
        {
            tileDataArray = new TileData[width,height];
            gameObjectArray = new GameObject[width, height];
        grid = new Grid(width, height, cellSize, new Vector3(0, 0, 0));
    }

    void Test()
    {
        
        foreach(TileData data in tileDataArray)
        {
            Debug.Log(data.position);
        }
    }
}











[System.Serializable]
public struct TileData
{
    public TileType tileType;
    public int id;
    public Vector2 position;

    public TileData(TileType tileType,int id,Vector2 position)
    {
        this.tileType = tileType;
        this.id = id;
        this.position = position;
    }
}