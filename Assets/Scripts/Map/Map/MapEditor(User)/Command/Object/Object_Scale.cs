using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Object_Scale
{
    PlaceMentSystem placeMentSystem;
    GameObject curObj;
    Vector3 orgScale;

   public Object_Scale()
    {
        placeMentSystem = MapEditor.Instance.placeMentSystem;
        curObj = placeMentSystem.CurbuildObject;
    }


    public void Execute()
    {
        orgScale = curObj.transform.localScale;
    }
    public void Undo()
    {
        curObj.transform.localScale = orgScale;
    }
}
