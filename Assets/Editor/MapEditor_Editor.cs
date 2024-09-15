
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;
using UGS;
using System.IO;
using System.Threading.Tasks;
using System.Reflection;

//TODO 0724 Develop code line : 435,506

[CustomEditor(typeof(MapEditor))]
public class MapEditor_Editor : Editor
{
    // public Dictionary<int, MapDataStruct> mapTileDataDictionary = new Dictionary<int, MapDataStruct>();
    public Dictionary<int, MapDataStruct> mapObjectDataDictionary = new Dictionary<int, MapDataStruct>();
    // public Dictionary<int, MapDataStruct> mapSceneDataDictionary = new Dictionary<int, MapDataStruct>();
    // public Dictionary<int, MapDataStruct> mapBackgroundDataDictionary = new Dictionary<int, MapDataStruct>();
    // public Dictionary<int, MapDataStruct> mapOtherDataDictionary = new Dictionary<int, MapDataStruct>();

    MapEditor mapEditor;//TODO 0822

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        mapEditor = target as MapEditor;

        GUILayout.Space(10);

        EditorGUILayout.LabelField("Map Editor------------------------------------", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox($"프로젝트 실행할때 꼭 개발자용 데이터 세이브 후 Reset 버튼 누른다음 실행하기.", MessageType.Info);

        if (GUILayout.Button("Load Data(인게임용)"))
        {
            mapEditor.LoadMap(mapEditor.mapID);
            
        }
        GUILayout.Space(10);

          if (GUILayout.Button("Reset Interactable Object Position(인게임용)"))
        {
            mapEditor.ResetInteractableObjectPosition();
        }
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

        if (GUILayout.Button("Load Data(개발자전용)")) //TODO 0822
        {
            _Reset(mapEditor);
            LoadMap(mapEditor);

        }

        if (GUILayout.Button("Save Data(개발자전용)"))
        {
            SaveMapData(mapEditor);
            
        }

        GUILayout.Space(10);

        if (GUILayout.Button("Reset"))
        {
            _Reset(mapEditor);
            mapEditor.CurMap = new Map();
            mapEditor.audioType = AudioType.None;

        }


      

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

       

    }
    void UGS_MapDataLoad()
    {
        UnityGoogleSheet.LoadAllData();
        foreach (var value in MapObjectData.TileData.TileDataList)
        {
            if (!mapObjectDataDictionary.ContainsKey(value.id))
            {
                mapObjectDataDictionary.Add(value.id, new MapDataStruct(value.id,value.name, value.type, value.path));
            }

        }
        //Object Data
        foreach (var value in MapObjectData.ObjectData.ObjectDataList)
        {
            if (!mapObjectDataDictionary.ContainsKey(value.id))
            {
                mapObjectDataDictionary.Add(value.id, new MapDataStruct(value.id,value.name, value.type, value.path));
            }

        }
        foreach (var value in MapObjectData.SceneData.SceneDataList)
        {
            if (!mapObjectDataDictionary.ContainsKey(value.id))
            {

                mapObjectDataDictionary.Add(value.id, new MapDataStruct(value.id,value.name, value.type, value.path));
            }
        }
        foreach (var value in MapObjectData.OtherData.OtherDataList)
        {
            if (!mapObjectDataDictionary.ContainsKey(value.id))
            {
                mapObjectDataDictionary.Add(value.id, new MapDataStruct(value.id,value.name, value.type, value.path));
            }

        }
        foreach (var value in MapObjectData.BackGroundData.BackGroundDataList)
        {
            if (!mapObjectDataDictionary.ContainsKey(value.id))
            {
                mapObjectDataDictionary.Add(value.id, new MapDataStruct(value.id,value.name, value.type, value.path));
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

            ////start Point
            GameObject startPoint = Instantiate(Resources.Load<GameObject>(mapObjectDataDictionary[302].path));
            mapEditor.startPositionObject = startPoint;
            startPoint.transform.position = map.startPosition;
            startPoint.transform.SetParent(mapEditor.dontSaveObjectTransform);
            //start Point

            Create_Tile(); 
            Create_Object(mapEditor.CurMap.mapObjectDataList,mapEditor.objectTransform);
            Create_Object(mapEditor.CurMap.mapButtonActivatableObjectDataList,mapEditor.buttonActivatableObjectTransform);
            Create_Object(mapEditor.CurMap.mapExitObjectDataList,mapEditor.exitDoorObjectTransform);
            Create_Object(mapEditor.CurMap.buttonObjectList,mapEditor.buttonObjectTransform);
            Create_Object(mapEditor.CurMap.dialogueDataList,mapEditor.triggerDialogueTransform);
            // mapEditor.Create_Object(mapEditor.objectTransform);
            // mapEditor.Create_Object(mapEditor.CurMap.mapButtonActivatableObjectDataList,mapEditor.buttonActivatableObjectTransform);
            // mapEditor.Create_Object(mapEditor.CurMap.mapExitObjectDataList,mapEditor.exitDoorObjectTransform);
            // mapEditor.Create_Object(mapEditor.CurMap.buttonObjectList,mapEditor.buttonObjectTransform);
            // mapEditor.Create_Object(mapEditor.CurMap.dialogueDataList,mapEditor.triggerDialogueTransform);

            mapEditor.stageLevel = mapEditor.CurMap.stageLevel;
            mapEditor.mapID = mapEditor.CurMap.mapID;
            mapEditor.audioType = mapEditor.CurMap.audioType;
            mapEditor.startPosition = mapEditor.CurMap.startPosition;
        }
        else
        {
            Debug.Log("Map not found");
        }

        ///
        /// 플로어 타일 맵의 바운드셀로 최솟값 최댓값 알 수 있음.
        ///
        Debug.Log("Lode Complete");
    }



     public void Create_Tile(){
        DrawTile(mapEditor.placeMentSystem.floorTileMap,mapEditor.CurMap.mapTileDataList);
        DrawTile(mapEditor.placeMentSystem.halfTileMap,mapEditor.CurMap.mapHalfTileDataList);
        DrawTile(mapEditor.placeMentSystem.backgroundTileMap,mapEditor.CurMap.mapBackgroundTileDataList);       
    }
    private void DrawTile(Tilemap tileMap,List<TileData> list){
         foreach (TileData data in list)
         {
            MapDataStruct mapDataStruct = mapObjectDataDictionary[data.id];
            tileMap.SetTile(data.position, Resources.Load<TileBase>(mapDataStruct.path));
            mapEditor.placeMentSystem.tileDic[data.position] = data.id;        
         }
    }


    public void Create_Object<T>(List<T> list ,Transform transform){
        MapDataStruct mapDataStruct;
        foreach(T data in list){
            var isField = typeof(T).GetField("id",BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            if(isField != null){
                var value = isField.GetValue(data);
                if(value is int intValue){
                    mapDataStruct = mapObjectDataDictionary[intValue];
                    Create(transform,mapDataStruct,data);
                }
               
            }
   
        }
    }
      void Create<T>(Transform transform,MapDataStruct mapDataStruct,T data){
        GameObject obj = Instantiate(Resources.Load<GameObject>(mapDataStruct.path));
        BuildObj buildObj = obj.GetComponent<BuildObj>();
        buildObj.SetData(data);
        buildObj.Editor_Setting(mapEditor.buttonActivatableObjectTransform);
        
        obj.transform.SetParent(transform);
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

        //mapEditor.interactionBtnDictionary = new(); //todo 0412
        //  CreateObj(map, mapEditor.placeMentSystem, 0);
        //     CreateObj(map, mapEditor.placeMentSystem, 1,mapEditor.objectTransform);
        //     CreateObj(map, mapEditor.placeMentSystem, 2,mapEditor.buttonActivatableObjectTransform);
        //     CreateObj(map, mapEditor.placeMentSystem, 3,mapEditor.exitDoorObjectTransform);
        //     CreateObj(map, mapEditor.placeMentSystem, 4,mapEditor.buttonObjectTransform);
        //     CreateObj(map, mapEditor.placeMentSystem, 5,mapEditor.triggerDialogueTransform);
      
      


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
            case MapType.Fork:
                return Resources.Load<TextAsset>($"MapDat/{mapType}/{id}");
        }


        return null;
    }

    #endregion


    #region SAVE
    public void SaveMapData(MapEditor mapEditor)
    {
        string folderPath = Path.Combine(Application.dataPath, "Resources/MapDat");

        CreateJsonFile(mapEditor, folderPath);



    }

    async void CreateJsonFile(MapEditor mapEditor, string folderPath)
    {
        string filePath = "";
        mapEditor.startPosition = FindObj(mapEditor.dontSaveObjectTransform, 302).transform.position;
        Map map = new Map(new Vector2(mapEditor.width, mapEditor.height), mapEditor.mapID, mapEditor.stageLevel, mapEditor.startPosition,
            GetExitObjStructsList(mapEditor.exitDoorObjectTransform, mapEditor),
            //tile
            GetTileData(mapEditor.placeMentSystem.floorTileMap),
            GetTileData(mapEditor.placeMentSystem.halfTileMap),
            GetTileData(mapEditor.placeMentSystem.backgroundTileMap),
            //object
            GetList(mapEditor.objectTransform),
            GetButtonActivatedObjectStructList(mapEditor),
            GetButtonObjectList(mapEditor),
            GetDialogueList(mapEditor.triggerDialogueTransform), // todo0724
            mapEditor.cellSize, 0, await CurrentMapScreenShot(mapEditor), mapEditor.audioType);

        //TestCode TOdo 0807
        //map.mapSize = new Vector2(
        //    map.mapTileDataList[0].position.x,
        //     map.mapTileDataList[map.mapTileDataList.Count - 1].position.x);
        var poss = map.GetStartEndPosition();
        Debug.Log($"Start : {poss.start},End : {poss.end}");

        string json = JsonUtility.ToJson(map, true);

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
            
        }else
        {
            filePath = Path.Combine(folderPath, $"{mapEditor.mapType}/{map.mapID}.json");
        }

       
        File.WriteAllText(filePath, json);
        UnityEditor.AssetDatabase.Refresh();
    }

        //else
        //{
        //    filePath = Path.Combine(folderPath, $"{mapEditor.mapType}/{map.mapID}.json");
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
        //Debug.Log($"min : {bounds.min}, max : {bounds.max}");
      
        return list;
    }




    List<ObjectData> GetList(Transform transform)
    {
        List<ObjectData> list = new();
        foreach (Transform cur in transform)
        {

            //cur.GetComponent<BuildObj>().SetTileData(cur.position, cur.rotation);
            cur.GetComponent<BuildObj>().SetTileData();
            list.Add(cur.GetComponent<BuildObj>().ObjectData);
        }
        return list;
    }

    List<DialogueData> GetDialogueList(Transform transform)
    {
        List<DialogueData> list = new();
        foreach(Transform tr in transform)
        {
            if(tr.TryGetComponent(out Trigger_Dialogue component))
            {
                list.Add(component.GetDialogueData());
            }
        }
        return list;
    }

    List<ButtonActivatableObjectStruct> GetButtonActivatedObjectStructList(MapEditor mapEditor)
    {
        List<ButtonActivatableObjectStruct> list = new();

        foreach (Transform cur in mapEditor.buttonActivatableObjectTransform)
        {
            list.Add(cur.GetComponent<BuildObj>().GetData<ButtonActivatableObjectStruct>());

        }
        return list;
    }

    List<ButtonObjectStruct> GetButtonObjectList(MapEditor mapEditor)
    {
        List<ButtonObjectStruct> list = new();
        foreach (Transform cur in mapEditor.buttonObjectTransform)
        {
            list.Add(cur.GetComponent<BuildObj>().GetData<ButtonObjectStruct>());
        }
        return list;
    }

    List<ExitObjStruct> GetExitObjStructsList(Transform transform,MapEditor mapEditor)
    {
        List<ExitObjStruct> list = new();
        int keyAmount = 0;
        foreach(Transform tr in mapEditor.objectTransform)
        {
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
    private async Task<byte[]> CurrentMapScreenShot(MapEditor mapEditor)
    {

        if (mapEditor.screenShotCamera == null)
        {
            mapEditor.screenShotCamera = Instantiate(Resources.Load<GameObject>("Prefabs/MapEditor/ScreenShotCamera"));
        }

        Camera camera = mapEditor.screenShotCamera.GetComponent<Camera>();

        Vector2 startPot = mapEditor.FindObj(mapEditor.dontSaveObjectTransform, 302).transform.position;
        Vector2 endPot = mapEditor.FindObj(mapEditor.exitDoorObjectTransform, 301).transform.position;

        var distance = (startPot + endPot) / 2;

        camera.gameObject.transform.position = distance;
        camera.gameObject.transform.position += new Vector3(0, 2, -1);

        //0910

        // camera.GetComponent<Camera>().orthographicSize = CalculateMinimumOrthographicSize(startPot,endPot,camera);
        
        //0910

        await CalculateMinimumOrthographicSize(startPot,endPot,camera);

        Task<byte[]> encodingTask = camera.GetComponent<ScreenShotCamera>().ScreenShot();
        return await encodingTask;
    }


     private async Task CalculateMinimumOrthographicSize(Vector2 pointA, Vector2 pointB,Camera camera)
    {
            // 최대, 최소 orthographic size를 설정
        float minSize = camera.orthographicSize;   // 현재 카메라 사이즈를 최소로 설정
        float maxSize = 100f;  // 임의의 큰 값으로 초기 최대 크기 설정 (적절히 조정 가능)
        float tolerance = 0.01f; // 원하는 오차 범위

        // 이진 탐색으로 최적의 orthographic size 찾기
        while (maxSize - minSize > tolerance)
        {
            float midSize = (minSize + maxSize) / 2;
            camera.orthographicSize = midSize;

            bool pointAVisible = IsPointInViewport(camera, pointA);
            bool pointBVisible = IsPointInViewport(camera, pointB);

            if (pointAVisible && pointBVisible)
            {
                // 두 좌표가 모두 보이면 사이즈를 더 줄여도 되는지 확인
                maxSize = midSize;
            }
            else
            {
                // 두 좌표 중 하나가 보이지 않으면 더 큰 사이즈가 필요
                minSize = midSize;
            }

            // 약간의 대기시간 추가
            await Task.Yield(); // 비동기 작업이므로 프레임 차단을 피하기 위한 대기
        }

        // 최종적으로 최소 사이즈로 설정
        camera.orthographicSize = maxSize;

    }

     bool IsPointInViewport(Camera cam, Vector3 point)
    {
        // 월드 좌표를 뷰포트 좌표로 변환 (뷰포트 좌표는 (0, 0) ~ (1, 1) 사이의 값)
        Vector3 viewportPos = cam.WorldToViewportPoint(point);

        // 뷰포트 좌표가 0 ~ 1 범위 내에 있는지 확인
        return viewportPos.x >= .05f && viewportPos.x <= .95 && viewportPos.y >= .05 && viewportPos.y <= .95;
    }
  
    #endregion
}
