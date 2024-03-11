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

    public GameObject[,] gameObjectArray;

    Transform mapObjBoxTransform;

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
        Init();
        //StartCoroutine(Co_LoadMap("TestMap"));
        SetMapSize();

    }

    public void Init()
    {
        mapObjBoxTransform = transform.Find("MapObjBox");

        if (mapObjBoxTransform != null)
        {
            Destroy(mapObjBoxTransform.gameObject);
        }

        mapObjBoxTransform = new GameObject("MapObjBox").transform;
        mapObjBoxTransform.SetParent(transform);
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

        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    SaveMapData<TileData>(mapTileDataList);
        //}

        //if (Input.GetKeyDown(KeyCode.A))
        //{
        //    Debug.Log("LLL");
        //    StartCoroutine(Co_LoadMap("TestMap"));
        //}
        //if (Input.GetKeyDown(KeyCode.C))
        //{
        //    Reset();

        //    foreach (var Value in MapData.Data.DataList)
        //    {
        //        Debug.Log(Value.ID);
        //    }
        //}

    }

    #region Interaciton

    public void CreateTile()
    {
        Debug.Log(Util.GetMouseWorldPosition(Input.mousePosition, Camera.main));
        Vector2Int pot = grid.GetXY(Util.GetMouseWorldPosition(Input.mousePosition, Camera.main));
        
        if(pot.x >= 0 && pot.x < width && pot.y >=0 && pot.y < height)
        {
            if (gameObjectArray[pot.x,pot.y] != null)
            {
                mapTileDataList.Remove(gameObjectArray[pot.x, pot.y].GetComponent<BuildObj>().tileData);
                Destroy(gameObjectArray[pot.x, pot.y]);
            }
            
            GameObject obj = Instantiate(curBuildObj, (Vector2)pot * (int)cellSize, Quaternion.identity);
            obj.transform.SetParent(mapObjBoxTransform);
            obj.GetComponent<BuildObj>().position = (Vector2)pot * (int)cellSize;
            mapTileDataList.Add(obj.GetComponent<BuildObj>().SetTileData());
            gameObjectArray[pot.x, pot.y] = obj;
            
        }
    }
    public void RemoveTile()
    {
        Vector2Int pot = grid.GetXY(Util.GetMouseWorldPosition(Input.mousePosition, Camera.main));

        if (pot.x >= 0 && pot.x < width && pot.y >= 0 && pot.y < height)
        {
            if (gameObjectArray[pot.x, pot.y] != null)
            {
                GameObject obj = gameObjectArray[pot.x, pot.y];
                mapTileDataList.Remove(obj.GetComponent<BuildObj>().tileData);
                Destroy(obj);
                gameObjectArray[pot.x, pot.y] = null;
              
                
            }
        }
    }
    #endregion


    #region Save 

    //데이터 시트 이름이 같으면 덮어 씌워짐.
    void SaveMapData<T>(List<T> list) //id,Spawn,Exit,tiledata
    {
        var newData = new MapData.Data();
        int id = MapData.Data.DataList.Count;
        newData.ID = mapName;
        string json = JsonUtility.ToJson(new SerializableList<T>(list));
        newData.TileData = json;
        newData.PlayerSpawnPot = playerSpawnPosition;
        newData.PlayerExitPot = playerExitPosition;
        //List<ObjectType>
        newData.MapSize = new Vector2(width, height);

        UnityGoogleSheet.Write<MapData.Data>(newData);
        Debug.Log("Save Data");
    }
    #endregion

    #region Load

    IEnumerator Co_LoadMap(string mapName)
    {
        while (!DataManager.Instance.mapDataReceiveComplete && MapData.Data.DataList.Count==0)
        {
            yield return null;
        }
        LoadMap(mapName);

    }

    void LoadMap(string name)
    {
        Init();

        mapName = name;
        Map map = MapManager.Instance.mapDictionary[name];

        width = (int)map.mapSize.x;
        height = (int)map.mapSize.y;

        playerSpawnPosition = map.playerSpawnPosition;
        playerExitPosition = map.playerExitPosition;

        mapTileDataList = map.mapTileDataList;


        SetMapSize();
        map.CreateMap(mapObjBoxTransform);

        foreach(Transform obj in mapObjBoxTransform)
        {
            TileData tileData = obj.GetComponent<BuildObj>().SetTileData();

            int x = (int)tileData.position.x / (int)cellSize;
            int y = (int)tileData.position.y / (int)cellSize;
            gameObjectArray[x, y] = obj.gameObject;

        }

    }

    #endregion

    #region Util 

    public void SetMapSize()
    {
        gameObjectArray = new GameObject[width, height];
        grid = new Grid(width, height, cellSize, new Vector3(0, 0, 0));
    }

    public void Reset()
    {
        Init();
        mapTileDataList.Clear();
    }

    #endregion


}




[System.Serializable]


public struct TileData
{
    public TileType tileType;
    public Vector2 position;
    public string path;

    public TileData(TileType tileType,Vector2 position,string path)
    {
        this.tileType = tileType;
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