using System.Collections;
using System.Collections.Generic;
using Org.BouncyCastle.Asn1.Mozilla;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;



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
    //TODO 1022
    public List<TileData> mapRopeTileDataList = new();
    public List<TileData> mapAccessoryTIleDataList = new();

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

    public int dataType; //0:Main,1:User
    public float cellSize;
    [HideInInspector]public byte[] bytesImage;
    public AudioType audioType;

    //1101
    public string nextMapId;

    public Map(Vector2 mapSize, string id,
        string nextMapId,
        int stageLevel, Vector2 startPosition,
        List<ExitObjStruct> mapExitObjectDataList,
        //tile
        List<TileData> tileList,
        List<TileData> halfTileList,
        List<TileData> mapBackgroundTileDataList,
        List<TileData> ropeTileDataList,
        List<TileData> accessoryTileDataList,
        //object
        List<ObjectData> objectList,
        List<ObjectData> backgroundObjectList,
        List<ObjectData> otherObjectList,
        List<ButtonActivatableObjectStruct> mapButtonActivatabledObjectDataList,
        List<ButtonObjectStruct> buttonObjectList,
        List<DialogueData> dialogueDataList,
        List<DroneStruct> droneStructList,
        float cellSize,int dataType = 0, byte[] bytesImage = null,AudioType audioType = AudioType.None
        )
    {
        mapID = id;
        this.nextMapId = nextMapId;
        this.stageLevel = stageLevel;
        //tile
        mapTileDataList = tileList;
        mapHalfTileDataList = halfTileList;
        this.mapBackgroundTileDataList = mapBackgroundTileDataList;
        mapRopeTileDataList = ropeTileDataList;
        mapAccessoryTIleDataList = accessoryTileDataList;
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


    public (Vector2 start,Vector2 end) GetStartEndPosition() //TODO 0807 GEt Map Size
    {
        Vector2 start = new Vector2(mapTileDataList[0].position.x, mapTileDataList[0].position.y);
        Vector2 end = new Vector2(mapTileDataList[mapTileDataList.Count-1].position.x, mapTileDataList[mapTileDataList.Count - 1].position.y);
        return (start, end);
    }
}


#region  Struct

[System.Serializable]
public struct ButtonObjectStruct
{
    public int id;
    public Vector2 position;
    public Vector3 scale;
    public List<Vector2> targetPositions;

    public ButtonObjectStruct(int id,List<Vector2> targetPositions,Vector2 position,Vector3 scale)
    {
        this.id = id;
        this.targetPositions = targetPositions;
        this.position = position;
        this.scale = scale;
    }

}


[System.Serializable]
public struct ButtonActivatableObjectStruct
{
    public int id;
    public int activeRequirAmount;
    public int jumpingPower; //JumpingPad
    public Vector2 position;
    public Quaternion quaternion;
    public Vector3 scale;
    public Vector2 talPot;//Potal
    //Turret
    public float rotateRate;
    public float fireRate;
    public bool onHoldRotation;
    public bool onLeft;
    //MovingPlatform
    public Vector2[] paths;
    public float moveSpeed;
    //Weight Detection Moving Platform
    public float moveDistance;

    #region Default
    public ButtonActivatableObjectStruct(int id, int activeRequirAmount, Vector2 position,
        Quaternion quaternion,
        Vector3 scale,
        Vector2 talPot = default
        )
    {
        this.id= id;
        this.activeRequirAmount = activeRequirAmount;
        this.position = position;
        this .quaternion = quaternion;
        this.scale = scale;
        this.talPot = talPot;
        jumpingPower = 0;
        rotateRate = 0;
        fireRate = 0;
        onLeft = false;
        onHoldRotation = false;
        paths = null;
        moveSpeed = 0;
        moveDistance = 0;

    }
    #endregion
    #region JumpingPad
    public ButtonActivatableObjectStruct(int id, int activeRequirAmount, Vector2 position,
       Quaternion quaternion,
       Vector3 scale,
       int jumpingPower
       )
    {
        this.id = id;
        this.activeRequirAmount = activeRequirAmount;
        this.position = position;
        this.quaternion = quaternion;
        this.scale = scale;
        talPot = Vector2.zero;
        this.jumpingPower = jumpingPower;
        rotateRate = 0;
        fireRate = 0;
        onLeft = false;
        onHoldRotation = false;
        paths = null;
        moveSpeed = 0;
        moveDistance = 0;
    }
    #endregion
    #region Turret
    public ButtonActivatableObjectStruct(int id, int activeRequirAmount, Vector2 position,
      Quaternion quaternion,
      Vector3 scale,
      float rotateRate,
      float fireRate,
      bool onHoldRotation = false,
      bool onLeft = false
      )
    {
        this.id = id;
        this.activeRequirAmount = activeRequirAmount;
        this.position = position;
        this.quaternion = quaternion;
        this.scale = scale;
        talPot = Vector2.zero;
        jumpingPower = 0;
        this.rotateRate = rotateRate;
        this.fireRate = fireRate;
        this.onHoldRotation = onHoldRotation;
        this.onLeft = onLeft;
        paths = null;
        moveSpeed = 0;
        moveDistance = 0;
    }
    #endregion
    #region MovingPlatform
     public ButtonActivatableObjectStruct(int id, int activeRequirAmount, Vector2 position,
        Quaternion quaternion,
        Vector3 scale,
        Vector2[] paths,
        float moveSpeed
        )
    {
        this.id= id;
        this.activeRequirAmount = activeRequirAmount;
        this.position = position;
        this .quaternion = quaternion;
        this.scale = scale;
        talPot = Vector2.zero;
        jumpingPower = 0;
        rotateRate = 0;
        fireRate = 0;
        onLeft = false;
        onHoldRotation = false;
        this.paths = paths;
        this.moveSpeed = moveSpeed;
        moveDistance = 0;

    }
    #endregion
    #region  Weight Detection Moving Platform
     public ButtonActivatableObjectStruct(int id, int activeRequirAmount, Vector2 position,
        Quaternion quaternion,
        Vector3 scale,
        float moveDistance,
        float moveSpeed
        
        )
    {
        this.id= id;
        this.activeRequirAmount = activeRequirAmount;
        this.position = position;
        this .quaternion = quaternion;
        this.scale = scale;
        talPot = Vector2.zero;
        jumpingPower = 0;
        rotateRate = 0;
        fireRate = 0;
        onLeft = false;
        onHoldRotation = false;
        paths = null;
        this.moveDistance = moveDistance;
        this.moveSpeed = moveSpeed;

    }
    #endregion
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
    //potal
    public Vector2 talPot;
    //WorldTextObject
    public Vector2 size;
    public string text;
    public float fontSize;
    public ObjectData(int id, Vector2 position, Vector3 scale, int dialogueId = 0, Vector2 talPot = default)
    {
        this.id = id;
        this.dialogueId = dialogueId;
        this.position = position;
        quaternion = Quaternion.identity;
        this.scale = scale;
        this.talPot = talPot;
        this.size = Vector2.zero;
        this.text = string.Empty;
        this.fontSize = 0;
    }
    public ObjectData(int id, Vector2 position, Quaternion quaternion, Vector3 scale, int dialogueId = 0)
    {
        this.id = id;
        this.dialogueId = dialogueId;
        this.position = position;
        this.quaternion = quaternion;
        this.scale = scale;
        this.talPot = Vector2.zero;
        this.size = Vector2.zero;
        this.text = string.Empty;
        this.fontSize = 0;
    }
    //WorldTextObject
    public ObjectData(int id,Vector2 position,Vector2 size,string text,float fontSize)
    {
        this.id = id;
        this.dialogueId = 0;
        this.position = position;
        this.quaternion = Quaternion.identity;
        this.scale = Vector3.one;
        this.talPot = Vector2.zero;
        this.size = size;
        this.text = text;
        this.fontSize = fontSize;
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
[System.Serializable]
public struct DroneStruct{
    public int id;
    public Vector2 position;
    public Quaternion quaternion;
    public Vector3 scale;
    public Vector2[] paths;
    public float moveSpeed;

    public DroneStruct(int id,Vector2 position,Vector3 scale,Vector2[] paths,float moveSpeed){
        this.id = id;
        this.position = position;
        this.scale = scale;
        this.paths = paths;
        this.moveSpeed = moveSpeed;
        quaternion = Quaternion.identity;
    }

}

#endregion
