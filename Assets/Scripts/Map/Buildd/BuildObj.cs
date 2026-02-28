using System.Collections;
using UnityEngine;
using System;
using UnityEngine.EventSystems;
using UnityEngine.Animations;
using Mirror;
using System.Linq;


public enum DistructionStatus
{
    Indestructible,
    Destructible,
    PermanentDestruction
}


[System.Serializable]
public class BuildObj : MousePointerEntity, IDamageable, IPooling
{
    [CustomHeader("BuildObj")]
    public int id;
    [Tooltip("Transform ID to be created")]
    [ReadOnly]
    public int transformID;
    [ReadOnly]
    public Vector2 position;

    [SerializeField] public DistructionStatus distructionStatus;

    [Space(20)]
    [Header(@"
    -------------IPowerConsumer Field
     * ↓ can use this field.
        - ToggleButton
        - Light Objects(현재 10개)

    ")]
    public bool chargeRequired = false;
    [Space(20)]

    #region Transform Item
    [ReadOnly]
    public bool isTransportItem;
    [ReadOnly]
    public uint carrierTransformNetId;
    [ReadOnly]
    public Transform carrierTransform;
    #endregion

    #region User Editor
    [Header("User Editor-only parameter")]
    [ReadOnly]
    public bool onPlaceable;
    [ReadOnly]
    public bool onRotateable;
    [ReadOnly]
    public bool onScaleable;
    [ReadOnly]
    public Vector2 offset; // Use this parameter in editor mode.
    [ReadOnly]
    public bool turnOff;
    #endregion


    private ObjectData _objectData;
    public ObjectData ObjectData
    {
        get
        {
            return _objectData;
        }
        set
        {
            _objectData = value;
            id = _objectData.id;
            position = value.position;
        }

    }
    private Rigidbody2D Rb;
    public Rigidbody2D _rb
    {
        get
        {
            if (Rb == null) Rb = GetComponent<Rigidbody2D>();
            return Rb;
        }
    }
    private Collider2D _collider;
    public Collider2D Col
    {
        get
        {
            if (_collider == null) _collider = GetComponent<Collider2D>();
            return _collider;

        }

    }

    [Header("Only use Editor mode")]
    [HideInInspector] public bool setPosition; // When created and placed set this parameter
    [HideInInspector] public Vector2 orgPosition;


    #region  Dissolve Effect
    [Header("Dissolve Effect")]
    // [SerializeField] SpriteRenderer _Dissolve_MainSprite;
    [SerializeField] SpriteRenderer[] _Dissolve_MainSprites;
    //private static readonly int DissolveAmount = Shader.PropertyToID("_DissolveAmount");

    //------------Dissolve Modify 0804
    // protected Material _dissolveMaterial;
    public Material[] _dissolveMaterials;
    // public Material DissolveMaterial => _dissolveMaterial;
    //------------Dissolve Modify 0804

    //public event Action<Vector2> OnDissolveAction;
    public event Action OnDissolveAction;
    public event Action OnDisableAction;
    public event Action OnInteractableObjectRelease;
    protected bool _IsDissolveObject;

    #endregion



    #region Audio
    protected virtual void StartSound()
    {

    }
    #endregion

    public void CallOnInterableObjectRelease()
    {
        OnInteractableObjectRelease?.Invoke();
    }


    public virtual void SetData(ObjectData data)
    {
        ObjectData = data;
        position = data.position;
        transform.position = data.position;
        transform.rotation = data.quaternion;
        transform.localScale = data.scale;

        StartSound();
    }

    public virtual T GetData<T>()
    {
        if (typeof(T) == typeof(ObjectData))
        {
            return (T)(object)new ObjectData(id, ConvertPosition(), transform.rotation, transform.localScale);
        }

        return default(T);
    }
    public virtual void SetData<T>(T data)
    {
        if (typeof(T) == typeof(ObjectData)) {
            ObjectData objData = (ObjectData)(object)data;
            SetData(objData);
        }

    }
    private Vector3 ConvertPosition()
    {
        Vector3 original = transform.position;

        Vector3 rounded = new Vector3(
            Mathf.Round(original.x * 100f) / 100f,
            Mathf.Round(original.y * 100f) / 100f,
            Mathf.Round(original.z * 100f) / 100f
        );
        return rounded;
    }
    #region Transport Item 
    public void SettingTransportItem(GameObject carrierObj) //Only Server
    {
        if (_rb == null)
        {
            Debug.Log("Can't find Rigidbody2D");
            return;
        }

        carrierTransformNetId = carrierObj.GetComponent<NetworkIdentity>().netId;
        carrierTransform = carrierObj.GetComponent<Drone_MultiPurpose>().itemPlacementPosition;

        Connection_TransportItem();

    }
    public void Connection_TransportItem()//Only Server
    {
        ParentConstraint constraint = gameObject.TryGetComponent(out ParentConstraint component) ? component : gameObject.AddComponent<ParentConstraint>();
        SetParentConstraint(constraint, carrierTransform);

        GetComponent<ITransportItem>().TransportItem_Constraint(carrierTransformNetId);
        
    }
    
    public void DropTransportItem()//Only Server
    {
        if (!NetworkServer.active) return;

        if (TryGetComponent(out ParentConstraint constraint))
        {
            //Destroy(constraint);
            // if (constraint.sourceCount > 0)
            //     constraint.RemoveSource(0); 
            ParentConstranintClean(constraint);
        }

        GetComponent<ITransportItem>().TransportItem_DropItem();
    }
    private void ParentConstranintClean(ParentConstraint constraint)
    {
        for(int i = constraint.sourceCount -1; i >=0; i--)
        {
            constraint.RemoveSource(i);
        }
    }
    private void SetParentConstraint(ParentConstraint constraint, Transform parent)//Only Server
    {
        constraint.weight = 1;
        transform.position = parent.position;
        ConstraintSource source = new ConstraintSource
        {
            sourceTransform = parent,
            weight = 1.0f
        };

        constraint.AddSource(source);

        // 트랜스폼 옵션 설정 (위치와 회전을 따라가도록 설정)
        constraint.translationAtRest = transform.localPosition;
        constraint.rotationAtRest = transform.localRotation.eulerAngles;

        // 위치와 회전을 활성화
        constraint.translationOffsets = new Vector3[constraint.sourceCount];
        constraint.rotationOffsets = new Vector3[constraint.sourceCount];
        constraint.constraintActive = true;

        // 속성 업데이트
        constraint.locked = true; // 소스가 변경되지 않도록 잠금

    }
    #endregion

    public virtual void TakeDamage(DamageType damageType = DamageType.Default)
    {
        if (damageType == DamageType.Destruction) return;

        if (distructionStatus == DistructionStatus.Destructible)
        {
            if (Managers.Game.CurrentState != GameState.Editor)
            {
                OnInteractableObjectRelease?.Invoke();
            }
            //OnDissolveAction?.Invoke(position);
            OnDissolveAction?.Invoke();
            OnDisableAction?.Invoke();
        }

        if (distructionStatus == DistructionStatus.PermanentDestruction)
        {
            Destroy(gameObject);
        }
    }


    public virtual void TurnOff()
    {
        if (setPosition)
        {
            transform.position = orgPosition;

        }

    }
    public virtual void TurnOn()
    {
        SetOrgPosition();
    }

    public virtual void Reset()
    {

    }

    public virtual void EditorMode_Destroy()
    {
        if (MapEditor.Instance.placeMentSystem.curPlaceObjList.Contains(this))
        {
            MapEditor.Instance.placeMentSystem.curPlaceObjList.Remove(this);
            Destroy(gameObject);
            return;
        }
        Destroy(gameObject);
    }


    public override void OnPointerClick(PointerEventData data)
    {
        if (!MapEditor.Instance) return;
        if (MapEditor.Instance.mapEditorState == MapEditorState.Object)
        {
            if (MapEditor.Instance.placeMentSystem.CurbuildObject != data.pointerCurrentRaycast.gameObject)
            {
                if (data.pointerCurrentRaycast.gameObject.GetComponent<BuildObj>())
                {
                    Debug.Log("BUildObj");
                    MapEditor.Instance.placeMentSystem.CurbuildObject = data.pointerCurrentRaycast.gameObject;
                }

            }
        }
    }
    //todo 0427

    //todo 0427
    [field: Header("Object Drop Sound")]
    [field: SerializeField] private LayerMask _floorLayerMask;
    public ObjectDropSoundEnum objectDropSound;
    public float soundVolume = 1f;
    public int soundDistance = 10;

    protected void OnCollisionEnter2D(Collision2D collision)
    {
        if ((_floorLayerMask.value & (1 << collision.gameObject.layer)) == 0) return;

        if (objectDropSound == ObjectDropSoundEnum.None) return;

        if (!GlobalText.DropSoundDictionary.TryGetValue(objectDropSound, out var sound)) return;

        Managers.Sound.PlaySound3D(sound, transform.position, soundVolume, false, soundDistance, true);
    }

    #region Destructible Obj Dissolve Effect Logic
    [ReadOnly]
    public bool canRespawn;
    public void DissolveInitSetting()
    { //all Client
      //------------Dissolve Modify 0804
        _dissolveMaterials = new Material[_Dissolve_MainSprites.Length];
        for (int i = 0; i < _Dissolve_MainSprites.Length; i++)
        {
            _dissolveMaterials[i] = _Dissolve_MainSprites[i].material;
        }
        //------------Dissolve Modify 0804

        _IsDissolveObject = true;
        AddDissolveAction(Respawn);

        // OnDissolveAction += Respawn;


        if (TryGetComponent(out InteractableObject component))
        {
            //OnInteractableObjectRelease += GetComponent<InteractableObject>().Destroyed;
            OnInteractableObjectRelease += component.Destroyed;
        }

        canRespawn = true;

        if (NetworkServer.active) //Server
            // MapEditor.Instance.event_reset += Respawn;
            MapEditor.Instance.AddEvent_Reset(Respawn);
    }
    public void AddDissolveAction(Action action)
    {
        if(OnDissolveAction == null || !OnDissolveAction.GetInvocationList().Contains(action))
        {
            OnDissolveAction += action;
        }
    }
    public void RemoveDissolveAction(Action action)
    {
        if(OnDissolveAction != null && OnDissolveAction.GetInvocationList().Contains(action))
        {
            OnDissolveAction -= action;
        }
    }
    public void DissolveClean()
    {
        _IsDissolveObject = false;
        // OnDissolveAction -= Respawn;
        RemoveDissolveAction(Respawn);

        if (TryGetComponent(out InteractableObject component))
        {
            OnInteractableObjectRelease -= component.Destroyed;
        }

        if (NetworkServer.active) //Server
            // MapEditor.Instance.event_reset -= Respawn;
            MapEditor.Instance.Remove_Event_Reset(Respawn);
    }

    public event Action respawnEvent;
    public void Respawn()
    {
        if (!canRespawn) return;
        if (this == null) return;

        respawnEvent?.Invoke(); //Only Server

        if(TryGetComponent(out InteractableObject obj))
        {
            obj.Server_Dissolve();   
        }

        // if (TryGetComponent(out TransportItemEntity component))
        // {
        //     // component.Server_Dissolve(); //Only Server
        //     canRespawn = false;
        // }

        // if(TryGetComponent(out Puzzle_1_Item item))
        // {
        //     // item.Server_Dissolve();
            
        // }

    }


    #endregion

    #region  Editor
    // public virtual void Editor_Setting(Transform transform){}
    public virtual void Editor_Setting(MapEditor mapEditor) { }

    public void SetOrgPosition()
    {
        setPosition = true;
        orgPosition = transform.position;
    }

    #endregion


    public uint GetNetworkId()
    {
        if (TryGetComponent(out NetworkIdentity identity))
        {
            return identity.netId;
        }

        return 9999;
    }

    public void D_ReleaseToPool()
    {
        Managers.Pooling.D_ReleaseToPool(gameObject);
    }
    public void N_ReleaseToPool()
    {
        Managers.Pooling.N_ReleaseToPool(gameObject);
    }

#region  Clean
    public virtual void Clean()
    {
        
    }
    
#endregion
}   

