using System.Collections;
using System.Collections.Generic;
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
public class BuildObj : MousePointerEntity,IDamageable
{
    [Header("BuildObj Script")]
    public int id;
    [Tooltip("Transform ID to be created")]
    public int transformID;
    public Vector2 position;
    [Tooltip("Use this parameter in editor mode")]
    public Vector2 offset; // Use this parameter in editor mode.
    public bool turnOff;
    [SerializeField] protected DistructionStatus distructionStatus;
    [Header("State")]
    public bool onPlaceable;
    public bool onRotateable;
    public bool onScaleable;
    
    private ObjectData _objectData;
    public ObjectData ObjectData { get { return _objectData; } set { _objectData = value; id = _objectData.id; } }

    public event Action<Vector2> OnDissolveAction;
    public event Action OnDisableAction;
    public event Action OnInteractableObjectRelease;


    [Header("Only use Editor mode")]
    [HideInInspector] public bool setPosition; // When created and placed set this parameter
    [HideInInspector] public Vector2 orgPosition;

    [Header("Indicator")]
    private bool onEnterPointer;

    public void CallOnInterableObjectRelease()
    {
        OnInteractableObjectRelease?.Invoke();
    }


    public void SetTileData(Vector2 position)
    {
        ObjectData = new ObjectData(id, position,transform.localScale);
    }

    public  void SetTileData(Vector2 position,Quaternion quaternion)
    {
        ObjectData = new ObjectData(id, position, quaternion,transform.localScale);
    }

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
    //public virtual void SetTileData(ObjectData data)
    //{

    //}

    public virtual void TakeDamage()
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



}
