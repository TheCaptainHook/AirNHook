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

    private Light2D globalLight;
    public Light2D GlobalLight
    {
        get
        {
            if(globalLight == null){
                globalLight = GetGlobalLight();
            }
            return globalLight;
        }
    } //-----------------------------------------------------------------------Light


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
    #endregion

    private void Awake()
    {
        if (Instance != null)
            Destroy(gameObject);
        else Instance = this;

        folderPath = Path.Combine(Application.dataPath, "Resources/MapDat"); //todo
        
        fadeInOutPanel.preMapLoadEvent+=ReleasePooling;
    }

    //todo
    public void Init()
    {
        stageClear = false;

        if (mapEditorState != MapEditorState.NoEditor) { editorUIController.gameObject.SetActive(true); }
        else { editorUIController.gameObject.SetActive(false); }

        CreateGridPalet();
        CreatePreviewPalet();

        if(mapObjBoxTransform != null) { Destroy(mapObjBoxTransform.gameObject); }

        mapObjBoxTransform = Util.CreateChildTransform("MapObjBox");

        // floorTransform = Util.CreateChildTransform(mapObjBoxTransform, "FloorTransform");
        objectTransform = Util.CreateChildTransform(mapObjBoxTransform, "objectTransform");
        exitDoorObjectTransform = Util.CreateChildTransform(mapObjBoxTransform, "exitDoorObjectTransform");
        buttonActivatableObjectTransform = Util.CreateChildTransform(mapObjBoxTransform, "buttonActivatableObjectTransform");
        buttonObjectTransform = Util.CreateChildTransform(mapObjBoxTransform, "buttonObjectTransform");
        dontSaveObjectTransform = Util.CreateChildTransform(mapObjBoxTransform, "dontSaveObjectTransform");
        // garbageTransform = Util.CreateChildTransform(mapObjBoxTransform, "garbageTransform");
        networkingObjectTransform = Util.CreateChildTransform(mapObjBoxTransform, "networkingObjectTransform");

        triggerDialogueTransform = Util.CreateChildTransform(mapObjBoxTransform, "triggerDialogueTransform");

        droneTransform = Util.CreateChildTransform(mapObjBoxTransform, "droneTransform");
        poolingContainer = Util.CreateChildTransform(mapObjBoxTransform, "poolingContainer");

        otherContainer = Util.CreateChildTransform(mapObjBoxTransform, "otherContainer");
        otherContainer.gameObject.AddComponent<OtherContainer>();
        backgroundObjectContainer = Util.CreateChildTransform(mapObjBoxTransform, "backgroundObjectContainer");
  
        collectableContainer = Util.CreateChildTransform(mapObjBoxTransform, "collectableContainer");

        shadowContainer = Util.CreateChildTransform(mapObjBoxTransform, "shadowContainer");
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
    private List<T> GetList<T>(Transform transform){
        List<T> list = new();
        foreach(Transform tr in transform){
          T data =(T)(object)tr.GetComponent<BuildObj>().GetData<ObjectData>();
          list.Add(data);
        }
        return list;
    }
    //todo 0918
  

    #endregion

 
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

        event_reset = null;

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
        Create_Shadow();
        //Light Setting
        SetGlobalLight();

        Create_Object();
        
        if(!string.IsNullOrEmpty(curMap.audioName))
            Managers.Sound.PlayBGM(curMap.audioName, 0.1f);
    }

    private void ParallaxCameraReset()
    {
        if (Camera.main.GetComponent<ParallaxCamera>().onCameraTranslate != null)
        {
            Camera.main.GetComponent<ParallaxCamera>().onCameraTranslate = null;
        }
        Camera.main.GetComponent<ParallaxCamera>().oldPosition = startPosition.x;
    }
    private void Create_Object()
    {
        CreateExitObject(curMap.mapExitObjectStruct);
        Create_Object(curMap.mapObjectDataList, objectTransform);
        Create_Object(curMap.mapBackgroundObjectList, backgroundObjectContainer);

        Create_OtherObject(curMap.mapOtherObjectList, otherContainer);

        try
        {
            Create_Object(curMap.mapButtonActivatableObjectDataList, buttonActivatableObjectTransform);
        }
        catch (Exception)
        {
            
        }
        Create_Object(curMap.buttonObjectList, buttonObjectTransform);

        Create_Object(Managers.Data.saveData.dic[curMap.mapID]._DialogueDataList, triggerDialogueTransform);
        Create_Object(curMap.droneStructList, droneTransform);
        Create_Object(curMap.collectableObjectStructList, collectableContainer);
    }
    #endregion

    #region Util 

    public void SetMapSize(int width, int height)
    {
        this.width = width;
        this.height = height;
    }

    #region Create
    public void Create_Tile()
    {
        DrawTile_C(placeMentSystem.floorTileMap, curMap.mapTileDataList); //rect
        DrawTile_C(placeMentSystem.halfTileMap, curMap.mapHalfTileDataList);
        DrawTile_C(placeMentSystem.backgroundTileMap, curMap.mapBackgroundTileDataList);
        DrawTile_C(placeMentSystem.ropeTileMap, curMap.mapRopeTileDataList);
        DrawTile_C(placeMentSystem.accessoryTileMap, curMap.mapAccessoryTIleDataList);
        DrawTile_C(placeMentSystem.hiddentTIleMap, curMap.mapHiddenTileDataList);
        //DrawTile(placeMentSystem.floorTileMap, curMap.mapTileDataList); //rect
        //DrawTile(placeMentSystem.halfTileMap, curMap.mapHalfTileDataList);
        //DrawTile(placeMentSystem.backgroundTileMap, curMap.mapBackgroundTileDataList);
        //DrawTile(placeMentSystem.ropeTileMap, curMap.mapRopeTileDataList);
        //DrawTile(placeMentSystem.accessoryTileMap, curMap.mapAccessoryTIleDataList);
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
    public void DrawTile_C(Tilemap tileMap, List<CompressedTileData> list)
    {
        foreach (var data in list)
        {
            MapDataStruct mapDataStruct = Managers.Data.mapData.mapObjectDataDictionary[data.TileId];
            TileBase tileBase = Resources.Load<TileBase>(mapDataStruct.path);

            var values = GetMaxMin(data);

            for (int i = values.minX; i <= values.maxX; i++)
            {
                for (int j = values.minY; j <= values.maxY; j++)
                {
                    tileMap.SetTile(new Vector3Int(i, j, 0), tileBase);
                }
            }


        }
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
        Managers.Stage.CmdBatchObject(mapDataStruct.name, data, exitDoorObjectTransform);
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
                Managers.Stage.CmdBatchObject(mapDataStruct.name, data, curTr);
        }
        else
        {
            Create(curTr, mapDataStruct, data);
        }


    }
    public void Create_Object<T>(List<T> list, Transform transform)
    {
        MapDataStruct mapDataStruct;
        Transform _TR;
        foreach (T data in list)
        {
            var isField = typeof(T).GetField("id", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            if (isField != null)
            {
                var value = isField.GetValue(data);
                if (value is int intValue)
                {
                    mapDataStruct = Managers.Data.mapData.mapObjectDataDictionary[intValue];
                    if (mapDataStruct.objectType == ObjectType.N_Object && Application.isPlaying)
                    {
                        if (transform == objectTransform)
                        {
                            _TR = networkingObjectTransform;
                        }
                        else
                        {
                            _TR = transform;
                        }
                        if (NetworkServer.active)
                            Managers.Stage.CmdBatchObject(mapDataStruct.name, data, _TR);
                    }
                    else
                    {
                        Create(transform, mapDataStruct, data);
                    }

                }

            }

        }
    }
    public List<WayPoint_Var2> wayPointList;
    void Create<T>(Transform transform, MapDataStruct mapDataStruct, T data)
    {
        try
        {
            GameObject obj = Instantiate(Resources.Load<GameObject>(mapDataStruct.path));
            obj.name = mapDataStruct.name;

            if (obj.name == "WayPoint" || obj.name == "WayPoint_Rusted")
            {
                if (wayPointList == null) wayPointList = new();
                wayPointList.Add(obj.GetComponent<WayPoint_Var2>());
            }

            obj.GetComponent<BuildObj>().SetData(data);
            obj.transform.SetParent(transform);
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

    private void Create_Shadow()
    {
        GameObject shadowPrefab =Resources.Load<GameObject>(GlobalText.SHADOW_PREFAB_PATH);
        foreach(ShadowCasterStruct data in curMap.mapShadowCasterDataList)
        {
            ShadowCasterSetting shadowSetting = Managers.Pooling.D_GetItem(shadowPrefab).GetComponent<ShadowCasterSetting>();
            shadowSetting.gameObject.SetActive(true);
            shadowSetting.gameObject.transform.SetParent(shadowContainer);    
            shadowSetting.transform.position = data.position;
            shadowSetting.SetShadowCasterData(data);
        }
    }
    private void SetGlobalLight()
    {
        FieldInfo sortingLayerField = typeof(Light2D).GetField("m_ApplyToSortingLayers", BindingFlags.NonPublic | BindingFlags.Instance);

        var lightData = curMap.globalLightStruct;

        if(lightData.type == default)
        {
            GlobalLight.lightType = Light2D.LightType.Global;
            GlobalLight.color = Color.white;
            GlobalLight.intensity = 1;
            sortingLayerField.SetValue(GlobalLight, new int[] { 0 });
            GlobalLight.blendStyleIndex = 0;
            GlobalLight.lightOrder = 0;
            GlobalLight.overlapOperation = 0;
            return;
        }

        GlobalLight.lightType = lightData.type;
        GlobalLight.color = lightData.color;
        GlobalLight.intensity = lightData.intensity;
        sortingLayerField.SetValue(GlobalLight, lightData.targetSorting);
        GlobalLight.blendStyleIndex = lightData.blendStyleIndex;
        GlobalLight.lightOrder = lightData.lightOrder;
        GlobalLight.overlapOperation = lightData.overlapOeration;
    }
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


    private Light2D GetGlobalLight()
    {
        foreach (Transform tr in transform)
        {
            if (tr.name == "Global Light")
            {
                return tr.GetComponent<Light2D>();
            }
        }
        return null;

    }
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


