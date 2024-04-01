using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum TileModeState
{
    Tile,
    Clear,
    Bundle
}

public class PlaceMentSystem : MonoBehaviour
{
    Util Util = new Util();
    private Camera _camera => Camera.main == null ? null : Camera.main;
    
   

    [Header("Tile")]
    public Dictionary<Vector3Int, int> tileDic = new();
    [SerializeField] Tilemap preViewTileMap;
    public Tilemap floorTileMap;
    public TileBase tileBase;
    //bundle
    public bool getTarget;
    public Vector3Int startPosition;
    public Vector3Int endPosition;


    [Header("Command")]
    public TileModeState tileModeState;
    public Invoker invoker;
    TileModeClient tileModeClient;


    [Header("Mouse")]
    bool inGridPlaneMousePosition;
    public LayerMask gridPlaneLayerMask;
    //Sprite default_TileMode_MouseIndicatorSprite;
    //Sprite defailt_MouseIndicatorSprite;
    public Vector3Int gridPosition;
    private Vector3Int curGridPosition;

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


    //private void Start()
    //{
    //    //_camera = Camera.main;
    //    //default_TileMode_MouseIndicatorSprite = mouseIndicator.GetComponent<SpriteRenderer>().sprite;
    //}

    private void Start()
    {
        invoker = new Invoker();
    }

    private void Update()
    {

        //tile
        if(MapEditor.Instance.mapEditorState== MapEditorState.Tile)
        {
            TileMode();
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

    #region INIT
    public void EditorMode_Init()
    {
        tileModeClient = new TileModeClient();
    }
    #endregion

    public void Undo()
    {
        invoker.Undo();
    }


    #region Tile

    //todo

    //todo

    void TileMode()
    {
        switch (tileModeState)
        {
            case TileModeState.Tile:
                if (tileBase != null && MapEditor.Instance.gridPlane.activeSelf)
                {
                    //Privew
                    if (curPosition != gridPosition)
                    {
                        lastPosition = curPosition;
                        curPosition = gridPosition;
                        UpdatePreview();
                    }
                    //Draw
                    if (Input.GetMouseButton(0) && (curGridPosition != gridPosition) && inGridPlaneMousePosition)
                    {
                        curGridPosition = gridPosition;
                        tileModeClient.DrawTile();
                    }
                }
                break;
            case TileModeState.Clear:
                if (MapEditor.Instance.gridPlane.activeSelf)
                {
                    //MouseIndocator

                    //MouseIndocator
                    //Clear
                    if (Input.GetMouseButton(0) && (curGridPosition != gridPosition) && inGridPlaneMousePosition)
                    {
                        if (floorTileMap.GetTile(gridPosition) != null)
                        {
                            curGridPosition = gridPosition;
                            tileModeClient.ClearTile();
                        }
                    }
                }
                break;
            case TileModeState.Bundle:
                if (Input.GetMouseButtonDown(0) && inGridPlaneMousePosition)
                {
                    if (!getTarget)
                    {
                        startPosition = gridPosition;
                        getTarget = true;
                    }
                    else
                    {
                        endPosition = gridPosition;
                        getTarget = false;
                        ResetPreviewTileMap();
                        tileModeClient.DrawBundleTile();
                    }

                }

                if (getTarget && Input.GetMouseButtonDown(1))
                {
                    preViewTileMap.ClearAllTiles();
                    getTarget = false;
                }

                if (getTarget && curPosition != gridPosition)
                {
                    lastPosition = curPosition;
                    curPosition = gridPosition;
                    preViewTileMap.ClearAllTiles();
                    UpdatePreview_Bundle();
                }

                break;
        }
       
       
    }

    void UpdatePreview()
    {
        preViewTileMap.SetTile(lastPosition, null);
        preViewTileMap.SetTile(curPosition, tileBase);
    }

    void UpdatePreview_Bundle()
    {
        int minX = startPosition.x < gridPosition.x ? startPosition.x : gridPosition.x;
        int maxX = startPosition.x > gridPosition.x ? startPosition.x : gridPosition.x;
        int minY = startPosition.y < gridPosition.y ? startPosition.y : gridPosition.y;
        int maxY = startPosition.y > gridPosition.y ? startPosition.y : gridPosition.y;

        for (int i = minX; i <= maxX; i++)
        {
            for (int j = minY; j <= maxY; j++)
            {
                preViewTileMap.SetTile(new Vector3Int(i, j), tileBase);
            }
        }
    }
    public void ResetPreviewTileMap()
    {
        preViewTileMap.ClearAllTiles();
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
        Collider2D collider = Physics2D.OverlapPoint(mousePot,gridPlaneLayerMask);
        if (collider != null)
        {
            inGridPlaneMousePosition = true;
            Vector3Int cellPot = floorTileMap.WorldToCell(mousePot);
            gridPosition = new Vector3Int(Mathf.FloorToInt(floorTileMap.CellToWorld(cellPot).x), Mathf.FloorToInt(floorTileMap.CellToWorld(cellPot).y));
        }
        else
        {
            inGridPlaneMousePosition = false;
            ResetPreviewTileMap();
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
