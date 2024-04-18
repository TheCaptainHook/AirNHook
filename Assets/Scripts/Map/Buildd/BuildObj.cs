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
    
    protected bool turnOff;
    [SerializeField] protected DistructionStatus distructionStatus;
    [Header("State")]
    public bool onPlaceable;
    public bool onRotateable;
    public bool onScaleable;
    
    private ObjectData _objectData;
    public ObjectData ObjectData { get { return _objectData; } set { _objectData = value; id = _objectData.id; } }

    public event Action<Vector2> OnDissolveAction;
    public event Action OnDisableAction;




    [Header("Only use Editor mode")]
    [HideInInspector] public bool setPosition; // When created and placed set this parameter
    [HideInInspector] public Vector2 orgPosition;

    public void SetTileData(Vector2 position)
    {
        ObjectData = new ObjectData(id, position,transform.localScale);
    }

    public void SetTileData(Vector2 position,Quaternion quaternion)
    {
        ObjectData = new ObjectData(id, position, quaternion,transform.localScale);
    }
    
   public virtual void TakeDamage()
   {
        if(distructionStatus == DistructionStatus.Destructible)
        {
            Debug.Log(gameObject.name);
            Debug.Log("Distruction");
            OnDissolveAction?.Invoke(ObjectData.position);
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


    private void ChangeObjectColor(Color color)
    {
        SpriteRenderer[] spriteRenderers = gameObject.GetComponentsInChildren<SpriteRenderer>();

        foreach(SpriteRenderer sp in spriteRenderers)
        {
            sp.color = color;
        }
    }


    public void SetOrgPosition()
    {
        setPosition = true;
        orgPosition = transform.position;
    }

}
