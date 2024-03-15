using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BuildObj : MonoBehaviour
{
    private int id;
    private Vector2 position;
   

    public ObjectData objectData;

    public void SetTileData(Vector2 position)
    {
        this.position = position;

        objectData = new ObjectData(id, position);
    }
}
// id, position 만 구조체로 가지고 있기.