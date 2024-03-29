using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class PlaceMentSystem : MonoBehaviour
{


    Util Util = new Util();
    private Camera _camera => Camera.main == null ? null : Camera.main;
    public LayerMask layerMask;    
   

    [Header("Tile")]
    public Dictionary<Vector3Int, int> tileDic = new();

    [Header("Mouse")]
    //Sprite default_TileMode_MouseIndicatorSprite;
    //Sprite defailt_MouseIndicatorSprite;
    public Vector3Int gridPosition;
    private Vector3Int curPosition;
    private Vector3Int lastPosition;
    public Vector3 mousePosition;
    //[SerializeField] GameObject mouseIndicator, cellIndicator;
    //public GameObject MouseIndicator { 
    //    get { return mouseIndicator; }
    //    set
    //    {
    //        if (value == null) { mouseIndicator.GetComponent<SpriteRenderer>().sprite = default_TileMode_MouseIndicatorSprite; }
    //        else if (mouseIndicator.GetComponent<SpriteRenderer>().sprite != value.GetComponent<SpriteRenderer>().sprite)
    //        {
    //            mouseIndicator.GetComponent<SpriteRenderer>().sprite = value.GetComponent<SpriteRenderer>().sprite;
             
    //        }}
    //    }
    [SerializeField] Tilemap preViewTileMap;
    public Tilemap floorTileMap;
    public TileBase tileBase;

    private void Start()
    {
        //_camera = Camera.main;
        //default_TileMode_MouseIndicatorSprite = mouseIndicator.GetComponent<SpriteRenderer>().sprite;
    }

    private void Update()
    {

        //tile
        if(MapEditor.Instance.mapEditorState== MapEditorState.Tile)
        {
            TileMode(); //이
        }
        //tile
    }
    private void LateUpdate()
    {
        if(MapEditor.Instance.mapEditorState != MapEditorState.NoEditor)
        {
            GetMousePosition();
        }

    }

    #region Tile

    public void TileModeInit()
    {
        //MouseIndicator.SetActive(true);
    }
    void TileMode()
    {
        //GetMousePosition();
        //if (MapEditor.Instance.mapEditorState == MapEditorState.Tile)
        //{
        //    mouseIndicator.transform.position = GetMousePosition();
        //    mouseIndicator.transform.position += new Vector3(1, 1);
        //}
        //else
        //{
        //    mouseIndicator.SetActive(false);
        //}

        if(tileBase != null)
        {
            //Privew
            if (curPosition != gridPosition)
            {
                lastPosition = curPosition;
                curPosition = gridPosition;
                UpdatePreview();
            }

            //Draw
            if (MapEditor.Instance.gridPlane.activeSelf)
            {
                if (Input.GetMouseButton(0))
                {
                    DrawTile();
                }
            }
        }
       
    }


    void UpdatePreview()
    {
        preViewTileMap.SetTile(lastPosition, null);
        preViewTileMap.SetTile(curPosition, tileBase);
    }

    void DrawTile()
    {
        if (tileBase == null)
        {
            if (tileDic.ContainsKey(gridPosition))
            {
                tileDic.Remove(gridPosition);
                floorTileMap.SetTile(gridPosition, tileBase);
            }
        }
        else
        {
            floorTileMap.SetTile(gridPosition, tileBase);
            tileDic[gridPosition] = int.Parse(tileBase.name);
        }
    }

    public void ResetTileMap()
    {
        floorTileMap.ClearAllTiles();
    }
    #endregion

    #region Object
    #endregion

    #region util
    public void GetMousePosition()
    {
        Vector3 mousePot = Util.GetMouseWorldPosition(Input.mousePosition, _camera);
        Collider2D collider = Physics2D.OverlapPoint(mousePot);
        if (collider != null)
        {
            Vector3Int cellPot = floorTileMap.WorldToCell(mousePot);
            gridPosition = new Vector3Int(Mathf.FloorToInt(floorTileMap.CellToWorld(cellPot).x), Mathf.FloorToInt(floorTileMap.CellToWorld(cellPot).y));
        }
    }

    private void OnDrawGizmos()
    {
       
            mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(mousePosition, 0.1f);
        
    }
    #endregion


}
