
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;
using UGS;
using System.IO;
using System.Threading.Tasks;

[CustomEditor(typeof(MapEditor))]
public class MapEditor_Editor : Editor
{
    public Dictionary<int, MapDataStruct> mapTileDataDictionary = new Dictionary<int, MapDataStruct>();
    public Dictionary<int, MapDataStruct> mapObjectDataDictionary = new Dictionary<int, MapDataStruct>();
    public Dictionary<int, MapDataStruct> mapSceneDataDictionary = new Dictionary<int, MapDataStruct>();
    public Dictionary<int, MapDataStruct> mapOtherDataDictionary = new Dictionary<int, MapDataStruct>();

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        MapEditor mapEditor = target as MapEditor;

        GUILayout.Space(10);

        EditorGUILayout.LabelField("Map Editor------------------------------------", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox($"프로젝트 실행할때 꼭 개발자용 데이터 세이브 후 Reset 버튼 누른다음 실행하기.", MessageType.Info);



        if (GUILayout.Button("Load Data(인게임용)"))
        {
            mapEditor.LoadMap(mapEditor.mapID,mapEditor.mapType);
        }
        //if (GUILayout.Button("Save Data(인게임용)"))
        //{
        //    mapEditor.SaveMapData();
        //}

        GUILayout.Space(10);


        if (GUILayout.Button("개발자용, 맵 새로만들 때 먼저 누르기,Init!"))
        {
            _Reset(mapEditor);
            mapEditor.Init();
            EditorApplication.ExecuteMenuItem("Window/2D/Tile Palette");
        }
        if (GUILayout.Button("- Object Create Tool -"))
        {
            CreateMap_Tool.ShowWindow();
        }
        GUILayout.Space(10);

        if (GUILayout.Button("Load Data(개발자전용)"))
        {
            _Reset(mapEditor);
            LoadMap(mapEditor);
        }


        if (GUILayout.Button("Save Data(개발자전용)"))
        {
            Debug.Log("Buttom");
            SaveMapData(mapEditor);
        }

        GUILayout.Space(10);

        if (GUILayout.Button("Reset"))
        {

            _Reset(mapEditor);


            Managers.Data.loadData.Save();

        }

        //if (GUILayout.Button("In Game Editor Test btn")) //에디터 모드로 진입할때 초기화
        //{
        //    mapEditor.EditorMode_Init();
        //    //Managers.Game.CurrentState = GameState.Editor;
        //    //mapEditor.mapEditorState = MapEditorState.Editor;
        //    //mapEditor.Init();
        //    //mapEditor.gridPlane = Instantiate(Resources.Load<GameObject>("Prefabs/MapEditor/GridPlane"));
        //    //mapEditor.gridPlane.SetActive(false);
        //    //mapEditor.placeMentSystem.EditorMode_Init();

        //}
        //GUILayout.Space(10);
        //if (GUILayout.Button("User map Test btn"))
        //{
        //    TestLoad(mapEditor);
        //}


        //GUILayout.Space(10);
        //if (GUILayout.Button("TEST SCREEN SHOT"))
        //{
        //    CurrentMapScreenShot(mapEditor);
        //}



    }

    private void _Reset(MapEditor mapEditor)
    {
        if (mapEditor.mapObjBoxTransform)
        {
            Undo.DestroyObjectImmediate(mapEditor.mapObjBoxTransform.gameObject);
        }
        if (mapEditor.GridPalette != null)
        {
            Undo.DestroyObjectImmediate(mapEditor.GridPalette);
        }
        if (mapEditor.PreviewPalette != null)
        {
            Undo.DestroyObjectImmediate(mapEditor.PreviewPalette);
        }
        if (mapEditor.screenShotCamera != null)
        {
            Undo.DestroyObjectImmediate(mapEditor.screenShotCamera.gameObject);
        }

        mapEditor.curMap = new Map();
    }

    void UGS_MapDataLoad()
    {
        //mapTileDataDictionary.Clear();
        //mapObjectDataDictionary.Clear();
        //mapSceneDataDictionary.Clear();
        //Tile Data
        UnityGoogleSheet.Load<MapObjectData.TileData>();
        foreach (var value in MapObjectData.TileData.TileDataList)
        {
            if (!mapTileDataDictionary.ContainsKey(value.id))
            {
                mapTileDataDictionary.Add(value.id, new MapDataStruct(value.name, value.type, value.path));
            }

        }
        //Object Data
        UnityGoogleSheet.Load<MapObjectData.ObjectData>();
        foreach (var value in MapObjectData.ObjectData.ObjectDataList)
        {
            if (!mapObjectDataDictionary.ContainsKey(value.id))
            {
                mapObjectDataDictionary.Add(value.id, new MapDataStruct(value.name, value.type, value.path));
            }

        }
        UnityGoogleSheet.Load<MapObjectData.SceneData>();
        foreach (var value in MapObjectData.SceneData.SceneDataList)
        {
            if (!mapSceneDataDictionary.ContainsKey(value.id))
            {

                mapSceneDataDictionary.Add(value.id, new MapDataStruct(value.name, value.type, value.path));
            }
        }
        UnityGoogleSheet.Load<MapObjectData.OtherData>();
        foreach (var value in MapObjectData.OtherData.OtherDataList)
        {
            if (!mapOtherDataDictionary.ContainsKey(value.id))
            {
                mapOtherDataDictionary.Add(value.id, new MapDataStruct(value.name, value.type, value.path));
            }

        }
    }

    #region  Create 

    void LoadMap(MapEditor mapEditor)
    {
        mapEditor.Init();
        UGS_MapDataLoad();
        TextAsset textAsset = GetTextAsset(mapEditor.mapType, mapEditor.mapID);
        if (textAsset != null)
        {
            Map map = JsonUtility.FromJson<Map>(textAsset.text);
            mapEditor.CurMap = map;
            mapEditor.SetMapSize((int)map.mapSize.x, (int)map.mapSize.y);

            //start Point
            GameObject startPoint = Instantiate(Resources.Load<GameObject>(mapObjectDataDictionary[302].path));
            mapEditor.startPositionObject = startPoint;
            startPoint.transform.position = map.startPosition;
            startPoint.transform.SetParent(mapEditor.dontSaveObjectTransform);
            //start Point

            mapEditor.interactionBtnDictionary = new(); //todo 0412

            CreateObj(mapEditor.floorTransform, map, mapEditor.placeMentSystem, mapEditor, 0);
            CreateObj(mapEditor.objectTransform, map, mapEditor.placeMentSystem, mapEditor, 1);
            CreateObj(mapEditor.interactionObjectTransform, map, mapEditor.placeMentSystem, mapEditor, 2);
            CreateObj(mapEditor.exitDoorObjectTransform, map, mapEditor.placeMentSystem, mapEditor, 3);

            MapDataStruct btn = mapObjectDataDictionary[306];

            foreach (int key in mapEditor.interactionBtnDictionary.Keys)
            {
                foreach (Vector2 pot in mapEditor.interactionBtnDictionary[key])
                {
                    GameObject btnActivated = Instantiate(Resources.Load<GameObject>(btn.path));
                    btnActivated.GetComponent<ButtonActivated>().SetLinkDoor(pot, key, mapEditor.interactionObjectTransform);
                    btnActivated.transform.SetParent(mapEditor.dontSaveObjectTransform);
                }
            }

        }
        else
        {
            Debug.Log("Map not found");
        }

    }

    //0422 testtest
    public void TestLoad(MapEditor mapEditor) // user map Test Code
    {
        UGS_MapDataLoad();
        string path = Path.Combine(Application.dataPath, "UserMapData");
        string[] filePaths = Directory.GetFiles(path, "*.json");

        string jsonString = File.ReadAllText(filePaths[0]);
        UserMapData data = JsonUtility.FromJson<UserMapData>(jsonString);
        Map map = data.LoadMap();

        mapEditor.Init();

        mapEditor.CurMap = map;
        mapEditor.SetMapSize((int)map.mapSize.x, (int)map.mapSize.y);

        //start Point
        GameObject startPoint = Instantiate(Resources.Load<GameObject>(mapObjectDataDictionary[302].path));
        mapEditor.startPositionObject = startPoint;
        startPoint.transform.position = map.startPosition;
        startPoint.transform.SetParent(mapEditor.dontSaveObjectTransform);
        //start Point

        mapEditor.interactionBtnDictionary = new(); //todo 0412

        CreateObj(mapEditor.floorTransform, map, mapEditor.placeMentSystem, mapEditor, 0);
        CreateObj(mapEditor.objectTransform, map, mapEditor.placeMentSystem, mapEditor, 1);
        CreateObj(mapEditor.interactionObjectTransform, map, mapEditor.placeMentSystem, mapEditor, 2);
        CreateObj(mapEditor.exitDoorObjectTransform, map, mapEditor.placeMentSystem, mapEditor, 3);

        MapDataStruct btn = mapObjectDataDictionary[306];

        foreach (int key in mapEditor.interactionBtnDictionary.Keys)
        {
            foreach (Vector2 pot in mapEditor.interactionBtnDictionary[key])
            {
                GameObject btnActivated = Instantiate(Resources.Load<GameObject>(btn.path));
                btnActivated.GetComponent<ButtonActivated>().SetLinkDoor(pot, key, mapEditor.interactionObjectTransform);
                btnActivated.transform.SetParent(mapEditor.dontSaveObjectTransform);
            }
        }

    }
    

    //0422 testtest

    TextAsset GetTextAsset(MapType mapType,string id)
    {
        switch (mapType)
        {
            case MapType.Scene:
                return Resources.Load<TextAsset>($"MapDat/{mapType}/{id}");
            case MapType.Main:
                string path = Path.Combine(Application.dataPath, "Resources/MapDat/Main");
                Debug.Log(path);
                for (int i = 0; i <= 4; i++)
                {
                    string checkPath = Path.Combine(path, $"{i}/{id}.json");
                    Debug.Log(checkPath);
                    if (File.Exists(checkPath))
                    {
                        return Resources.Load<TextAsset>($"MapDat/{mapType}/{i}/{id}");
                    }
                }
                break;
            case MapType.User:
                return Resources.Load<TextAsset>($"MapDat/{mapType}/{id}");
        }


        return null;
    }

    public void CreateObj(Transform transform,Map map,PlaceMentSystem placeMentSystem,MapEditor mapEditor,int num)
    {

        switch (num)
        {
            case 0:

                foreach (TileData data in map.mapTileDataList)
                {
                    MapDataStruct mapDataStruct = mapTileDataDictionary[data.id];
                    placeMentSystem.floorTileMap.SetTile(data.position, Resources.Load<TileBase>(mapDataStruct.path));
                    placeMentSystem.tileDic[data.position] = data.id;
                }
                break;
            case 1:
                foreach (ObjectData data in map.mapObjectDataList)
                {
                    if (mapSceneDataDictionary.ContainsKey(data.id))
                    {
                        MapDataStruct mapDataStruct = mapSceneDataDictionary[data.id];
                        Create(transform, mapDataStruct, data);
                    }
                    else if (mapOtherDataDictionary.ContainsKey(data.id))
                    {
                        MapDataStruct mapDataStruct = mapOtherDataDictionary[data.id];
                        Create(transform, mapDataStruct, data);
                    }
                    else
                    {
                        MapDataStruct mapDataStruct = mapObjectDataDictionary[data.id];
                        Create(transform, mapDataStruct, data);
                    }
                  
                }
                break;
            case 2:
                
                foreach (ButtonActivatedDoorStruct data in map.mapButtonActivatedDoorDataList)
                {
                    MapDataStruct mapDataStruct = mapObjectDataDictionary[data.id];
                    Create(transform, mapDataStruct, data,mapEditor);
                }
                break;
            case 3:
                foreach (ExitObjStruct data in map.mapExitObjectDataList)
                {
                    MapDataStruct mapDataStruct = mapObjectDataDictionary[data.id];
                    Create(transform, mapDataStruct, data);
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
    }
    void Create(Transform transform, MapDataStruct mapDataStruct, ButtonActivatedDoorStruct data,MapEditor mapEditor)
    {
        GameObject obj = Instantiate(Resources.Load<GameObject>(mapDataStruct.path));
        ButtonActivatedDoor door = obj.GetComponent<ButtonActivatedDoor>();
        door.ButtonActivatedDoorStruct = data;
        obj.transform.SetParent(transform);

        if (!mapEditor.interactionBtnDictionary.ContainsKey(data.linkId))
        {
            mapEditor.interactionBtnDictionary[data.linkId] = new HashSet<Vector2>();
        }

        foreach (Vector2 pot in data.buttonActivatePositionList)
        {
            mapEditor.interactionBtnDictionary[data.linkId].Add(pot);
        }

        MapDataStruct mapDataStruct1 = mapObjectDataDictionary[312];

        foreach(Vector2 pot in data.leverPositionList)
        {
            GameObject leverBody = Instantiate(Resources.Load<GameObject>(mapDataStruct1.path));
            leverBody.GetComponent<LeverBody>().SetLinkDoor(pot, data.linkId, mapEditor.interactionObjectTransform);
            leverBody.transform.SetParent(mapEditor.dontSaveObjectTransform);
        }

        //todo
    }

        


    void Create(Transform transform,MapDataStruct mapDataStruct,ExitObjStruct data)
    {
        GameObject obj = Instantiate(Resources.Load<GameObject>(mapDataStruct.path));
        obj.transform.position = data.position;
        obj.transform.SetParent(transform);
        ExitPointObj door = obj.GetComponent<ExitPointObj>();
        door.SetData(data);

    }
    #endregion


    #region SAVE
    public void SaveMapData(MapEditor mapEditor)
    {
        string folderPath = Path.Combine(Application.dataPath, "Resources/MapDat");
        if (mapEditor.mapEditorType == MapEditorType.New)
        {
            string path = Path.Combine(folderPath, $"{mapEditor.mapID}.json");
            bool fileExists = File.Exists(path);
            while (fileExists)
            {
                int num = 1;
                path = Path.Combine(folderPath, $"{mapEditor.mapID}{num}.json");
                if (!File.Exists(path))
                {
                    mapEditor.mapID = $"{mapEditor.mapID}{num}";
                    fileExists = false;
                }

                num++;
            }
            CreateJsonFile(mapEditor, folderPath);
        }
        else
        {
            CreateJsonFile(mapEditor, folderPath);
        }

    }

    async void CreateJsonFile(MapEditor mapEditor, string folderPath)
    {
        string filePath = "";
        mapEditor.mapTileDataList = GetTileData(mapEditor.placeMentSystem.floorTileMap);
        mapEditor.mapObjectDataList = GetList(mapEditor.objectTransform);
        mapEditor.startPosition = FindObj(mapEditor.dontSaveObjectTransform, 302).transform.position;
        Map map = new Map(new Vector2(mapEditor.width, mapEditor.height), mapEditor.mapID, mapEditor.stageLevel,mapEditor.startPosition,
            GetExitObjStructsList(mapEditor.exitDoorObjectTransform,mapEditor),
            mapEditor.mapTileDataList,
            mapEditor.mapObjectDataList,
            GetButtonActivateDoorStructList(mapEditor),
            mapEditor.cellSize,0, await CurrentMapScreenShot(mapEditor)) ;
        string json = JsonUtility.ToJson(map, true);
        //todo 0417
        if(mapEditor.mapType == MapType.Main)
        {
            filePath = Path.Combine(folderPath,$"{mapEditor.mapType}/{mapEditor.stageLevel}");
            if (Directory.Exists(filePath))
            {
                filePath = Path.Combine(filePath, $"{map.mapID}.json");
            }
            else
            {
                Directory.CreateDirectory(filePath);
                filePath = Path.Combine(filePath, $"{map.mapID}.json");
            }
            
        }
        else
        {
            filePath = Path.Combine(folderPath, $"{mapEditor.mapType}/{map.mapID}.json");
        }
        //todo 0417

        File.WriteAllText(filePath, json);
        UnityEditor.AssetDatabase.Refresh();
    }

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
        foreach (Transform cur in transform)
        {
            cur.GetComponent<BuildObj>().SetTileData(cur.position, cur.rotation);
            list.Add(cur.GetComponent<BuildObj>().ObjectData);
        }
        return list;
    }
    List<ButtonActivatedDoorStruct> GetButtonActivateDoorStructList(MapEditor mapEditor)
    {
        List<ButtonActivatedDoorStruct> list = new();

        foreach(Transform cur in mapEditor.dontSaveObjectTransform)
        {
            if (cur.GetComponent<ButtonActivated>())
            {
                cur.GetComponent<ButtonActivated>().LinkDoor();
            }else if (cur.GetComponent<LeverBody>())
            {
                cur.GetComponent<LeverBody>().LinkDoor();
            }
            
        }

        foreach (Transform cur in mapEditor.interactionObjectTransform)
        {
            ButtonActivatedDoor curDoor = cur.GetComponent<ButtonActivatedDoor>();
            curDoor.SetTileData(cur.position, cur.rotation);
          
            list.Add(curDoor.GetButtonActivatedDoorStruct());
        }
        return list;
    }
    List<ExitObjStruct> GetExitObjStructsList(Transform transform,MapEditor mapEditor)
    {
        List<ExitObjStruct> list = new();
        int keyAmount = 0;
        foreach(Transform tr in mapEditor.objectTransform)
        {
            Debug.Log(tr.name);
            if(tr.GetComponent<BuildObj>().id == 307)
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


    GameObject FindObj(Transform transform,int id)
    {
        foreach(Transform cur in transform)
        {
            if(cur.GetComponent<BuildObj>().id == id)
            {
                return cur.gameObject;
            }
        }
        return null;
    }
    #endregion


    #region Util
    private Task<byte[]> CurrentMapScreenShot(MapEditor mapEditor)
    {

        if (mapEditor.screenShotCamera == null)
        {
            mapEditor.screenShotCamera = Instantiate(Resources.Load<GameObject>("Prefabs/MapEditor/ScreenShotCamera"));
        }


        GameObject camera = mapEditor.screenShotCamera;

        Vector2 startPot = mapEditor.FindObj(mapEditor.dontSaveObjectTransform, 302).transform.position;
        Vector2 endPot = mapEditor.FindObj(mapEditor.exitDoorObjectTransform, 301).transform.position;

        var distance = (startPot + endPot) / 2;

        camera.gameObject.transform.position = distance;
        camera.gameObject.transform.position += new Vector3(0, 2, -1);

        Task<byte[]> encodingTask = camera.GetComponent<ScreenShotCamera>().ScreenShot();

        return encodingTask;

    }

    #endregion
}
