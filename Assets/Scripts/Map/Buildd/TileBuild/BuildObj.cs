using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BuildObj : MonoBehaviour
{
    [SerializeField] protected int id;

    private ObjectData _objectData;
    public ObjectData ObjectData { get { return _objectData; } set { _objectData = value; id = _objectData.id; } }

    public void SetTileData(Vector2 position)
    {
        ObjectData = new ObjectData(id, position,transform.localScale);
    }

    public void SetTileData(Vector2 position,Quaternion quaternion)
    {
        ObjectData = new ObjectData(id, position, quaternion,transform.localScale);
    }
}
