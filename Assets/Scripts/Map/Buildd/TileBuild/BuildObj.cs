using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BuildObj:MonoBehaviour
{
    [SerializeField] int id;

    public ObjectData objectData;

    public void SetTileData(Vector2 position)
    {
        objectData = new ObjectData(id, position);

    }

    public void SetTileData(Vector2 position,Quaternion quaternion)
    {
        objectData = new ObjectData(id, position, quaternion);

    }
}
// id, position 만 구조체로 가지고 있기.