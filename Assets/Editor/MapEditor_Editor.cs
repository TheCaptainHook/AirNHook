
using System.Collections.Generic;
using UnityEngine;
using Debug = UnityEngine.Debug;
using UnityEngine.Tilemaps;
using UnityEditor;
using UGS;
using System.IO;
using System.Threading.Tasks;
using System.Reflection;

using System.Diagnostics;



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
    bool onLoad;
    public override void OnInspectorGUI()
    {  
        mapEditor = target as MapEditor;
       
        GUILayout.Space(10);

        EditorGUILayout.LabelField("Map Editor",GetGUIStyle_Label(Color.black,14,FontStyle.Bold));
        EditorGUILayout.HelpBox($"프로젝트 실행할때 꼭 개발자용 데이터 세이브 후 Reset 버튼 누른다음 실행하기.", MessageType.Info);
    
        GUILayout.BeginVertical(onLoad ? "Save" : "Load", new GUIStyle(GUI.skin.window));
        mapEditor.mapType = (MapType)EditorGUILayout.EnumPopup("Map Type",mapEditor.mapType);
        mapEditor.stageLevel = EditorGUILayout.IntField("Stage Level",mapEditor.stageLevel);
        mapEditor.mapID = EditorGUILayout.TextField("Map ID",mapEditor.mapID);
        if(!onLoad){
              using (new EditorGUI.DisabledScope(true))
                {
                    mapEditor.audioType = (AudioType)EditorGUILayout.EnumPopup("Audio Type",mapEditor.audioType);
                }
        }else{
            mapEditor.audioType = (AudioType)EditorGUILayout.EnumPopup("Audio Type",mapEditor.audioType);
        }
        
        GUILayout.EndVertical();

        GUILayout.Space(10);
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        
        if (GUILayout.Button("Object Create Tool",GetGUIStyle_Button(Color.green,14,FontStyle.Bold),GUILayout.Width(300),GUILayout.Height(40)))
        {
            CreateMap_Tool.ShowWindow();
        }
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GUILayout.Space(20);

        if(!Application.isPlaying){
            GUILayout.FlexibleSpace();
            GUILayout.Label("개발자 전용",GetGUIStyle_Label((Color.blue),14,FontStyle.Bold));
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Load Data(개발자전용)",GUILayout.Width(150),GUILayout.Height(30)))//TODO 0822
            {
                _Reset(mapEditor);
                LoadMap(mapEditor);
                onLoad =true;
            }

            if (GUILayout.Button("Save Data(개발자전용)",GUILayout.Width(150),GUILayout.Height(30)))
            {
                SaveMapData(mapEditor);
                onLoad = false;
                
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            if (GUILayout.Button("개발자용, 맵 새로만들 때 먼저 누르기,Init!"))
            {
                _Reset(mapEditor);
                mapEditor.Init();
                EditorApplication.ExecuteMenuItem("Window/2D/Tile Palette");
          
            }
            GUILayout.FlexibleSpace();
        }
       


        if(Application.isPlaying){
        onLoad = false;
        GUILayout.Space(10);

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label("인게임 전용");
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

            if (GUILayout.Button("Load Data(인게임용)"))
            {
                //mapEditor.LoadMap(mapEditor.mapID);
                mapEditor.MoveNextStage(mapEditor.mapID);
                
            }

            if (GUILayout.Button("Reset Interactable Object Position(인게임용)"))
            {
                mapEditor.ResetInteractableObjectPosition();
            }

        GUILayout.Space(10);
        }   
       

        GUILayout.Space(10);
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Reset",GetGUIStyle_Button(Color.red,14,FontStyle.Bold),GUILayout.Width(100),GUILayout.Height(30)))
        {
            _Reset(mapEditor);
            mapEditor.CurMap = new Map();
            mapEditor.mapID = "";
            mapEditor.mapType = MapType.Main;
            mapEditor.audioType = AudioType.None;
            mapEditor.stageLevel = 0;

            onLoad = false;      
              

        }
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        


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

            Create_StartPoint(map);
            Create_Tile(); 
            Create_Object();
     
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

    #region  Generate
    private void Create_StartPoint(Map map){
        GameObject startPoint = Instantiate(Resources.Load<GameObject>(mapObjectDataDictionary[302].path));
        mapEditor.startPositionObject = startPoint;
        startPoint.transform.position = map.startPosition;
        startPoint.transform.SetParent(mapEditor.dontSaveObjectTransform);
    }
    public void Create_Tile(){
        DrawTile(mapEditor.placeMentSystem.floorTileMap,mapEditor.CurMap.mapTileDataList);
        DrawTile(mapEditor.placeMentSystem.halfTileMap,mapEditor.CurMap.mapHalfTileDataList);
        DrawTile(mapEditor.placeMentSystem.backgroundTileMap,mapEditor.CurMap.mapBackgroundTileDataList);       
    }
    public void Create_Object(){
            Create_Object(mapEditor.CurMap.mapObjectDataList,mapEditor.objectTransform);
            Create_Object(mapEditor.CurMap.mapButtonActivatableObjectDataList,mapEditor.buttonActivatableObjectTransform);
            Create_Object(mapEditor.CurMap.mapExitObjectDataList,mapEditor.exitDoorObjectTransform);
            Create_Object(mapEditor.CurMap.buttonObjectList,mapEditor.buttonObjectTransform);
            Create_Object(mapEditor.CurMap.dialogueDataList,mapEditor.triggerDialogueTransform);
            Create_Object(mapEditor.CurMap.droneStructList,mapEditor.droneTransform);
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
      
        Map map = await CreateMap(mapEditor);

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

private async  Task<Map> CreateMap(MapEditor mapEditor){
    Map map =  new Map(new Vector2(mapEditor.width, mapEditor.height), mapEditor.mapID, mapEditor.stageLevel, mapEditor.startPosition,
            GetExitObjStructsList(mapEditor.exitDoorObjectTransform, mapEditor),
            //tile
            GetTileData(mapEditor.placeMentSystem.floorTileMap),
            GetTileData(mapEditor.placeMentSystem.halfTileMap),
            GetTileData(mapEditor.placeMentSystem.backgroundTileMap),
            //object
            GetList<ObjectData>(mapEditor.objectTransform),
            // GetButtonActivatedObjectStructList(mapEditor),
            GetList<ButtonActivatableObjectStruct>(mapEditor.buttonActivatableObjectTransform),
            // GetButtonObjectList(mapEditor),
            GetList<ButtonObjectStruct>(mapEditor.buttonObjectTransform),
            GetList<DialogueData>(mapEditor.triggerDialogueTransform),
            GetList<DroneStruct>(mapEditor.droneTransform),
            // GetDialogueList(mapEditor.triggerDialogueTransform), // todo0724
            mapEditor.cellSize, 0, await CurrentMapScreenShot(mapEditor), mapEditor.audioType);
    return  map;
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
                    TileData tileData = new TileData(tilePos,int.Parse(tile.name));
                    list.Add(tileData);
                }
            }

           
        }
        //Debug.Log($"min : {bounds.min}, max : {bounds.max}");
      
        return list;
    }

    //todo 0918
    private List<T> GetList<T>(Transform transform){
        List<T> list = new();
        foreach(Transform tr in transform){
            T data =(T)(object)tr.GetComponent<BuildObj>().GetData<T>();
            list.Add(data);
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

            // 약간의 대기시간
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


    #region  GUI
    private GUIStyle GetGUIStyle_Button(Color color,int font_Size = 12,FontStyle font_Style = FontStyle.Normal){

        return new GUIStyle(GUI.skin.button){
                normal = {textColor = color},
                fontSize = font_Size,
                fontStyle = font_Style,
                hover = {textColor = color},
                };

    }

     private GUIStyle GetGUIStyle_Label(Color color,int font_Size = 12,FontStyle font_Style = FontStyle.Normal){

        return new GUIStyle(GUI.skin.label){
                normal = {textColor = color},
                fontSize = font_Size,
                fontStyle = font_Style,
                hover = {textColor = color},
                };

    }
    #endregion
}
