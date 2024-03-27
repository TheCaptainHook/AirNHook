using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;
using UGS;
using NPOI.OpenXmlFormats.Spreadsheet;
using System.IO;
using UnityEditor.UI;

[CustomEditor(typeof(MapEditor))]
public class MapEditor_Editor : Editor
{

    public Dictionary<int, MapDataStruct> mapTileDataDictionary = new Dictionary<int, MapDataStruct>();

    public Dictionary<int, MapDataStruct> mapObjectDataDictionary = new Dictionary<int, MapDataStruct>();

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        MapEditor mapEditor = target as MapEditor;

        EditorGUILayout.LabelField("테스트", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox($"1. 인게임용은 프로젝트 실행했을때만 누르기.\n2.맵을 만들때 Init 버튼 눌러주기.\n3.맵 세이브하고 인게임에서 확인할때 만들던 맵 꼭 개발자용 세이브 하고 Reset 버튼 누른다음 확인하기.", MessageType.Info);



        if (GUILayout.Button("Load Data(인게임용)"))
        {
            mapEditor.LoadMap(mapEditor.mapID);
        }
        if (GUILayout.Button("Save Data(인게임용)"))
        {
            mapEditor.SaveMapData();
        }

        if (GUILayout.Button("개발자용, 맵 새로만들 때 먼저 누르기,Init!"))
        {
            _Reset(mapEditor);
            mapEditor.Init();
            EditorApplication.ExecuteMenuItem("Window/2D/Tile Palette");
        }
        if (GUILayout.Button("Map Create Tool"))
        {
            CreateMap_Tool.ShowWindow();
        }


        if (GUILayout.Button("Load Data(개발자전용)"))
        {
            //mapEditor.LoadMap(mapEditor.mapID);
            _Reset(mapEditor);
            LoadMap(mapEditor);
        }


        if (GUILayout.Button("Save Data(개발자전용)"))
        {
            Debug.Log("Buttom");
            SaveMapData(mapEditor);
        }

        if (GUILayout.Button("Reset"))
        {
            //if (mapEditor.mapObjBoxTransform)
            //{
            //    Undo.DestroyObjectImmediate(mapEditor.mapObjBoxTransform.gameObject);
            //    Undo.DestroyObjectImmediate(mapEditor.gridPalette);
            //}           
            //mapEditor.curMap = new Map();
            _Reset(mapEditor);
        }

    }

    private void _Reset(MapEditor mapEditor)
    {
        if (mapEditor.mapObjBoxTransform)
        {
            Undo.DestroyObjectImmediate(mapEditor.mapObjBoxTransform.gameObject);
            Undo.DestroyObjectImmediate(mapEditor.gridPalette);
        }
        mapEditor.curMap = new Map();
    }

    void UGS_MapDataLoad()
    {
        mapTileDataDictionary.Clear();
        mapObjectDataDictionary.Clear();
        //Tile Data
        UnityGoogleSheet.Load<MapObjectData.TileData>();
        foreach (var value in MapObjectData.TileData.TileDataList)
        {
            if (!mapTileDataDictionary.ContainsKey(value.id))
            {
                mapTileDataDictionary.Add(value.id, new MapDataStruct(value.type, value.path));
            }
            
        }
        //Object Data
        UnityGoogleSheet.Load<MapObjectData.ObjectData>();
        foreach (var value in MapObjectData.ObjectData.ObjectDataList)
        {
            if (!mapObjectDataDictionary.ContainsKey(value.id))
            {
                mapObjectDataDictionary.Add(value.id, new MapDataStruct(value.type, value.path));
            }
            
        }
    }

    #region  Create 

    void LoadMap(MapEditor mapEditor)
    {
        mapEditor.Init();
        UGS_MapDataLoad();
        TextAsset textAsset = Resources.Load<TextAsset>($"MapDat/{mapEditor.mapType}/{mapEditor.mapID}");
        if(textAsset != null)
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


            CreateObj(mapEditor.floorTransform, map, mapEditor.placeMentSystem, mapEditor);
            CreateObj(mapEditor.objectTransform, map, mapEditor.placeMentSystem, mapEditor);
            CreateObj(mapEditor.interactionObjectTransform, map, mapEditor.placeMentSystem, mapEditor);
            CreateObj(mapEditor.exitDoorObjectTransform, map, mapEditor.placeMentSystem, mapEditor);
        }
        else
        {
            Debug.Log("Map not found");
        }

    }

    public void CreateObj(Transform transform,Map map,PlaceMentSystem placeMentSystem,MapEditor mapEditor)
    {
        switch (transform.name)
        {
            case "FloorTransform":

                foreach (TileData data in map.mapTileDataList)
                {
                    MapDataStruct mapDataStruct = mapTileDataDictionary[data.id];
                    placeMentSystem.floorTileMap.SetTile(data.position, Resources.Load<TileBase>(mapDataStruct.path));
                    placeMentSystem.tileDic[data.position] = data.id;

                }

                break;
            case "ObjectTransform":
                foreach (ObjectData data in map.mapObjectDataList)
                {
                    MapDataStruct mapDataStruct = mapObjectDataDictionary[data.id];
                    Create(transform, mapDataStruct, data);
                }
                break;
            case "InteractionObjectTransform":
                foreach (ButtonActivatedDoorStruct data in map.mapButtonActivatedDoorDataList)
                {
                    MapDataStruct mapDataStruct = mapObjectDataDictionary[data.id];
                    Create(transform, mapDataStruct, data, mapEditor.dontSaveObjectTransform);
                }
                break;
            case "ExitDoorObjectTransform":
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
        GameObject obj = Object.Instantiate(Resources.Load<GameObject>(mapDataStruct.path));
        obj.GetComponent<BuildObj>().ObjectData = data;
        obj.transform.position = data.position;
        obj.transform.rotation = data.quaternion;
        obj.transform.localScale = data.scale;
        obj.transform.SetParent(transform);
    }
    void Create(Transform transform, MapDataStruct mapDataStruct, ButtonActivatedDoorStruct data,Transform dontSaveObject)
    {
        GameObject obj = Object.Instantiate(Resources.Load<GameObject>(mapDataStruct.path));
        ButtonActivatedDoor door = obj.GetComponent<ButtonActivatedDoor>();
        door.ButtonActivatedDoorStruct = data;
        obj.transform.SetParent(transform);
        MapDataStruct btn = mapObjectDataDictionary[306];
        foreach (Vector2 pot in data.buttonActivatePositionList)
        {
            GameObject btnActivated = Object.Instantiate(Resources.Load<GameObject>(btn.path));
            btnActivated.GetComponent<ButtonActivated>().SetLinkDoor(pot, door);
            btnActivated.transform.SetParent(dontSaveObject);

        }
    }
    void Create(Transform transform,MapDataStruct mapDataStruct,ExitObjStruct data)
    {
        GameObject obj = Object.Instantiate(Resources.Load<GameObject>(mapDataStruct.path));
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

    void CreateJsonFile(MapEditor mapEditor, string folderPath)
    {
        mapEditor.mapTileDataList = GetTileData(mapEditor.placeMentSystem.floorTileMap);
        mapEditor.mapObjectDataList = GetList(mapEditor.objectTransform);
        mapEditor.startPosition = FindObj(mapEditor.dontSaveObjectTransform, 302).transform.position;
        Map map = new Map(new Vector2(mapEditor.width, mapEditor.height), mapEditor.mapID, mapEditor.startPosition,
            GetExitObjStructsList(mapEditor.exitDoorObjectTransform),
            mapEditor.mapTileDataList,
            mapEditor.mapObjectDataList,
            GetButtonActivateDoorStructList(mapEditor),
            mapEditor.cellSize) ;
        string json = JsonUtility.ToJson(map, true);
        string filePath = Path.Combine(folderPath, $"{mapEditor.mapType}/{map.mapID}.json");
        File.WriteAllText(filePath, json);

        UnityEditor.AssetDatabase.Refresh();
    }

    List<TileData> GetTileData(Tilemap tileMap)
    {
        List<TileData> list = new();
        BoundsInt bounds = tileMap.cellBounds;
        TileBase[] tileBases = tileMap.GetTilesBlock(bounds);

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
    List<ExitObjStruct> GetExitObjStructsList(Transform transform)
    {
        List<ExitObjStruct> list = new();

        foreach (Transform cur in transform)
        {

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
}
