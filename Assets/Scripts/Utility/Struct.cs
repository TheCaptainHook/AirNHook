using System;
using System.Collections.Generic;
using Unity.Mathematics;
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
    public Quaternion quaternion;
    public Vector3 scale;
    public List<Vector2> targetPositions;
    public List<Vector2> lightPositions;
    public List<Vector2> encapsulationItems;
    public bool chargeRequired;
    //puzzle_1
    public bool onHint;
    public Vector2[] partsPositions;
    public Vector2[] itemPositions;
    public Vector2 hintPosition;
    //CapsulationItem

   

    #region Primary Constructor
    private ButtonObjectStruct(
        int id, Vector2 position, Quaternion quaternion, Vector3 scale,
        List<Vector2> targetPositions,
        List<Vector2> lightPositions,
        List<Vector2> encapsulationItems,
        bool chargeRequired,
        bool onHint,
        Vector2[] partsPositions,
        Vector2[] itemPositions,
        Vector2 hintPosition
    )
    {
        this.id = id;
        this.position = position;
        this.quaternion = quaternion;
        this.scale = scale;

        this.targetPositions = targetPositions;
        this.lightPositions = lightPositions;
        this.encapsulationItems = encapsulationItems;

        this.chargeRequired = chargeRequired;

        this.onHint = onHint;
        this.partsPositions = partsPositions;
        this.itemPositions = itemPositions;
        this.hintPosition = hintPosition;
        
    }
    private ButtonObjectStruct(ButtonObjectStruct other) : this(
        other.id, other.position, other.quaternion, other.scale,
        other.targetPositions, other.lightPositions, other.encapsulationItems,
        other.chargeRequired,
        other.onHint, other.partsPositions, other.itemPositions, other.hintPosition
      
    ) { }
 
    #endregion
    private static ButtonObjectStruct Base(int id, Vector2 position, Quaternion quaternion, Vector3 scale,
        List<Vector2> targetPositions,
        List<Vector2> lightPositions,
        List<Vector2> encapsulationItems,
        bool chargeRequired) => new ButtonObjectStruct(
            id, position, quaternion, scale,
            targetPositions, lightPositions, encapsulationItems,
            chargeRequired,
            onHint: false,
            partsPositions: null,
            itemPositions: null,
            hintPosition: Vector2.zero
            );


    public ButtonObjectStruct(int id, Vector2 position, Quaternion quaternion, Vector3 scale,
        List<Vector2> targetPositions,
        List<Vector2> lightPositions,
        List<Vector2> encapsulationItems,
        bool chargeRequired) 
        : this(Base(id, position, quaternion, scale, targetPositions, lightPositions, encapsulationItems, chargeRequired)) { }

    //Puzzle_1
    public ButtonObjectStruct(int id, Vector2 position, Quaternion quaternion, Vector3 scale,
       List<Vector2> targetPositions,List<Vector2> lightPositions,List<Vector2> encapsulationItems,
       bool onHint,Vector2[] partsPositions,Vector2[] itemPositions,Vector2 hintPosition)
       : this(Base(id, position, quaternion, scale, targetPositions, lightPositions, encapsulationItems, false)) 
    {
        this.onHint = onHint;
        this.partsPositions = partsPositions;
        this.itemPositions = itemPositions;
        this.hintPosition = hintPosition;
    }

}
#endregion

#region  Button Activatable Object Struct
[System.Serializable]
public struct IndicatorStruct
{
    public INDICATOR indicator;
    public Vector2 indicator_1_position;
    public Vector2 indicator_2_position;
    public bool isHorizontal;
    public IndicatorStruct(
        INDICATOR indicator,
        Vector2 indicator_1_position,
        Vector2 indicator_2_position,
        bool isHorizontal
        )
    {
        this.indicator = indicator;
        this.indicator_1_position = indicator_1_position;
        this.indicator_2_position = indicator_2_position;
        this.isHorizontal = isHorizontal;
    }
}

[System.Serializable]
public struct ButtonActivatableObjectStruct
{
    public int id;
    public Vector2 position;
    public Quaternion quaternion;
    public Vector3 scale;
    public int activeRequirAmount;
    public IndicatorStruct indicatorStruct;
    //JumpingPad
    public int jumpingPower; 
    //Potal
    public Vector2 talPot;
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

    #region Primary Constructor
    private  ButtonActivatableObjectStruct
    (
        int id,
        Vector2 position,
        Quaternion quaternion,
        Vector3 scale,
        int activeRequirAmount,
        IndicatorStruct indicatorStruct,
        int jumpingPower,
        Vector2 talPot,
        float rotateRate,
        float fireRate,
        bool onHoldRotation,
        bool onLeft,
        Vector2[] paths,
        float moveSpeed,
        float moveDistance,
        float bridgeLength,
        Vector2 connectionPoint
    )
    {
        //Default
        this.id = id;
        this.position = position;
        this.quaternion = quaternion;
        this.scale = scale;
        this.activeRequirAmount = activeRequirAmount;
        this.indicatorStruct = indicatorStruct;
        //JumpingPad
        this.jumpingPower = jumpingPower;
        //Portal
        this.talPot = talPot;
        //Turret
        this.rotateRate = rotateRate;
        this.fireRate = fireRate;
        this.onHoldRotation = onHoldRotation;
        this.onLeft = onLeft;   
        //Moving Platform
        this.paths = paths;
        this.moveSpeed = moveSpeed;
        this.moveDistance = moveDistance;
        //Bridge
        this.bridgeLength = bridgeLength;
        this.connectionPoint = connectionPoint;
    }
    private ButtonActivatableObjectStruct(ButtonActivatableObjectStruct other) : this(
        other.id, other.position, other.quaternion, other.scale,other.activeRequirAmount, other.indicatorStruct,
        other.jumpingPower,
        other.talPot,
        other.rotateRate, other.fireRate, other.onHoldRotation, other.onLeft,
        other.paths, other.moveSpeed, other.moveDistance,
        other.bridgeLength, other.connectionPoint
    )
    {}
    #endregion

    private static ButtonActivatableObjectStruct Base(int id, Vector2 position, Quaternion quaternion, Vector3 scale, int activeRequirAmount, IndicatorStruct indicatorStruct) => new ButtonActivatableObjectStruct(
        id, position, quaternion, scale, activeRequirAmount, indicatorStruct,
        jumpingPower: 0,
        talPot: Vector2.zero,
        rotateRate: 0,
        fireRate: 0,
        onHoldRotation: false,
        onLeft: false,
        paths: null,
        moveSpeed: 0,
        moveDistance: 0,
        bridgeLength: 0,
        connectionPoint: Vector2.zero
    );
    //indicatorStruct [X]
     private static ButtonActivatableObjectStruct Base(int id, Vector2 position, Quaternion quaternion, Vector3 scale, int activeRequirAmount) => new ButtonActivatableObjectStruct(
       id, position, quaternion, scale, activeRequirAmount, default,
       jumpingPower: 0,
       talPot: Vector2.zero,
       rotateRate: 0,
       fireRate: 0,
       onHoldRotation: false,
       onLeft: false,
       paths: null,
       moveSpeed: 0,
       moveDistance: 0,
       bridgeLength: 0,
       connectionPoint: Vector2.zero
   );

    public ButtonActivatableObjectStruct(int id, Vector2 position, Quaternion quaternion, Vector3 scale, int activeRequirAmount, IndicatorStruct indicatorStruct)
    : this(Base(id, position, quaternion, scale, activeRequirAmount, indicatorStruct))
    { }
    
    //Portal
    public ButtonActivatableObjectStruct(int id, Vector2 position, Quaternion quaternion, Vector3 scale, int activeRequirAmount, IndicatorStruct indicatorStruct, Vector2 talPot)
    : this(Base(id, position, quaternion, scale, activeRequirAmount, indicatorStruct))
    {
        this.talPot = talPot;
    }
    //Jumping Pad
    public ButtonActivatableObjectStruct(int id, Vector2 position, Quaternion quaternion, Vector3 scale, int activeRequirAmount, IndicatorStruct indicatorStruct,int jumpingPower)
    : this(Base(id, position, quaternion, scale, activeRequirAmount, indicatorStruct))
    {
        this.jumpingPower = jumpingPower;
    }
    //Turret
    public ButtonActivatableObjectStruct(int id, Vector2 position, Quaternion quaternion, Vector3 scale, int activeRequirAmount, float rotateRate, float fireRate, bool onHoldRotation, bool onLeft)
    : this(Base(id, position, quaternion, scale, activeRequirAmount))
    {
        this.rotateRate = rotateRate;
        this.fireRate = fireRate;
        this.onHoldRotation = onHoldRotation;
        this.onLeft = onLeft;
    }
    //Moving Platform
    public ButtonActivatableObjectStruct(int id, Vector2 position, Quaternion quaternion, Vector3 scale, int activeRequirAmount, IndicatorStruct indicatorStruct,Vector2[] paths,float moveSpeed)
    : this(Base(id, position, quaternion, scale, activeRequirAmount, indicatorStruct))
    {
        this.paths = paths;
        this.moveSpeed = moveSpeed;
    }
    //WDMP
    public ButtonActivatableObjectStruct(int id, Vector2 position, Quaternion quaternion, Vector3 scale, int activeRequirAmount, IndicatorStruct indicatorStruct, float moveDistance, float moveSpeed)
    : this(Base(id, position, quaternion, scale, activeRequirAmount, indicatorStruct))
    {
        this.moveDistance = moveDistance;
        this.moveSpeed = moveSpeed;
    }
    //Bridge Box
    public ButtonActivatableObjectStruct(int id, Vector2 position, Quaternion quaternion, Vector3 scale, int activeRequirAmount, IndicatorStruct indicatorStruct, float bridgeLength, Vector2 connectionPoint)
    : this(Base(id, position, quaternion, scale, activeRequirAmount, indicatorStruct))
    {
        this.bridgeLength = bridgeLength;
        this.connectionPoint = connectionPoint;
    }
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
    //WorldTextObject
    public Vector2 size;
    public string text;
    public float fontSize;
    //Light Object
    public bool chargeRequired;
    //NPC
    public AnimationTriggerType animationTriggerType;
    //SpkieTrap
    public float attackStartTime;
    public float attackCooldown;
    //Interactable Object
    public int activeRequireAmount;
    public bool onEncapsulationItem;
    public INDICATOR indicator;
    //Tutorial_Drone
    public int tutorialCode;

    #region Primary Constructor
    private ObjectData(
        int id,
        Vector2 position,
        Quaternion quaternion,
        Vector3 scale,

        int dialogueId,
        Vector2 size,
        string text,
        float fontSize,
        bool chargeRequired,
        AnimationTriggerType animationTriggerType,
        float attackStartTime,
        float attackCooldown,
        bool onEncapsulationItem,
        int activeRequireAmount,
        INDICATOR indicator,
        int tutorialCode
        )
    {
        //Base
        this.id = id;
        this.position = position;
        this.quaternion = quaternion;
        this.scale = scale;
        //Dialogue
        this.dialogueId = dialogueId;
        //Fog
        this.size = size;
        //WorldTextObject
        this.text = text ?? string.Empty;
        this.fontSize = fontSize;
        //IPowerConsumer
        this.chargeRequired = chargeRequired;
        //NPC Object
        this.animationTriggerType = animationTriggerType;
        //SpikeTrap
        this.attackStartTime = attackStartTime;
        this.attackCooldown = attackCooldown;
        //Interactable Object
        this.onEncapsulationItem = onEncapsulationItem;
        this.activeRequireAmount = activeRequireAmount;
        this.indicator = indicator;
        //Tutorial_Drone
        this.tutorialCode = tutorialCode;
    }
    private ObjectData(ObjectData other) : this(
       other.id, other.position, other.quaternion, other.scale,
       other.dialogueId,
       other.size,
       other.text, other.fontSize,
       other.chargeRequired,
       other.animationTriggerType,
       other.attackStartTime, other.attackCooldown,
       other.onEncapsulationItem, other.activeRequireAmount,
       other.indicator,
       other.tutorialCode)
    { }

    #endregion

    private static ObjectData Base(int id, Vector2 position, Quaternion quaternion, Vector3 scale) => new ObjectData
    (
        id, position, quaternion, scale,
        dialogueId: 0,
        size: Vector2.zero,
        text: string.Empty,
        fontSize: 0f,
        chargeRequired: false,
        animationTriggerType: AnimationTriggerType.Idle,
        attackStartTime: 0f,
        attackCooldown: 0f,
        onEncapsulationItem: false,
        activeRequireAmount: 0,
        indicator: INDICATOR.NONE,
        tutorialCode: 0
    );
    private static ObjectData Base(int id, Vector2 position, Vector3 scale) => new ObjectData
    (
        id, position, Quaternion.identity, scale,
        dialogueId: 0,
        size: Vector2.zero,
        text: string.Empty,
        fontSize: 0f,
        chargeRequired: false,
        animationTriggerType: AnimationTriggerType.Idle,
        attackStartTime: 0f,
        attackCooldown: 0f,
        onEncapsulationItem: false,
        activeRequireAmount: 0,
        indicator: INDICATOR.NONE,
        tutorialCode: 0
    );

    public ObjectData(int id, Vector2 position, Quaternion quaternion, Vector3 scale)
    : this(Base(id, position, quaternion, scale))
    { }
    public ObjectData(int id, Vector2 position, Vector3 scale)
    : this(Base(id, position, scale))
    { }
    //EventEchoBlockTriggerObject
    public ObjectData(int id, Vector2 position, Vector3 offset, Vector3 size)
    : this(Base(id, position, offset))
    {
        this.size = size;
    }
    //IPowerConsumer
    public ObjectData(int id, Vector2 position, Quaternion quaternion, Vector3 scale, bool chargeRequired)
    : this(Base(id, position, quaternion, scale))
    {
        this.chargeRequired = chargeRequired;
    }

    //Fog
    public ObjectData(int id, Vector2 position, Quaternion quaternion, Vector2 scale, Vector2 size)
    : this(Base(id, position, quaternion, scale))
    {
        this.size = size;
    }

    //WorldTextObject
    public ObjectData(int id, Vector2 position, Quaternion quaternion, Vector2 scale, Vector2 size, string text, float fontSize)
    : this(Base(id, position, quaternion, scale))
    {
        this.size = size;
        this.fontSize = fontSize;
    }

    //NPC Object
    public ObjectData(int id, Vector2 position, Quaternion quaternion, Vector2 scale, AnimationTriggerType type)
    : this(Base(id, position, quaternion, scale))
    {
        animationTriggerType = type;
    }

    //SpikeTrap
    public ObjectData(int id, Vector2 position, Quaternion quaternion, Vector2 scale, float attackStartTime, float attackCooldown)
    : this(Base(id, position, quaternion, scale))
    {
        this.attackStartTime = attackStartTime;
        this.attackCooldown = attackCooldown;
    }

    //Interactable Object
    public ObjectData(int id, Vector2 position, Quaternion quaternion, Vector2 scale, bool onEncapsulationItem, int activeRequireAmount, INDICATOR indicator = INDICATOR.NONE)
    : this(Base(id, position, quaternion, scale))
    {
        this.onEncapsulationItem = onEncapsulationItem;
        this.activeRequireAmount = activeRequireAmount;
        this.indicator = indicator;
    }

    //Tutorial Drone
    public ObjectData(int id, Vector2 position, Quaternion quaternion, Vector2 scale, int tutorialCode)
    : this(Base(id, position, quaternion, scale))
    {
        this.tutorialCode = tutorialCode;
    }

}
#endregion

// [System.Serializable]
// public struct TileData
// {
//     public int id;
//     public Vector3Int position;

//     public TileData(Vector3Int position, int id)
//     {
//         this.id = id;
//         this.position = position;
//     }
// }
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

#region Network
[Serializable]
public struct Host_MapData
{
    public string mapId;
    public string subMapName;
    public bool clear;
    public bool onOpenStage;
    public Host_MapData(string mapId,string subMapName, bool clear, bool onOpenStage)
    {
        this.mapId = mapId;
        this.subMapName = subMapName;
        this.clear = clear;
        this.onOpenStage = onOpenStage;
    }
}
//[Serializable]
//public struct Host_MapDatas
//{
//    public Host_MapData[] data;
//    public Host_MapDatas(Host_MapData[] data)
//    {
//        this.data = data;
//    }
//}
#endregion

[System.Serializable]
public struct LaserEffectAudio
{
    public AudioSourceController source;
    public Vector2 hitPoint;
    public LaserEffectAudio(AudioSourceController source, Vector2 hitPoint)
    {
        this.source = source;
        this.hitPoint = hitPoint;
    }
}

namespace ANH_MapEditor
{
    public enum MapType
    {
        Scene,
        Main,
        User,
        Fork
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


}