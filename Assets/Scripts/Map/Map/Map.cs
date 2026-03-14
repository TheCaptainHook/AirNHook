
using System;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Map
{
    public Vector2 mapSize;
    public string mapID;
    public string subMapName;
    public int stageLevel;
    public Vector2 startPosition;

    [Header("Tile")]
    public List<CompressedTileData> mapTileDataList = new(); //rec
    public List<CompressedTileData> mapHalfTileDataList = new();
    public List<CompressedTileData> mapBackgroundTileDataList = new();
    //TODO 1022
    public List<CompressedTileData> mapRopeTileDataList = new();
    public List<CompressedTileData> mapAccessoryTIleDataList = new();
    public List<CompressedTileData> mapHiddenTileDataList = new();
    // 1130
    public List<CompressedTileData> mapSpecialTileDataList = new();
    //public List<TileData> mapTileDataList = new(); //rec
    //public List<TileData> mapHalfTileDataList = new();
    //public List<TileData> mapBackgroundTileDataList = new();
    //public List<TileData> mapRopeTileDataList = new();
    //public List<TileData> mapAccessoryTIleDataList = new();
    //------------------------------------------------------------------------------------------------------250107 Shadow
    [Header("Shadow")]
    public List<ShadowCasterStruct> mapShadowCasterDataList = new();
//------------------------------------------------------------------------------------------------------250107 Shadow
//------------------------------------------------------------------------------------------------------250112 Global Light
    public LightStruct globalLightStruct;
//------------------------------------------------------------------------------------------------------250112 Global Light
    [Header("Object")]
    public List<ObjectData> mapObjectDataList = new List<ObjectData>();
    //TODO 1024
    public List<ObjectData> mapBackgroundObjectList = new List<ObjectData>();
    public List<ObjectData> mapOtherObjectList = new List<ObjectData>();
    //TODO 1024
    public List<ButtonActivatableObjectStruct> mapButtonActivatableObjectDataList = new();
    public List<ButtonObjectStruct> buttonObjectList = new();
    public ExitObjStruct mapExitObjectStruct;
    public List<DialogueData> dialogueDataList = new();
    public List<DroneStruct> droneStructList = new(); //TODO 0922

    public List<CollectableObjectStruct> collectableObjectStructList = new();//TODO 1129

    public int dataType; //0:Main,1:User
    public float cellSize;
    [HideInInspector]public byte[] bytesImage;
    // public AudioType audioType;
    public string audioName;

    //1101
    public string nextMapId;

    //250314
    public int stageDifficulty;

    public Map(Vector2 mapSize, string id, string subMapName,string nextMapId,int stageLevel, Vector2 startPosition,int stageDifficulty,
        ExitObjStruct mapExitObjectStruct,
        //tile
        List<CompressedTileData> tileList, // refc
        List<CompressedTileData> halfTileList,
        List<CompressedTileData> mapBackgroundTileDataList,
        List<CompressedTileData> ropeTileDataList,
        List<CompressedTileData> accessoryTileDataList,
        List<CompressedTileData> mapHiddenTileDataList,
        List<CompressedTileData> mapSpecialTileDataList,
        //Shadow 250109
        //List<ShadowCasterStruct> shadowCasterStructs,
        //Light 250112
        LightStruct globalLightStruct,
        //object
        List<ObjectData> objectList,
        List<ObjectData> backgroundObjectList,
        List<ObjectData> otherObjectList,
        List<ButtonActivatableObjectStruct> mapButtonActivatabledObjectDataList,
        List<ButtonObjectStruct> buttonObjectList,
        List<DialogueData> dialogueDataList,
        List<DroneStruct> droneStructList,
        List<CollectableObjectStruct> collectableObjectStructList,
        float cellSize,int dataType = 0, byte[] bytesImage = null,string audioName =""
        )
    {
        mapID = id;
        this.subMapName = subMapName;
        this.nextMapId = nextMapId;
        this.stageLevel = stageLevel;
        this.stageDifficulty = Mathf.Clamp(stageDifficulty,0,5);
        //tile
        mapTileDataList = tileList;
        mapHalfTileDataList = halfTileList;
        this.mapBackgroundTileDataList = mapBackgroundTileDataList;
        mapRopeTileDataList = ropeTileDataList;
        mapAccessoryTIleDataList = accessoryTileDataList;
        this.mapHiddenTileDataList = mapHiddenTileDataList;
        this.mapSpecialTileDataList = mapSpecialTileDataList;
        //shadow
        //this.mapShadowCasterDataList = shadowCasterStructs;
        //Light
        this.globalLightStruct = globalLightStruct;
        //object
        mapObjectDataList = objectList;
        mapBackgroundObjectList = backgroundObjectList;
        mapOtherObjectList = otherObjectList;
        this.startPosition = FloorVector2(startPosition);
        this.mapExitObjectStruct = mapExitObjectStruct;
        this.mapButtonActivatableObjectDataList = mapButtonActivatabledObjectDataList;
        this.buttonObjectList = buttonObjectList;
        this.dialogueDataList = dialogueDataList;
        this.droneStructList = droneStructList;
        this.collectableObjectStructList = collectableObjectStructList;

        this.mapSize = mapSize;
        this.cellSize = cellSize;
        this.dataType = dataType;//main and userData
        this.bytesImage = bytesImage;
        this.audioName = audioName; 
    }

    public Map() { } //dont delet


    public ObjectData FindObjectData(int id)
    {
        foreach (ObjectData objectData in mapObjectDataList)
        {
            if (objectData.id == id)
            {
                return objectData;
            }
        }

        return new ObjectData();
    }


    // public (Vector2 start,Vector2 end) GetStartEndPosition() //TODO 0807 GEt Map Size
    // {
    //     Vector2 start = new Vector2(mapTileDataList[0].position.x, mapTileDataList[0].position.y);
    //     Vector2 end = new Vector2(mapTileDataList[mapTileDataList.Count-1].position.x, mapTileDataList[mapTileDataList.Count - 1].position.y);
    //     return (start, end);
    // }

    private Vector2 FloorVector2(Vector2 vec)
    {
        return new Vector2(FloorValue_2(vec.x), FloorValue_2(vec.y));
    }
    private float FloorValue_2(float val)
    {
        float num = val * 100;
        return Mathf.Floor(num)/100f;
    }
}



