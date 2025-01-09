
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Lumin;




[System.Serializable]
public class Map
{
    public Vector2 mapSize;
    public string mapID;
    public string subMapName;
    public int stageLevel;
    public Vector2 startPosition;

    [Header("Tile")]
    public List<TileData> mapTileDataList = new();
    public List<TileData> mapHalfTileDataList = new();
    public List<TileData> mapBackgroundTileDataList = new();
    //TODO 1022
    public List<TileData> mapRopeTileDataList = new();
    public List<TileData> mapAccessoryTIleDataList = new();
//------------------------------------------------------------------------------------------------------250107 Shadow
    [Header("Shadow")]
    public List<ShadowCasterStruct> mapShadowCasterDataList = new();
//------------------------------------------------------------------------------------------------------250107 Shadow
    [Header("Object")]
    public List<ObjectData> mapObjectDataList = new List<ObjectData>();
    //TODO 1024
    public List<ObjectData> mapBackgroundObjectList = new List<ObjectData>();
    public List<ObjectData> mapOtherObjectList = new List<ObjectData>();
    //TODO 1024
    public List<ButtonActivatableObjectStruct> mapButtonActivatableObjectDataList = new();
    public List<ButtonObjectStruct> buttonObjectList = new();
    public List<ExitObjStruct> mapExitObjectDataList = new();
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

    public Map(Vector2 mapSize, string id, string subMapName,
        string nextMapId,
        int stageLevel, Vector2 startPosition,
        List<ExitObjStruct> mapExitObjectDataList,
        //tile
        List<TileData> tileList,
        List<TileData> halfTileList,
        List<TileData> mapBackgroundTileDataList,
        List<TileData> ropeTileDataList,
        List<TileData> accessoryTileDataList,
        //Shadow 250109
        List<ShadowCasterStruct> shadowCasterStructs,
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
        //tile
        mapTileDataList = tileList;
        mapHalfTileDataList = halfTileList;
        this.mapBackgroundTileDataList = mapBackgroundTileDataList;
        mapRopeTileDataList = ropeTileDataList;
        mapAccessoryTIleDataList = accessoryTileDataList;
        //shadow
        this.mapShadowCasterDataList = shadowCasterStructs;
        //object
        mapObjectDataList = objectList;
        mapBackgroundObjectList = backgroundObjectList;
        mapOtherObjectList = otherObjectList;
        this.startPosition = startPosition;
        this.mapExitObjectDataList = mapExitObjectDataList;
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


    public (Vector2 start,Vector2 end) GetStartEndPosition() //TODO 0807 GEt Map Size
    {
        Vector2 start = new Vector2(mapTileDataList[0].position.x, mapTileDataList[0].position.y);
        Vector2 end = new Vector2(mapTileDataList[mapTileDataList.Count-1].position.x, mapTileDataList[mapTileDataList.Count - 1].position.y);
        return (start, end);
    }
}



