using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
public class Interaction_BuildItem : MonoBehaviour
{
    public Tile tilebase;
    public GameObject buildObj;
    Image image;
    Button button;
    TileType tileType;

    private void Awake()
    {
        image = GetComponent<Image>();
        button = GetComponent<Button>();
        button.onClick.AddListener(ChoiceItem);
       
    }


    void ChoiceItem()
    {
        if(tileType == TileType.Object)
        {
            MapEditor.Instance.CurBuildObj = buildObj;
        }else if(tileType == TileType.Floor)
        {
            MapEditor.Instance.placeMentSystem.tileBase = tilebase;
        }
      
    }


    public void Init(GameObject tile,TileType tileType)
    {
        buildObj = tile;
        this.tileType = tileType;
        SetImage();
    }
    public void Init(Tile tilebase,TileType tileType)
    {
        this.tilebase = tilebase;
        this.tileType = tileType;
        image.sprite = tilebase.sprite;
        image.color = tilebase.color;
    }
    void SetImage()
    {
        image.sprite = buildObj.GetComponent<SpriteRenderer>().sprite;
        image.color = buildObj.GetComponent<SpriteRenderer>().color;
    }

}
