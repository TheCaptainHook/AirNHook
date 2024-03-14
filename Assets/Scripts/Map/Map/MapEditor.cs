using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UGS;
using System.IO;
//using static UGS.Editor.GoogleDriveExplorerGUI;
using System.Runtime.InteropServices.ComTypes;
using GoogleSheet.Core.Type;
using System.Net;

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

[UGS(typeof(TileType))]
public enum TileType
{
    OutLine,
    Floor,
}


public class MapEditor : MonoBehaviour
{
    Util Util = new Util();
    
    public static MapEditor Instance;
    public Grid grid;
    public LineRenderer lineRenderer;

    [Header("Map Info")]
    public MapEditorType mapEditorType;
    public MapState mapState;
    public float cellSize;
    public Transform mapObjBoxTransform;
    string folderPath;
    


    [Header("Init")]
    [SerializeField] Transform buildTransform;

    public Transform outLineTransform;
    public Transform floorTransform;
    public Transform objectTransform;


    [Header("Create")]
    public GameObject[,] gameObjectArray;

    [SerializeField] GameObject spawnPoint;
    public GameObject SpawnPoint { get { return spawnPoint; } set { if (spawnPoint != null) Destroy(spawnPoint); spawnPoint = value; playerSpawnPosition = value.transform.position; } }
    [SerializeField] GameObject exitPoint;
    public GameObject ExitPoint { get { return exitPoint; } set { if (exitPoint != null) Destroy(exitPoint); spawnPoint = value; playerExitPosition = value.transform.position; } }
    [Header("Current")]
    public GameObject curBuildObj;
    Dictionary<Transform, List<GameObject>> generatedDictionary = new Dictionary<Transform, List<GameObject>>();



    [Header("OutLine")]
    public MapOutLineSO mapOutLineSO;

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

        lineRenderer = GetComponent<LineRenderer>();
        folderPath = Path.Combine(Application.dataPath, "Resources/MapDat");

    }


    //todo
    public void Init()
    {
        mapObjBoxTransform = transform.Find("MapObjBox");

        if (mapObjBoxTransform != null)
        {
            Destroy(mapObjBoxTransform.gameObject);
        }

        mapObjBoxTransform = CreateChildTransform(transform, "MapObjBox");

        outLineTransform = CreateChildTransform(mapObjBoxTransform, "OutLineTransform");
        floorTransform = CreateChildTransform(mapObjBoxTransform, "FloorTransform");
        objectTransform = CreateChildTransform(mapObjBoxTransform, "ObjectTransform");

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
        if(grid != null)
        {
            Vector2Int pot = grid.GetXY(Util.GetMouseWorldPosition(Input.mousePosition, Camera.main));

            if (pot.x >= 0 && pot.x < width && pot.y >= 0 && pot.y < height)
            {
                if (gameObjectArray[pot.x, pot.y] != null)
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

        //UnityEditor.AssetDatabase.Refresh();
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
        mapID = name;
        Map map = MapManager.Instance.mapDictionary[name];

        SetMapSize((int)map.mapSize.x, (int)map.mapSize.y);

        playerSpawnPosition = map.playerSpawnPosition;
        playerExitPosition = map.playerExitPosition;

        mapTileDataList = map.mapTileDataList;

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

    public void SetMapSize(int width, int height)
    {
        ClearLine();
        //gameObjectArray = new GameObject[width, height];

        grid = new Grid(cellSize, new Vector3(0, 0, 0));
        this.width = width;
        this.height = height;
         DrawLine(width,height);
        Init();
        GenerateMapOutLine();

    }

    public void Reset()
    {
        Init();
        mapTileDataList.Clear();
    }

    void GenerateMapOutLine()
    {
        int width = this.width - 1;
        int height = this.height - 1;

        GameObject curveBL = Instantiate(mapOutLineSO.curveBL, new Vector2(0, 0), Quaternion.identity, outLineTransform);
        GameObject curveBR = Instantiate(mapOutLineSO.curveBR, new Vector2(width, 0) * cellSize, Quaternion.identity, outLineTransform);
        GameObject curveTL = Instantiate(mapOutLineSO.curveTL, new Vector2(0, height) * cellSize, Quaternion.identity, outLineTransform);
        GameObject curveTR = Instantiate(mapOutLineSO.curveTR, new Vector2(width, height) * cellSize, Quaternion.identity, outLineTransform);

        for (int i = 1; i < width; i++)
        {
            GameObject bottom = Instantiate(mapOutLineSO.bottom, new Vector2(i, 0)*cellSize, Quaternion.identity, outLineTransform);
            GameObject top = Instantiate(mapOutLineSO.top, new Vector2(i, height) * cellSize, Quaternion.identity, outLineTransform);
        }
        for (int i = 1; i < height; i++)
        {
            GameObject left = Instantiate(mapOutLineSO.left, new Vector2(0, i) * cellSize, Quaternion.identity, outLineTransform);
            GameObject right = Instantiate(mapOutLineSO.right, new Vector2(width, i) * cellSize, Quaternion.identity, outLineTransform);
        }
    }

    void DestroyAll(Transform transform)
    {
       foreach(Transform obj in transform)
        {
            Destroy(obj.gameObject);
        }
    }

    void DestroyAll(Transform transform,int width,int height)
    {
        foreach(Transform obj in transform)
        {
            if(obj.position.x >= width ||  obj.position.y >= height)
            {
                Destroy(obj.gameObject);
            }
        }
    }

    Transform CreateChildTransform(Transform parent, string name)
    {
        GameObject childObject = new GameObject(name);
        Transform childTransform = childObject.transform;
        childTransform.SetParent(parent);
        return childTransform;
    }

    void DrawLine(int width,int height)
    {
        int num = 0;
        for (int i = 0; i < width; i++)
        {
            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(num, new Vector2(i,0)*cellSize);
            lineRenderer.SetPosition(num+1, new Vector2(i,height)*cellSize);
            num += 2;
        }
    }
    void ClearLine()
    {
        lineRenderer.positionCount = 0;
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