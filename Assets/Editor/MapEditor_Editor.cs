
using System.Collections.Generic;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;
using UnityEngine.Tilemaps;
using UnityEditor;
using UGS;
using System.IO;
using System.Threading.Tasks;
using System.Reflection;
using System.Linq;
using System;
using UnityEngine.Rendering.Universal;
using Mono.CecilX.Cil;


/**250107 Shadow
1. 중첩 쉐도우 캐스터 문제.
    - 그림자 설정 오브젝트 일일이 설정해주기
    - 저장할때 Shadow cater 설정 값만 데이터로 저장,
    - 타일 만들고난 후 위 데이터로 그림자 오브젝트 생성.
**/


//TODO 0724 Develop code line : 435,506

[CustomEditor(typeof(MapEditor))]
public class MapEditor_Editor : Editor
{
    public Dictionary<int, MapDataStruct> mapObjectDataDictionary = new Dictionary<int, MapDataStruct>();

    MapEditor mapEditor;//TODO 0822


    #region AUDIO
    private List<string> autoCompleteOptions = new List<string>
    {
        GlobalText.TITLE_SOUND,
        GlobalText.LOBBY_SOUND,
        GlobalText.TUTORIAL_SOUND,
        GlobalText.STAGE_1_FINAL_SOUND,
        GlobalText.STAGE_1_NORMAL_SOUND
    };
    private List<string> filteredOptions = new List<string>();
    private bool showDropdown = false;
    #endregion

    public override void OnInspectorGUI()
    {
        mapEditor = target as MapEditor;

        GUILayout.Space(10);
        Draw_MainContents();
        GUILayout.Space(10);
        Draw_ToolContent();
        GUILayout.Space(20);

        if (!Application.isPlaying) {
            Draw_DevContents();
        } else {
            Draw_InGameContents();
        }
        GUILayout.Space(10);
        Draw_ResetContent();
    }
    #region  Draw

    private void Draw_MainContents() {
        EditorGUILayout.LabelField("Map Editor", GetGUIStyle_Label(Color.black, 14, FontStyle.Bold));
        EditorGUILayout.HelpBox($"프로젝트 실행할때 꼭 개발자용 데이터 세이브 후 Reset 버튼 누른다음 실행하기.", MessageType.Info);
        GUILayout.BeginVertical(mapEditor.onLoad ? "Save" : "Load", new GUIStyle(GUI.skin.window));
        mapEditor.mapType = (MapType)EditorGUILayout.EnumPopup("Map Type", mapEditor.mapType);
        // mapEditor.mapID = EditorGUILayout.TextField("Map ID",mapEditor.mapID);

        Draw_MainContents_MapId();

        if (!mapEditor.onLoad)
        {
            using (new EditorGUI.DisabledScope(true))
            {
                mapEditor.audioName = EditorGUILayout.TextField(new GUIContent("BGM", "BGM"), mapEditor.audioName);
            }
        } else {
            DrawShadow();

            mapEditor.stageLevel = EditorGUILayout.IntField("Stage Level", mapEditor.stageLevel);
            mapEditor.subMapName = EditorGUILayout.TextField(new GUIContent("Map Sub Name", "This is the sub-name for the map, but it’s okay to leave it empty."), mapEditor.subMapName);
            DrawBGMContents();
        }
        GUILayout.EndVertical();
    }

    private void DrawShadow()
    {
        GUILayout.Space(20);

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.BeginVertical("Shadow Test Container", new GUIStyle(GUI.skin.window));
        if (GUILayout.Button("그림자 생성", GUILayout.Width(150), GUILayout.Height(30)))
        {
            Tilemap map = mapEditor.placeMentSystem.floorTileMap;
            CreateShadow(map);

        }
        if (GUILayout.Button("그림자 제거", GUILayout.Width(150), GUILayout.Height(30)))
        {
            DestroyShadow(mapEditor.placeMentSystem.floorTileMap);
        }

        GUILayout.EndHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.EndVertical();

        GUILayout.Space(20);
    }

    private void Draw_MainContents_MapId() {
        Color orgCol = GUI.backgroundColor;
        if (string.IsNullOrEmpty(mapEditor.mapID)) {
            GUI.backgroundColor = Color.red;
        }
        mapEditor.mapID = EditorGUILayout.TextField(new GUIContent("Map ID", "Unique identifier for the map. This field is required."), mapEditor.mapID);
        GUI.backgroundColor = orgCol;
    }

    private void DrawBGMContents() {
        GUI.SetNextControlName("BGM");
        mapEditor.audioName = EditorGUILayout.TextField(new GUIContent("BGM", ""), mapEditor.audioName);

        if (!string.IsNullOrEmpty(mapEditor.audioName)) {
            filteredOptions = autoCompleteOptions
                   .FindAll(option => option.ToLower().Contains(mapEditor.audioName.ToLower()));
            showDropdown = filteredOptions.Count > 0;
        } else if (GUI.GetNameOfFocusedControl() == "BGM" && string.IsNullOrEmpty(mapEditor.audioName)) {
            filteredOptions = new(autoCompleteOptions);
            showDropdown = filteredOptions.Count > 0;
        } else {
            showDropdown = false;
        }
        if (showDropdown) DrawAudioDropDown();
    }

    private void DrawAudioDropDown() {
        GUILayout.BeginVertical("Box");
        foreach (string option in filteredOptions)
        {
            if (GUILayout.Button(option, GUILayout.ExpandWidth(true)))
            {
                // 선택한 값을 TextField에 반영
                mapEditor.audioName = option;
                // 드롭다운 숨김
                showDropdown = false;

                GUI.FocusControl(null);
                Repaint();
            }
        }
        GUILayout.EndVertical();
    }

    private void Draw_ToolContent() {
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        if (GUILayout.Button("Object Create Tool", GetGUIStyle_Button(Color.green, 14, FontStyle.Bold), GUILayout.Width(300), GUILayout.Height(40)))
        {
            CreateMap_Tool.ShowWindow();
        }
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
    }
    private void Draw_DevContents() {
        GUILayout.FlexibleSpace();
        GUILayout.BeginHorizontal();

        GUILayout.FlexibleSpace();
        GUILayout.Label("개발자 전용", GetGUIStyle_Label((Color.white), 14, FontStyle.Bold));
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Load Data(개발자전용)", GUILayout.Width(150), GUILayout.Height(30)))
        {
            if (!Check_DuplicateMapId(mapEditor.mapID)) {
                EditorUtility.DisplayDialog(
                   "Map not found",
                   "The map does not exist. Please try again",
                   "OK"
               );
                return;
            }

            _Reset(mapEditor);
            LoadMap(mapEditor);
            mapEditor.onLoad = true;
            mapEditor.isLoadMap = true;
        }

        if (GUILayout.Button("Save Data(개발자전용)", GUILayout.Width(150), GUILayout.Height(30)))
        {

            if (!mapEditor.isLoadMap && Check_DuplicateMapId(mapEditor.mapID)) {
                EditorUtility.DisplayDialog(
                    "Duplicate ID Detected",
                    "The Map ID already exists. Please use a different MapID",
                    "OK"
                );
                return;
            }

            try {
                SaveMapData(mapEditor);
            } catch (Exception ex) {
                Debug.Log(ex);
            }


        }
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("개발자용, 맵 새로만들 때 먼저 누르기,Init!", GUILayout.Width(300), GUILayout.Height(30)))
        {
            _Reset(mapEditor);
            mapEditor.Init();
            EditorApplication.ExecuteMenuItem("Window/2D/Tile Palette");
            mapEditor.onLoad = true;

        }
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.FlexibleSpace();
    }
    //Check for duplicate Map ID TODO 1116
    private bool Check_DuplicateMapId(string mapId) {
        string path = Path.Combine(Application.dataPath, $"Resources/MapDat/{mapEditor.mapType}");
        string[] jsonFiles = Directory.GetFiles(path, "*.json", SearchOption.AllDirectories);
        List<string> fileNames = jsonFiles.Select(file => Path.GetFileNameWithoutExtension(file)).ToList();

        if (fileNames.Contains(mapId)) {
            return true;
        }

        return false;

    }
    private void Draw_InGameContents() {
        mapEditor.onLoad = false;
        GUILayout.Space(10);

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label("인게임 전용", GetGUIStyle_Label((Color.white), 14, FontStyle.Bold));
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Load Data(인게임용)", GUILayout.Width(300), GUILayout.Height(30)))
        {
            //mapEditor.LoadMap(mapEditor.mapID);
            mapEditor.MoveNextStage(mapEditor.mapID);

        }
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Reset Interactable Object Position(인게임용)", GUILayout.Width(300), GUILayout.Height(30)))
        {
            mapEditor.ResetInteractableObjectPosition();
        }
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GUILayout.Space(10);
    }
    private void Draw_ResetContent() {
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Reset", GetGUIStyle_Button(Color.red, 14, FontStyle.Bold), GUILayout.Width(100), GUILayout.Height(30)))
        {
            _Reset(mapEditor);
            mapEditor.CurMap = new Map();
            mapEditor.mapID = "";
            mapEditor.mapType = MapType.Main;
            mapEditor.audioName = "";
            mapEditor.stageLevel = 0;

            mapEditor.subMapName = "";

            mapEditor.onLoad = false;
            mapEditor.isLoadMap = false;

        }
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
    }
    #endregion

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
                mapObjectDataDictionary.Add(value.id, new MapDataStruct(value.id, value.name, value.type, value.path));
            }

        }
        //Object Data
        foreach (var value in MapObjectData.ObjectData.ObjectDataList)
        {
            if (!mapObjectDataDictionary.ContainsKey(value.id))
            {
                mapObjectDataDictionary.Add(value.id, new MapDataStruct(value.id, value.name, value.type, value.path));
            }

        }
        foreach (var value in MapObjectData.SceneData.SceneDataList)
        {
            if (!mapObjectDataDictionary.ContainsKey(value.id))
            {

                mapObjectDataDictionary.Add(value.id, new MapDataStruct(value.id, value.name, value.type, value.path));
            }
        }
        foreach (var value in MapObjectData.OtherData.OtherDataList)
        {
            if (!mapObjectDataDictionary.ContainsKey(value.id))
            {
                var type = GetObjectType(value.type);
                mapObjectDataDictionary.Add(value.id, new MapDataStruct(value.id, value.name, type.type, value.path, type.subType));
            }

        }
        foreach (var value in MapObjectData.BackGroundData.BackGroundDataList)
        {
            if (!mapObjectDataDictionary.ContainsKey(value.id))
            {
                mapObjectDataDictionary.Add(value.id, new MapDataStruct(value.id, value.name, value.type, value.path));
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

            MapEditorFieldSetting(map);
        }
    }


    private void MapEditorFieldSetting(Map map)
    {
        mapEditor.stageLevel = map.stageLevel;
        mapEditor.mapID = map.mapID;
        mapEditor.audioName = map.audioName;
        mapEditor.startPosition = map.startPosition;
        mapEditor.nextMapId = map.nextMapId;
        mapEditor.subMapName = map.subMapName;
    }
    #region  Generate
    private void Create_StartPoint(Map map) {
        GameObject startPoint = Instantiate(Resources.Load<GameObject>(mapObjectDataDictionary[302].path));
        mapEditor.startPositionObject = startPoint;
        startPoint.transform.position = map.startPosition;
        startPoint.transform.SetParent(mapEditor.dontSaveObjectTransform);
    }
    public void Create_Tile() {
        DrawTile(mapEditor.placeMentSystem.floorTileMap, mapEditor.CurMap.mapTileDataList);
        DrawTile(mapEditor.placeMentSystem.halfTileMap, mapEditor.CurMap.mapHalfTileDataList);
        DrawTile(mapEditor.placeMentSystem.backgroundTileMap, mapEditor.CurMap.mapBackgroundTileDataList);
        DrawTile(mapEditor.placeMentSystem.ropeTileMap, mapEditor.CurMap.mapRopeTileDataList);
        DrawTile(mapEditor.placeMentSystem.accessoryTileMap, mapEditor.CurMap.mapAccessoryTIleDataList);
    }
    public void Create_Object() {
        Create_Object(mapEditor.CurMap.mapObjectDataList, mapEditor.objectTransform);
        Create_Object(mapEditor.CurMap.mapBackgroundObjectList, mapEditor.backgroundObjectContainer);
        // Create_Object(mapEditor.CurMap.mapOtherObjectList,mapEditor.otherContainer);
        Create_OtherObject(mapEditor.CurMap.mapOtherObjectList);
        Create_Object(mapEditor.CurMap.mapButtonActivatableObjectDataList, mapEditor.buttonActivatableObjectTransform);
        Create_Object(mapEditor.CurMap.mapExitObjectDataList, mapEditor.exitDoorObjectTransform);
        Create_Object(mapEditor.CurMap.buttonObjectList, mapEditor.buttonObjectTransform);
        Create_Object(mapEditor.CurMap.dialogueDataList, mapEditor.triggerDialogueTransform);
        Create_Object(mapEditor.CurMap.droneStructList, mapEditor.droneTransform);
        Create_Object(mapEditor.CurMap.collectableObjectStructList, mapEditor.collectableContainer);
    }

    private void DrawTile(Tilemap tileMap, List<TileData> list) {
        foreach (TileData data in list)
        {
            MapDataStruct mapDataStruct = mapObjectDataDictionary[data.id];
            tileMap.SetTile(data.position, Resources.Load<TileBase>(mapDataStruct.path));
            mapEditor.placeMentSystem.tileDic[data.position] = data.id;
        }
  
    }
    public void Create_OtherObject(List<ObjectData> list) {
        MapDataStruct mapDataStruct;
        foreach (ObjectData data in list) {
            mapDataStruct = mapObjectDataDictionary[data.id];
            Create_OtherObject(mapDataStruct, data);
        };
    }
    private void Create_OtherObject(MapDataStruct mapDataStruct, ObjectData data) {
        string[] tags = mapDataStruct.name.Split("_");
        Transform curTr = mapEditor.otherContainer;
        OtherContainer otherContainer = curTr.GetComponent<OtherContainer>();
        for (int i = 0; i < tags.Length - 1; i++) {
            Transform transform = curTr.Find(tags[i]);
            if (transform == null) {
                transform = new GameObject(tags[i]).transform;
                transform.SetParent(curTr);
            }
            curTr = transform;
        }

        otherContainer.SetGroup(curTr);
        Create(curTr, mapDataStruct, data);

    }
    public void Create_Object<T>(List<T> list, Transform transform) {
        MapDataStruct mapDataStruct;
        foreach (T data in list) {
            var isField = typeof(T).GetField("id", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            if (isField != null) {
                var value = isField.GetValue(data);
                if (value is int intValue) {
                    mapDataStruct = mapObjectDataDictionary[intValue];
                    Create(transform, mapDataStruct, data);
                }

            }
        }
    }

    void Create<T>(Transform transform, MapDataStruct mapDataStruct, T data) {
        try {
            GameObject obj = Instantiate(Resources.Load<GameObject>(mapDataStruct.path));
            BuildObj buildObj = obj.GetComponent<BuildObj>();
            buildObj.SetData<T>(data);
            buildObj.Editor_Setting(mapEditor);

            obj.transform.SetParent(transform);
        } catch (Exception ex) {
            Debug.Log($"{ex},{mapDataStruct.id}");
        }

    }

    TextAsset GetTextAsset(MapType mapType, string id)
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

    #region Shadow
    //----------------------------------------------------------------------------------------------------------------------Shadow 250107
    private void DestroyShadow(Tilemap map)
    {
        foreach(Transform tr in map.gameObject.transform)
        {
            Undo.DestroyObjectImmediate(tr.gameObject);
        }
    }
    private async void CreateShadow(Tilemap map)
    {
        CompositeCollider2D tilemapCollider = map.gameObject.GetComponent<CompositeCollider2D>();
        await CreateShadowCastersAsync(tilemapCollider);
        //CreateShadowCastersAsync(tilemapCollider);
    }



    public async Task CreateShadowCastersAsync(CompositeCollider2D tilemapCollider)
    //public void CreateShadowCastersAsync(CompositeCollider2D tilemapCollider)
    {
        FieldInfo meshField = typeof(ShadowCaster2D).GetField("m_Mesh", BindingFlags.NonPublic | BindingFlags.Instance);
        FieldInfo shapePathField = typeof(ShadowCaster2D).GetField("m_ShapePath", BindingFlags.NonPublic | BindingFlags.Instance);
        FieldInfo shapePathHashField = typeof(ShadowCaster2D).GetField("m_ShapePathHash", BindingFlags.NonPublic | BindingFlags.Instance);
        MethodInfo generateShadowMeshMethod = typeof(ShadowCaster2D)
                                        .Assembly
                                        .GetType("UnityEngine.Rendering.Universal.ShadowUtility")
                                        .GetMethod("GenerateShadowMesh", BindingFlags.Public | BindingFlags.Static);

        if (meshField == null || shapePathField == null || shapePathHashField == null || generateShadowMeshMethod == null)
        {
            Debug.LogError("Reflection 실패");
            return;
        }

        // 기존 ShadowCaster 삭제 (옵션)
        foreach(Transform tr in tilemapCollider.gameObject.transform)
        {
            Undo.DestroyObjectImmediate(tr.gameObject);
        }

        GameObject shadowContainer = new GameObject("Shadow_Container");
        shadowContainer.transform.SetParent(tilemapCollider.transform);

        int pathCount = tilemapCollider.pathCount;
        Debug.Log(pathCount);
        for (int i = 0; i < pathCount; i++)
        {
            Vector2[] pathVertices = new Vector2[tilemapCollider.GetPathPointCount(i)];
            foreach(var  pathVertex in pathVertices)
            {
                Debug.Log(pathVertex);
            }
            tilemapCollider.GetPath(i, pathVertices);

            // ShadowCaster 생성
            GameObject shadowCaster = new GameObject("shadow_caster_" + i);
            shadowCaster.transform.SetParent(shadowContainer.transform);

            ShadowCaster2D shadowCasterComponent = shadowCaster.AddComponent<ShadowCaster2D>();
            shadowCasterComponent.selfShadows = true;

            Vector3[] testPath = new Vector3[pathVertices.Length];
            for (int j = 0; j < pathVertices.Length; j++)
            {
                testPath[j] = pathVertices[j];
            }

            shapePathField.SetValue(shadowCasterComponent, testPath);
            shapePathHashField.SetValue(shadowCasterComponent, UnityEngine.Random.Range(int.MinValue, int.MaxValue));
            meshField.SetValue(shadowCasterComponent, new Mesh());
            generateShadowMeshMethod.Invoke(shadowCasterComponent,
                new object[] { meshField.GetValue(shadowCasterComponent), shapePathField.GetValue(shadowCasterComponent) });

            // 작업 진행을 10번 단위로 나누어 UI 업데이트 및 멈춤 제공
            if (i % 10 == 0)
            {
                await Task.Yield(); // 다른 작업과 병렬 실행 가능
            }
        }
    }

    #endregion

    //----------------------------------------------------------------------------------------------------------------------Shadow 250107
    #endregion

    #endregion


    #region SAVE
    public async void SaveMapData(MapEditor mapEditor)
    {
        string folderPath;
        try{
            EditorUtility.DisplayProgressBar("Saving Map Data","Initializing save process...",0f);
            folderPath = Path.Combine(Application.dataPath, "Resources/MapDat");
            EditorUtility.DisplayProgressBar("Saving Map Data","Preparing map data...",0.1f);

            await CreateJsonFile(mapEditor, folderPath);

            EditorUtility.ClearProgressBar();
            //mapEditor.onLoad = false;
        }catch(Exception ex){
            Debug.Log(ex);
            EditorUtility.ClearProgressBar();
            EditorUtility.DisplayDialog("Error",$"Please check the following.\n 1. Did you press the init button before creating the map.\n2.Is the MapID field empty?","Confirm");
        }
    }
    
    async Task CreateJsonFile(MapEditor mapEditor, string folderPath)
    {
        try{
            EditorUtility.DisplayProgressBar("Saving Map Data", "Gathering map information...", 0.4f);
            
            string filePath = "";
            
            mapEditor.startPosition = FindObj(mapEditor.dontSaveObjectTransform, 302).transform.position;
            Map map = await CreateMap(mapEditor);
            EditorUtility.DisplayProgressBar("Saving Map Data", "Serializing data to JSON...", 0.6f);
            
            string json = JsonUtility.ToJson(map, true);
            EditorUtility.DisplayProgressBar("Saving Map Data", "Writing data to file...", 0.8f);
            if(mapEditor.mapType == MapType.Main)
            {
                filePath = CheckDirectory(folderPath,map);
            }else
            {
                filePath = Path.Combine(folderPath, $"{mapEditor.mapType}/{map.mapID}.json");
            }
            File.WriteAllText(filePath, json);
            AssetDatabase.Refresh();
        }finally{
             EditorUtility.DisplayProgressBar("Saving Map Data", "Finalizing...", 0.9f);
        }
    }

    private string CheckDirectory(string folderPath,Map map){
        string filePath = "";
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

            return filePath;

    }
        //TestCode TOdo 0807
        //map.mapSize = new Vector2(
        //    map.mapTileDataList[0].position.x,
        //     map.mapTileDataList[map.mapTileDataList.Count - 1].position.x);
        // var poss = map.GetStartEndPosition();


        //else
        //{
        //    filePath = Path.Combine(folderPath, $"{mapEditor.mapType}/{map.mapID}.json");
        //}

private async Task<Map> CreateMap(MapEditor mapEditor){
    Map map =  new Map(new Vector2(mapEditor.width, mapEditor.height), mapEditor.mapID, mapEditor.subMapName,GetNextMapId(),mapEditor.stageLevel, mapEditor.startPosition,
            GetExitObjStructsList(mapEditor.exitDoorObjectTransform, mapEditor),
            //tile
            
            GetTileData(mapEditor.placeMentSystem.floorTileMap),

            GetTileData(mapEditor.placeMentSystem.halfTileMap),
            GetTileData(mapEditor.placeMentSystem.backgroundTileMap),
            GetTileData(mapEditor.placeMentSystem.ropeTileMap),
            GetTileData(mapEditor.placeMentSystem.accessoryTileMap),
            //object
            GetList<ObjectData>(mapEditor.objectTransform),
            GetList<ObjectData>(mapEditor.backgroundObjectContainer),
            GetList_Depth<ObjectData>(mapEditor.otherContainer.GetComponent<OtherContainer>()),
            GetList<ButtonActivatableObjectStruct>(mapEditor.buttonActivatableObjectTransform),
            GetList<ButtonObjectStruct>(mapEditor.buttonObjectTransform),
            GetList<DialogueData>(mapEditor.triggerDialogueTransform),
            GetList<DroneStruct>(mapEditor.droneTransform),
            GetList<CollectableObjectStruct>(mapEditor.collectableContainer),
            mapEditor.cellSize, 0, await CurrentMapScreenShot(mapEditor), mapEditor.audioName);
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
    private List<T> GetList_Depth<T>(OtherContainer otherContainer){
        return otherContainer.GetTypeObject<T>();
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
            if(cur.TryGetComponent(out ExitPointObj component)){
                component.condition_KeyAmount = keyAmount;
                list.Add(component.GetExitObjectStruct());
            }

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

    private string GetNextMapId(){
        try{
            ExitPointObj eObj = FindObj<ExitPointObj>(mapEditor.exitDoorObjectTransform);
            return eObj.nextMapId;
        }catch{
            Debug.Log("Exception");
            return "";
        }
        
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

    private (ObjectType type,string[] subType) GetObjectType(string objectType){
        string[] arr = objectType.Split("/");
        if(arr.Length>1){
            return(GetType(arr[0]),arr.Skip(1).ToArray());
            
        }else{
            return (GetType(arr[0]),null);
        }
    }

    private ObjectType GetType(string type){
        switch(type){
            case "Tile":
            return ObjectType.Tile;
            case "Object":
            return ObjectType.Object;
            case "N_Object":
            return ObjectType.N_Object;
            case "Background":
            return ObjectType.Background;
            case "Other":
            default :
            return ObjectType.Other;
            
            
        }
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
