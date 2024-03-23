using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Tilemaps;
public class TESTScripte : MonoBehaviour
{
    [SerializeField] Button button;
    [SerializeField] TextMeshProUGUI text;


    private void Awake()
    {
        button.onClick.AddListener(OnClick);
    }



    void OnClick()
    {

        //Map map = Managers.Data.mapData.mapDictionary["Tutorial_1"];
        //foreach(TileData data in map.mapTileDataList)
        //{
        //    text.text += data.position;
        //}
        MapEditor.Instance.LoadMap("Tutorial_3");
        //foreach(Vector3Int key in MapEditor.Instance.placeMentSystem.tileDic.Keys)
        //{
        //    text.text += key;
        //}
        TileBase tileBase = Resources.Load<TileBase>("Arts/Tiles/15");
        text.text = tileBase.name;
        MapEditor.Instance.placeMentSystem.floorTileMap.SetTile(new Vector3Int(0, 0, 0), tileBase);
        //text.text += MapEditor.Instance.placeMentSystem.floorTileMap.name;


    }


}
