using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using System.IO;
using UnityEngine.UI;


[System.Serializable]
public class Map
{
    public Vector2 mapSize;
    public string mapID;
    public int stageLevel;
    public Vector2 startPosition;

    [Header("Tile")]
    public List<TileData> mapTileDataList = new();
    public List<TileData> mapHalfTileDataList = new();
    public List<TileData> mapBackgroundTileDataList = new();

    [Header("Object")]
    public List<ObjectData> mapObjectDataList = new List<ObjectData>();
    public List<ButtonActivatedDoorStruct> mapButtonActivatedDoorDataList = new List<ButtonActivatedDoorStruct>();
    public List<ButtonActivatedObject> buttonActivatedObjectList = new();
    public List<ExitObjStruct> mapExitObjectDataList = new();
    public List<DialogueData> dialogueDataList = new();

    public int dataType; //0:Main,1:User
    public float cellSize;
    [HideInInspector]public byte[] bytesImage;
    public AudioType audioType;

    public Map(Vector2 mapSize, string id, int stageLevel, Vector2 startPosition,
        List<ExitObjStruct> mapExitObjectDataList,
        //tile
        List<TileData> tileList,
        List<TileData> halfTileList,
        List<TileData> mapBackgroundTileDataList,
        //object
        List<ObjectData> objectList,
        List<ButtonActivatedDoorStruct> mapButtonActivatedDoorDataList,
        List<ButtonActivatedObject> buttonActivatedObjectList,
        List<DialogueData> dialogueDataList,
        float cellSize,int dataType = 0, byte[] bytesImage = null,AudioType audioType = AudioType.None)
    {
        mapID = id;
        this.stageLevel = stageLevel;
        //tile
        mapTileDataList = tileList;
        mapHalfTileDataList = halfTileList;
        this.mapBackgroundTileDataList = mapBackgroundTileDataList;
        //object
        mapObjectDataList = objectList;
        this.startPosition = startPosition;
        this.mapExitObjectDataList = mapExitObjectDataList;
        this.mapButtonActivatedDoorDataList = mapButtonActivatedDoorDataList;
        this.buttonActivatedObjectList = buttonActivatedObjectList;
        this.dialogueDataList = dialogueDataList;

        this.mapSize = mapSize;
        this.cellSize = cellSize;
        this.dataType = dataType;//main and userData
        this.bytesImage = bytesImage;
        this.audioType = audioType;
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


   
    //Box,stringBox,key,
    //public Sprite LoadImage(int width, int height)
    //{
    //    Texture2D texture = new Texture2D(width, height, TextureFormat.ARGB32, false);
    //    texture.LoadImage(bytesImage);
    //    Sprite sprite = Sprite.Create(texture, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f), 100f);
    //    return sprite;
    //}


    //todo 0603

    //public void Test_CheckTile()
    //{
    //   foreach(TileData data in mapTileDataList)
    //    {
    //        if(min.sqrMagnitude > data.position.sqrMagnitude) { min = data.position; }
    //        if(max.sqrMagnitude < data.position.sqrMagnitude) { max = data.position; }
    //    }

    //    Debug.Log($"min : {min}, max : {max}");
    //}

}




[System.Serializable]
public struct ButtonActivatedObject
{
    public int id;
    public int linkId;
    public Vector2 position;
    public Vector3 scale;

    public ButtonActivatedObject(int id,int linkId,Vector2 position,Vector3 scale)
    {
        this.id = id;
        this.linkId = linkId;
        this.position = position;
        this.scale = scale;
    }

}


[System.Serializable]
public struct ButtonActivatedDoorStruct
{
    public int id;
    public int linkId;
    public int activeRequirAmount;
    public Vector2 position;
    public Quaternion quaternion;
    public Vector3 scale;

    public ButtonActivatedDoorStruct(int id, int linkId, int activeRequirAmount, Vector2 position,
        Quaternion quaternion,
        Vector3 scale)
    {
        this.id= id;
        this.linkId= linkId;
        this.activeRequirAmount = activeRequirAmount;
        this.position = position;
        this .quaternion = quaternion;
        this.scale = scale;
    }
}

[System.Serializable]
public struct ExitObjStruct
{
    public int id;
    public Vector2 position;
    public int condition_KeyAmount;
    public string nextMapId;

    public ExitObjStruct(int id,Vector2 position,int condition_KeyAmount,string nextMapId)
    {
        this.id = id;
        this.position = position;
        this.condition_KeyAmount = condition_KeyAmount;
        this.nextMapId = nextMapId;
    }

}


[System.Serializable]
public struct ObjectData
{
    public int id;
    public int dialogueId;
    public Vector2 position;
    public Quaternion quaternion;
    public Vector3 scale;
    
    public ObjectData(int id,Vector2 position,Vector3 scale,int dialogueId = 0)
    {
        this.id = id;
        this.dialogueId = dialogueId;
        this.position = position;
        quaternion = Quaternion.identity;
        this.scale = scale;
    }
    public ObjectData(int id, Vector2 position, Quaternion quaternion, Vector3 scale, int dialogueId = 0)
    {
        this.id = id;
        this.dialogueId = dialogueId;
        this.position = position;
        this.quaternion = quaternion;
        this.scale = scale;
    }



}
[System.Serializable]
public struct TileData
{
    public int id;
    public Vector3Int position;

    public TileData(Vector3Int position, int id)
    {
        this.id = id;
        this.position = position;
    }
}

[System.Serializable]
public struct DialogueData
{
    public int id;
    public int dialogueId;
    public bool excuted;
    public Vector2 position;
    public Quaternion quaternion;
    public Vector3 scale;

    public DialogueData(int id, int dialogueId,bool excuted, Vector2 position, Quaternion quaternion, Vector3 scale)
    {
        this.id = id;
        this.dialogueId = dialogueId;
        this.excuted = excuted;
        this.position = position;
        this.quaternion = quaternion;
        this.scale = scale;

    }
   
}
