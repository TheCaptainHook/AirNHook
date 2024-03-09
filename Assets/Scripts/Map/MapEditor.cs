using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UGS;

public class MapEditor : MonoBehaviour
{
    Util Util = new Util();
    
    public static MapEditor Instance;
    public Grid grid;


    [Header("Map Status")]
    public int width;
    public int height;
    public float cellSize;

    public GameObject curBuildObj;

    public TileData[,] tileDataArray;
    public GameObject[,] gameObjectArray;


    [Header("Save Data")]
    public string mapName;
    public Vector2 playerSpawnPosition;
    public Vector2 playerExitPosition;
    public List<TileData> mapTileDataList = new List<TileData>();

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

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Test();
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
                mapTileDataList.Remove(tileDataArray[pot.x, pot.y]);
            }
            
            GameObject obj = Instantiate(curBuildObj, (Vector2)pot * (int)cellSize, Quaternion.identity);
            gameObjectArray[pot.x, pot.y] = obj;
            obj.GetComponent<BuildObj>().position = (Vector2)pot * (int)cellSize;
            tileDataArray[pot.x, pot.y] = obj.GetComponent<BuildObj>().SetTileData();
            mapTileDataList.Add(tileDataArray[pot.x, pot.y]);
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
                tileDataArray[pot.x, pot.y] = new TileData();
                mapTileDataList.Remove(tileDataArray[pot.x, pot.y]);
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
        //Load And Create
        //MapEditor.Instance.mapTileDataList = Util.FromJsonData<TileData>(MapData.Data.DataList[0].TileData);

        //StartCoroutine(Delay());


        //Save Data
        SaveMapData<TileData>(mapTileDataList);
    }


    void SaveMapData<T>(List<T> list) //id,Spawn,Exit,tiledata
    {
        var newData = new MapData.Data();
        int id = MapData.Data.DataList.Count;
        newData.ID = mapName;
        string json = JsonUtility.ToJson(new SerializableList<T>(list));
        newData.TileData = json;

        //playerSpawnPosition
        //playerExitPosition
        //List<ObjectType>


        UnityGoogleSheet.Write<MapData.Data>(newData);

    }

   

    IEnumerator Delay()
    {
        yield return new WaitForSeconds(1f);
        foreach (TileData tileData in mapTileDataList)
        {
            GameObject obj = Resources.Load(tileData.path) as GameObject;
            obj.transform.position = tileData.position;
            //transform parent - MapManger 
        }
    }
}




[System.Serializable]


public struct TileData
{
    public int id;
    public TileType tileType;
    public Vector2 position;
    public string path;

    public TileData(TileType tileType,int id,Vector2 position,string path)
    {
        this.tileType = tileType;
        this.id = id;
        this.position = position;
        this.path = path;
    }
}

[System.Serializable]
public class SerializableList<T>
{
    public List<T> list;

    public SerializableList(List<T> list)
    {
        this.list = list;
    }
}