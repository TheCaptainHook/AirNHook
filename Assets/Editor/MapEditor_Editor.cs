using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;
using UGS;
using NPOI.OpenXmlFormats.Spreadsheet;
using System.IO;
using GoogleSheet.Type;

[CustomEditor(typeof(MapEditor))]
public class MapEditor_Editor : Editor
{

    public Dictionary<int, MapDataStruct> mapTileDataDictionary = new Dictionary<int, MapDataStruct>();

    //todo
    public Dictionary<int, MapDataStruct> mapObjectDataDictionary = new Dictionary<int, MapDataStruct>();
    //todo

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        MapEditor mapEditor = target as MapEditor;

        EditorGUILayout.LabelField("테스트", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox($"Set Size -> 맵 에디터 크기조절 후 느르면 아웃라인 생성\nSave Data 오브젝트가 적절하게 각 트랜스폼에 들어가있으면 맵 데이터 Json파일 저장", MessageType.Info);



        if (GUILayout.Button("Set Size"))
        {
            mapEditor.Init();
            mapEditor.SetMapSize(mapEditor.width, mapEditor.height);
        }
        if (GUILayout.Button("Load Data"))
        {
            //mapEditor.LoadMap(mapEditor.mapID);
            LoadMap(mapEditor);
        }

        if (GUILayout.Button("Save Data"))
        {
            Debug.Log("Buttom");
            SaveMapData(mapEditor);
        }

        if (GUILayout.Button("Reset"))
        {
            if (mapEditor.mapObjBoxTransform)
            {
                Undo.DestroyObjectImmediate(mapEditor.mapObjBoxTransform.gameObject);
            }           
            mapEditor.placeMentSystem.ResetTileMap();
            mapEditor.curMap = new Map();
        }

    }

    void UGS_MapDataLoad()
    {
        //Tile Data
        UnityGoogleSheet.Load<MapObjectData.TileData>();
        foreach (var value in MapObjectData.TileData.TileDataList)
        {
            mapTileDataDictionary.Add(value.id, new MapDataStruct(value.type, value.path));
        }
        //Object Data
        UnityGoogleSheet.Load<MapObjectData.ObjectData>();
        foreach (var value in MapObjectData.ObjectData.ObjectDataList)
        {
            mapObjectDataDictionary.Add(value.id, new MapDataStruct(value.type, value.path));
        }
    }

    #region  Create 

    void LoadMap(MapEditor mapEditor)
    {
        mapEditor.Init();
        UGS_MapDataLoad();
        TextAsset textAsset = Resources.Load<TextAsset>($"MapDat/{mapEditor.mapID}");
        Map map = JsonUtility.FromJson<Map>(textAsset.text);
        mapEditor.curMap = map;

        mapEditor.SetMapSize((int)map.mapSize.x, (int)map.mapSize.y);

        mapEditor.playerSpawnPosition = map.playerSpawnPosition;
        mapEditor.playerExitPosition = map.exitObjStruct.position;
        mapEditor.condition_KeyAmount = map.exitObjStruct.condition_KeyAmount;

        CreateObj(mapEditor.floorTransform, map, mapEditor.placeMentSystem, mapEditor);
        CreateObj(mapEditor.objectTransform, map, mapEditor.placeMentSystem, mapEditor);
        CreateObj(mapEditor.interactionObjectTransform, map, mapEditor.placeMentSystem, mapEditor);
    }
    public void CreateObj(Transform transform,Map map,PlaceMentSystem placeMentSystem,MapEditor mapEditor)
    {
        switch (transform.name)
        {
            case "FloorTransform":
                foreach (TileData data in map.mapTileDataList)
                {
                    MapDataStruct mapDataStruct = mapTileDataDictionary[data.id];
                    Debug.Log(data.position);
                    placeMentSystem.floorTileMap.SetTile(data.position, Resources.Load<Tile>(mapDataStruct.path));
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
                    Create(transform, mapDataStruct, data, mapEditor.dontSaveObject);
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
        obj.transform.SetParent(transform);
    }
    void Create(Transform transform, MapDataStruct mapDataStruct, ButtonActivatedDoorStruct data,Transform dontSaveObject)
    {
        GameObject obj = Object.Instantiate(Resources.Load<GameObject>(mapDataStruct.path));
        ButtonActivatedDoor door = obj.GetComponent<ButtonActivatedDoor>();
        door.ButtonActivatedDoorStruct = data;
        obj.transform.position = data.position;
        obj.transform.rotation = data.quaternion;
        obj.transform.SetParent(transform);
        MapDataStruct btn = Managers.Data.mapData.mapObjectDataDictionary[306];
        foreach (Vector2 pot in data.buttonActivatePositionList)
        {
            GameObject btnActivated = Object.Instantiate(Resources.Load<GameObject>(btn.path));
            btnActivated.GetComponent<ButtonActivated>().linkId = data.linkId;
            btnActivated.transform.position = pot;
            btnActivated.transform.SetParent(dontSaveObject);

        }
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
            CreateJsonFile(mapEditor,folderPath);
        }
        else
        {
            CreateJsonFile(mapEditor, folderPath);
        }

    }

    void CreateJsonFile(MapEditor mapEditor,string folderPath)
    {
        mapEditor.mapTileDataList = GetTileData(mapEditor.placeMentSystem.floorTileMap);
        mapEditor.mapObjectDataList = GetList(mapEditor.objectTransform);

        Map map = new Map(new Vector2(mapEditor.width, mapEditor.height), mapEditor.mapID, mapEditor.playerSpawnPosition, new ExitObjStruct(mapEditor.playerExitPosition, mapEditor.condition_KeyAmount),
            mapEditor.mapTileDataList,
            mapEditor.mapObjectDataList,
            GetButtonActivateDoorStructList(mapEditor.interactionObjectTransform),
            mapEditor.cellSize);
        string json = JsonUtility.ToJson(map, true);
        Debug.Log(json);
        string filePath = Path.Combine(folderPath, $"{map.mapID}.json");
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
                if(tile != null)
                {
                    TileData tileData = new TileData(int.Parse(tile.name), tilePos);
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
    List<ButtonActivatedDoorStruct> GetButtonActivateDoorStructList(Transform transform)
    {
        List<ButtonActivatedDoorStruct> list = new();
        foreach (Transform cur in transform)
        {
            Debug.Log("asdasd");
            ButtonActivatedDoor curDoor = cur.GetComponent<ButtonActivatedDoor>();
            curDoor.SetTileData(cur.position, cur.rotation);
            ButtonActivatedDoorStruct buttonActivatedDoorStruct = new ButtonActivatedDoorStruct(curDoor.ObjectData.id,
                curDoor.linkId,
                cur.transform.position,
                curDoor.buttonActivatedBtnList,
                transform.rotation);
            list.Add(buttonActivatedDoorStruct);
        }
        return list;
    }
    #endregion
}
