using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Object_Create 
{
    GameObject curObj;
    PlaceMentSystem placeMentSystem;
    BuildObj buildObj;
    public Object_Create()
    {
        placeMentSystem = MapEditor.Instance.placeMentSystem;
        curObj = MapEditor.Instance.placeMentSystem.CurbuildObject;
        buildObj = curObj.GetComponent<BuildObj>();
    }


    public void Create()
    {
        switch (buildObj.id)
        {
            case 302:
                CheckTransform(MapEditor.Instance.dontSaveObjectTransform,302);
                curObj.transform.SetParent(MapEditor.Instance.dontSaveObjectTransform);
                break;
            case 301:
                CheckTransform(MapEditor.Instance.exitDoorObjectTransform,301);
                curObj.transform.SetParent(MapEditor.Instance.exitDoorObjectTransform);
                break;
            case 305:
                curObj.transform.SetParent(MapEditor.Instance.interactionObjectTransform);
                break;
            case 306:
                curObj.transform.SetParent(MapEditor.Instance.dontSaveObjectTransform);
                break;
            default:
                curObj.transform.SetParent(MapEditor.Instance.objectTransform);
                break;
        }
    }



    public void Undo()
    {
        if(curObj != null)
        {
            Object.Destroy(curObj);
        }
        
    }


    private void CheckTransform(Transform transform,int id)
    {
        foreach(Transform curT in transform)
        {
            BuildObj curBuild = curT.GetComponent<BuildObj>();
            if(curBuild.id == id)
            {
                if (placeMentSystem.curPlaceObjList.Contains(curBuild))
                {
                    placeMentSystem.curPlaceObjList.Remove(curBuild);
                }
                Object.Destroy(curBuild.gameObject);
                return;
            }
        }
    }
}
