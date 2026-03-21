
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
using static UnityEngine.Rendering.Universal.Light2D;

using TileData = ANH_MapEditor.TileData;
using MapType = ANH_MapEditor.MapType;
using FunkyCode;

using SceneView = UnityEditor.SceneView;

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
        GlobalText.TRACK_RACE_SOUND,
        GlobalText.TRACK_HACKERS_SOUND,
        GlobalText.TRACK_LEGEND_SOUND,
        GlobalText.TRACK_LASTSTOP_SOUND
    };
    private List<string> filteredOptions = new List<string>();
    private bool showDropdown = false;
    #endregion


    //Light Field
    private int sortingLayerMask = 0; // 비트 플래그 값
    private bool[] selectedLayers;   // 선택된 레이어 상태
    private string[] sortingLayerNames;  
    private int[] curSortingLayers;
    FieldInfo sortingLayerField;

    private Texture2D onImg;
    private Texture2D offImg;
    //Light Field


    // private void LightFieldSetting()
    // {
    //     sortingLayerNames = SortingLayer.layers.Select(layer=>layer.name).ToArray();
    //     selectedLayers = new bool[sortingLayerNames.Length];
    //     sortingLayerField = typeof(Light2D).GetField("m_ApplyToSortingLayers", BindingFlags.NonPublic | BindingFlags.Instance);
    //     curSortingLayers = (int[])sortingLayerField.GetValue(mapEditor.GlobalLight.GetComponent<Light2D>());
    //     sortingLayerMask = ConvertSortingLayerIDsToBitFlag(curSortingLayers);
    //     onImg = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Artwork/Sprites/Assets/UI Elements/White/1x/down arrow.png");
    //     offImg =  AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Artwork/Sprites/Assets/UI Elements/White/1x/menu2.png");
    // }

    public override void OnInspectorGUI()
    {
        mapEditor = target as MapEditor;
        // LightFieldSetting();

        GUILayout.Space(10);
        Draw_MainContents();
        GUILayout.Space(10);
        Draw_ToolContent();
        GUILayout.Space(20);

        if (!Application.isPlaying) {
            Draw_DevContents();
        } else {
            //Draw_InGameContents();
        }
        GUILayout.Space(10);
        Draw_ResetContent();
    }
    #region  Draw
    // private bool isLight;
    private void Draw_MainContents()
    {
        EditorGUILayout.LabelField("Map Editor", GetGUIStyle_Label(Color.black, 14, FontStyle.Bold));
        EditorGUILayout.HelpBox($"프로젝트 실행할때 꼭 개발자용 데이터 세이브 후 Reset 버튼 누른다음 실행하기.", MessageType.Info);
        GUILayout.BeginVertical(mapEditor.onLoad ? "Save" : "Load", new GUIStyle(GUI.skin.window));
        mapEditor.mapType = (MapType)EditorGUILayout.EnumPopup("Map Type", mapEditor.mapType);

        Draw_MainContents_MapId();

        if (!mapEditor.onLoad)
        {
            using (new EditorGUI.DisabledScope(true))
            {
                mapEditor.audioName = EditorGUILayout.TextField(new GUIContent("BGM", "BGM"), mapEditor.audioName);
            }
        }
        else
        {
            // Draw_ShadwAndLight();
            //globalLightIntensity가 높을수록 manager darknessAlpha 낮아짐 .
            // mapEditor.globalLightIntensity = EditorGUILayout.IntSlider("Global Light", mapEditor.globalLightIntensity, 0, 100);
            int newGlobalLightIntensity = EditorGUILayout.IntSlider("Global Light", mapEditor.globalLightIntensity, 0, 100);
            if(newGlobalLightIntensity != mapEditor.globalLightIntensity)
            {
                mapEditor.globalLightIntensity = newGlobalLightIntensity;
                Color colr = mapEditor.LightingManager.profile.DarknessColor;
                mapEditor.LightingManager.profile.DarknessColor = new Color(colr.r, colr.g, colr.b, 1 - (mapEditor.globalLightIntensity * 0.01f));
                EditorUtility.SetDirty(mapEditor);
                EditorUtility.SetDirty(mapEditor.LightingManager.profile);
                SceneView.RepaintAll();
            }
            
            mapEditor.stageLevel = EditorGUILayout.IntField("Stage Level", mapEditor.stageLevel);
            mapEditor.subMapName = EditorGUILayout.TextField(
                new GUIContent("Map Sub Name", "This is the sub-name for the map, but it’s okay to leave it empty."),
                mapEditor.subMapName
                );

            //Game difficulty
            // mapEditor.stageDifficulty = EditorGUILayout.IntField("Stage Difficulty",mapEditor.stageDifficulty);
            mapEditor.stageDifficulty = EditorGUILayout.IntSlider("Stage Difficulty", mapEditor.stageDifficulty, 0, 3);
            //Game difficulty

            DrawBGMContents();
        }
        GUILayout.EndVertical();
    }

    // private void Draw_ShadwAndLight()
    // {
    //     GUILayout.Space(20);
    //         GUILayout.BeginHorizontal();
          
    //             GUILayout.BeginVertical(new GUIStyle(GUI.skin.window));
    //                 GUILayout.FlexibleSpace();
    //                 GUILayout.Label("Shadow And Global Light",GetGUIStyle_Label(Color.white,15,FontStyle.Bold,TextAnchor.MiddleCenter));
    //                 GUILayout.FlexibleSpace();

    //                 GUILayout.BeginVertical();

    //                     // DrawShadow();
            
    //                     GUILayout.BeginVertical(new GUIStyle(GUI.skin.window));
    //                             HorizontalScope(()=>{
    //                                 GUILayout.FlexibleSpace();
    //                                 EditorGUILayout.LabelField("Global Light",GetGUIStyle_Label(
    //                                     Color.white,12,FontStyle.Bold,TextAnchor.MiddleLeft),GUILayout.Width(80),GUILayout.Height(25)
    //                                     );
    //                                 if (GUILayout.Button(new GUIContent(isLight ? onImg : offImg),GUILayout.Width(25), GUILayout.Height(25)))
    //                                 {
    //                                     isLight = !isLight;
    //                                 }
    //                                 GUILayout.FlexibleSpace();
    //                             });
                                
    //                             if(isLight)
    //                             {
    //                                 // DrawLight();
    //                             }

    //                     GUILayout.EndVertical();
                        
                       

    //                 GUILayout.EndVertical();

    //             GUILayout.EndVertical();
    //         GUILayout.EndHorizontal();

    //         GUILayout.Space(20);    
    // }
    // private void DrawShadow()
    // {
    //     VerticalScope(()=>{
    //         GUILayout.FlexibleSpace();
    //         if (GUILayout.Button("그림자 생성", GUILayout.Width(100), GUILayout.Height(30)))
    //         {
    //             CreateShadow();
    //         }
    //         GUILayout.FlexibleSpace();
    //     });  
    // }

#region Light
    // private void DrawLight()
    // {
    //     GUILayout.BeginVertical();

    //     VerticalScope(() => {
    //         HorizontalScope(() => {
    //             EditorGUILayout.LabelField("Color", GetGUIStyle_Label(Color.white, 10, FontStyle.Normal), GUILayout.Width(105));
    //             Color newColor = EditorGUILayout.ColorField(mapEditor.GlobalLight.color);
    //             if (newColor != mapEditor.GlobalLight.color)
    //             {
    //                 Undo.RecordObject(mapEditor, "Change Global Light Color");
    //                 mapEditor.GlobalLight.color = newColor;
    //                 EditorUtility.SetDirty(mapEditor);
    //             }
    //         });
    //         HorizontalScope(() => {
    //             EditorGUILayout.LabelField("Intensity", GetGUIStyle_Label(Color.white, 10, FontStyle.Normal), GUILayout.Width(105));
    //             float newIntensity = EditorGUILayout.Slider(mapEditor.GlobalLight.intensity, 0f, 5f);
    //             if (!Mathf.Approximately(newIntensity, mapEditor.GlobalLight.intensity))
    //             {
    //                 Undo.RecordObject(mapEditor, "Change Global Light Intensity");
    //                 mapEditor.GlobalLight.intensity = newIntensity;
    //                 EditorUtility.SetDirty(mapEditor); // 변경 사항 저장
    //             }
    //         });
    //         HorizontalScope(() => {
    //             EditorGUILayout.LabelField("Target Sorting Layers", GetGUIStyle_Label(Color.white, 10, FontStyle.Normal), GUILayout.Width(105));

    //             if (EditorGUILayout.DropdownButton(new GUIContent(GetCurrentState_SortingLayer()), FocusType.Keyboard))
    //             {
    //                 ShowSortingLayerPopup();
    //             }
    //         });
    //         HorizontalScope(() =>{
    //             DrawHorizontalLine(Color.white);
    //         });
    //         HorizontalScope(() => {
    //             EditorGUILayout.LabelField("Blend Style", GetGUIStyle_Label(Color.white, 10, FontStyle.Normal), GUILayout.Width(105));
    //             mapEditor.GlobalLight.blendStyleIndex = EditorGUILayout.IntPopup(
    //                 "",
    //                 mapEditor.GlobalLight.blendStyleIndex,
    //                 new string[] { "Multiply", "Additive", "Multiply with Mask (R)", "Additive with Mask (R)" },
    //                 new int[] { 0, 1, 2, 3 }
    //             );
    //             EditorUtility.SetDirty(mapEditor);
    //         });
    //         HorizontalScope(() => {
    //             EditorGUILayout.LabelField("Light Order", GetGUIStyle_Label(Color.white, 10, FontStyle.Normal), GUILayout.Width(105));
    //             mapEditor.GlobalLight.lightOrder = EditorGUILayout.IntField(mapEditor.GlobalLight.lightOrder);
    //             EditorUtility.SetDirty(mapEditor);
    //         });
    //         HorizontalScope(() => {
    //             EditorGUILayout.LabelField("Overlap Operation", GetGUIStyle_Label(Color.white, 10, FontStyle.Normal), GUILayout.Width(105));
    //             mapEditor.GlobalLight.overlapOperation = (OverlapOperation)EditorGUILayout.EnumPopup(mapEditor.GlobalLight.overlapOperation);
    //             EditorUtility.SetDirty(mapEditor);
    //         });
    //     });

    //     GUILayout.EndVertical();
    // }
   
    // private string GetCurrentState_SortingLayer()
    // {
    //     if(sortingLayerMask == 0) return "Nothing";
    //     if(sortingLayerMask == (1 << sortingLayerNames.Length)-1) return "Everything";

    //     int selectCount = 0;
    //     int selectIdx = 0;
    //     for(int i = 0; i<sortingLayerNames.Length;i++)
    //     {
    //         if((sortingLayerMask & (1 << i)) != 0)
    //         {
    //             selectCount++;
    //             if(selectCount>1) return "Mixed...";
    //             selectIdx = i;
    //         };
    //     }
        
    //     return sortingLayerNames[selectIdx];
    // }

    // private void ShowSortingLayerPopup()
    // {
    //     GenericMenu menu = new GenericMenu();

    //     menu.AddItem(new GUIContent("Everything"), false, () =>
    //     {
    //         sortingLayerMask = (1 << sortingLayerNames.Length) - 1; 
    //         curSortingLayers = ConvertBitFlagToSortingLayerIDs(sortingLayerMask);
    //         UpdateSortingLayerField();

    //         Repaint(); 
    //         EditorUtility.SetDirty(mapEditor); 
    //     });

    //     menu.AddItem(new GUIContent("Nothing"), false, () =>
    //     {
    //         sortingLayerMask = 0; 
    //         curSortingLayers = null;
    //         UpdateSortingLayerField();

    //         Repaint();
    //         EditorUtility.SetDirty(mapEditor); 
    //     });          
    //     menu.AddSeparator("");

    //     for (int i = 0; i < sortingLayerNames.Length; i++)
    //     {
    //         int index = i; 
    //         bool isSelected = (sortingLayerMask & (1 << index)) != 0;
               
                
    //         menu.AddItem(
    //             new GUIContent(sortingLayerNames[i]),
    //             isSelected,
    //             () =>
    //             {
    //                 // 선택 상태 토글
    //                 if (isSelected)
    //                 {
    //                     sortingLayerMask &= ~(1 << index); // 선택 해제
    //                 }
    //                 else
    //                 {
    //                     sortingLayerMask |= (1 << index); // 선택
    //                 }

    //                 curSortingLayers = ConvertBitFlagToSortingLayerIDs(sortingLayerMask);
    //                 UpdateSortingLayerField();

    //                 Repaint(); 
    //                 EditorUtility.SetDirty(mapEditor); 
    //             }
    //         );
    //     }

    //      menu.ShowAsContext();

         
    // // }
    // private void UpdateSortingLayerField()
    // {
    //     sortingLayerField.SetValue(mapEditor.GlobalLight.GetComponent<Light2D>(), curSortingLayers);
    // }
    //  private int ConvertSortingLayerIDsToBitFlag(int[] sortingLayerIDs)
    // {
    //     if(sortingLayerIDs == null) return 0;
    //     int bitFlag = 0;

    //     for (int i = 0; i < sortingLayerIDs.Length; i++)
    //     {
    //         int layerIndex = GetSortingLayerIndexFromID(sortingLayerIDs[i]);

    //         if (layerIndex >= 0)
    //         {
    //             bitFlag |= 1 << layerIndex; 
    //         }
    //     }

    //     return bitFlag;
    // // }
    // private int GetSortingLayerIndexFromID(int id)
    // {
    //     SortingLayer[] layers = SortingLayer.layers;

    //     for (int i = 0; i < layers.Length; i++)
    //     {
    //         if (layers[i].id == id)
    //         {
    //             return i; 
    //         }
    //     }

    //     return -1; 
    // }
    // private int[] ConvertBitFlagToSortingLayerIDs(int sortingLayerMask)
    // {
    //     List<int> sortingLayerIDs = new List<int>();
    //     SortingLayer[] layers = SortingLayer.layers;

    //     for (int i = 0; i < layers.Length; i++)
    //     {
    //         if ((sortingLayerMask & (1 << i)) != 0) // 해당 비트가 켜져 있는지 확인
    //         {
    //             sortingLayerIDs.Add(layers[i].id); // 해당 Layer의 ID 추가
    //         }
    //     }

    //     return sortingLayerIDs.ToArray();
    // }
#endregion

    private void Draw_MainContents_MapId() {
        Color orgCol = GUI.backgroundColor;
        if (string.IsNullOrEmpty(mapEditor.mapID)) {
            GUI.backgroundColor = Color.red;
        }
        mapEditor.mapID = EditorGUILayout.TextField(new GUIContent("Map ID", "Unique identifier for the map. This field is required."), mapEditor.mapID);
        GUI.backgroundColor = orgCol;
    }

    private void DrawBGMContents()
    {
        GUI.SetNextControlName("BGM");
        mapEditor.audioName = EditorGUILayout.TextField(new GUIContent("BGM", ""), mapEditor.audioName);

        if (!string.IsNullOrEmpty(mapEditor.audioName))
        {
            filteredOptions = autoCompleteOptions
                   .FindAll(option => option.ToLower().Contains(mapEditor.audioName.ToLower()));
            showDropdown = filteredOptions.Count > 0;
        }
        else if (GUI.GetNameOfFocusedControl() == "BGM" && string.IsNullOrEmpty(mapEditor.audioName))
        {
            filteredOptions = new(autoCompleteOptions);
            showDropdown = filteredOptions.Count > 0;
        }
        else
        {
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
                mapEditor.onLoad = true;
                mapEditor.isLoadMap = true;
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
            // GlobalLightReset();
            _Reset(mapEditor);
            mapEditor.Init();
            EditorApplication.ExecuteMenuItem("Window/2D/Tile Palette");
            

            MpaEditorFieldReset();
            mapEditor.onLoad = true;

        }
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.FlexibleSpace();
    }
    // private void GlobalLightReset()
    // {
    //     mapEditor.GlobalLight.color = Color.white;
    //     mapEditor.GlobalLight.intensity = 1;
    //     sortingLayerField.SetValue(mapEditor.GlobalLight, new int[] { 0 });
    //     mapEditor.GlobalLight.blendStyleIndex = 0;
    //     mapEditor.GlobalLight.lightOrder = 0;
    //     mapEditor.GlobalLight.overlapOperation = 0;
    // }
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
    //private void Draw_InGameContents() {
    //    mapEditor.onLoad = false;
    //    GUILayout.Space(10);

    //    GUILayout.BeginHorizontal();
    //    GUILayout.FlexibleSpace();
    //    GUILayout.Label("인게임 전용", GetGUIStyle_Label((Color.white), 14, FontStyle.Bold));
    //    GUILayout.FlexibleSpace();
    //    GUILayout.EndHorizontal();

    //    GUILayout.BeginHorizontal();
    //    GUILayout.FlexibleSpace();
    //    if (GUILayout.Button("Load Data(인게임용)", GUILayout.Width(300), GUILayout.Height(30)))
    //    {
    //        //mapEditor.LoadMap(mapEditor.mapID);
    //        mapEditor.MoveNextStage(mapEditor.mapID);

    //    }
    //    GUILayout.FlexibleSpace();
    //    GUILayout.EndHorizontal();

    //    GUILayout.BeginHorizontal();
    //    GUILayout.FlexibleSpace();
    //    if (GUILayout.Button("Reset Interactable Object Position(인게임용)", GUILayout.Width(300), GUILayout.Height(30)))
    //    {
    //        mapEditor.ResetInteractableObjectPosition();
    //    }
    //    GUILayout.FlexibleSpace();
    //    GUILayout.EndHorizontal();
    //    GUILayout.Space(10);
    //}

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
            mapEditor.stageDifficulty = 0;

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

        mapEditor.LightingManager.profile.DarknessColor = new Color(0, 0, 0, 0); 

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

            //Shadow Setting
            // Create_Shadow();

            //Light Setting
            // SetGlobalLight(map.globalLightStruct);

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
        mapEditor.stageDifficulty = map.stageDifficulty;

        mapEditor.globalLightIntensity = map.globalLightStruct.ConvertGlobalLightIntensityToInt();
        mapEditor.LightingManager.profile.DarknessColor = new Color(0, 0, 0, map.globalLightStruct.darknessAlpha);
    }
    private void MpaEditorFieldReset()
    {
           mapEditor.CurMap = new Map();
            mapEditor.mapID = "";
            mapEditor.mapType = MapType.Main;
            mapEditor.audioName = "";
            mapEditor.stageLevel = 0;

            mapEditor.subMapName = "";
            mapEditor.stageDifficulty = 0;

            mapEditor.onLoad = false;
            mapEditor.isLoadMap = false;
    }
    #region  Generate
    private void Create_StartPoint(Map map) {
        GameObject startPoint = Instantiate(Resources.Load<GameObject>(mapObjectDataDictionary[302].path));
        mapEditor.startPositionObject = startPoint;
        startPoint.transform.position = map.startPosition;
        startPoint.transform.SetParent(mapEditor.dontSaveObjectTransform);
    }
    public void Create_Tile() {
        DrawTile_C(mapEditor.placeMentSystem.floorTileMap, mapEditor.CurMap.mapTileDataList); //rect
        DrawTile_C(mapEditor.placeMentSystem.halfTileMap, mapEditor.CurMap.mapHalfTileDataList);
        DrawTile_C(mapEditor.placeMentSystem.backgroundTileMap, mapEditor.CurMap.mapBackgroundTileDataList);
        DrawTile_C(mapEditor.placeMentSystem.ropeTileMap, mapEditor.CurMap.mapRopeTileDataList);
        DrawTile_C(mapEditor.placeMentSystem.accessoryTileMap, mapEditor.CurMap.mapAccessoryTIleDataList);
        DrawTile_C(mapEditor.placeMentSystem.hiddentTIleMap, mapEditor.CurMap.mapHiddenTileDataList);
        DrawTile_C(mapEditor.placeMentSystem.specialTileMap, mapEditor.CurMap.mapSpecialTileDataList);
        //DrawTile(mapEditor.placeMentSystem.floorTileMap, mapEditor.CurMap.mapTileDataList); //rect
        //DrawTile(mapEditor.placeMentSystem.halfTileMap, mapEditor.CurMap.mapHalfTileDataList);
        //DrawTile(mapEditor.placeMentSystem.backgroundTileMap, mapEditor.CurMap.mapBackgroundTileDataList);
        //DrawTile(mapEditor.placeMentSystem.ropeTileMap, mapEditor.CurMap.mapRopeTileDataList);
        //DrawTile(mapEditor.placeMentSystem.accessoryTileMap, mapEditor.CurMap.mapAccessoryTIleDataList);
    }
    //------------------------------------------------------------------------------------------------------250107 Shadow
    // public void Create_Shadow()
    // {
    //     foreach(ShadowCasterStruct data in mapEditor.CurMap.mapShadowCasterDataList)
    //     {
    //         ShadowCasterSetting shadowSetting = Instantiate(Resources.Load<GameObject>(GlobalText.SHADOW_PREFAB_PATH)).GetComponent<ShadowCasterSetting>();
    //         shadowSetting.gameObject.transform.SetParent(mapEditor.shadowContainer);    
    //         shadowSetting.transform.position = data.position;
    //         shadowSetting.SetShadowCasterData(data);
    //     }

    // }
    //------------------------------------------------------------------------------------------------------250107 Shadow
    public void Create_Object() {
        CreateExitObject(mapEditor.CurMap.mapExitObjectStruct);
        Create_Object(mapEditor.CurMap.mapObjectDataList, mapEditor.objectTransform);
        Create_Object(mapEditor.CurMap.mapBackgroundObjectList, mapEditor.backgroundObjectContainer);
        Create_OtherObject(mapEditor.CurMap.mapOtherObjectList);
        Create_Object(mapEditor.CurMap.mapButtonActivatableObjectDataList, mapEditor.buttonActivatableObjectTransform);
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
    private void CreateExitObject(ExitObjStruct data)
    {
        MapDataStruct mapDataStruct = mapObjectDataDictionary[data.id];
        Create(mapEditor.exitDoorObjectTransform,mapDataStruct,data);

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
    public void Create_Object<T>(List<T> list, Transform transform)
    {
        MapDataStruct mapDataStruct;
        foreach (T data in list)
        {
            var isField = typeof(T).GetField("id", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            if (isField != null)
            {
                var value = isField.GetValue(data);
                if (value is int intValue)
                {
                    mapDataStruct = mapObjectDataDictionary[intValue];
                    Create(transform, mapDataStruct, data);
                }

            }
        }
    }
   
    async void Create<T>(Transform transform, MapDataStruct mapDataStruct, T data) {
        try {
            GameObject obj = Instantiate(Resources.Load<GameObject>(mapDataStruct.path));
            BuildObj buildObj = obj.GetComponent<BuildObj>();
            buildObj.SetData<T>(data);
            obj.transform.SetParent(transform);

            
            await Task.Delay(500);
            buildObj.Editor_Setting(mapEditor);

            
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
                for (int i = 0; i <= 4; i++)
                {
                    string checkPath = Path.Combine(path, $"{i}/{id}.json");
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


    private void CreateShadow()
    {
      GameObject shadowObj = Instantiate(Resources.Load<GameObject>(GlobalText.SHADOW_PREFAB_PATH));
      shadowObj.transform.SetParent(mapEditor.shadowContainer);
      shadowObj.transform.position = GetSceneViewCenter();
      Selection.activeGameObject = shadowObj;
    }
   
    private Vector3 GetSceneViewCenter(){
        SceneView sceneView = SceneView.lastActiveSceneView;
        if (sceneView != null)
        {
            return sceneView.pivot;
        }
        else
        {
            return Vector3.zero;
        }
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
        for (int i = 0; i < pathCount; i++)
        {
            Vector2[] pathVertices = new Vector2[tilemapCollider.GetPathPointCount(i)];
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
            Map map = CreateMap(mapEditor);

            //Create Screen Shot
            await CurrentMapScreenShot(mapEditor);

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

    private Map CreateMap(MapEditor mapEditor)
    {
        Map map = new Map(new Vector2(mapEditor.width, mapEditor.height), mapEditor.mapID, mapEditor.subMapName,
                GetNextMapId(), mapEditor.stageLevel, mapEditor.startPosition, mapEditor.stageDifficulty,
                GetExitObjStructsList(mapEditor.exitDoorObjectTransform, mapEditor),
                //tile
                GetCompressedTileData(mapEditor.placeMentSystem.floorTileMap),
                GetCompressedTileData(mapEditor.placeMentSystem.halfTileMap),
                GetCompressedTileData(mapEditor.placeMentSystem.backgroundTileMap),
                GetCompressedTileData(mapEditor.placeMentSystem.ropeTileMap),
                GetCompressedTileData(mapEditor.placeMentSystem.accessoryTileMap),
                GetCompressedTileData(mapEditor.placeMentSystem.hiddentTIleMap),
                GetCompressedTileData(mapEditor.placeMentSystem.specialTileMap),
                //Shadow
                // GetShadowData(),
                //Light
                GetGlobalLightStruct(),
                //object
                GetList<ObjectData>(mapEditor.objectTransform),
                GetList<ObjectData>(mapEditor.backgroundObjectContainer),
                GetList_Depth<ObjectData>(mapEditor.otherContainer.GetComponent<OtherContainer>()),
                GetList<ButtonActivatableObjectStruct>(mapEditor.buttonActivatableObjectTransform),
                GetList<ButtonObjectStruct>(mapEditor.buttonObjectTransform),
                GetList<DialogueData>(mapEditor.triggerDialogueTransform),
                GetList<DroneStruct>(mapEditor.droneTransform),
                GetList<CollectableObjectStruct>(mapEditor.collectableContainer),
                mapEditor.cellSize,
                0,
                null,
                mapEditor.audioName);
        return map;
    }

    //------------------------------------------------------------------------------------------------------250107 Shadow
//     private List<ShadowCasterStruct> GetShadowData()
// {

//     List<ShadowCasterStruct> list = new();
//     foreach(Transform tr in mapEditor.shadowContainer)
//     {
//         Debug.Log($"{tr.name}");
//         ShadowCasterSetting setting = tr.GetComponent<ShadowCasterSetting>();
//         list.Add(setting.GetShadowCasterStruct());
//     }
//     return list;
// }
//------------------------------------------------------------------------------------------------------250107 Shadow
//------------------------------------------------------------------------------------------------------250112 Light
private LightStruct GetGlobalLightStruct()
{
    LightingManager2D target = mapEditor.LightingManager;

    return new LightStruct(
        target.profile.DarknessColor.a
    );
}
//------------------------------------------------------------------------------------------------------250112 Light

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
    //---------------------------------------------------------------------------------------------------- 250120 Tile Data Refactoring

    List<CompressedTileData> GetCompressedTileData(Tilemap tileMap)
    {
        List<TileData> list = GetTileData(tileMap);
        return CompressTileData_Second(CompressTileData(list));
        //return CompressTileData(list);
    }

     public List<CompressedTileData> CompressTileData(List<TileData> tileDataList) //first compress
    {
        List<CompressedTileData> compressedList = new List<CompressedTileData>();
        CompressedTileData? currentCompressedData = null;

        foreach (var tileData in tileDataList)
        {
            if (currentCompressedData == null ||
                tileData.id != currentCompressedData.Value.TileId ||
                !IsAdjacent((Vector2Int)tileData.position, currentCompressedData.Value.End))
            {
                if (currentCompressedData != null) compressedList.Add(currentCompressedData.Value);
                currentCompressedData = new CompressedTileData(tileData.id, (Vector2Int)tileData.position, (Vector2Int)tileData.position);
            }
            else
            {
                var updatedData = currentCompressedData.Value;
                updatedData.Extend((Vector2Int)tileData.position);
                currentCompressedData = updatedData;
            }
        }
        if (currentCompressedData != null)
        {
            compressedList.Add(currentCompressedData.Value);
        }

        return compressedList;
    }

    public List<CompressedTileData> CompressTileData_Second(List<CompressedTileData> list)
    {
       List<CompressedTileData> compressedList = new List<CompressedTileData>();
       CompressedTileData? curData = null;

       foreach (var tileData in list)
       {
            if(curData == null || 
                tileData.TileId != curData.Value.TileId ||
                !IsAdjacent_2(tileData,curData.Value)
            )
            {
                if(curData != null) compressedList.Add(curData.Value);
                curData = new CompressedTileData(tileData.TileId,tileData.Start,tileData.End);
            }else{
                var updateData = curData.Value;
                updateData.Extend(tileData.End);
                curData = updateData;
            }
            
       }
       if(curData != null)
       {
        compressedList.Add(curData.Value);
       }
       return compressedList;
    }
     private bool IsAdjacent(Vector2Int current, Vector2Int previous)
    {
        //return (current.x == previous.x && Mathf.Abs(current.y - previous.y) == 1) ||
        //       (current.y == previous.y && Mathf.Abs(current.x - previous.x) == 1);
        return (current.x == previous.x && Mathf.Abs(current.y - previous.y) == 1);
    }
    private bool IsAdjacent_2(CompressedTileData cur,CompressedTileData pre)
    {
        return (cur.Start.y == pre.Start.y) &&
                (cur.End.y == pre.End.y) &&
                (Mathf.Abs(cur.End.x - pre.End.x) ==1);
    }
    public void DrawTile_C(Tilemap tileMap,List<CompressedTileData> list)
    {
        foreach (var data in list)
        {
            MapDataStruct mapDataStruct = mapObjectDataDictionary[data.TileId];
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
    public void Sorting(List<TileData> tileList)
    {
        tileList = tileList
        .OrderBy(td => td.id)
        .ThenBy(td => td.position.x)
        .ThenBy(td => td.position.y)
        .ToList();
    }
    //---------------------------------------------------------------------------------------------------- 250120 Tile Data Refactoring

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

    ExitObjStruct GetExitObjStructsList(Transform transform,MapEditor mapEditor)
    {
        int keyAmount = 0;
        foreach(Transform tr in mapEditor.objectTransform)
        {
            if(tr.GetComponent<BuildObj>().id == 307)
            {
                keyAmount++;
            }
        }

        ExitPointObj exitObj = transform.GetChild(0).GetComponent<ExitPointObj>();
        exitObj.condition_KeyAmount = keyAmount;
        
        return exitObj.GetComponent<ExitPointObj>().GetExitObjectStruct();
  
    }
    
    public void AddExitPointObjKeyAmount(){
        ExitPointObj exit = mapEditor.exitDoorObjectTransform.GetChild(0).GetComponent<ExitPointObj>();
        exit.condition_KeyAmount++;
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
        Task<byte[]> encodingTask = camera.GetComponent<ScreenShotCamera>().ScreenShot(mapEditor.mapID);
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
    // private void SetGlobalLight(LightStruct? data)
    // {
    //     if(data == null)
    //     {
    //         mapEditor.GlobalLight.lightType = Light2D.LightType.Global;
    //         mapEditor.GlobalLight.color = Color.white;
    //         mapEditor.GlobalLight.intensity = 1;
    //         sortingLayerField.SetValue(mapEditor.GlobalLight, new int[] { 0 });
    //         mapEditor.GlobalLight.blendStyleIndex = 0;
    //         mapEditor.GlobalLight.lightOrder = 0;
    //         mapEditor.GlobalLight.overlapOperation = 0;
    //         return;
    //     }

    //     var lightData = data.Value;

    //     if(lightData.type == default)
    //     {
    //         mapEditor.GlobalLight.lightType = Light2D.LightType.Global;
    //         mapEditor.GlobalLight.color = Color.white;
    //         mapEditor.GlobalLight.intensity = 1;
    //         sortingLayerField.SetValue(mapEditor.GlobalLight, new int[] { 0 });
    //         mapEditor.GlobalLight.blendStyleIndex = 0;
    //         mapEditor.GlobalLight.lightOrder = 0;
    //         mapEditor.GlobalLight.overlapOperation = 0;
    //         return;
    //     }

    //     mapEditor.GlobalLight.lightType = lightData.type;
    //     mapEditor.GlobalLight.color = lightData.color;
    //     mapEditor.GlobalLight.intensity = lightData.intensity;
    //     sortingLayerField.SetValue(mapEditor.GlobalLight, lightData.targetSorting);
    //     mapEditor.GlobalLight.blendStyleIndex = lightData.blendStyleIndex;
    //     mapEditor.GlobalLight.lightOrder = lightData.lightOrder;
    //     mapEditor.GlobalLight.overlapOperation = lightData.overlapOeration;
    // }

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

     private GUIStyle GetGUIStyle_Label(Color color,int font_Size = 12,FontStyle font_Style = FontStyle.Normal,TextAnchor textAnchor = TextAnchor.MiddleLeft){

        return new GUIStyle(GUI.skin.label) {
            alignment = textAnchor,
            normal = { textColor = color },
            fontSize = font_Size,
            fontStyle = font_Style,
            hover = { textColor = color },
        };

    }

    //   using(new GUILayout.VerticalScope("Shadow And Light",new GUIStyle(GUI.skin.window))){

    //         }

     private void VerticalScope(Action action,string label = "",GUIStyle style = null)
    {
        if (style == null)
        style = GUIStyle.none;

        using (new GUILayout.VerticalScope(label,style))
        {
            action?.Invoke();
        }
    }
    private void HorizontalScope(Action action,string label = "",GUIStyle style = null)
    {
        if (style == null)
        style = GUIStyle.none;

        using (new GUILayout.HorizontalScope(label,style))
        {
            action?.Invoke();
        }
    }
    private void DrawHorizontalLine(Color color, float thickness = 1.0f)
    {
        Rect rect = EditorGUILayout.GetControlRect(false, thickness);
        EditorGUI.DrawRect(rect, color);
    }
    #endregion
}
