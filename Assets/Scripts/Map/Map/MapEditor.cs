using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.IO;
using System.Runtime.InteropServices.ComTypes;
using GoogleSheet.Core.Type;
using System.Net;
using Unity.VisualScripting;
using Newtonsoft.Json;
using System.Runtime.ConstrainedExecution;
using UnityEditor.UI;

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
    Eraser,
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

    //test
    public PlaceMentSystem placeMentSystem;
    public GameObject gridPlane;
    //test


    

    [Header("Map Info")]
    [HideInInspector] public MapEditorType mapEditorType;
    public MapEditorState mapEditorState;
    [HideInInspector] public float cellSize;
   
    string folderPath;
    //[SerializeField] GameObject contorollerUI;
    //[SerializeField] GameObject buildSelectUI;

    [Header("Init")]
    [HideInInspector] public Transform mapObjBoxTransform;
    [HideInInspector] public Transform gridPlateTransform;
    private Transform outLineTransform;
    [HideInInspector] public Transform floorTransform;
    [HideInInspector] public Transform objectTransform;
    [HideInInspector] public Transform exitDoorObjectTransform;
    [HideInInspector] public Transform interactionObjectTransform;
    [HideInInspector] public Transform dontSaveObjectTransform;


    [Header("Create")]
    public GameObject[,] tileObjectArray;

    //[SerializeField] GameObject spawnPoint;
    //public GameObject SpawnPoint { get { return spawnPoint; } set { if (spawnPoint != null) Destroy(spawnPoint); spawnPoint = value; playerSpawnPosition = value.transform.position; } }
    //[SerializeField] GameObject exitPoint;
    //public GameObject ExitPoint { get { return exitPoint; } set { if (exitPoint != null) Destroy(exitPoint); exitPoint = value; playerExitPosition = value.transform.position; } }

    //todo

    public Vector2 startPosition;

    //todo
    [Header("Current")]
    public GameObject curBuildObj;
    public GameObject CurBuildObj { get { return curBuildObj; } set { curBuildObj = value; placeMentSystem.MouseIndicator = value; } }
    public Transform curTransform;
    [Space(10)]
    [Header("----------------------------------------------------")]
    public Map curMap;
    [Header("----------------------------------------------------")]
    [Space(10)]
    [Header("OutLine")]
    [SerializeField] MapOutLineSO mapOutLineSO;

    [Header("Save Data")]

    [HideInInspector] public int width;
    [HideInInspector] public int height;
    public string mapID;
    //public Vector2 playerSpawnPosition;
    //public Vector2 playerExitPosition;
    [HideInInspector] public int condition_KeyAmount;
    [HideInInspector] public List<TileData> mapTileDataList = new List<TileData>();
    [HideInInspector] public List<ObjectData> mapObjectDataList = new List<ObjectData>();
 
    private void Awake()
    {
        if (Instance != null)
            Destroy(gameObject);
        else Instance = this;

        folderPath = Path.Combine(Application.dataPath, "Resources/MapDat");

    }

    private void Start()
    {
        //Init();
    }
    //todo
    public void Init()
    {
        mapObjBoxTransform = Util.CreateChildTransform(transform, "MapObjBox");
        floorTransform = Util.CreateChildTransform(mapObjBoxTransform, "FloorTransform");
        objectTransform = Util.CreateChildTransform(mapObjBoxTransform, "ObjectTransform");
        exitDoorObjectTransform = Util.CreateChildTransform(mapObjBoxTransform, "ExitDoorObjectTransform");
        interactionObjectTransform = Util.CreateChildTransform(mapObjBoxTransform, "InteractionObjectTransform");
        dontSaveObjectTransform = Util.CreateChildTransform(mapObjBoxTransform, "DontSaveObjectTransform");
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
                obj.GetComponent<BuildObj>().SetTileData((Vector2)pot * (int)cellSize);
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

    #region GetList

    List<TileData> GetTileList(Transform transform)
    {
        List<TileData> list = new();
        foreach (Vector3Int cur in placeMentSystem.tileDic.Keys)
        {
            TileData tileData = new TileData(cur);
            list.Add(tileData);

        }
        return list;
    }

    List<ObjectData> GetList(Transform transform)
    {
        List<ObjectData> list = new();
        foreach(Transform cur in transform)
        {
            cur.GetComponent<BuildObj>().SetTileData(cur.position, cur.rotation);
            list.Add(cur.GetComponent<BuildObj>().ObjectData);
        }
        return list;
    }
    List<ButtonActivatedDoorStruct> GetButtonActivateDoorStructList(Transform transform)
    {
        List<ButtonActivatedDoorStruct> list = new();
        foreach(Transform cur in transform)
        {
            Debug.Log("asdasd");
            ButtonActivatedDoor curDoor = cur.GetComponent<ButtonActivatedDoor>();
            curDoor.SetTileData(cur.position,cur.rotation);
         
            list.Add(curDoor.GetButtonActivatedDoorStruct());
        }
        return list;
    }

    List<ExitObjStruct> GetExitObjStructsList(Transform transform)
    {
        List<ExitObjStruct> list = new();

        foreach(Transform cur in transform)
        {
       
                list.Add(cur.GetComponent<ExitPointObj>().GetExitObjectStruct());
        }
        
        return list;
    }
    #endregion

    void CreateJsonFile()
    {
        mapTileDataList = GetTileList(floorTransform);
        mapObjectDataList = GetList(objectTransform);

        Map map = new Map(new Vector2(width, height), mapID,  startPosition,
            GetExitObjStructsList(exitDoorObjectTransform),
            mapTileDataList, 
            mapObjectDataList,
            GetButtonActivateDoorStructList(interactionObjectTransform),
            cellSize);
        string json = JsonUtility.ToJson(map, true);
        Debug.Log(json);
        string filePath = Path.Combine(folderPath, $"{map.mapID}.json");
        File.WriteAllText(filePath, json);

        UnityEditor.AssetDatabase.Refresh();
    }


    int FindKey()
    {
        int sum = 0;
        foreach(Transform transform in objectTransform)
        {
            if(transform.gameObject.layer == LayerMask.NameToLayer("Key"))
            {
                sum++;
            }
        }
        return sum;
    }


    #endregion

    #region Load

    public void LoadMap(string name)
    {
        if (!Managers.Data.mapData.mapDictionary.ContainsKey(name))
        {
            Debug.Log("Can't find Map");
            Init();
            mapEditorType = MapEditorType.New;
            return;
        }
        Init();
        mapEditorType = MapEditorType.Load;
        mapID = name;
        curMap = Managers.Data.mapData.mapDictionary[name];
        SetMapSize((int)curMap.mapSize.x, (int)curMap.mapSize.y);

        //start Point
        startPosition = curMap.startPosition;
        GameObject startPoint = Object.Instantiate(Resources.Load<GameObject>(Managers.Data.mapData.mapObjectDataDictionary[302].path));
        startPoint.transform.position = curMap.startPosition;
        startPoint.transform.SetParent(dontSaveObjectTransform);
        //start Point

        CreateObj(floorTransform);
        CreateObj(objectTransform);
        CreateObj(interactionObjectTransform);
        CreateObj(exitDoorObjectTransform);
    }


    #endregion
    //todo
    #region Util 

    public void SetMapSize(int width, int height)
    {
        this.width = width;
        this.height = height;
        //if(mapEditorState != MapEditorState.NoEditor)
        //{
        //    gridPlane.SetActive(true);
        //    gridPlane.GetComponent<GridPlane>().SetSize(width, height);
        //}
        //else
        //{
        //    gridPlane.SetActive(false);
        //}

        //GenerateMapOutLine();
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
    public void CreateObj(Transform transform)
    {
        switch (transform.name)
        {
            case "FloorTransform":
                foreach (TileData data in curMap.mapTileDataList)
                {
                    MapDataStruct mapDataStruct = Managers.Data.mapData.mapTileDataDictionary[data.id];
                    Debug.Log(data.position);
                    placeMentSystem.floorTileMap.SetTile(data.position, Resources.Load<TileBase>(mapDataStruct.path));
                    placeMentSystem.tileDic[data.position] = data.id;
                }
                break;
            case "ObjectTransform":
                foreach (ObjectData data in curMap.mapObjectDataList)
                {
                    MapDataStruct mapDataStruct = Managers.Data.mapData.mapObjectDataDictionary[data.id];
                    Create(transform, mapDataStruct, data);
                }
                break;
            case "InteractionObjectTransform":
                foreach (ButtonActivatedDoorStruct data in curMap.mapButtonActivatedDoorDataList)
                {
                    MapDataStruct mapDataStruct = Managers.Data.mapData.mapObjectDataDictionary[data.id];
                    Create(transform, mapDataStruct, data);
                }
                break;
            case "ExitDoorObjectTransform":
                foreach (ExitObjStruct data in curMap.mapExitObjectDataList)
                {
                    MapDataStruct mapDataStruct = Managers.Data.mapData.mapObjectDataDictionary[data.id];
                    Create(transform, mapDataStruct, data);
                }
                break;
        }

    }

    void Create(Transform transform, MapDataStruct mapDataStruct, ObjectData data)
    {
        GameObject obj = Object.Instantiate(Resources.Load<GameObject>(mapDataStruct.path));
        obj.GetComponent<BuildObj>().ObjectData = data;
        obj.transform.position = data.position;
        obj.transform.rotation = data.quaternion;
        obj.transform.SetParent(transform);
    }
    void Create(Transform transform, MapDataStruct mapDataStruct, ButtonActivatedDoorStruct data)
    {
        GameObject obj = Object.Instantiate(Resources.Load<GameObject>(mapDataStruct.path));
        ButtonActivatedDoor door = obj.GetComponent<ButtonActivatedDoor>();
        door.ButtonActivatedDoorStruct = data; 
        obj.transform.SetParent(transform);
        MapDataStruct btn = Managers.Data.mapData.mapObjectDataDictionary[306];
        foreach (Vector2 pot in data.buttonActivatePositionList)
        {
            GameObject btnActivated = Object.Instantiate(Resources.Load<GameObject>(btn.path));
            btnActivated.GetComponent<ButtonActivated>().SetLinkDoor(pot, door);
            btnActivated.transform.SetParent(dontSaveObjectTransform);

        }
    }

    void Create(Transform transform, MapDataStruct mapDataStruct, ExitObjStruct data)
    {
        GameObject obj = Object.Instantiate(Resources.Load<GameObject>(mapDataStruct.path));
        obj.transform.position = data.position;
        obj.transform.SetParent(transform);
        ExitPointObj door = obj.GetComponent<ExitPointObj>();
        door.SetData(data, dontSaveObjectTransform);

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
