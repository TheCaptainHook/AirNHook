using System.Collections;
using UnityEngine;
using System;
using UnityEngine.EventSystems;

public enum DistructionStatus
{
    Indestructible,
    Destructible,
    PermanentDestruction
}


[System.Serializable]
public class BuildObj : MousePointerEntity, IDamageable
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
    [Tooltip("The default value of this variable is false, and to use it, the IPowerConsumer interface must be implemented.")]
    public bool chargeRequired = false;
    [Space(20)]

    #region Transform Item
    private bool isTransformItem;
    private Transform carrierTransform;
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
    private static readonly int DissolveAmount = Shader.PropertyToID("_DissolveAmount");

    protected Material _dissolveMaterial;
    protected Rigidbody2D _rb;
    protected Collider2D _collider;
    float dissolveRate = 0.015f;

    public event Action<Vector2> OnDissolveAction;
    public event Action OnDisableAction;
    public event Action OnInteractableObjectRelease;
    protected bool _IsDissolveObject;

    #endregion
    public void CallOnInterableObjectRelease()
    {
        OnInteractableObjectRelease?.Invoke();
    }


    public void SetTileData(Vector2 position)
    {
        ObjectData = new ObjectData(id, position,transform.localScale);
    }

    // public  void SetTileData(Vector2 position,Quaternion quaternion)
    // {
    //     ObjectData = new ObjectData(id, position, quaternion,transform.localScale);
    // }

    public virtual void SetTileData()
    {
        ObjectData = new ObjectData(id, transform.position, transform.rotation, transform.localScale);
    }

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
    public void SettingTransportItem(Transform carrierTransform)
    {
        _rb.gravityScale = 0;
        _collider.enabled = false;
        this.carrierTransform = carrierTransform;
        transform.position = carrierTransform.position;
        //연결시키기
        // transform.SetParent(carrierTransform);
        isTransformItem = true;
    }
    public void DropTransportItem()
    {
        // transform.SetParent(MapEditor.Instance.networkingObjectTransform);
        //연결해제
        _collider.enabled =true;
        _rb.gravityScale =1;
    }
#endregion

    public virtual void TakeDamage(DamageType damageType = DamageType.Default)
   {
        if(distructionStatus == DistructionStatus.Destructible)
        {
            Debug.Log(gameObject.name);
            Debug.Log("Distruction");
            if(Managers.Game.CurrentState != GameState.Editor)
            {
                OnInteractableObjectRelease?.Invoke();
            }
            OnDissolveAction?.Invoke(position);
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


    public void SetOrgPosition()
    {
        setPosition = true;
        orgPosition = transform.position;
    }


    #region Destructible Obj Dissolve Effect Logic
    protected void DissolveInitSetting(){
        _dissolveMaterial = _Dissolve_MainSprite.material;
        _rb = GetComponent<Rigidbody2D>();
        _collider = GetComponent<Collider2D>();
        _IsDissolveObject = true;
        OnDissolveAction += Dissolve;
        OnInteractableObjectRelease += GetComponent<InteractableObject>().Destroyed;

    }

    public void Dissolve(Vector2 pot)
    {
        if(MapEditor.Instance.mapEditorState != MapEditorState.NoEditor)
        {
            //EditorMode_Destroy();
            
            StartCoroutine(Co_Dissolve(orgPosition));
        }
        else
        {
            StartCoroutine(Co_Dissolve(pot));
        }
        
    }

    IEnumerator Co_Dissolve(Vector2 pot)
    {
        float percent = 1;
        _collider.enabled = false;
        _rb.velocity = Vector2.zero;
        _rb.gravityScale = 0;
        while (percent> 0)
        {
            percent -= dissolveRate;
            _dissolveMaterial.SetFloat(DissolveAmount, percent);
            yield return null;
        }
       if(isTransformItem)
       {

       }else{
        transform.position = pot;
       }
        

        while(percent < 1)
        {
            percent += dissolveRate;
            _dissolveMaterial.SetFloat(DissolveAmount, percent);
            yield return null;
        }
        _collider.enabled = true;
        _rb.gravityScale = 1;
        GetComponent<InteractableObject>().Respawned();

        //CustomEditor
        // if (MapEditor.Instance.mapEditorState == MapEditorState.Object)
        // {
        //     TurnOff();
        // }
       
    }

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
    #endregion
}   
