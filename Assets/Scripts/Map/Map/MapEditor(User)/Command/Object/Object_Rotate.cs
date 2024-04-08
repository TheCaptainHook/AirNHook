using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Object_Rotate
{
    PlaceMentSystem placeMentSystem;
    GameObject curBuildObj;
    Quaternion beforeRotation;

    public Object_Rotate()
    {
        placeMentSystem = MapEditor.Instance.placeMentSystem;
        curBuildObj = placeMentSystem.CurbuildObject;
        beforeRotation = curBuildObj.transform.rotation;
    }


    public void Rotate()
    {
        beforeRotation = curBuildObj.transform.rotation;
    }
    public void Undo()
    {
        curBuildObj.transform.rotation = beforeRotation;
    }


}
