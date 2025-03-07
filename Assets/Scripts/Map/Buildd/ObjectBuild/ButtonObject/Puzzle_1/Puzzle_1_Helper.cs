using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System;

[ExecuteInEditMode]
public class Puzzle_1_Helper : MonoBehaviour
{
    #if UNITY_EDITOR
    private Puzzle_1 puzzle_1;

    //Dummy obj
    private string circleTexturePath;

    //Parts
    private string partsPath; 
    private Puzzle_1_Parts parts;
    private Transform partsContainer;
    //Item
    public List<GameObject> items;
    private Transform itemContainer;


    //Editor
    private float increasingPartsX;
    private GameObject recent_Parts;
    private GameObject recent_Item;

    public void Init(){
        circleTexturePath = "Assets/Artwork/Sprites/Assets/UI Elements/Extras/circle64.png";
        partsPath = "Assets/Prefabs/Map/Puzzle_1/Puzzle_1_Parts.prefab";

        puzzle_1 = GetComponent<Puzzle_1>();
        items = new();
        increasingPartsX = 1;
        try
        {
            parts = AssetDatabase.LoadAssetAtPath<Puzzle_1_Parts>(partsPath);
            partsContainer = GetTransform("Puzzle_1_Parts_Container");
            itemContainer = GetTransform("Puzzle_1_Item_Container");

            GetNumberOfParts();
        }
        catch(Exception ex)
        {
            Debug.Log(ex);
        }

    }

  
    public void RoundPosition()
    {
        RoundPosition(puzzle_1.transform);

        foreach(Transform t in partsContainer)
        {
            RoundPosition(t);
        }


    }


    private void RoundPosition(Transform t)
    {
        t.position = new Vector2(
            Mathf.RoundToInt(t.position.x),
            Mathf.RoundToInt(t.position.y));
    }
    #region Get
    public (int number,GameObject recentParts) GetPartsField()
    {
        return (partsContainer.childCount, recent_Parts);
    }
    #endregion


    // private void CreateDebugTransform(){
    //     foreach(Transform tr in transform){
    //         if(tr.name == "DebugTransform"){
    //             debugTransform = tr;
    //             return;
    //         }
    //     }    

    //     GameObject obj = new GameObject("DebugTransform");
    //     obj.transform.SetParent(transform);
    //     debugTransform = obj.transform;
    // }


    #region INIT
    private Transform GetTransform(string name)
    {
        foreach (Transform tr in transform)
        {
            if (tr.name == name) return tr;
        }

        return null;
    }
    private void GetNumberOfParts()
    {
        if (!partsContainer)
        {
            return;
        } 

        int index = partsContainer.childCount;
        if (index > 0)
        {
            recent_Parts = partsContainer.GetChild(index - 1).gameObject;
        }

    }


    #endregion  

    // public void Destroy()
    // {
    //     if (debugTransform)
    //         Undo.DestroyObjectImmediate(debugTransform.gameObject);
    // }
    #region Util
  
    #region Item
    public GameObject Add_Item(){
        try
        {
            GameObject obj = new GameObject("Dummy Item");
            SpriteRenderer sprite = obj.AddComponent<SpriteRenderer>();
            sprite.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(circleTexturePath);
            sprite.sortingLayerName = "ForeGround";
            sprite.sortingOrder = 2;
            sprite.color = new Color(1, 1, 1, 0.5f);
            obj.transform.SetParent(itemContainer);

            obj.transform.position = GetSceneViewCenter();

            items.Add(obj);
            recent_Item = obj;
            return obj;
        }
        catch(Exception ex)
        {
            Debug.Log(ex);
            return null;
        }
      
    }
    public void Remove_Item()
    {
        if (recent_Item != null)
        {
            Undo.DestroyObjectImmediate(recent_Item);

            int index = itemContainer.childCount;
            if(index > 0)
            {
                recent_Item = itemContainer.GetChild(index - 1).gameObject;
            }
        }

    }
    public int GetItemField()
    {
        return itemContainer.childCount;
    }
    public GameObject GetItem(int index)
    {
        return itemContainer.GetChild(index).gameObject;
    }
    #endregion

    #region Parts
    public GameObject Add_Parts()
    {
        if (!partsContainer) return null;
        puzzle_1.RefrashPartsList();
        try
        {
            GameObject obj = Instantiate(parts).gameObject;
           
            obj.transform.SetParent(partsContainer);   
            puzzle_1.AddParts(obj.GetComponent<Puzzle_1_Parts>());
            obj.transform.position =
                recent_Parts ? recent_Parts.transform.position + new Vector3(increasingPartsX, 0) : puzzle_1.transform.position + new Vector3(increasingPartsX,1,0);
            recent_Parts = obj;


            obj.name = $"Puzzle_1_Parts ({partsContainer.childCount})";
            return obj;
        }
        catch(Exception ex)
        {
            Debug.Log(ex);
            return null;
        }
      
    }
    
    public void RemoveParts()
    {
        if (partsContainer.childCount <= 0) return;

        int partsListCount = puzzle_1.GetPartsCount();
        if(partsListCount < partsContainer.childCount)
        {
            for(int i =0; i<partsContainer.childCount - partsListCount;i++)
            {
                Undo.DestroyObjectImmediate(partsContainer.GetChild(partsContainer.childCount-1).gameObject);
                recent_Parts = partsContainer.GetChild(partsContainer.childCount - 1).gameObject;
            }
        }

        puzzle_1.RemoveParts();
        Undo.DestroyObjectImmediate(recent_Parts);

        int index = partsContainer.childCount;
        if(index <= 0)
        {
            recent_Parts = null;
        }
        else
        {
            recent_Parts = partsContainer.GetChild(partsContainer.childCount - 1).gameObject;
        }
       

    }
    public GameObject GetParts(int index)
    {
        return partsContainer.GetChild(index).gameObject;
    }
    #endregion

    #region Hint
    public void SetHintText()
    {
        puzzle_1.SetHintScreenText(GetPartsField().number);
    }
    #endregion

    #endregion

    private Vector3 GetSceneViewCenter()
    {
        SceneView sceneView = SceneView.lastActiveSceneView;
        if (sceneView != null)
        {
            return sceneView.pivot;
        }
        else
        {
            return Vector3.zero;
        }
    }


#endif
}
