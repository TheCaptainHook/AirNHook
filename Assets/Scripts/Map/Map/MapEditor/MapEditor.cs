
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.IO;
using GoogleSheet.Core.Type;
using TMPro;
using System;
using System.Threading.Tasks;
using System.Reflection;

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

[UGS(typeof(ObjectType))]
public enum ObjectType
{
    Tile,
    Object,
    N_Object,
    Background,
    Other,//Grapic

}

//TODO FIXED CODE LINE 0829 : 
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

    //TOdo 0723
    [HideInInspector] public Transform triggerDialogueTransform;
    //TOdo 0723
    [HideInInspector] public Transform droneTransform;

   
    [HideInInspector] public Transform poolingContainer;

    //TODO 1024
    [HideInInspector] public Transform otherContainer; 
    [HideInInspector] public Transform backgroundObjectContainer;
    //TODO 1024
    //TODO 1202
    [HideInInspector] public Transform collectableContainer;

    //0107 Shadow
    [HideInInspector] public Transform shadowContainer;
    public bool stageClear;
    [Space(10)]

    [Header("Save Data")]
    public MapType mapType;
    [HideInInspector] public int width;
    [HideInInspector] public int height;
    [Tooltip("Only use MapType.Main")]
    public int stageLevel;
    public string mapID; // Map main id
    [Tooltip("A simple explanation of the sub-name for a map.")]
    public string subMapName; // 1116
    [ReadOnly]
    public string nextMapId;
    [ReadOnly]
    public Vector2 startPosition;
    [ReadOnly]
    public GameObject startPositionObject;
    //1122//1122//1122//1122//1122//1122//1122//1122
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
        buttonActivatableObjectTransform = Util.CreateChildTransform(mapObjBoxTransform, "ButtonActivatableObjectTransform");
        buttonObjectTransform = Util.CreateChildTransform(mapObjBoxTransform, "ButtonObjectTransform");
        dontSaveObjectTransform = Util.CreateChildTransform(mapObjBoxTransform, "DontSaveObjectTransform");
        garbageTransform = Util.CreateChildTransform(mapObjBoxTransform, "GarbageTransform");
        networkingObjectTransform = Util.CreateChildTransform(mapObjBoxTransform, "NetworkingObjectTransform");
        //TODO 0723
        triggerDialogueTransform = Util.CreateChildTransform(mapObjBoxTransform, "TriggerDialogueTransform");
        //TODO 0723
        droneTransform = Util.CreateChildTransform(mapObjBoxTransform,"DroneTransform");
        poolingContainer = Util.CreateChildTransform(mapObjBoxTransform, "PoolingContainer");
        //TODO 1024
        otherContainer = Util.CreateChildTransform(mapObjBoxTransform,"OtherContainer");
        otherContainer.gameObject.AddComponent<OtherContainer>();
        backgroundObjectContainer = Util.CreateChildTransform(mapObjBoxTransform,"BackgroundObjectContainer");
        //TODO 1024
        //TODO 1202
        collectableContainer = Util.CreateChildTransform(mapObjBoxTransform, "CollectableContainer");
    
        //0107 Shadow
        shadowContainer = Util.CreateChildTransform(mapObjBoxTransform,"ShadowContainer");
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
    // public void SaveMapData() 
    // {
    //     if(mapEditorType == MapEditorType.New ){
    //         string path = Path.Combine(folderPath, $"{mapID}.json");
    //         bool fileExists = File.Exists(path);
    //         while (fileExists)
    //         {
    //             int num = 1;
    //             path = Path.Combine(folderPath, $"{mapID}{num}.json");
    //             if (!File.Exists(path))
    //             {
    //                 mapID = $"{mapID}{num}";
    //                 fileExists = false;
    //             }

    //             num++;
    //         }
    //         CreateJsonFile();
    //     }
    //     else
    //     {
    //         CreateJsonFile();
    //     }

    // }

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

    // async void CreateJsonFile()
    // {
    //     mapTileDataList = GetTileData(placeMentSystem.floorTileMap);

    //     mapObjectDataList = GetList(objectTransform);
    //     startPosition = FindObj(dontSaveObjectTransform, 302).transform.position;

    //     byte[] bytesImage = await CurrentMapScreenShot();

    //     Map map = new Map(new Vector2(width, height), mapID, stageLevel, startPosition,
    //         GetExitObjStructsList(exitDoorObjectTransform),
    //         //tile
    //         mapTileDataList,
    //         GetTileData(placeMentSystem.halfTileMap),
    //         GetTileData(placeMentSystem.backgroundTileMap),
    //         //object
    //         mapObjectDataList,
    //         GetButtonActivateObjectStructList(),
    //         GetButtonObjectList(),
    //         GetDialogueList(),
    //         cellSize,1,bytesImage,audioType);

    //     string mapDatajson = JsonUtility.ToJson(map, true);
    //     string dateTimedate = JsonUtility.ToJson(new DateTimeData(System.DateTime.Now), true);
        

    //     //string filePath = Path.Combine(folderPath, $"User/{map.mapID}.json");
    //     string filePath = Path.Combine(Application.dataPath, $"UserMapData/{mapID}.json");

    //     string json = JsonUtility.ToJson(new UserMapData(mapDatajson,bytesImage , dateTimedate,GetHashValue(map.mapID)),true);

    //     Debug.Log(filePath);
    //     File.WriteAllText(filePath, json);

    //     Managers.Data.mapData.RefreshUserMapData();
    // }


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
        Init();
        placeMentSystem.ResetTileMap();
        mapEditorType = MapEditorType.Load;
        mapID = name;
        CurMap = Managers.Data.mapData.mapAllDictionary[name];
        
        //start Point
        CreateStartPosition();
        ParallaxCameraReset();

        Create_Tile();
        Create_Object();
        
        // Managers.Sound.PlayBGM(CurMap.audioType, AudioMixerGroupType.BGM, true,.1f);
        if(!string.IsNullOrEmpty(audioName))
            Managers.Sound.PlayBGM(audioName, 0.1f);
    }

// SetMapSize((int)curMap.mapSize.x, (int)curMap.mapSize.y);

    private void ParallaxCameraReset(){
        if (Camera.main.GetComponent<ParallaxCamera>().onCameraTranslate != null) 
        { 
            Camera.main.GetComponent<ParallaxCamera>().onCameraTranslate = null;
        }
        Camera.main.GetComponent<ParallaxCamera>().oldPosition = startPosition.x;
    }
    private void Create_Object(){
        Create_Object(curMap.mapObjectDataList,objectTransform);
        Create_Object(curMap.mapBackgroundObjectList,backgroundObjectContainer);
        // Create_Object(curMap.mapOtherObjectList,otherContainer);
        Create_OtherObject(curMap.mapOtherObjectList,otherContainer);
        Create_Object(curMap.mapButtonActivatableObjectDataList,buttonActivatableObjectTransform);
        Create_Object(curMap.mapExitObjectDataList,exitDoorObjectTransform);
        Create_Object(curMap.buttonObjectList,buttonObjectTransform);
        Create_Object(Managers.Data.saveData.dic[curMap.mapID]._DialogueDataList,triggerDialogueTransform);
        Create_Object(curMap.droneStructList,droneTransform);
        Create_Object(curMap.collectableObjectStructList,collectableContainer);
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
    public void Create_Tile(){
        DrawTile(placeMentSystem.floorTileMap,curMap.mapTileDataList);
        DrawTile(placeMentSystem.halfTileMap,curMap.mapHalfTileDataList);
        DrawTile(placeMentSystem.backgroundTileMap,curMap.mapBackgroundTileDataList);       
        DrawTile(placeMentSystem.ropeTileMap,curMap.mapRopeTileDataList);
        DrawTile(placeMentSystem.accessoryTileMap,curMap.mapAccessoryTIleDataList);
    }
    private void DrawTile(Tilemap tileMap,List<TileData> list){
         foreach (TileData data in list)
         {
            MapDataStruct mapDataStruct = Managers.Data.mapData.mapObjectDataDictionary[data.id];
            tileMap.SetTile(data.position, Resources.Load<TileBase>(mapDataStruct.path));
            placeMentSystem.tileDic[data.position] = data.id;        
         }
    }
    public void Create_OtherObject(List<ObjectData> list,Transform transform){
       MapDataStruct mapDataStruct;
        foreach(ObjectData data in list){
            mapDataStruct = Managers.Data.mapData.mapObjectDataDictionary[data.id];
            Create_OtherObject(mapDataStruct,data,transform);
        };
    }
    private void Create_OtherObject(MapDataStruct mapDataStruct,ObjectData data,Transform transform){
        string[] tags = mapDataStruct.name.Split("_");
        Transform curTr = transform;
        OtherContainer otherContainer = curTr.GetComponent<OtherContainer>();
        for(int i =0;i<tags.Length-1;i++){
            Transform findTr =curTr.Find(tags[i]);
            if(findTr == null){
                findTr = new GameObject(tags[i]).transform;
                findTr.SetParent(curTr);
            }
            curTr = findTr;
        }

        otherContainer.SetGroup(curTr);
        Create(curTr,mapDataStruct,data);

    }
    public void Create_Object<T>(List<T> list ,Transform transform){
        MapDataStruct mapDataStruct;
        Transform _TR;
        foreach(T data in list){
            var isField = typeof(T).GetField("id",BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            if(isField != null){
                var value = isField.GetValue(data);
                if(value is int intValue){
                    mapDataStruct = Managers.Data.mapData.mapObjectDataDictionary[intValue];
                     if(mapDataStruct.objectType == ObjectType.N_Object && Application.isPlaying){
                        if(transform == objectTransform){
                            _TR = networkingObjectTransform;
                        }else{
                            _TR = transform;
                        }
                        Managers.Stage.CmdBatchObject(mapDataStruct.name,data,_TR);
                     }else{
                        Create(transform,mapDataStruct,data);
                     }
                }
               
            }
   
        }
    }

    void Create<T>(Transform transform,MapDataStruct mapDataStruct,T data){
        try{
            GameObject obj = Instantiate(Resources.Load<GameObject>(mapDataStruct.path));
            obj.GetComponent<BuildObj>().SetData(data);
            obj.transform.SetParent(transform);
        }catch(Exception ex){
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

    public void ResetInteractableObjectPosition(){
        foreach(Transform tr in networkingObjectTransform){
            BuildObj obj = tr.GetComponent<BuildObj>();
            if(obj != null && obj.GetDissolveObject()){
                obj.Dissolve(obj.position);
            }
        }
    }


    #endregion


}


