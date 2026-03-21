using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.IO;
using GoogleSheet.Core.Type;
using TMPro;
using System;
using System.Reflection;
using UnityEngine.Rendering.Universal;
using Mirror;

using TileData = ANH_MapEditor.TileData;
using MapType = ANH_MapEditor.MapType;
using System.Collections;
using System.Buffers;
using System.Linq;
using FunkyCode;



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
public enum TransformType
{
    None,
    objectTransform,
    exitDoorObjectTransform,
    buttonActivatableObjectTransform,
    buttonObjectTransform,
    dontSaveObjectTransform,
    networkingObjectTransform,
    triggerDialogueTransform,
    droneTransform,
    poolingContainer,
    otherContainer,
    backgroundObjectContainer,
    collectableContainer,
    shadowContainer
}

[UGS(typeof(ObjectType))]
public enum ObjectType
{
    Tile,
    Object,
    N_Object,
    Background,
    Other,//Grapic

}

public class MapEditor : MonoBehaviour
{
    public static MapEditor Instance;
    Util Util = new Util();

    //private Grid grid;

    [Header("EditorMode")]
    public bool onLoad;
    public bool isLoadMap;
    public PlaceMentSystem placeMentSystem;
    [HideInInspector] public GameObject gridPlane;
    [Header("UI")]
    public MapEditorControllerUI editorUIController;
    public FadeInOutPanel fadeInOutPanel;
    public bool _onMapTransition_Complete;
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

    [HideInInspector] public Transform triggerDialogueTransform;

    [HideInInspector] public Transform droneTransform;

    [HideInInspector] public Transform poolingContainer;

    [HideInInspector] public Transform otherContainer; 
    [HideInInspector] public Transform backgroundObjectContainer;
 
    [HideInInspector] public Transform collectableContainer;

    //0107 Shadow
    [HideInInspector] public Transform shadowContainer;
    public bool stageClear;

    // private Light2D globalLight;
    // public Light2D GlobalLight
    // {
    //     get
    //     {
    //         if(globalLight == null){
    //             globalLight = GetGlobalLight();
    //         }
    //         return globalLight;
    //     }
    // } //-----------------------------------------------------------------------Light


    [Space(10)]
    [Header("Save Data")]
    public MapType mapType;
    [HideInInspector] public int width;
    [HideInInspector] public int height;
    [Tooltip("Only use MapType.Main")]
    public int stageLevel;
    public string mapID; // Map main id
    [Tooltip("A simple explanation of the sub-name for a map.")]
    public string subMapName; 
    public int stageDifficulty; 
    [ReadOnly]
    public string nextMapId;
    [ReadOnly]
    public Vector2 startPosition;
    [ReadOnly]
    public GameObject startPositionObject;
    public string audioName;
    public int globalLightIntensity = 0;
    private LightingManager2D lightingManager;
    public LightingManager2D LightingManager
    {
        get
        {
            if (lightingManager == null)
            {
                lightingManager = FindObjectOfType<LightingManager2D>();
            }
            return lightingManager;
        }
    }
    // public AudioType audioType = AudioType.None;
    //[HideInInspector] public int condition_KeyAmount;
    [HideInInspector] public List<TileData> mapTileDataList = new List<TileData>();
    [HideInInspector] public List<ObjectData> mapObjectDataList = new List<ObjectData>();
    [ReadOnly]
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
    [ReadOnly]
    public GameObject screenShotCamera;
    #region ----------------------------------------Event Action
    // public event Action OnStageMove;
    // public event Action OnScreen;
    public event Action blinkingBoxEvent_Red;
    public event Action blinkingBoxEvent_Blue;
    #endregion
    #region  Blinking Box 
    public void CallBlinkingBoxEvent_Red()
    {
        blinkingBoxEvent_Red?.Invoke();
    }
    public void CallBlinkingBoxEvent_Blue()
    {
        blinkingBoxEvent_Blue?.Invoke();
    }
    public void EventClean()
    {
        blinkingBoxEvent_Red = null;
        blinkingBoxEvent_Blue = null;
    }



    #endregion
    #region Map Transition Value
    // public bool _onMapTransition_Complete;
    #endregion

    private void Awake()
    {
        if (Instance != null)
            Destroy(gameObject);
        else Instance = this;

        folderPath = Path.Combine(Application.dataPath, "Resources/MapDat"); //todo

        fadeInOutPanel.preMapLoadEvent += ReleasePooling;
    }

    //todo
    public void Init()
    {
        stageClear = false;

        if (mapEditorState != MapEditorState.NoEditor) { editorUIController.gameObject.SetActive(true); }
        else { editorUIController.gameObject.SetActive(false); }

        CreateGridPalet();
        CreatePreviewPalet();

        if (mapObjBoxTransform != null) { Destroy(mapObjBoxTransform.gameObject); }

        mapObjBoxTransform = Util.CreateChildTransform("MapObjBox");

        // floorTransform = Util.CreateChildTransform(mapObjBoxTransform, "FloorTransform");
        objectTransform = Util.CreateChildTransform(mapObjBoxTransform, TransformType.objectTransform.ToString());
        exitDoorObjectTransform = Util.CreateChildTransform(mapObjBoxTransform, TransformType.exitDoorObjectTransform.ToString());
        buttonActivatableObjectTransform = Util.CreateChildTransform(mapObjBoxTransform, TransformType.buttonActivatableObjectTransform.ToString());
        buttonObjectTransform = Util.CreateChildTransform(mapObjBoxTransform, TransformType.buttonObjectTransform.ToString());
        dontSaveObjectTransform = Util.CreateChildTransform(mapObjBoxTransform, TransformType.dontSaveObjectTransform.ToString());
        // garbageTransform = Util.CreateChildTransform(mapObjBoxTransform, "garbageTransform");
        networkingObjectTransform = Util.CreateChildTransform(mapObjBoxTransform, TransformType.networkingObjectTransform.ToString());

        triggerDialogueTransform = Util.CreateChildTransform(mapObjBoxTransform, TransformType.triggerDialogueTransform.ToString());

        droneTransform = Util.CreateChildTransform(mapObjBoxTransform, TransformType.droneTransform.ToString());
        poolingContainer = Util.CreateChildTransform(mapObjBoxTransform, TransformType.poolingContainer.ToString());

        otherContainer = Util.CreateChildTransform(mapObjBoxTransform, TransformType.otherContainer.ToString());
        otherContainer.gameObject.AddComponent<OtherContainer>();
        backgroundObjectContainer = Util.CreateChildTransform(mapObjBoxTransform, TransformType.backgroundObjectContainer.ToString());

        collectableContainer = Util.CreateChildTransform(mapObjBoxTransform, TransformType.collectableContainer.ToString());

        shadowContainer = Util.CreateChildTransform(mapObjBoxTransform, TransformType.shadowContainer.ToString());

        if (_d_activePoolingObject == null) _d_activePoolingObject = new();
        if (_n_activePoolingObject == null) _n_activePoolingObject = new();
    }
    public bool GetTransformByType(TransformType type, out Transform tr)
    {
        tr = type switch
        {
            TransformType.objectTransform => objectTransform,
            TransformType.exitDoorObjectTransform => exitDoorObjectTransform,
            TransformType.buttonActivatableObjectTransform => buttonActivatableObjectTransform,
            TransformType.buttonObjectTransform => buttonObjectTransform,
            TransformType.dontSaveObjectTransform => dontSaveObjectTransform,
            TransformType.networkingObjectTransform => networkingObjectTransform,
            TransformType.triggerDialogueTransform => triggerDialogueTransform,
            TransformType.droneTransform => droneTransform,
            TransformType.poolingContainer => poolingContainer,
            TransformType.otherContainer => otherContainer,
            TransformType.backgroundObjectContainer => backgroundObjectContainer,
            TransformType.collectableContainer => collectableContainer,
            TransformType.shadowContainer => shadowContainer,
            _ => null,
        };

        return tr != null;
    }
    public TransformType GetTypeFromTransform(Transform tr)
    {
        if (tr == objectTransform) return TransformType.objectTransform;
        if (tr == exitDoorObjectTransform) return TransformType.exitDoorObjectTransform;
        if (tr == buttonActivatableObjectTransform) return TransformType.buttonActivatableObjectTransform;
        if (tr == buttonObjectTransform) return TransformType.buttonObjectTransform;
        if (tr == dontSaveObjectTransform) return TransformType.dontSaveObjectTransform;
        if (tr == networkingObjectTransform) return TransformType.networkingObjectTransform;
        if (tr == triggerDialogueTransform) return TransformType.triggerDialogueTransform;
        if (tr == droneTransform) return TransformType.droneTransform;
        if (tr == poolingContainer) return TransformType.poolingContainer;
        if (tr == otherContainer) return TransformType.otherContainer;
        if (tr == backgroundObjectContainer) return TransformType.backgroundObjectContainer;
        if (tr == collectableContainer) return TransformType.collectableContainer;
        if (tr == shadowContainer) return TransformType.shadowContainer;

        return default;
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
        placeMentSystem.ropeTileMap = GridPalette.transform.Find("RopeTiles").GetComponent<Tilemap>();
        placeMentSystem.accessoryTileMap = GridPalette.transform.Find("AccessoryTiles").GetComponent<Tilemap>();
        placeMentSystem.hiddentTIleMap = GridPalette.transform.Find("HiddenTiles").GetComponent<Tilemap>();
        placeMentSystem.specialTileMap = GridPalette.transform.Find("SpecialTiles").GetComponent<Tilemap>();
    }
    void CreatePreviewPalet()
    {
        PreviewPalette = Instantiate(previewTileMap);
        placeMentSystem.preViewTileMap = PreviewPalette.transform.Find("PreviewTilemap").GetComponent<Tilemap>();
    }



    #region Save 

    #region GetList

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


    //todo 0918
    private List<T> GetList<T>(Transform transform)
    {
        List<T> list = new();
        foreach (Transform tr in transform)
        {
            T data = (T)(object)tr.GetComponent<BuildObj>().GetData<ObjectData>();
            list.Add(data);
        }
        return list;
    }
    //todo 0918


    #endregion


    #endregion

    #region Load
    // def_obj,back_obj,other_obj,buttonActivatable_obj,dialogue_obj,drone_obj,collect_obj,button_obj
    public bool _l_complete_def_obj, _l_complete_buttonActivatable_obj, _l_complete_dialouge_obj, _l_complete_drone_obj, _l_complete_button_obj;
    private void Load_Clean()
    {
        _l_complete_def_obj = false;
        _l_complete_buttonActivatable_obj = false;
        _l_complete_dialouge_obj = false;
        _l_complete_drone_obj = false;
        _l_complete_button_obj = false;
    }
    

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

        // CreateObj(0); //floorTransform
        // CreateObj(1,objectTransform); //objectTransform
        // CreateObj(2,buttonActivatableObjectTransform); //interactionObjectTransform
        // CreateObj(3,exitDoorObjectTransform); //exitDoorObjectTransform
        // CreateObj(4,buttonObjectTransform); //interactionObjectTransform
        // //todo 0723
        // CreateObj(5,triggerDialogueTransform); //triggerDialogueTransform
    }


    public void LoadMap(string name)
    {
        stageClear = false;
        // event_reset = null;

        if(wayPointList != null) wayPointList.Clear();

        Init();
        placeMentSystem.ResetTileMap();
        mapEditorType = MapEditorType.Load;
        mapID = name;
        CurMap = Managers.Data.mapData.mapAllDictionary[name];
        
        //start Point
        CreateStartPosition();
        ParallaxCameraReset();

        Create_Tile();
        //Shadow Setting
        // Create_Shadow();
        //Light Setting
        // SetGlobalLight();

        Create_Object();

        if (!string.IsNullOrEmpty(curMap.audioName))
            Managers.Sound.PlayBGM(curMap.audioName, 0.1f);

        _onMapTransition_Complete = true;
    }
    //----------------------------------------1003 refactoring
    public IEnumerator LoadMapCo(string name)
    {
        stageClear = false;

        Load_Clean();
        // event_reset = null;

        if (wayPointList != null) wayPointList.Clear();

        Init();
        placeMentSystem.ResetTileMap();
        mapEditorType = MapEditorType.Load;
        mapID = name;
        CurMap = Managers.Data.mapData.mapAllDictionary[name];
        //start Point
        CreateStartPosition();
        ParallaxCameraReset();
        //start Point
        yield return StartCoroutine(Create_Tile_Co());

        //Light, Shadow
        // Create_Shadow();
        // SetGlobalLight();
        //Light, Shadow

        yield return StartCoroutine(Create_Obejct_Co());
        // Create_Object();

        if (!string.IsNullOrEmpty(curMap.audioName))
            Managers.Sound.PlayBGM(curMap.audioName, 0.1f);
            
    }

    //----------------------------------------1003 refactoring

    private void ParallaxCameraReset()
    {
        if (Camera.main.GetComponent<ParallaxCamera>().onCameraTranslate != null)
        {
            Camera.main.GetComponent<ParallaxCamera>().onCameraTranslate = null;
        }
        Camera.main.GetComponent<ParallaxCamera>().oldPosition = startPosition.x;
    }
    private void Create_Object() //
    {
        CreateExitObject(curMap.mapExitObjectStruct);

        Create_Object(curMap.mapObjectDataList, objectTransform);
        Create_Object(curMap.mapBackgroundObjectList, backgroundObjectContainer);

        Create_OtherObject(curMap.mapOtherObjectList, otherContainer);
        Create_Object(curMap.mapButtonActivatableObjectDataList, buttonActivatableObjectTransform);


        Create_Object(Managers.Data.saveData.dic[curMap.mapID]._DialogueDataList, triggerDialogueTransform);
        Create_Object(curMap.droneStructList, droneTransform);
        Create_Object(curMap.collectableObjectStructList, collectableContainer);

        Create_Object(curMap.buttonObjectList, buttonObjectTransform);

    }
    private IEnumerator Create_Obejct_Co()
    {
        CreateExitObject(curMap.mapExitObjectStruct);
        yield return StartCoroutine(Create_Obejct_Co(curMap.mapObjectDataList, objectTransform));
        _l_complete_def_obj = true;

        StartCoroutine(Create_Obejct_Co(curMap.mapBackgroundObjectList, backgroundObjectContainer));
        Create_OtherObject(curMap.mapOtherObjectList, otherContainer);

        yield return StartCoroutine(Create_Obejct_Co(curMap.mapButtonActivatableObjectDataList, buttonActivatableObjectTransform));
        _l_complete_buttonActivatable_obj = true;

         yield return StartCoroutine(Create_Obejct_Co(curMap.buttonObjectList, buttonObjectTransform));
        _l_complete_button_obj = true;

        yield return StartCoroutine(Create_Obejct_Co(Managers.Data.saveData.dic[curMap.mapID]._DialogueDataList, triggerDialogueTransform));
        _l_complete_dialouge_obj = true;

        yield return StartCoroutine(Create_Obejct_Co(curMap.droneStructList, droneTransform));
        _l_complete_drone_obj = true;
        StartCoroutine(Create_Obejct_Co(curMap.collectableObjectStructList, collectableContainer));

       

    }
    #endregion

    #region Util 

    public void SetMapSize(int width, int height)
    {
        this.width = width;
        this.height = height;
    }

    #region Create
    public Queue<BuildObj> _d_activePoolingObject; 
    public Queue<BuildObj> _n_activePoolingObject; 
    public void Create_Tile()
    {
        DrawTile_C(placeMentSystem.floorTileMap, curMap.mapTileDataList); //rect
        DrawTile_C(placeMentSystem.halfTileMap, curMap.mapHalfTileDataList);
        DrawTile_C(placeMentSystem.backgroundTileMap, curMap.mapBackgroundTileDataList);
        DrawTile_C(placeMentSystem.ropeTileMap, curMap.mapRopeTileDataList);
        DrawTile_C(placeMentSystem.accessoryTileMap, curMap.mapAccessoryTIleDataList);
        DrawTile_C(placeMentSystem.hiddentTIleMap, curMap.mapHiddenTileDataList);
        DrawTile_C(placeMentSystem.specialTileMap, curMap.mapSpecialTileDataList);
        //DrawTile(placeMentSystem.floorTileMap, curMap.mapTileDataList); //rect
        //DrawTile(placeMentSystem.halfTileMap, curMap.mapHalfTileDataList);
        //DrawTile(placeMentSystem.backgroundTileMap, curMap.mapBackgroundTileDataList);
        //DrawTile(placeMentSystem.ropeTileMap, curMap.mapRopeTileDataList);
        //DrawTile(placeMentSystem.accessoryTileMap, curMap.mapAccessoryTIleDataList);
    }
    private IEnumerator Create_Tile_Co()
    {
        yield return StartCoroutine(DrawTile_C_Co(placeMentSystem.floorTileMap, curMap.mapTileDataList));
        yield return StartCoroutine(DrawTile_C_Co(placeMentSystem.halfTileMap, curMap.mapHalfTileDataList));
        yield return StartCoroutine(DrawTile_C_Co(placeMentSystem.backgroundTileMap, curMap.mapBackgroundTileDataList));
        yield return StartCoroutine(DrawTile_C_Co(placeMentSystem.ropeTileMap, curMap.mapRopeTileDataList));
        yield return StartCoroutine(DrawTile_C_Co(placeMentSystem.accessoryTileMap, curMap.mapAccessoryTIleDataList));
        yield return StartCoroutine(DrawTile_C_Co(placeMentSystem.hiddentTIleMap,curMap.mapHiddenTileDataList));
    }

    private void DrawTile(Tilemap tileMap, List<TileData> list)
    {
        foreach (TileData data in list)
        {
            MapDataStruct mapDataStruct = Managers.Data.mapData.mapObjectDataDictionary[data.id];
            tileMap.SetTile(data.position, Resources.Load<TileBase>(mapDataStruct.path));
            placeMentSystem.tileDic[data.position] = data.id;
        }
    }
    private Dictionary<int, TileBase> _tileCache = new();
    public void DrawTile_C(Tilemap tileMap, List<CompressedTileData> list)
    {
        foreach (var data in list)
        {
            TileBase tileBase = GetTileBase(data.TileId);
            var values = GetMaxMin(data);

            int width = values.maxX - values.minX + 1;
            int height = values.maxY - values.minY + 1;
            
            BoundsInt bounds = new BoundsInt(values.minX, values.minY, 0, width, height, 1);
            TileBase[] tiles = new TileBase[width * height];
            
            Array.Fill(tiles, tileBase);
            tileMap.SetTilesBlock(bounds, tiles);

        }
    }
    private int _tile_batchSize = 50;
    private int _object_batchSize = 10;
    private IEnumerator DrawTile_C_Co(Tilemap tileMap, List<CompressedTileData> list)
    {
        int counter = 0;
        foreach (var data in list)
        {
            TileBase tileBase = GetTileBase(data.TileId);
            var values = GetMaxMin(data);

            int width = values.maxX - values.minX + 1;
            int height = values.maxY - values.minY + 1;

            BoundsInt bounds = new BoundsInt(values.minX, values.minY, 0, width, height, 1);
            // TileBase[] tiles = new TileBase[width * height];
            var tiles = ArrayPool<TileBase>.Shared.Rent(width * height);

            try
            {
                for (int i = 0; i < width * height; i++)
                {
                    tiles[i] = tileBase;
                }
                tileMap.SetTilesBlock(bounds, tiles);
            }
            finally
            {
                ArrayPool<TileBase>.Shared.Return(tiles, clearArray: true);
            }

            counter++;
            if (counter >= _tile_batchSize)
            {
                counter = 0;
                yield return null;
            }
        }

    }
    private TileBase GetTileBase(int id)
    {
        if (!_tileCache.TryGetValue(id, out var tileBase))
        {
            var mapDataStruct = Managers.Data.mapData.mapObjectDataDictionary[id];
            tileBase = Resources.Load<TileBase>(mapDataStruct.path);
            _tileCache[id] = tileBase;
        }
        return tileBase;
    }


    private (int maxX, int minX, int maxY, int minY) GetMaxMin(CompressedTileData data)
    {

        Vector2Int start = data.Start; //0 ,5
        Vector2Int end = data.End; // 5 , 7

        int maxX = Mathf.Max(start.x, end.x);
        int minX = Mathf.Min(start.x, end.x);
        int maxY = Mathf.Max(start.y, end.y);
        int minY = Mathf.Min(start.y, end.y);

        return (maxX, minX, maxY, minY);

    }

    public void Create_OtherObject(List<ObjectData> list, Transform transform)
    {
        MapDataStruct mapDataStruct;
        foreach (ObjectData data in list)
        {
            mapDataStruct = Managers.Data.mapData.mapObjectDataDictionary[data.id];
            Create_OtherObject(mapDataStruct, data, transform);
        }
        ;
    }
    private void CreateExitObject(ExitObjStruct data)
    {
        MapDataStruct mapDataStruct = Managers.Data.mapData.mapObjectDataDictionary[data.id];
        //Create(exitDoorObjectTransform,mapDataStruct,data);
        if(NetworkServer.active)
        // Managers.Stage.ServerBatchObject(mapDataStruct.name, data, exitDoorObjectTransform);
        Managers.Stage.ServerBatchObject(mapDataStruct.name, data, TransformType.exitDoorObjectTransform);
    }

    private void Create_OtherObject(MapDataStruct mapDataStruct, ObjectData data, Transform transform)
    {
        string[] tags = mapDataStruct.name.Split("_");
        Transform curTr = transform;
        OtherContainer otherContainer = curTr.GetComponent<OtherContainer>();
        for (int i = 0; i < tags.Length - 1; i++)
        {
            Transform findTr = curTr.Find(tags[i]);
            if (findTr == null)
            {
                findTr = new GameObject(tags[i]).transform;
                findTr.SetParent(curTr);
            }
            curTr = findTr;
        }
        otherContainer.SetGroup(curTr);

        if (mapDataStruct.objectType == ObjectType.N_Object && Application.isPlaying)
        {
            if (NetworkServer.active)
                Managers.Stage.ServerBatchObject(mapDataStruct.name, data, GetTypeFromTransform(curTr));
        }
        else
        {
            Create(curTr, mapDataStruct, data);
        }


    }
    public void Create_Object<T>(List<T> list, Transform transform)
    {
        var dic = Managers.Data.mapData.mapObjectDataDictionary;

        Transform _TR;
        var isField = typeof(T).GetField("id", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        if (isField == null) return;

        foreach (T data in list)
        {
            if (isField.GetValue(data) is not int intValue) continue;
            if (!dic.TryGetValue(intValue, out var mapDataStruct)) continue;

            if (mapDataStruct.objectType == ObjectType.N_Object && Application.isPlaying)
            {
                _TR = (transform == objectTransform) ? networkingObjectTransform : transform;

                if (NetworkServer.active)
                    Managers.Stage.ServerBatchObject(mapDataStruct.name, data, GetTypeFromTransform(_TR));
            }
            else
            {
                Create(transform, mapDataStruct, data);
            }
        }
    }

    private IEnumerator Create_Obejct_Co<T>(List<T> list, Transform transform)
    {
        var dic = Managers.Data.mapData.mapObjectDataDictionary;
        int counter = 0;

        Transform _TR;
        var isField = typeof(T).GetField("id", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        if (isField == null) yield break;

        foreach (T data in list)
        {
            if (isField.GetValue(data) is not int intValue) continue;
            if (!dic.TryGetValue(intValue, out var mapDataStruct)) continue;

            if (mapDataStruct.objectType == ObjectType.N_Object && Application.isPlaying)
            {
                _TR = (transform == objectTransform) ? networkingObjectTransform : transform;

                if (NetworkServer.active)
                    Managers.Stage.ServerBatchObject(mapDataStruct.name, data, GetTypeFromTransform(_TR));
            }
            else
            {
                Create(transform, mapDataStruct, data);
            }

            counter++;
            if (counter >= _object_batchSize)
            {
                counter = 0;
                yield return null;
            }

        }

    }
    public List<WayPoint_Var2> wayPointList;
   
    void Create<T>(Transform transform, MapDataStruct mapDataStruct, T data)
    {
        try
        {
            GameObject obj = Managers.Pooling.D_GetItem(ResourceManager.Load<GameObject>(mapDataStruct.path));
            obj.name = mapDataStruct.name;

            if (obj.name == "WayPoint" || obj.name == "WayPoint_Rusted")
            {
                if (wayPointList == null) wayPointList = new();
                wayPointList.Add(obj.GetComponent<WayPoint_Var2>());
            }

            if (obj.TryGetComponent(out BuildObj build))
            {
                build.SetData(data);
                build.transform.SetParent(transform);
                build.gameObject.SetActive(true);
                _d_activePoolingObject.Enqueue(build);
            }          
        }
        catch (Exception ex)
        {
            Debug.Log($"{ex},{mapDataStruct.id}");
        }

    }

    void CreateStartPosition()
    {
        startPosition = curMap.startPosition;
        startPositionObject = Instantiate(Resources.Load<GameObject>(Managers.Data.mapData.mapObjectDataDictionary[302].path));
        startPositionObject.transform.position = curMap.startPosition;
        startPositionObject.transform.SetParent(dontSaveObjectTransform);
    }

    // private void Create_Shadow()
    // {
    //     GameObject shadowPrefab =Resources.Load<GameObject>(GlobalText.SHADOW_PREFAB_PATH);
    //     foreach(ShadowCasterStruct data in curMap.mapShadowCasterDataList)
    //     {
    //         ShadowCasterSetting shadowSetting = Managers.Pooling.D_GetItem(shadowPrefab).GetComponent<ShadowCasterSetting>();
    //         shadowSetting.gameObject.SetActive(true);
    //         shadowSetting.gameObject.transform.SetParent(shadowContainer);    
    //         shadowSetting.transform.position = data.position;
    //         shadowSetting.SetShadowCasterData(data);
    //     }
    // }
    // private void SetGlobalLight()
    // {
    //     FieldInfo sortingLayerField = typeof(Light2D).GetField("m_ApplyToSortingLayers", BindingFlags.NonPublic | BindingFlags.Instance);

    //     var lightData = curMap.globalLightStruct;

    //     if(lightData.type == default)
    //     {
    //         GlobalLight.lightType = Light2D.LightType.Global;
    //         GlobalLight.color = Color.white;
    //         GlobalLight.intensity = 1;
    //         sortingLayerField.SetValue(GlobalLight, new int[] { 0 });
    //         GlobalLight.blendStyleIndex = 0;
    //         GlobalLight.lightOrder = 0;
    //         GlobalLight.overlapOperation = 0;
    //         return;
    //     }

    //     GlobalLight.lightType = lightData.type;
    //     GlobalLight.color = lightData.color;
    //     GlobalLight.intensity = lightData.intensity;
    //     sortingLayerField.SetValue(GlobalLight, lightData.targetSorting);
    //     GlobalLight.blendStyleIndex = lightData.blendStyleIndex;
    //     GlobalLight.lightOrder = lightData.lightOrder;
    //     GlobalLight.overlapOperation = lightData.overlapOeration;
    // }
    #endregion


    public void MoveNextStage(string mapId)
    {
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
    public T FindObj<T>(Transform transform) where T :class
    {
        foreach(Transform item in transform){
            if(item.TryGetComponent(out T component)){
                return item.GetComponent<T>();
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

    public event Action event_reset;
    public void ResetInteractableObjectPosition()
    {
        event_reset?.Invoke();
    }
    public void AddEvent_Reset(Action action)
    {
        if(event_reset == null || !event_reset.GetInvocationList().Contains(action))
        {
            event_reset += action;
        }
    }
    public void Remove_Event_Reset(Action action)
    {
        if(event_reset != null && event_reset.GetInvocationList().Contains(action))
        {
            event_reset -= action;
        }
    }

    // private Light2D GetGlobalLight()
    // {
    //     foreach (Transform tr in transform)
    //     {
    //         if (tr.name == "Global Light")
    //         {
    //             return tr.GetComponent<Light2D>();
    //         }
    //     }
    //     return null;

    // }
    private void ReleasePooling()
    {
       for(int i = shadowContainer.childCount-1;i>=0;i--)
       {
            Transform tr = shadowContainer.GetChild(i);
            tr.GetComponent<IPooling>().D_ReleaseToPool();
       }
    }

    #endregion


}


