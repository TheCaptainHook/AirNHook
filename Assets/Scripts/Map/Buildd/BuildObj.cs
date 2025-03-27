using System.Collections;
using UnityEngine;
using System;
using UnityEngine.EventSystems;
using UnityEngine.Animations;
using Mirror;
public enum DistructionStatus
{
    Indestructible,
    Destructible,
    PermanentDestruction
}


[System.Serializable]
public class BuildObj : MousePointerEntity, IDamageable,IPooling
{
    [CustomHeader("BuildObj")]
    public int id;
    [Tooltip("Transform ID to be created")]
    [ReadOnly]
    public int transformID;
    [ReadOnly]
    public Vector2 position;

    [SerializeField] protected DistructionStatus distructionStatus;

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
    public ObjectData ObjectData{
         get{ 
                return _objectData;
            } 
         set{
                _objectData = value; 
                id = _objectData.id; 
                position = value.position;
            } 
    }


    [Header("Only use Editor mode")]
    [HideInInspector] public bool setPosition; // When created and placed set this parameter
    [HideInInspector] public Vector2 orgPosition;


    #region  Dissolve Effect
    [Header("Dissolve Effect")]
    [SerializeField] SpriteRenderer _Dissolve_MainSprite;
    //private static readonly int DissolveAmount = Shader.PropertyToID("_DissolveAmount");

    protected Material _dissolveMaterial;
    public Material DissolveMaterial => _dissolveMaterial;
    private Rigidbody2D Rb;
    protected Rigidbody2D _rb
    {
        get{
            if(Rb == null) Rb = GetComponent<Rigidbody2D>();
            return Rb;
        }
    }
    protected Collider2D Collider;
    protected Collider2D _collider{
        get{
            if(Collider == null) Collider = GetComponent<Collider2D>();
            return Collider;
        }
    }
    //float dissolveRate = 0.015f;

    //public event Action<Vector2> OnDissolveAction;
    public event Action OnDissolveAction;
    public event Action OnDisableAction;
    public event Action OnInteractableObjectRelease;
    protected bool _IsDissolveObject;

    #endregion





    public void CallOnInterableObjectRelease()
    {
        OnInteractableObjectRelease?.Invoke();
    }


    // public void SetTileData(Vector2 position)
    // {
    //     ObjectData = new ObjectData(id, position,transform.localScale);
    // }

    // public  void SetTileData(Vector2 position,Quaternion quaternion)
    // {
    //     ObjectData = new ObjectData(id, position, quaternion,transform.localScale);
    // }

    // public virtual void SetTileData()
    // {
    //     ObjectData = new ObjectData(id, transform.position, transform.rotation, transform.localScale);
    // }

    public virtual void SetData(ObjectData data)
    {
        ObjectData = data;
        transform.position = data.position;
        transform.rotation = data.quaternion;
        transform.localScale = data.scale;
    }
  
    public virtual T GetData<T>()  
    {
        if(typeof(T)==typeof(ObjectData)){
            return (T)(object)new ObjectData(id,transform.position,transform.rotation,transform.localScale);
        }

       return default(T);
    }
    public virtual void SetData<T>(T data)  
    {
        if(typeof(T) == typeof(ObjectData)){
            ObjectData objData = (ObjectData)(object)data;
            SetData(objData);
        }

    }
#region Transport Item 
    public void SettingTransportItem(GameObject carrierObj) //Only Server
    {
      if(_rb == null)
      {
        Debug.Log("Can't find Rigidbody2D");
        return;
      }
      //-------------------------------------------Network Sync
        // _rb.gravityScale = 0;
        // _collider.enabled = false;

        // this.carrierTransform = carrierTransform;
        // transform.position = carrierTransform.position;
       

        // // transform.SetParent(carrierTransform);
        // isTransportItem = true;
        // GetComponent<ITransportItem>().TransportItem_Constraint(carrierTransform);
        // if(TryGetComponent(out NetworkIdentity component))
        // {
        //     // GetComponent<ITransportItem>().TransportItem_Constraint(component.netId);
        //     //  //연결시키기
        //     // ParentConstraint constraint = gameObject.AddComponent<ParentConstraint>();
        //     // SetParentConstraint(constraint,carrierTransform);
        // }
        // carrierTransformNetId = netId;
            // carrierTransformNetId = netId;
            // GetComponent<ITransportItem>().TransportItem_Constraint(netId);

             //연결시키기
            // if(NetworkClient.spawned.TryGetValue(netId,out NetworkIdentity identity))
            // {
            //     var drone = identity.GetComponent<Drone_MultiPurpose>();
            //     carrierTransform = drone.itemPlacementPosition;
                
               
            // }
        // this.carrierTransform = carrierTransform;
        carrierTransformNetId = carrierObj.GetComponent<NetworkIdentity>().netId;
        carrierTransform = carrierObj.GetComponent<Drone_MultiPurpose>().itemPlacementPosition;

        
        Connection_TransportItem();
       
        //-------------------------------------------Network Sync
        
    }
    public void Connection_TransportItem()//Only Server
    {

        //ParentConstraint constraint = gameObject.AddComponent<ParentConstraint>();
        ParentConstraint constraint = gameObject.TryGetComponent(out ParentConstraint component) ? component : gameObject.AddComponent<ParentConstraint>();
        SetParentConstraint(constraint,carrierTransform);

        GetComponent<ITransportItem>().TransportItem_Constraint(carrierTransformNetId);
    }
    public void DropTransportItem()//Only Server
    {
        if(!NetworkServer.active) return;
        // transform.SetParent(MapEditor.Instance.networkingObjectTransform);
        if(TryGetComponent(out ParentConstraint constraint))
        {
            Destroy(constraint);
        }
        
        // _collider.enabled =true;
        // _rb.gravityScale =1;
        GetComponent<ITransportItem>().TransportItem_DropItem();
    }
    private void SetParentConstraint(ParentConstraint constraint,Transform parent)//Only Server
    {
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
        if(distructionStatus == DistructionStatus.Destructible)
        {
            if(Managers.Game.CurrentState != GameState.Editor)
            {
                OnInteractableObjectRelease?.Invoke();
            }
            //OnDissolveAction?.Invoke(position);
            OnDissolveAction?.Invoke();
            OnDisableAction?.Invoke();
        }

        if(distructionStatus == DistructionStatus.PermanentDestruction)
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
        if(MapEditor.Instance.mapEditorState == MapEditorState.Object)
        {
            if(MapEditor.Instance.placeMentSystem.CurbuildObject != data.pointerCurrentRaycast.gameObject)
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

    //public void SelectObjAndApplyOutline_EditorMode()
    //{
    //    //if(outlineBox != null) Destroy(outlineBox);
    //    outlineBox = new GameObject("OutileBox");
    //    outlineBox.transform.SetParent(transform);

    //    SpriteRenderer[] spriteRenderers = transform.GetComponentsInChildren<SpriteRenderer>();

    //    for (int i = 0; i < spriteRenderers.Length; i++)
    //    {
    //        GameObject obj = new GameObject("MeshObj");
    //        obj.transform.SetParent(outlineBox.transform);
    //        obj.transform.SetPositionAndRotation(transform.position, transform.rotation);

    //        MeshRenderer mr = obj.AddComponent<MeshRenderer>();
    //        MeshFilter mf = obj.AddComponent<MeshFilter>();

    //        Mesh mesh = new Mesh();
    //        spriteRenderers[i].Bake
    //    }
    //} 




    #region Destructible Obj Dissolve Effect Logic
    public bool canRespawn;
    protected void DissolveInitSetting(){
        _dissolveMaterial = _Dissolve_MainSprite.material;
        // _rb = GetComponent<Rigidbody2D>();
        // _collider = GetComponent<Collider2D>();
        _IsDissolveObject = true;
        OnDissolveAction += Respawn;
        OnInteractableObjectRelease += GetComponent<InteractableObject>().Destroyed;
        canRespawn = true;

        if(NetworkServer.active)
        MapEditor.Instance.event_reset += Respawn;
    }

    //public void Dissolve(Vector2 pot)
    //{
    //    Debug.Log($"Name : {gameObject.name}\n[BuildObject] code line - 366");
    //    //StartCoroutine(Co_Dissolve(pot));
    //    //if(MapEditor.Instance.mapEditorState != MapEditorState.NoEditor)
    //    //{
    //    //    //EditorMode_Destroy();


    //    //}
    //    //else
    //    //{
    //    //    StartCoroutine(Co_Dissolve(pot));
    //    //}

    //}
    public void Respawn()
    {
        if (!canRespawn) return;

        if (TryGetComponent(out InteractableObject component))
        {
            var root = component.GetFixedPointRootTransform();
            if (root != null) if (root.TryGetComponent(out HookSM hook)) hook.ReleaseItem();

            if (!component.CanInteract())
            component.Release();
            component.Cmd_Dissolve();
        }

        
        //Dissolve(position);
      
    }
    // IEnumerator Co_Dissolve(Vector2 pot)
    //{
    //    canRespawn = false;

    //    float percent = 1;
    //    _collider.enabled = false;
    //    _rb.simulated = false;
    //    _rb.gravityScale = 0;
    //    _rb.velocity = Vector2.zero;
    //    while (percent> 0)
    //    {
    //        percent -= dissolveRate;
    //        _dissolveMaterial.SetFloat(DissolveAmount, percent);
    //        yield return null;
    //    }

    //   if(isTransportItem)
    //   {    
    //        if(carrierTransform != null) 
    //        // SettingTransportItem(carrierTransform);
    //        Connection_TransportItem();
    //        // SettingTransportItem(carrierTransform);
    //   }else{
    //        transform.position = pot;
    //   }
        

    //    while(percent < 1)
    //    {
    //        percent += dissolveRate;
    //        _dissolveMaterial.SetFloat(DissolveAmount, percent);
    //        yield return null;
    //    }

    //    if (!isTransportItem)
    //    {
    //        _collider.enabled = true;
    //        _rb.gravityScale = 1; 
    //    }
    //    _rb.simulated = true;

    //    GetComponent<InteractableObject>().Respawned();

    //    //CustomEditor
    //    // if (MapEditor.Instance.mapEditorState == MapEditorState.Object)
    //    // {
    //    //     TurnOff();
    //    // }

    //    canRespawn = true;
       
    //}

    public bool GetDissolveObject(){
        if(_IsDissolveObject){
            return true;
        }
         return false;
    }
    
    #endregion

    #region  Editor
        // public virtual void Editor_Setting(Transform transform){}
        public virtual void Editor_Setting(MapEditor mapEditor){}






    public void SetOrgPosition()
    {
        setPosition = true;
        orgPosition = transform.position;
    }

    #endregion


    public void D_ReleaseToPool()
     {
        Managers.Pooling.D_ReleaseToPool(gameObject);
     }
    public void N_ReleaseToPool()
    {
        Managers.Pooling.N_ReleaseToPool(gameObject);
    }



}   
