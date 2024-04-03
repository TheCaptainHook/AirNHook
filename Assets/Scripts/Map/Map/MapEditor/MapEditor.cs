using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;
using System.IO;
using System.Runtime.InteropServices.ComTypes;
using GoogleSheet.Core.Type;

public enum MapType
{
    Tutorial,
    Main,
    User
}

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

    //private Grid grid;

    [Header("EditorMode")]
    public PlaceMentSystem placeMentSystem;
    [HideInInspector] public GameObject gridPlane;
    [Header("UI")]
    public MapEditorControllerUI editorUIController;

    [Space(5)]

    [Header("Map Info")]
    public Transform poolingContainer;
    [HideInInspector] public MapEditorType mapEditorType;
    [HideInInspector] public float cellSize;   
    public MapEditorState mapEditorState;
    string folderPath;

    [Space(5)]
    [Header("Init")]
    [SerializeField] GameObject grid;
    [HideInInspector] public GameObject gridPalette;
    public Transform mapObjBoxTransform;
    //[HideInInspector] public Transform gridPlateTransform;
    [HideInInspector] public Transform floorTransform;
    [HideInInspector] public Transform objectTransform;
    [HideInInspector] public Transform exitDoorObjectTransform;
    [HideInInspector] public Transform interactionObjectTransform;
    [HideInInspector] public Transform dontSaveObjectTransform;

    

    //[Space(5)]
    //[Header("Create")]
    //public GameObject[,] tileObjectArray;

    [Space(10)]
    [Header("----------------------------------------------------")]
    public Map curMap;
    public Map CurMap {
        get { return curMap; }
        set { curMap = value; stageClear = false; } }
    [Header("----------------------------------------------------")]
    public bool stageClear;
    [Space(10)]

    [Header("Save Data")]
    public MapType mapType;
    [HideInInspector] public int width;
    [HideInInspector] public int height;
    public string mapID;
    public Vector2 startPosition;
    public GameObject startPositionObject;
    //[HideInInspector] public int condition_KeyAmount;
    [HideInInspector] public List<TileData> mapTileDataList = new List<TileData>();
    [HideInInspector] public List<ObjectData> mapObjectDataList = new List<ObjectData>();
 
    private void Awake()
    {
        if (Instance != null)
            Destroy(gameObject);
        else Instance = this;

        folderPath = Path.Combine(Application.dataPath, "Resources/MapDat");

    }

    //todo
    public void Init()
    {
        if(mapEditorState != MapEditorState.NoEditor) { editorUIController.gameObject.SetActive(true); }
        else { editorUIController.gameObject.SetActive(false); }
        if(gridPalette != null) { Destroy(gridPalette); }

        CreateGridPalet();
        mapObjBoxTransform = Util.CreateChildTransform(transform, "MapObjBox");
        floorTransform = Util.CreateChildTransform(mapObjBoxTransform, "FloorTransform");
        objectTransform = Util.CreateChildTransform(mapObjBoxTransform, "ObjectTransform");
        exitDoorObjectTransform = Util.CreateChildTransform(mapObjBoxTransform, "ExitDoorObjectTransform");
        interactionObjectTransform = Util.CreateChildTransform(mapObjBoxTransform, "InteractionObjectTransform");
        dontSaveObjectTransform = Util.CreateChildTransform(mapObjBoxTransform, "DontSaveObjectTransform");
    }

  
    void CreateGridPalet()
    {
        gridPalette = Instantiate(grid);
        placeMentSystem.floorTileMap = gridPalette.transform.Find("Floor").GetComponent<Tilemap>();

    }

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
        string filePath = Path.Combine(folderPath, $"Tutorial/{mapType}/{map.mapID}.json");
        //if (mapType == MapType.Tutorial)
        //{
        //    filePath = Path.Combine(folderPath, $"Tutorial/{map.mapID}.json");
        //}
        //else if (mapType == MapType.Main)
        //{
        //    filePath = Path.Combine(folderPath, $"Main/{map.mapID}.json");
        //}
        //else
        //{
        //    filePath = Path.Combine(folderPath, $"User/{map.mapID}.json");
        //}
        File.WriteAllText(filePath, json);

        //AssetDatabase.Refresh();
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
        if (!Managers.Data.mapData.GetDictionary(mapType).ContainsKey(name))
        {
            Debug.Log("Can't find Map");
            Init();
            mapEditorType = MapEditorType.New;
            return;
        }
        Init();
        placeMentSystem.ResetTileMap();
        mapEditorType = MapEditorType.Load;
        mapID = name;
        CurMap = Managers.Data.mapData.GetDictionary(mapType)[name];
        SetMapSize((int)curMap.mapSize.x, (int)curMap.mapSize.y);

        //start Point
        startPosition = curMap.startPosition;
        startPositionObject = Object.Instantiate(Resources.Load<GameObject>(Managers.Data.mapData.mapObjectDataDictionary[302].path));
        startPositionObject.transform.position = curMap.startPosition;
        startPositionObject.transform.SetParent(dontSaveObjectTransform);
        //start Point

        CreateObj(floorTransform);
        CreateObj(objectTransform);
        CreateObj(interactionObjectTransform);
        CreateObj(exitDoorObjectTransform);
    }


    #endregion

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

    public void CreateObj(Transform transform)
    {
        switch (transform.name)
        {
            case "FloorTransform":
                foreach (TileData data in curMap.mapTileDataList)
                {
                    MapDataStruct mapDataStruct = Managers.Data.mapData.mapTileDataDictionary[data.id];
                    placeMentSystem.floorTileMap.SetTile(data.position, Resources.Load<TileBase>(mapDataStruct.path));
                    placeMentSystem.tileDic[data.position] = data.id;
                }
                break;
            case "ObjectTransform":
                
                foreach (ObjectData data in curMap.mapObjectDataList)
                {
                    if (Managers.Data.mapData.mapSceneDataDictionary.ContainsKey(data.id))
                    {
                        MapDataStruct mapDataStruct = Managers.Data.mapData.mapSceneDataDictionary[data.id];
                        Create(transform, mapDataStruct, data);
                    }
                    else
                    {
                        if (data.id == 307) { continue; }
                        MapDataStruct mapDataStruct = Managers.Data.mapData.mapObjectDataDictionary[data.id];
                        Create(transform, mapDataStruct, data);
                    }
                   
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
        door.SetData(data);

    }

    public List<Transform> GetEditorTransform()
    {
        List<Transform> list = new();
        list.Add(floorTransform);
        list.Add(objectTransform);
        list.Add(exitDoorObjectTransform);
        list.Add(interactionObjectTransform);
        list.Add(dontSaveObjectTransform);
        return list;

    }
    #endregion


}
