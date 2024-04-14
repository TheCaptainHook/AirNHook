using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Object_Clear
{
    PlaceMentSystem placeMentSystem;
    GameObject curBuildObj;
    BuildObj buildObj;

    Vector3 position;
    Quaternion rotation;
    Vector3 scale;


    public Object_Clear()
    {
        placeMentSystem = MapEditor.Instance.placeMentSystem;
        curBuildObj = placeMentSystem.CurbuildObject;
        buildObj = curBuildObj.GetComponent<BuildObj>();
    }


    public void Execute()
    {
        position = curBuildObj.transform.position;
        rotation = curBuildObj.transform.rotation;
        scale = curBuildObj.transform.localScale;

        //Clear Effect

        GameObject effectObj = Object.Instantiate(placeMentSystem.particleEffect_ObejctClear.gameObject);
        effectObj.transform.position = buildObj.transform.position;
        effectObj.GetComponent<ParticleSystem>().Play();
        Object.Destroy(effectObj, 3f);
        //Clear Effect

        buildObj.EditorMode_Destroy();
    }
    public void Undo()
    {
        MapDataStruct data = Managers.Data.mapData.mapObjectDataDictionary[buildObj.id];
        GameObject undoObj = Object.Instantiate(Resources.Load<GameObject>(data.path));
        switch (buildObj.transformID)
        {
            case 1:
                if(buildObj.id == 306)
                {
                    undoObj.GetComponent<ButtonActivated>().LinkDoor();
                    undoObj.transform.SetParent(MapEditor.Instance.dontSaveObjectTransform);
                }else if(buildObj.id == 302)
                {
                    undoObj.transform.SetParent(MapEditor.Instance.dontSaveObjectTransform);
                }
                else
                {
                    undoObj.transform.SetParent(MapEditor.Instance.objectTransform);
                }
                break;
            case 2:
                undoObj.transform.SetParent(MapEditor.Instance.interactionObjectTransform);
                break;
            case 3:
                undoObj.transform.SetParent(MapEditor.Instance.exitDoorObjectTransform);
                break;
        }

        SetData(undoObj);
        placeMentSystem.curPlaceObjList.Add(undoObj.GetComponent<BuildObj>());
    }



    private void SetData(GameObject obj)
    {
        obj.transform.position = position;
        obj.transform.rotation = rotation;
        obj.transform.localScale = scale;
    }
}
