using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class Puzzle_1_Helper : MonoBehaviour
{
    #if UNITY_EDITOR
    private Transform debugTransform;
    private Puzzle_1 puzzle_1;

    //Dummy obj
    private string circleTexturePath  = "Assets/Artwork/Sprites/Assets/UI Elements/Extras/Circle128/circle128.png";
    private int numberOfdummy;



    public List<GameObject> items;

    public void Init(){
        CreateDebugTransform();
        puzzle_1 = GetComponent<Puzzle_1>();
        items = new();
        
    }


    private void CreateDebugTransform(){
        foreach(Transform tr in transform){
            if(tr.name == "DebugTransform"){
                debugTransform = tr;
                return;
            }
        }    

        GameObject obj = new GameObject("DebugTransform");
        obj.transform.SetParent(transform);
        debugTransform = obj.transform;
    }



    #region Util
    public GameObject Add_Item(){
        numberOfdummy++;
        GameObject obj = new GameObject("Dummy Item");
        SpriteRenderer sprite = obj.AddComponent<SpriteRenderer>();
        sprite.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(circleTexturePath); 
        obj.transform.SetParent(debugTransform);
        items.Add(obj);

        return obj;
    }


    #endregion



    
    #endif
}
