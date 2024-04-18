using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.IO;
using System.Runtime.InteropServices.ComTypes;
using GoogleSheet.Core.Type;
using UnityEditor.UI;

public enum MapType
{
    Scene,
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
    Object,
    InteractionObject
}

[UGS(typeof(TileType))]
public enum TileType
{
    Floor,
    Object,
    InteractionObject

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
    public FadeInOutPanel fadeInOutPanel;
    [Space(5)]

    [Header("Map Info")]
    public Transform poolingContainer;
    [HideInInspector] public MapEditorType mapEditorType;
    [HideInInspector] public float cellSize;   
    public MapEditorState mapEditorState;
    string folderPath;

    [Space(5)]
    [Header("Init")]
    [SerializeField] GameObject floorTileMap;
    [SerializeField] GameObject previewTileMap;
    private GameObject gridPalette;
    public GameObject GridPalette { get { return gridPalette; } set { { if (gridPalette != null) { Destroy(gridPalette); } gridPalette = value; } } }
    private GameObject previewPalette;
    public GameObject PreviewPalette { get { return previewPalette; } set { { if (previewPalette != null) { Destroy(previewPalette); } previewPalette = value; } } }
    public Transform mapObjBoxTransform;
    //[HideInInspector] public Transform gridPlateTransform;
    [HideInInspector] public Transform floorTransform;
    [HideInInspector] public Transform objectTransform;
    [HideInInspector] public Transform exitDoorObjectTransform;
    [HideInInspector] public Transform interactionObjectTransform;
    [HideInInspector] public Transform dontSaveObjectTransform;
    [HideInInspector] public Transform networkingObjectTransform;
    [HideInInspector] public Transform garbageTransform;
    //todo 0412
    public Dictionary<int, HashSet<Vector2>> interactionBtnDictionary;
    //todo 0412

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
    [Tooltip("Only use MapType.Main")]
    public int stageLevel;
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

        folderPath = Path.Combine(Application.dataPath, "Resources/MapDat"); //todo

    }

    //todo
    public void Init()
    {
        if (mapEditorState != MapEditorState.NoEditor) { editorUIController.gameObject.SetActive(true); }
        else { editorUIController.gameObject.SetActive(false); }

        CreateGridPalet();
        CreatePreviewPalet();

        mapObjBoxTransform = Util.CreateChildTransform(transform, "MapObjBox");
        floorTransform = Util.CreateChildTransform(mapObjBoxTransform, "FloorTransform");
        objectTransform = Util.CreateChildTransform(mapObjBoxTransform, "ObjectTransform");
        exitDoorObjectTransform = Util.CreateChildTransform(mapObjBoxTransform, "ExitDoorObjectTransform");
        interactionObjectTransform = Util.CreateChildTransform(mapObjBoxTransform, "InteractionObjectTransform");
        dontSaveObjectTransform = Util.CreateChildTransform(mapObjBoxTransform, "DontSaveObjectTransform");
        garbageTransform = Util.CreateChildTransform(mapObjBoxTransform, "GarbageTransform");
        networkingObjectTransform = Util.CreateChildTransform(mapObjBoxTransform, "networkingObjectTransform");
    }

    public void EditorMode_Init()
    {
        Managers.Game.CurrentState = GameState.Editor;
        mapEditorState = MapEditorState.Editor;
        Init();
        gridPlane = Instantiate(Resources.Load<GameObject>("Prefabs/MapEditor/GridPlane"));
        gridPlane.SetActive(false);
        placeMentSystem.EditorMode_Init();
    }

    void CreateGridPalet()
    {

        GridPalette = Instantiate(floorTileMap);
        placeMentSystem.floorTileMap = GridPalette.transform.Find("Floor").GetComponent<Tilemap>();

    }
    void CreatePreviewPalet()
    {
        PreviewPalette = Instantiate(previewTileMap);
        placeMentSystem.preViewTileMap = PreviewPalette.transform.Find("PreviewTilemap").GetComponent<Tilemap>();
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

        foreach (Transform cur in transform)
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

        CreateObj(floorTransform, 0); //floorTransform
        CreateObj(objectTransform, 1); //objectTransform
        CreateObj(interactionObjectTransform, 2); //interactionObjectTransform
        CreateObj(exitDoorObjectTransform, 3); //exitDoorObjectTransform
    }

    public void LoadMap(string name, MapType mapType)
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

        CreateObj(floorTransform,0); //floorTransform
        CreateObj(objectTransform,1); //objectTransform
        CreateObj(interactionObjectTransform,2); //interactionObjectTransform
        CreateObj(exitDoorObjectTransform,3); //exitDoorObjectTransform
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

    public void CreateObj(Transform transform,int num)//스위치문 스트링값 대체하기.
    {
        switch (num)
        {
            case 0:
                foreach (TileData data in curMap.mapTileDataList)
                {
                    MapDataStruct mapDataStruct = Managers.Data.mapData.mapTileDataDictionary[data.id];
                    placeMentSystem.floorTileMap.SetTile(data.position, Resources.Load<TileBase>(mapDataStruct.path));
                    placeMentSystem.tileDic[data.position] = data.id;
                }
                break;
            case 1:
                
                foreach (ObjectData data in curMap.mapObjectDataList)
                {
                    if (Managers.Data.mapData.mapSceneDataDictionary.ContainsKey(data.id))
                    {
                        MapDataStruct mapDataStruct = Managers.Data.mapData.mapSceneDataDictionary[data.id];
                        if (data.id == 1001 || data.id == 1002)
                        {
                            Managers.Stage.CmdBatchObject(mapDataStruct.name, data);
                        }
                        else
                        {
                            Create(transform, mapDataStruct, data);
                        }
                        
                    }
                    else if (Managers.Data.mapData.mapOtherDataDictionary.ContainsKey(data.id))
                    {
                        MapDataStruct mapDataStruct = Managers.Data.mapData.mapOtherDataDictionary[data.id];

                        Create(transform, mapDataStruct, data);

                    }
                    else
                    {
                        MapDataStruct mapDataStruct = Managers.Data.mapData.mapObjectDataDictionary[data.id];
                        if (data.id == 307 || data.id == 300 || data.id == 311 || data.id == 313)
                        {
                            Managers.Stage.CmdBatchObject(mapDataStruct.name, data);
                        }
                        else
                        {       
                            Create(transform, mapDataStruct, data);
                        }

                        
                    }
                   
                }
                break;
            case 2:
                foreach (ButtonActivatedDoorStruct data in curMap.mapButtonActivatedDoorDataList)
                {
                    MapDataStruct mapDataStruct = Managers.Data.mapData.mapObjectDataDictionary[data.id];
                    Create(transform, mapDataStruct, data);
                }
                break;
            case 3:
                foreach (ExitObjStruct data in curMap.mapExitObjectDataList)
                {
                    MapDataStruct mapDataStruct = Managers.Data.mapData.mapObjectDataDictionary[data.id];
                    Managers.Stage.CmdBatchObject(mapDataStruct.name, data);
                    //Create(transform, mapDataStruct, data);
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
        obj.transform.localScale = data.scale;
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
        foreach (Vector2 pot in data.leverPositionList)
        {
            Managers.Stage.CmdBatchObject("LeverBody", pot);
        }
        //Lever


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
        list.Add(garbageTransform);
        return list;

    }


    public void MoveNextStage(string mapId,MapType mapType)
    {
        fadeInOutPanel.MoveNextStage(mapId, mapType);
    }
    #endregion


}
