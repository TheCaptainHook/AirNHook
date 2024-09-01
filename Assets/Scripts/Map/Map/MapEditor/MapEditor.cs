
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.IO;
using GoogleSheet.Core.Type;
using TMPro;
using System;
using System.Threading.Tasks;
using UnityEditor;

public enum MapType
{
    Scene,
    Main,
    User,
    Fork
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
    Background
}

[UGS(typeof(TileType))]
public enum TileType
{
    Floor,
    Background,
    Object,
    InteractionObject

}

//TODO FIXED CODE LINE 0829 : 
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
    // [HideInInspector] public Transform floorTransform;
    [HideInInspector] public Transform objectTransform;
    [HideInInspector] public Transform exitDoorObjectTransform;
    [HideInInspector] public Transform buttonActivatableObjectTransform; //TODO 0829
    [HideInInspector] public Transform buttonObjectTransform; //TODO 0829
    [HideInInspector] public Transform dontSaveObjectTransform;
    [HideInInspector] public Transform networkingObjectTransform;
    [HideInInspector] public Transform garbageTransform;

    //TOdo 0723
    [HideInInspector] public Transform triggerDialogueTransform;
    //TOdo 0723

    [HideInInspector] public Transform poolingContainer;

   
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
    public AudioType audioType = AudioType.None;
    //[HideInInspector] public int condition_KeyAmount;
    [HideInInspector] public List<TileData> mapTileDataList = new List<TileData>();
    [HideInInspector] public List<ObjectData> mapObjectDataList = new List<ObjectData>();

    public bool onStageSelect;

    public TextMeshProUGUI stageText;

   

    [Space(10)]
    [Header("----------------------------------------------------")]
    private Map curMap;
    public Map CurMap
    {
        get { return curMap; }
        set { curMap = value; stageClear = false; }
    }
    [Header("----------------------------------------------------")]
    [Header("ScreenShot")]
    public GameObject screenShotCamera;
    #region event Action
    public event Action OnStageMove;
    public event Action OnScreen;
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

        // floorTransform = Util.CreateChildTransform(mapObjBoxTransform, "FloorTransform");
        objectTransform = Util.CreateChildTransform(mapObjBoxTransform, "ObjectTransform");
        exitDoorObjectTransform = Util.CreateChildTransform(mapObjBoxTransform, "ExitDoorObjectTransform");
        buttonActivatableObjectTransform = Util.CreateChildTransform(mapObjBoxTransform, "buttonActivatableObjectTransform");
        buttonObjectTransform = Util.CreateChildTransform(mapObjBoxTransform, "buttonObjectTransform");
        dontSaveObjectTransform = Util.CreateChildTransform(mapObjBoxTransform, "DontSaveObjectTransform");
        garbageTransform = Util.CreateChildTransform(mapObjBoxTransform, "GarbageTransform");
        networkingObjectTransform = Util.CreateChildTransform(mapObjBoxTransform, "networkingObjectTransform");
        //TODO 0723
        triggerDialogueTransform = Util.CreateChildTransform(mapObjBoxTransform, "triggerDialogueTransform");
        //TODO 0723
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

        placeMentSystem.halfTileMap = GridPalette.transform.Find("HalfTiles").GetComponent<Tilemap>();
        placeMentSystem.backgroundTileMap = GridPalette.transform.Find("BackgroundTiles").GetComponent<Tilemap>();
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
                    TileData tileData = new TileData(tilePos,int.Parse(tile.name));
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
            cur.GetComponent<BuildObj>().SetTileData();

            list.Add(cur.GetComponent<BuildObj>().ObjectData);
        }
        return list;
    }



    List<ButtonActivatableObjectStruct> GetButtonActivateObjectStructList()
    {
        List<ButtonActivatableObjectStruct> list = new();

        foreach (Transform cur in buttonActivatableObjectTransform)
        {
           list.Add(cur.GetComponent<BuildObj>().GetData<ButtonActivatableObjectStruct>()); 
        }
        return list;
    }

    List<ButtonObjectStruct> GetButtonObjectList()
    {
        List<ButtonObjectStruct> list = new();
        foreach (Transform cur in buttonObjectTransform)
        {
           list.Add(cur.GetComponent<BuildObj>().GetData<ButtonObjectStruct>()); 
            


            // if (cur.GetComponent<BuildObj>().id == 306)
            // {
            //     list.Add(cur.GetComponent<ButtonActivated>().GetData());
            // }
            // else if (cur.GetComponent<BuildObj>().id == 312)
            // {
            //     list.Add(cur.GetComponent<LeverBody>().GetData());
            // }
        }
        return list;
    }

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


    List<DialogueData> GetDialogueList()
    {
        List<DialogueData> list = new();
        foreach (Transform tr in triggerDialogueTransform)
        {
            if (tr.TryGetComponent(out Trigger_Dialogue component))
            {
                list.Add(component.GetDialogueData());
            }
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
            //tile
            mapTileDataList,
            GetTileData(placeMentSystem.halfTileMap),
            GetTileData(placeMentSystem.backgroundTileMap),
            //object
            mapObjectDataList,
            GetButtonActivateObjectStructList(),
            GetButtonObjectList(),
            GetDialogueList(),
            cellSize,1,bytesImage,audioType);

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

        CreateObj(0); //floorTransform
        CreateObj(1,objectTransform); //objectTransform
        CreateObj(2,buttonActivatableObjectTransform); //interactionObjectTransform
        CreateObj(3,exitDoorObjectTransform); //exitDoorObjectTransform
        CreateObj(4,buttonObjectTransform); //interactionObjectTransform
        //todo 0723
        CreateObj(5,triggerDialogueTransform); //triggerDialogueTransform
    }


    public void LoadMap(string name) // main Load 
    {
        Init();
        placeMentSystem.ResetTileMap();
        mapEditorType = MapEditorType.Load;
        mapID = name;
        CurMap = Managers.Data.mapData.mapAllDictionary[name];
        SetMapSize((int)curMap.mapSize.x, (int)curMap.mapSize.y);

        //start Point
        CreateStartPosition();

        //ParallaxCamera Reset
        if (Camera.main.GetComponent<ParallaxCamera>().onCameraTranslate != null) { Camera.main.GetComponent<ParallaxCamera>().onCameraTranslate = null; }
        Camera.main.GetComponent<ParallaxCamera>().oldPosition = startPosition.x;

        //interactionBtnDictionary = new(); //todo 0412

        CreateObj(0); //floorTransform
        CreateObj(1,objectTransform); //objectTransform
        CreateObj(2,buttonActivatableObjectTransform); //
        CreateObj(3,exitDoorObjectTransform); //
        CreateObj(4,buttonObjectTransform); //
        //todo 0723
        CreateObj(5,triggerDialogueTransform); //triggerDialogueTransform
        //todo 0723

        Managers.Sound.PlayBGM(CurMap.audioType, AudioMixerGroupType.BGM, true,.1f);
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
    public void CreateObj(int num,Transform transform = null)
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

                foreach (TileData data in curMap.mapHalfTileDataList)
                {
                    MapDataStruct mapDataStruct = Managers.Data.mapData.mapTileDataDictionary[data.id];
                    placeMentSystem.halfTileMap.SetTile(data.position, Resources.Load<TileBase>(mapDataStruct.path));
                    placeMentSystem.tileDic[data.position] = data.id;
                }
                foreach (TileData data in curMap.mapBackgroundTileDataList)
                {
                    MapDataStruct mapDataStruct = Managers.Data.mapData.mapTileDataDictionary[data.id];
                    placeMentSystem.backgroundTileMap.SetTile(data.position, Resources.Load<TileBase>(mapDataStruct.path));
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
                    else if (Managers.Data.mapData.mapBackgroundDataDictionary.ContainsKey(data.id))
                    {
                        MapDataStruct mapDataStruct = Managers.Data.mapData.mapBackgroundDataDictionary[data.id];

                        Create(transform, mapDataStruct, data);

                    }
                    else if (Managers.Data.mapData.mapOtherDataDictionary.ContainsKey(data.id))
                    {
                        MapDataStruct mapDataStruct = Managers.Data.mapData.mapOtherDataDictionary[data.id];

                        Create(transform, mapDataStruct, data);

                    }
                    else
                    {
                        MapDataStruct mapDataStruct = Managers.Data.mapData.mapObjectDataDictionary[data.id];
                        if (Managers.Game.CurrentState != GameState.Editor &&
                            (
                            data.id == 307 ||
                            data.id == 300 ||
                            data.id == 311 ||
                            data.id == 313 ||
                            data.id == 315 ||
                            data.id == 317 ||
                            data.id == 318 ||
                            data.id == 319
                            ))
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
                foreach (ButtonActivatableObjectStruct data in curMap.mapButtonActivatableObjectDataList)
                {
                    MapDataStruct mapDataStruct = Managers.Data.mapData.mapObjectDataDictionary[data.id];
                    

                    if(Managers.Game.CurrentState != GameState.Editor)
                    {
                        Managers.Stage.CmdBatchObject(mapDataStruct.name,data);

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
            case 4:
                foreach (ButtonObjectStruct data in curMap.buttonObjectList)
                {
                    MapDataStruct mapDataStruct = Managers.Data.mapData.mapObjectDataDictionary[data.id];

                    if (Managers.Game.CurrentState != GameState.Editor)
                    {
                        Managers.Stage.CmdBatchObject(mapDataStruct.name, data);
                    }
                    else
                    {
                        Create(transform, mapDataStruct, data);
                    }

                }
                break;
            //case 5:
            //    foreach (DialogueData data in curMap.dialogueDataList)
            //    {
            //        MapDataStruct mapDataStruct = Managers.Data.mapData.mapSceneDataDictionary[data.id];
            //        if (Managers.Game.CurrentState != GameState.Editor)
            //        {
            //            Managers.Stage.CmdBatchObject(mapDataStruct.name, data);
            //        }
            //        else
            //        {
            //            Create(transform, mapDataStruct, data);
            //        }

            //    }
            //    break;
            //Trigger Dialogue Obj create SaveData.SerializableSaveMapDataDictionary in MapSaveData
            case 5:
                foreach (DialogueData data in Managers.Data.saveData.dic[curMap.mapID]._DialogueDataList)
                {
                    MapDataStruct mapDataStruct = Managers.Data.mapData.mapSceneDataDictionary[data.id];
                    if (Managers.Game.CurrentState != GameState.Editor)
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
        obj.GetComponent<BuildObj>().SetData(data);

        obj.transform.SetParent(transform);

        if(mapEditorState != MapEditorState.NoEditor)
        {
            obj.GetComponent<BuildObj>().TurnOff();
            placeMentSystem.curPlaceObjList.Add(obj.GetComponent<BuildObj>());
        }
    }
    void Create(Transform transform, MapDataStruct mapDataStruct, DialogueData data)
    {
        GameObject obj = Instantiate(Resources.Load<GameObject>(mapDataStruct.path));
        obj.GetComponent<Trigger_Dialogue>().SetDialogueData(data);

        obj.transform.SetParent(transform);

    }
    //todo 0829
    void Create(Transform transform, MapDataStruct mapDataStruct, ButtonActivatableObjectStruct data)
    {
        GameObject obj = Instantiate(Resources.Load<GameObject>(mapDataStruct.path));
        obj.GetComponent<BuildObj>().SetData(data); //todo 0829
        // ButtonActivatedDoor door = obj.GetComponent<ButtonActivatedDoor>();

        
        // door.ButtonActivatedDoorStruct = data;
        // obj.transform.SetParent(transform);

        if (mapEditorState != MapEditorState.NoEditor)
        {
            obj.GetComponent<BuildObj>().TurnOff();
            placeMentSystem.curPlaceObjList.Add(obj.GetComponent<BuildObj>());
        }

    }

    //todo 0522
    void Create(Transform transform, MapDataStruct mapDataStruct, ButtonObjectStruct data)
    {
        GameObject obj = Instantiate(Resources.Load<GameObject>(mapDataStruct.path));       
        obj.GetComponent<ButtonEntity>().SetData(data);
        
        obj.GetComponent<ButtonEntity>().FindTargetObject();

        obj.transform.SetParent(transform);
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




    void CreateStartPosition()
    {
        startPosition = curMap.startPosition;
        startPositionObject = Instantiate(Resources.Load<GameObject>(Managers.Data.mapData.mapObjectDataDictionary[302].path));
        startPositionObject.transform.position = curMap.startPosition;
        startPositionObject.transform.SetParent(dontSaveObjectTransform);
    }
    #endregion

    public void MoveNextStage(string mapId)
    {
        OnStageMove?.Invoke();
        fadeInOutPanel.MoveNextStage(mapId);

        if(mapId == "Lobby")
        {
            OnScreen?.Invoke();
        }
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


