using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Object_Clear
{
    PlaceMentSystem placeMentSystem;
    GameObject curBuildObj;
    BuildObj buildObj;

    public Object_Clear()
    {
        placeMentSystem = MapEditor.Instance.placeMentSystem;
        curBuildObj = placeMentSystem.CurbuildObject;
        buildObj = curBuildObj.GetComponent<BuildObj>();
    }


    public void Execute()
    {
 
        GameObject effectObj = Object.Instantiate(placeMentSystem.particleEffect_ObejctClear.gameObject);
        effectObj.transform.position = buildObj.transform.position;
        effectObj.GetComponent<ParticleSystem>().Play();
        Object.Destroy(effectObj, 3f);
        //Clear Effect

        if(placeMentSystem.curPlaceObjList.Contains(buildObj))
        {
            placeMentSystem.curPlaceObjList.Remove(buildObj);
        }
        curBuildObj.transform.SetParent(MapEditor.Instance.garbageTransform);
        curBuildObj.SetActive(false);
        placeMentSystem.CurbuildObject = null;
    
    }
    public void Undo()
    {
        curBuildObj.SetActive(true);

        switch (buildObj.transformID)
        {
            case 1:
                if (buildObj.id == 306)
                {
                    curBuildObj.transform.SetParent(MapEditor.Instance.dontSaveObjectTransform);
                }
                else if (buildObj.id == 302)
                {
                    curBuildObj.transform.SetParent(MapEditor.Instance.dontSaveObjectTransform);
                }
                else
                {
                    curBuildObj.transform.SetParent(MapEditor.Instance.objectTransform);
                }
                break;
            case 2:
                curBuildObj.transform.SetParent(MapEditor.Instance.interactionObjectTransform);
                break;
            case 3:
                curBuildObj.transform.SetParent(MapEditor.Instance.exitDoorObjectTransform);
                break;
            default:
                curBuildObj.transform.SetParent(MapEditor.Instance.dontSaveObjectTransform);
                break;
        }

        placeMentSystem.curPlaceObjList.Add(curBuildObj.GetComponent<BuildObj>());
    }

}
