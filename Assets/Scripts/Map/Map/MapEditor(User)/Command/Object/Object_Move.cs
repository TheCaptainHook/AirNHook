using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Object_Move
{
    PlaceMentSystem placeMentSystem;
    GameObject curBuildObj;
    Vector3 beforePosition;

    public Object_Move()
    {
        placeMentSystem = MapEditor.Instance.placeMentSystem;
        curBuildObj = placeMentSystem.CurbuildObject;
    }


    public void Move()
    {
        beforePosition = placeMentSystem.mousePosition;
        curBuildObj.GetComponent<BuildObj>().SetOrgPosition();
        Debug.Log("Move");
    }
    public void Undo()
    {
        curBuildObj.transform.position = beforePosition;
    }
}
