using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public enum DistructionStatus
{
    Indestructible,
    Destructible,
    PermanentDestruction
}

[System.Serializable]
public class BuildObj : MonoBehaviour,IDamageable
{
    [SerializeField] protected int id;
    [SerializeField] protected DistructionStatus distructionStatus;

    private ObjectData _objectData;
    public ObjectData ObjectData { get { return _objectData; } set { _objectData = value; id = _objectData.id; } }

    public event Action<Vector2> OnDissolveAction;

    public void SetTileData(Vector2 position)
    {
        ObjectData = new ObjectData(id, position,transform.localScale);
    }

    public void SetTileData(Vector2 position,Quaternion quaternion)
    {
        ObjectData = new ObjectData(id, position, quaternion,transform.localScale);
    }
    
   public void TakeDamage()
   {
        if(distructionStatus == DistructionStatus.Destructible)
        {
            Debug.Log("Distruction");
            OnDissolveAction?.Invoke(ObjectData.position);
        }

        if(distructionStatus == DistructionStatus.PermanentDestruction)
        {
            Destroy(gameObject);
        }
   }
}