
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.IO;
using GoogleSheet.Core.Type;
using TMPro;
using System;
using System.Threading.Tasks;

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

    [HideInInspector] public Transform poolingContainer;

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

    public bool onStageSelect;

    public TextMeshProUGUI stageText;

    [Header("ScreenShot")]
    public GameObject screenShotCamera;

    #region event Action
    public event Action OnStageMove;
    #endregion

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

        if(mapObjBoxTransform != null) { Destroy(mapObjBoxTransform.gameObject); }

        mapObjBoxTransform = Util.CreateChildTransform("MapObjBox");

        floorTransform = Util.CreateChildTransform(mapObjBoxTransform, "FloorTransform");
        objectTransform = Util.CreateChildTransform(mapObjBoxTransform, "ObjectTransform");
        exitDoorObjectTransform = Util.CreateChildTransform(mapObjBoxTransform, "ExitDoorObjectTransform");
        interactionObjectTransform = Util.CreateChildTransform(mapObjBoxTransform, "InteractionObjectTransform");
        dontSaveObjectTransform = Util.CreateChildTransform(mapObjBoxTransform, "DontSaveObjectTransform");
        garbageTransform = Util.CreateChildTransform(mapObjBoxTransform, "GarbageTransform");
        networkingObjectTransform = Util.CreateChildTransform(mapObjBoxTransform, "networkingObjectTransform");

        poolingContainer = Util.CreateChildTransform(mapObjBoxTransform, "PoolingContainer");
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
    
    /// <summary>
    /// This function is only used when in game Editor.
    /// </summary>
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

    //List<TileData> GetTileList(Transform transform)
    //{
    //    List<TileData> list = new();
    //    foreach (Vector3Int cur in placeMentSystem.tileDic.Keys)
    //    {
    //        TileData tileData = new TileData(cur);
    //        list.Add(tileData);

    //    }
    //    return list;
    //}

    List<TileData> GetTileData(Tilemap tileMap)
    {
        List<TileData> list = new();
        BoundsInt bounds = tileMap.cellBounds;

        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3Int tilePos = new Vector3Int(x, y, 0);
                TileBase tile = tileMap.GetTile(tilePos);
                if (tile != null)
                {
                    TileData tileData = new TileData(tilePos);
                    list.Add(tileData);
                }
            }
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


    //List<ButtonActivatedDoorStruct> GetButtonActivateDoorStructList(Transform transform)
    //{
    //    List<ButtonActivatedDoorStruct> list = new();
    //    foreach(Transform cur in transform)
    //    {
    //        Debug.Log("asdasd");
    //        ButtonActivatedDoor curDoor = cur.GetComponent<ButtonActivatedDoor>();
    //        curDoor.SetTileData(cur.position,cur.rotation);

    //        list.Add(curDoor.GetButtonActivatedDoorStruct());
    //    }
    //    return list;
    //}
    List<ButtonActivatedDoorStruct> GetButtonActivateDoorStructList()
    {
        List<ButtonActivatedDoorStruct> list = new();

        foreach (Transform cur in dontSaveObjectTransform)
        {
            if (cur.GetComponent<ButtonActivated>())
            {
                cur.GetComponent<ButtonActivated>().LinkDoor();
            }
            else if (cur.GetComponent<LeverBody>())
            {
                cur.GetComponent<LeverBody>().LinkDoor();
            }

        }

        foreach (Transform cur in interactionObjectTransform)
        {
            ButtonActivatedDoor curDoor = cur.GetComponent<ButtonActivatedDoor>();
            curDoor.SetTileData(cur.position, cur.rotation);

            list.Add(curDoor.GetButtonActivatedDoorStruct());
        }
        return list;
    }

    //List<ExitObjStruct> GetExitObjStructsList(Transform transform)
    //{
    //    List<ExitObjStruct> list = new();

    //    foreach (Transform cur in transform)
    //    {

    //        list.Add(cur.GetComponent<ExitPointObj>().GetExitObjectStruct());
    //    }

    //    return list;
    //}
    List<ExitObjStruct> GetExitObjStructsList(Transform transform)
    {
        List<ExitObjStruct> list = new();
        int keyAmount = 0;
        foreach (Transform tr in objectTransform)
        {
            Debug.Log(tr.name);
            if (tr.GetComponent<BuildObj>().id == 307)
            {
                keyAmount++;
            }
        }

        foreach (Transform cur in transform)
        {
            cur.GetComponent<ExitPointObj>().condition_KeyAmount = keyAmount;
            list.Add(cur.GetComponent<ExitPointObj>().GetExitObjectStruct());
        }

        return list;
    }



    #endregion

    async void CreateJsonFile()
    {
        mapTileDataList = GetTileData(placeMentSystem.floorTileMap);
        mapObjectDataList = GetList(objectTransform);
        startPosition = FindObj(dontSaveObjectTransform, 302).transform.position;

        byte[] bytesImage = await CurrentMapScreenShot();

        Map map = new Map(new Vector2(width, height), mapID, stageLevel, startPosition,
            GetExitObjStructsList(exitDoorObjectTransform),
            mapTileDataList,
            mapObjectDataList,
            GetButtonActivateDoorStructList(),
            cellSize,1,bytesImage);

        string mapDatajson = JsonUtility.ToJson(map, true);
        string dateTimedate = JsonUtility.ToJson(new DateTimeData(System.DateTime.Now), true);
        

        //string filePath = Path.Combine(folderPath, $"User/{map.mapID}.json");
        string filePath = Path.Combine(Application.dataPath, $"UserMapData/{mapID}.json");

        string json = JsonUtility.ToJson(new UserMapData(mapDatajson,bytesImage , dateTimedate,GetHashValue(map.mapID)),true);

        Debug.Log(filePath);
        File.WriteAllText(filePath, json);

        Managers.Data.mapData.RefreshUserMapData();


    }


    #endregion

    #region Load

    public void LoadMap(Map map) // in game Editor, load user map data
    {
        mapEditorType = MapEditorType.Load;
        mapID = map.mapID;
        CurMap = map;
        SetMapSize((int)CurMap.mapSize.x, (int)CurMap.mapSize.y);

        //start Point
        startPosition = curMap.startPosition;
        startPositionObject = Instantiate(Resources.Load<GameObject>(Managers.Data.mapData.mapObjectDataDictionary[302].path));
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
        startPositionObject = Instantiate(Resources.Load<GameObject>(Managers.Data.mapData.mapObjectDataDictionary[302].path));
        startPositionObject.transform.position = curMap.startPosition;
        startPositionObject.transform.SetParent(dontSaveObjectTransform);
        //start Point


        if (Camera.main.GetComponent<ParallaxCamera>().onCameraTranslate != null) { Camera.main.GetComponent<ParallaxCamera>().onCameraTranslate = null; }
        Camera.main.GetComponent<ParallaxCamera>().oldPosition = startPosition.x;

        CreateObj(floorTransform, 0); //floorTransform
        CreateObj(objectTransform, 1); //objectTransform
        CreateObj(interactionObjectTransform, 2); //interactionObjectTransform
        CreateObj(exitDoorObjectTransform, 3); //exitDoorObjectTransform


        //
    }

    public void LoadMap(string name)
    {
        //if (!Managers.Data.mapData.mapMainAndSceneDictionary.ContainsKey(name))
        //{
        //    Debug.Log("Can't find Map");
        //    Init();
        //    mapEditorType = MapEditorType.New;
        //    return;
        //}


        Init();
        placeMentSystem.ResetTileMap();
        mapEditorType = MapEditorType.Load;
        mapID = name;
        CurMap = Managers.Data.mapData.mapAllDictionary[name];
        SetMapSize((int)curMap.mapSize.x, (int)curMap.mapSize.y);

        //start Point
        startPosition = curMap.startPosition;
        startPositionObject = Instantiate(Resources.Load<GameObject>(Managers.Data.mapData.mapObjectDataDictionary[302].path));
        startPositionObject.transform.position = curMap.startPosition;
        startPositionObject.transform.SetParent(dontSaveObjectTransform);
        //start Point


        if (Camera.main.GetComponent<ParallaxCamera>().onCameraTranslate != null) { Camera.main.GetComponent<ParallaxCamera>().onCameraTranslate = null; }
        Camera.main.GetComponent<ParallaxCamera>().oldPosition = startPosition.x;

        CreateObj(floorTransform, 0); //floorTransform
        CreateObj(objectTransform, 1); //objectTransform
        CreateObj(interactionObjectTransform, 2); //interactionObjectTransform
        CreateObj(exitDoorObjectTransform, 3); //exitDoorObjectTransform


        //
    }
    #endregion

    #region Util 

    public void SetMapSize(int width, int height)
    {
        this.width = width;
        this.height = height;
    }

    public void Reset()
    {
        Init();
        placeMentSystem.ResetTileMap();
    }

    #region Create
    public async void CreateObj(Transform transform, int num)
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
                        if (Managers.Game.CurrentState != GameState.Editor && (data.id == 1001 || data.id == 1002))
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
                        if (Managers.Game.CurrentState != GameState.Editor && (data.id == 307 || data.id == 300 || data.id == 311 || data.id == 313 || data.id == 315))
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
                    

                    if(Managers.Game.CurrentState != GameState.Editor)
                    {
                        await Managers.Stage.CmdBatchObject(mapDataStruct.name,data);
                        //ButtonActivatedDoor door = obj.GetComponent<ButtonActivatedDoor>();

                        MapDataStruct btn = Managers.Data.mapData.mapObjectDataDictionary[306];
                        Debug.Log("asdasdasdas");
                        foreach (Vector2 pot in data.buttonActivatePositionList)
                        {
                            Debug.Log(pot);
                            //GameObject btnActivated = Instantiate(Resources.Load<GameObject>(btn.path));
                            GameObject btnActivated = ResourceManager.Instantiate(Managers.Network.spawnPrefabDict[btn.name]);
                            btnActivated.GetComponent<ButtonActivated>().SetLinkDoor(pot, data.linkId);
                            btnActivated.transform.SetParent(dontSaveObjectTransform);
                        }
                        foreach (Vector2 pot in data.leverPositionList)
                        {
                            //GameObject leverBody = CmdBatchObject("LeverBody", MapEditor.Instance.dontSaveObjectTransform, pot);

                            GameObject leverBody = Managers.Stage.CmdBatchObject("LeverBody", dontSaveObjectTransform, pot);
                            if (leverBody is not null)
                                leverBody.GetComponent<LeverBodyNet>().CmdSetLinkDoor(pot, data.linkId);

                        }

                    }
                    else
                    {
                        Create(transform, mapDataStruct, data); //interaction 
                    }

                }
                break;
            case 3:
                foreach (ExitObjStruct data in curMap.mapExitObjectDataList)
                {
                    MapDataStruct mapDataStruct = Managers.Data.mapData.mapObjectDataDictionary[data.id];
                    if(Managers.Game.CurrentState != GameState.Editor)
                    {
                        Managers.Stage.CmdBatchObject(mapDataStruct.name, data);
                    }
                    else
                    {
                        Create(transform, mapDataStruct, data); 
                    }
                    
                }
                break;
        }

    }



    void Create(Transform transform, MapDataStruct mapDataStruct, ObjectData data)
    {
        GameObject obj = Instantiate(Resources.Load<GameObject>(mapDataStruct.path));
        obj.GetComponent<BuildObj>().ObjectData = data;
        obj.transform.position = data.position;
        obj.transform.rotation = data.quaternion;
        obj.transform.localScale = data.scale;
        obj.transform.SetParent(transform);

        if(mapEditorState != MapEditorState.NoEditor)
        {
            obj.GetComponent<BuildObj>().TurnOff();
            placeMentSystem.curPlaceObjList.Add(obj.GetComponent<BuildObj>());
        }
    }
    void Create(Transform transform, MapDataStruct mapDataStruct, ButtonActivatedDoorStruct data)
    {
        GameObject obj = Instantiate(Resources.Load<GameObject>(mapDataStruct.path));
        ButtonActivatedDoor door = obj.GetComponent<ButtonActivatedDoor>();
        door.ButtonActivatedDoorStruct = data;
        obj.transform.SetParent(transform);
        MapDataStruct btn = Managers.Data.mapData.mapObjectDataDictionary[306];
        MapDataStruct leverBodyData = Managers.Data.mapData.mapObjectDataDictionary[312];
       
        foreach (Vector2 pot in data.buttonActivatePositionList)
        {
            GameObject btnActivated = Instantiate(Resources.Load<GameObject>(btn.path));
            btnActivated.GetComponent<ButtonActivated>().SetLinkDoor(pot, door);
            btnActivated.transform.SetParent(dontSaveObjectTransform);
        }
        foreach (Vector2 pot in data.leverPositionList)
        {    
            GameObject leverBody;
            if (mapEditorState != MapEditorState.NoEditor)
            {
                leverBody = Instantiate(Resources.Load<GameObject>(leverBodyData.path));
                leverBody.transform.SetParent(dontSaveObjectTransform);
                leverBody.GetComponent<LeverBody>().SetLinkDoor(pot, door.id, interactionObjectTransform);
            }
            else
            {
                leverBody = Managers.Stage.CmdBatchObject("LeverBody", dontSaveObjectTransform, pot);
                if (leverBody is not null)
                    leverBody.GetComponent<LeverBodyNet>().CmdSetLinkDoor(pot, data.linkId);
            }


            if (mapEditorState != MapEditorState.NoEditor)
            {
                obj.GetComponent<BuildObj>().TurnOff();
                placeMentSystem.curPlaceObjList.Add(obj.GetComponent<BuildObj>());
            }
        }
        //Lever

        if (mapEditorState != MapEditorState.NoEditor)
        {
            obj.GetComponent<BuildObj>().TurnOff();
            placeMentSystem.curPlaceObjList.Add(obj.GetComponent<BuildObj>());
        }

    }

    void Create(Transform transform, MapDataStruct mapDataStruct, ExitObjStruct data)
    {
        GameObject obj = Instantiate(Resources.Load<GameObject>(mapDataStruct.path));
        obj.transform.position = data.position;
        obj.transform.SetParent(transform);
        ExitPointObj door = obj.GetComponent<ExitPointObj>();
        door.SetData(data);
        if (mapEditorState != MapEditorState.NoEditor)
        {
            obj.GetComponent<BuildObj>().TurnOff();
            placeMentSystem.curPlaceObjList.Add(obj.GetComponent<BuildObj>());
        }
    }
    #endregion

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

    public void MoveNextStage(string mapId)
    {
        OnStageMove?.Invoke();
        fadeInOutPanel.MoveNextStage(mapId);
    }

    public GameObject FindObj(Transform transform, int id)
    {
        foreach (Transform cur in transform)
        {
            if (cur.GetComponent<BuildObj>().id == id)
            {
                return cur.gameObject;
            }
        }
        return null;
    }


    private int GetHashValue(string input)
    {
        System.Random random = new System.Random();
        int randomNumber = random.Next();

        int hash = 0;
        for (int i = 0; i < input.Length; i++)
        {
            hash = (hash * 31) + input[i];
        }
        Debug.Log(hash);
        return hash + randomNumber;

    }

    private Task<byte[]> CurrentMapScreenShot()
    {
        if (screenShotCamera == null)
        {
            screenShotCamera = Instantiate(Resources.Load<GameObject>("Prefabs/MapEditor/ScreenShotCamera"));
        }

        GameObject camera = screenShotCamera;

        Vector2 startPot = FindObj(dontSaveObjectTransform, 302).transform.position;
        Vector2 endPot = FindObj(exitDoorObjectTransform, 301).transform.position;

        var distance = (startPot + endPot) / 2;

        camera.gameObject.transform.position = distance;
        camera.gameObject.transform.position += new Vector3(0, 2, -1);

        Task<byte[]> encodingTask = camera.GetComponent<ScreenShotCamera>().ScreenShot();

        return encodingTask;

    }


    #endregion


}


