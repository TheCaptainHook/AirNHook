using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.InteropServices.ComTypes;
using GoogleSheet.Core.Type;
using System.Net;
using Unity.VisualScripting;
using Newtonsoft.Json;

public enum MapEditorType
{
    New,
    Load
}

public enum MapEditorState
{
    NoEditor,
    Editor,
    Tile,
    Object,
    InteractionObject
}

[UGS(typeof(TileType))]
public enum TileType
{
    OutLine,
    Floor,
    Object
}


public class MapEditor : MonoBehaviour
{
    public static MapEditor Instance;
    Util Util = new Util();

    private Grid grid;
  
    [Header("Map Info")]
    [SerializeField] MapEditorType mapEditorType;
    [SerializeField] MapEditorState mapEditorState;
    [SerializeField] float cellSize;
   
    string folderPath;
    [SerializeField] GameObject contorollerUI;
    [SerializeField] GameObject buildSelectUI;

    [Header("Init")]
    [SerializeField] Transform buildTransform;
    
    private Transform mapObjBoxTransform;
    private Transform gridPlateTransform;
    private Transform outLineTransform;
    private Transform floorTransform;
    private Transform objectTransform;
    private Transform interactionObjectTransform;


    [Header("Create")]
    public GameObject[,] tileObjectArray;

    [SerializeField] GameObject spawnPoint;
    public GameObject SpawnPoint { get { return spawnPoint; } set { if (spawnPoint != null) Destroy(spawnPoint); spawnPoint = value; playerSpawnPosition = value.transform.position; } }
    [SerializeField] GameObject exitPoint;
    public GameObject ExitPoint { get { return exitPoint; } set { if (exitPoint != null) Destroy(exitPoint); exitPoint = value; playerExitPosition = value.transform.position; } }
    
    [Header("Current")]
    public GameObject curBuildObj;
    public Transform curTransform;
   

    [Header("OutLine")]
    [SerializeField] MapOutLineSO mapOutLineSO;

    [Header("Save Data")]
    public int width;
    public int height;
    public string mapID;
    public Vector2 playerSpawnPosition;
    public Vector2 playerExitPosition;
    public int condition_KeyAmount;
    public List<TileData> mapTileDataList = new List<TileData>();
    public List<TileData> mapObjectDataList = new List<TileData>();


    private void Awake()
    {
        if (Instance != null)
            Destroy(gameObject);
        else Instance = this;

        folderPath = Path.Combine(Application.dataPath, "Resources/MapDat");

    }

    private void Start()
    {
        grid = new Grid(cellSize);

        Init();
    }
    //todo
    public void Init()
    {
        mapObjBoxTransform = Util.CreateChildTransform(transform, "MapObjBox");
        floorTransform = Util.CreateChildTransform(mapObjBoxTransform, "FloorTransform");
        objectTransform = Util.CreateChildTransform(mapObjBoxTransform, "ObjectTransform");
        interactionObjectTransform = Util.CreateChildTransform(mapObjBoxTransform, "interactionObjectTransform");
    }

    //todo
    private void Update()
    {
        if(mapEditorState != MapEditorState.NoEditor)
        {
            if (Input.GetMouseButtonDown(0))
            {
                CreateTile();
            }

            if (Input.GetMouseButtonDown(1))
            {
                RemoveTile();
            }

         

            if (Input.GetKeyDown(KeyCode.A))
            {
                Debug.Log("LLL");
                SaveMapData();
            }

        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            LoadMap("Test2");
        }

    }

    #region Interaciton


    public void Create()
    {
        if(mapEditorState == MapEditorState.Tile)
        {
            CreateTile();
        }
        //else if(MapEditorState == MapEditorState.Object)
        //{

        //}
    }

    public void Remove()
    {
        if (mapEditorState == MapEditorState.Tile)
        {
            RemoveTile();
        }
        else if (mapEditorState == MapEditorState.Object)
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
                if (tileObjectArray[pot.x, pot.y] != null)
                {
                    Destroy(tileObjectArray[pot.x, pot.y]);
                }

                GameObject obj = Instantiate(curBuildObj, (Vector2)pot * (int)cellSize, Quaternion.identity);
                obj.transform.SetParent(floorTransform);
                obj.GetComponent<BuildObj>().SetTileData(TileType.Floor ,(Vector2)pot * (int)cellSize);
                tileObjectArray[pot.x, pot.y] = obj;

            }
        }
       
    }
    public void RemoveTile()
    {
        Vector2Int pot = grid.GetXY(Util.GetMouseWorldPosition(Input.mousePosition, Camera.main));

        if (pot.x >= 0 && pot.x < width && pot.y >= 0 && pot.y < height)
        {
            if (tileObjectArray[pot.x, pot.y] != null)
            {
                GameObject obj = tileObjectArray[pot.x, pot.y];
                Destroy(obj);
                tileObjectArray[pot.x, pot.y] = null;
              
                
            }
        }
    }
    #endregion

    //todo
    #region Save 

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

    }

    List<TileData> GetList(Transform transform) {
        List<TileData> list = new List<TileData>();
        foreach (Transform cur in transform)
        {
            list.Add(cur.GetComponent<BuildObj>().tileData);
        }
        return list;
    }



   
    void CreateJsonFile()
    {
        mapTileDataList = GetList(floorTransform);
        mapObjectDataList = GetList(objectTransform);
        Map map = new Map(new Vector2(width, height), mapID,  playerSpawnPosition, new ExitObjStruct(playerExitPosition, condition_KeyAmount), mapTileDataList,mapObjectDataList, cellSize);
        string json = JsonUtility.ToJson(map, true);
        string filePath = Path.Combine(folderPath, $"{map.mapID}.json");
        File.WriteAllText(filePath, json);

        UnityEditor.AssetDatabase.Refresh();
    }




    #endregion

    #region Load

    void LoadMap(string name)
    {
        mapEditorType = MapEditorType.Load;
        mapID = name;
        Map map = Managers.Data.mapData.mapDictionary[name];

        SetMapSize((int)map.mapSize.x, (int)map.mapSize.y);

        playerSpawnPosition = map.playerSpawnPosition;
        playerExitPosition = map.exitObjStruct.position;
        condition_KeyAmount = map.exitObjStruct.condition_KeyAmount;

        map.CreateMap_Tile(floorTransform);
        map.CreateMap_Object(objectTransform);

        foreach (Transform obj in floorTransform)
        {
            TileData tileData = obj.GetComponent<BuildObj>().tileData;

            int x = (int)tileData.position.x / (int)cellSize;
            int y = (int)tileData.position.y / (int)cellSize;
            tileObjectArray[x, y] = obj.gameObject;

        }

    }

    //void LoadMap(string name) //일반 게임에서 맵 로드할때
    //{
    //    if(mapEditorState == MapEditorState.NoEditor)
    //    {
    //        Init();
    //        mapEditorType = MapEditorType.Load;
    //        Map map = Managers.Data.mapData.mapDictionary[name];
    //        mapID = name;

    //        SetMapSize((int)map.mapSize.x, (int)map.mapSize.y);

    //        map.CreateMap_Tile(floorTransform);
    //    }
      
    //}


    #endregion
    //todo
    #region Util 

    public void SetMapSize(int width, int height)
    {
        this.width = width;
        this.height = height;
        tileObjectArray = new GameObject[width, height];
        //게임오브젝트배열 초기화 ,            

        GenerateMapOutLine();
    }

    public void Reset()
    {
        Init();
        mapTileDataList.Clear();
    }

    void GenerateMapOutLine()
    {
        outLineTransform = Util.CreateChildTransform(mapObjBoxTransform, "OutLineTransform");
        int width = this.width - 1;
        int height = this.height - 1;
        //범위 밖 오브젝트, 타일 삭제
        DestroyAll(objectTransform, width, height);
        DestroyAll(floorTransform, width, height);
        GenerateGridPlate();

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

    void GenerateGridPlate()
    {
        if(mapEditorState != MapEditorState.NoEditor)
        {
            gridPlateTransform = Util.CreateChildTransform(mapObjBoxTransform, "gridPlateTransform");
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    GameObject grid = Instantiate(Resources.Load("Prefabs/Map/Gird"), new Vector2(i, j) * cellSize, Quaternion.identity, gridPlateTransform) as GameObject;
                    grid.transform.localScale *= 0.95f;
                }
            }
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
            if(obj.position.x > width*cellSize ||  obj.position.y > height*cellSize)
            {
                Destroy(obj.gameObject);
            }
        }
    }


    #endregion


}






//id,position,path 구조체 따로 만들어서 통합하기
