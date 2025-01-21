using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.Universal.Light2D;

#region  Struct

[System.Serializable]
  public struct CollectableObjectStruct{
    public int id;
    public Vector2 position;
    public Quaternion quaternion;
    public bool isFound;

    public CollectableObjectStruct(int id,Vector2 position,Quaternion quaternion,bool isFound = false){
        this.id = id;
        this.position= position;
        this.quaternion= quaternion;
        this.isFound = isFound;
    }
  }

#region  Button Object Struct
[System.Serializable]
public struct ButtonObjectStruct
{
    public int id;
    public Vector2 position;
    public Vector3 scale;
    public List<Vector2> targetPositions;
    //puzzle_1
    public bool onHint;
    public Vector2[] partsPositions;
    public Vector2[] itemPositions;
    public Vector2 hintPosition;
    public bool chargeRequired;
    public ButtonObjectStruct(int id,List<Vector2> targetPositions,Vector2 position,Vector3 scale,bool chargeRequired = false)
    {
        this.id = id;
        this.targetPositions = targetPositions;
        this.position = position;
        this.scale = scale;
        partsPositions = null;
        itemPositions = null;
        onHint = false;
        hintPosition = Vector2.zero;
        this.chargeRequired = chargeRequired;
    }

    public ButtonObjectStruct(int id, List<Vector2> targetPositions, Vector2 position, Vector3 scale,
    Vector2[] partsPositions,
    Vector2[] itemPositions,
    bool onHint,
    Vector2 hintPosition = default)
    {
        this.id = id;
        this.targetPositions = targetPositions;
        this.position = position;
        this.scale = scale;
        this.partsPositions = partsPositions;
        this.itemPositions = itemPositions;
        this.onHint = onHint;
        this.hintPosition = hintPosition;
        chargeRequired = false;
    }

}
#endregion

#region  Button Activatable Object Struct
[System.Serializable]
public struct ButtonActivatableObjectStruct
{
    public int id;
    public int activeRequirAmount;
    public bool chargeRequired;
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
    //Bridge Box
    public float bridgeLength;
    public Vector2 connectionPoint;

    #region Default
    public ButtonActivatableObjectStruct(int id, int activeRequirAmount, Vector2 position,
        Quaternion quaternion,
        Vector3 scale,
        Vector2 talPot = default,
        bool chargeRequired = false
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
        bridgeLength = 0;
        connectionPoint = Vector2.zero;
        this.chargeRequired = chargeRequired;

    }
    #endregion
    #region JumpingPad
    public ButtonActivatableObjectStruct(int id, int activeRequirAmount, Vector2 position,
       Quaternion quaternion,
       Vector3 scale,
       int jumpingPower,
       bool chargeRequired = false

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
        bridgeLength = 0;
        connectionPoint = Vector2.zero;
        this.chargeRequired = chargeRequired;
    }

    #endregion
    #region Turret
    public ButtonActivatableObjectStruct(int id, int activeRequirAmount, Vector2 position,
      Quaternion quaternion,
      Vector3 scale,
      float rotateRate,
      float fireRate,
      bool onHoldRotation,
      bool onLeft = false,
      bool chargeRequired = false
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
        bridgeLength = 0;
        connectionPoint = Vector2.zero;
        this.chargeRequired = chargeRequired;
        
    }
    #endregion
    #region MovingPlatform
     public ButtonActivatableObjectStruct(int id, int activeRequirAmount, Vector2 position,
        Quaternion quaternion,
        Vector3 scale,
        Vector2[] paths,
        float moveSpeed,
        bool chargeRequired = false
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
        bridgeLength = 0;
        connectionPoint = Vector2.zero;
        this.chargeRequired = chargeRequired;

    }
    #endregion
    #region  Weight Detection Moving Platform
     public ButtonActivatableObjectStruct(int id, int activeRequirAmount, Vector2 position,
        Quaternion quaternion,
        Vector3 scale,
        float moveDistance,
        float moveSpeed,
        bool chargeRequired = false
        
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
        bridgeLength = 0;
        connectionPoint = Vector2.zero;
        this.chargeRequired = chargeRequired;
    }
    #endregion
    #region  BridgeBox
      public ButtonActivatableObjectStruct(int id, int activeRequirAmount, Vector2 position,
        Quaternion quaternion,
        Vector3 scale,
        float bridgeLength,
        Vector2 connectionPoint,
        bool chargeRequired = false
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
        moveDistance = 0;
        moveSpeed = 0;
        this.bridgeLength = bridgeLength;
        this.connectionPoint= connectionPoint;
        this.chargeRequired = chargeRequired;
    }
    #endregion

}
#endregion


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

#region  Object Data
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
    public ObjectData(int id, Vector2 position,Vector2 size)
    {
        this.id = id;
        this.dialogueId = 0;
        this.position = position;
        quaternion = Quaternion.identity;
        this.scale = Vector3.one;
        this.talPot = Vector2.zero;
        this.size = size;
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
#endregion

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
//----------------------------------------------------------------250121 Refectoring
[System.Serializable]
public struct CompressedTileData
{
    public int TileId;
    public Vector2Int Start;
    public Vector2Int End;

    public CompressedTileData(int tileId, Vector2Int start,Vector2Int end)
    {
        TileId = tileId;
        Start = start;
        End = end;
    }

    public void Extend(Vector2Int newEnd)
    {
        End = newEnd;
    }
}
//----------------------------------------------------------------250121 Refectoring
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
#region  Drone
[System.Serializable]
public struct DroneStruct{
    public int id;
    public Vector2 position;
    public Quaternion quaternion;
    public Vector3 scale;
    public Vector2[] paths;
    public float moveSpeed;
    public Drone_TransportItemType drone_TransportItemType;

    public DroneStruct(int id,Vector2 position,Vector3 scale,Vector2[] paths,float moveSpeed,Drone_TransportItemType drone_TransportItemType = Drone_TransportItemType.None){
        this.id = id;
        this.position = position;
        this.scale = scale;
        this.paths = paths;
        this.moveSpeed = moveSpeed;
        quaternion = Quaternion.identity;
        this.drone_TransportItemType = drone_TransportItemType;
    }


}
#endregion
#endregion
#region Shadow
[System.Serializable]
public struct ShadowCasterStruct{
    public Vector2 position;
    public bool selfShadows;
    public int[] sortingLayers;
    public Vector3[] vertices;

    public ShadowCasterStruct(Vector2 position ,bool selfShadows,int[] sortingLayers,Vector3[] vertices)
    {
        this.position = position;
        this.selfShadows = selfShadows;
        this.sortingLayers = sortingLayers;
        this.vertices = vertices;
    }
}
#endregion
#region Light
[System.Serializable]
public struct LightStruct{
    public UnityEngine.Rendering.Universal.Light2D.LightType type;
    public Color color;
    public float intensity;
    public int[] targetSorting;
    public int blendStyleIndex;
    public int lightOrder;
    public OverlapOperation overlapOeration;

    public LightStruct(
        UnityEngine.Rendering.Universal.Light2D.LightType type,
        Color color,
        float intensity, 
        int[] targetSorting,
        int blendStyleIndex,
        int lightOrder,
         OverlapOperation overlapOeration
        )
    {
        this.type = type;
        this.color = color;
        this.intensity = intensity;
        this.targetSorting = targetSorting;
        this.blendStyleIndex = blendStyleIndex;
        this.lightOrder = lightOrder;
        this.overlapOeration = overlapOeration;
    }

    public LightStruct Default()
    {
        return new LightStruct(
            UnityEngine.Rendering.Universal.Light2D.LightType.Global, 
            Color.white,                                             
            1f,
            new int[] {0},
            0,
            0,
            OverlapOperation.Additive
        );
    }
}
#endregion