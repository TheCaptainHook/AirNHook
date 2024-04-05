using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum TileModeState
{
    None,
    Tile,
    Clear,
    TileBox,
    ClearBox
}

public class PlaceMentSystem : MonoBehaviour
{
    Util Util = new Util();
    private Camera _camera => Camera.main == null ? null : Camera.main;

    [Header("Tile")]
    public Dictionary<Vector3Int, int> tileDic = new();
    [SerializeField] Tilemap preViewTileMap;
    [HideInInspector] public Tilemap floorTileMap;
    [HideInInspector] public TileBase tileBase;
    public TileBase previewTileBase;
    //box
    public bool getTarget;
    public Vector3Int startPosition;
    public Vector3Int endPosition;

    [Header("Object")]
    public GameObject curBuildObject;
    public GameObject first_holdingObj;// Click Interaction_BuildItem
    public GameObject curIndicatior;//Move,Rotation,Scale indicator

    [Header("Command")]
    public TileModeState tileModeState;
    public Invoker invoker;
    TileModeClient tileModeClient;


    [Header("Mouse")]
    bool inGridPlaneMousePosition;
    public LayerMask gridPlaneLayerMask;
    public Vector3Int gridPosition;
    private Vector3Int curGridPosition;

    private Vector3Int curPosition;
    private Vector3Int lastPosition;

    public Vector3 mousePosition;

    [Header("Current Placed Object")]
    public List<BuildObj> curPlaceObjList = new();


    private void Start()
    {
        invoker = new Invoker();
    }

    private void Update()
    {

        //tile
        if(MapEditor.Instance.mapEditorState == MapEditorState.Tile)
        {
            TileMode();
        }else if(MapEditor.Instance.mapEditorState == MapEditorState.Object)
        {
            ObjectMode();
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
                    if (curPosition != gridPosition)
                    {
                        lastPosition = curPosition;
                        curPosition = gridPosition;

                        preViewTileMap.SetTile(lastPosition, null);
                        preViewTileMap.SetTile(curPosition, previewTileBase);
                    }
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
            case TileModeState.TileBox:
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
                        tileModeClient.DrawBoxTile();
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
                    UpdatePreview_DrawBox();
                }

                break;
            case TileModeState.ClearBox:
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
                        tileModeClient.ClearBoxTile();
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
                    UpdatePreview_ClearBox();
                }

                break;
        }
       
       
    }

    void UpdatePreview()
    {
        preViewTileMap.SetTile(lastPosition, null);
        preViewTileMap.SetTile(curPosition, tileBase);
    }

    void UpdatePreview_DrawBox()
    {
        preViewTileMap.ClearAllTiles();
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
    void UpdatePreview_ClearBox()
    {
        preViewTileMap.ClearAllTiles();
        int minX = startPosition.x < gridPosition.x ? startPosition.x : gridPosition.x;
        int maxX = startPosition.x > gridPosition.x ? startPosition.x : gridPosition.x;
        int minY = startPosition.y < gridPosition.y ? startPosition.y : gridPosition.y;
        int maxY = startPosition.y > gridPosition.y ? startPosition.y : gridPosition.y;

        for (int i = minX; i <= maxX; i++)
        {
            for (int j = minY; j <= maxY; j++)
            {
                preViewTileMap.SetTile(new Vector3Int(i, j), previewTileBase);
            }
        }
    }

    public void ResetPreviewTileMap()
    {
        preViewTileMap.ClearAllTiles();
        getTarget = false;
    }

    public void ResetTileMap()
    {
        floorTileMap.ClearAllTiles();
    }
    #endregion

    #region Object
    private void ObjectMode()
    {
        if (first_holdingObj != null)
        {
            first_holdingObj.transform.position = new Vector3(mousePosition.x, mousePosition.y, 0);
            if (Input.GetMouseButtonDown(0) && CheckMousePosition_InGridBoundary())
            {
                curPlaceObjList.Add(first_holdingObj.GetComponent<BuildObj>());

                //Object_CreateModeCommand.Create();


                first_holdingObj = null;// Create;
            }
        }
        
    }
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

    public void Reset()
    {
        if(curIndicatior != null){ Destroy(curIndicatior); }
        if(first_holdingObj != null){ Destroy(first_holdingObj); }
        if(curBuildObject != null) { curBuildObject = null; }

    }

    public void curPlacedObjTurnOff()
    {
        foreach(BuildObj build in curPlaceObjList)
        {
            build.TurnOff();
        }
    }
    public void curPlacedObjTurnOn()
    {
        foreach (BuildObj build in curPlaceObjList)
        {
            build.TurnOn();
        }
    }

    public bool CheckMousePosition_InGridBoundary()
    {
        int x = (int)MapEditor.Instance.gridPlane.transform.localScale.x;
        int y = (int)MapEditor.Instance.gridPlane.transform.localScale.y;

        int maxX = x / 2;
        int minX = -(x / 2);
        int maxY = y / 2;
        int minY = -(y / 2);

        if(mousePosition.x < minX || mousePosition.x > maxX || mousePosition.y < minY || mousePosition.y > maxY)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
    #endregion


}
