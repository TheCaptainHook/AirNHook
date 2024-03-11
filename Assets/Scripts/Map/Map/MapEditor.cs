using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UGS;
using System.IO;
using static UGS.Editor.GoogleDriveExplorerGUI;
using System.Runtime.InteropServices.ComTypes;

public enum MapEditorType
{
    New,
    Load
}

public enum MapState
{
    None,
    Tile,
    Object,
    InteractionObject
}

public class MapEditor : MonoBehaviour
{
    Util Util = new Util();
    
    public static MapEditor Instance;
    public Grid grid;


    [Header("Map Info")]
    public MapEditorType mapEditorType;
    public MapState mapState;
    public float cellSize;
    public Transform mapObjBoxTransform;
    string folderPath;

    [Header("Create")]
    public GameObject curBuildObj;
    public GameObject[,] gameObjectArray;

    [SerializeField] GameObject spawnPoint;
    public GameObject SpawnPoint { get { return spawnPoint; } set { if (spawnPoint != null) Destroy(spawnPoint); spawnPoint = value; playerSpawnPosition = value.transform.position; } }
    [SerializeField] GameObject exitPoint;
    public GameObject ExitPoint { get { return exitPoint; } set { if (exitPoint != null) Destroy(exitPoint); spawnPoint = value; playerExitPosition = value.transform.position; } }


    [Header("Save Data")]
    public int width;
    public int height;
    public string mapID;
    public Vector2 playerSpawnPosition;
    public Vector2 playerExitPosition;
    public List<TileData> mapTileDataList = new List<TileData>();


    private void Awake()
    {
        if (Instance != null)
            Destroy(gameObject);
        else Instance = this;


        folderPath = Path.Combine(Application.dataPath, "Resources/MapDat");
    }

    private void Start()
    {
        Init();
        //StartCoroutine(Co_LoadMap("TestMap"));
        SetMapSize();

    }
    //todo
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
    //todo
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
            
            //CreateJsonFile();
            LoadMap("test1");
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            Debug.Log("LLL");
            SaveMapData();
        }

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


    public void Create()
    {
        if(mapState == MapState.Tile)
        {
            CreateTile();
        }
        else if(mapState == MapState.Object)
        {

        }
    }

    public void Remove()
    {
        if (mapState == MapState.Tile)
        {
            RemoveTile();
        }
        else if (mapState == MapState.Object)
        {

        }
    }

    public void CreateTile()
    {
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

    //todo
    #region Save 
    //GUS
    void SaveMapData<T>(List<T> list) 
    {
        var newData = new MapData.Data();
        int id = MapData.Data.DataList.Count;
        newData.ID = mapID;
        string json = JsonUtility.ToJson(new SerializableList<T>(list));
        newData.TileData = json;
        newData.PlayerSpawnPot = playerSpawnPosition;
        newData.PlayerExitPot = playerExitPosition;
        //List<ObjectType>
        newData.MapSize = new Vector2(width, height);

        UnityGoogleSheet.Write<MapData.Data>(newData);
        Debug.Log("Save Data");
    }
    //Json 파일로 저장

    public void SaveMapData()
    {
        if(mapEditorType == MapEditorType.New ){
            string path = Path.Combine(folderPath, $"{mapID}.json");
            bool fileExists = File.Exists(path);
            while (fileExists)
            {
                int num = 1;
                path = Path.Combine(folderPath, $"{mapID}{num}.json");
                if (!File.Exists(path))
                {
                    mapID = $"{mapID}{num}";
                    fileExists = false;
                }

                num++;
            }
            CreateJsonFile();
        }
        else
        {
            CreateJsonFile();
        }



        //Map map = new Map(mapID, mapTileDataList, playerSpawnPosition, playerExitPosition, new Vector2(width, height));
     
        
    }
    void CreateJsonFile()
    {
        Map map = new Map(mapID, mapTileDataList, playerSpawnPosition, playerExitPosition, new Vector2(width, height));
        string json = JsonUtility.ToJson(map);
        string filePath = Path.Combine(folderPath, $"{map.mapID}.json");
        File.WriteAllText(filePath, json);

        UnityEditor.AssetDatabase.Refresh();
    }


   

    #endregion

    #region Load

    //IEnumerator Co_LoadMap(string mapID)
    //{
    //    while (!DataManager.Instance.mapDataReceiveComplete && MapData.Data.DataList.Count==0)
    //    {
    //        yield return null;
    //    }
    //    LoadMap(mapID);

    //}

    void LoadMap(string name)
    {
        Init();

        mapID = name;
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
    //todo
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


//todo
//public class JsonFileCreator
//{
//    public void CreateJsonFile()
//    {
//        // JSON 데이터를 저장할 객체 또는 데이터를 생성합니다.
//        MyDataObject dataObject = new MyDataObject();
//        dataObject.name = "John";
//        dataObject.age = 25;

//        // 객체를 JSON 형식의 문자열로 변환합니다.
//        string json = JsonUtility.ToJson(dataObject);

//        // JSON 파일을 생성할 경로를 지정합니다.
//        string folderPath = Path.Combine(Application.dataPath, "Resources/MapData");
//        string filePath = Path.Combine(folderPath, "data.json");

//        // JSON 파일을 생성하고 데이터를 저장합니다.
//        File.WriteAllText(filePath, json);
//    }
//}

//// 예시를 위해 사용할 데이터 객체입니다.
//[System.Serializable]
//public class MyDataObject
//{
//    public string name;
//    public int age;
//}