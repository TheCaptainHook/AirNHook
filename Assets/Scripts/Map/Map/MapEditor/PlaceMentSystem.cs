using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum ModeState
{
    None,
    Tile_Draw,
    Tile_Clear,
    Tile_BoxDraw,
    Tile_ClearBox,
    Obj_Move,
    Obj_Rotation,
    Obj_Scale,
    Obj_Clear
}


public class PlaceMentSystem : MonoBehaviour
{
    Util Util = new Util();
    private Camera _camera => Camera.main == null ? null : Camera.main;

    [Header("Tile")]
    public Dictionary<Vector3Int, int> tileDic = new();
     public Tilemap preViewTileMap;//only use,Editor mode
     public Tilemap floorTileMap;
    [HideInInspector] public TileBase tileBase;
    public TileBase previewTileBase;
    //box
    [HideInInspector] public bool getTarget;
    [HideInInspector] public Vector3Int startPosition;
    [HideInInspector] public Vector3Int endPosition;

    [Header("Object")]
    private GameObject curBuildObject;
    public GameObject CurbuildObject {
        get { return curBuildObject;}
        set
        {
            if (curArrowIndicatorTrack != null) { Destroy(curArrowIndicatorTrack); }
            if(curAdditionalIndicatorTrack != null) { Destroy(curAdditionalIndicatorTrack);}
            CurIndicatior = null;
            curBuildObject = value;
            SelectCurBuildObj();//after
        }

    }
    [HideInInspector]  public GameObject first_holdingObj;// Click Interaction_BuildItem
    public GameObject curIndicatior;//Move,Rotation,Scale indicator
    public GameObject CurIndicatior
    {
        get { return curIndicatior; }
        set
        {
            if (curIndicatior != null)
            {
                Destroy(curIndicatior);
            }

            curIndicatior = value;
        }
    }


    private GameObject curArrowIndicatorTrack;
    //todo
    private GameObject curAdditionalIndicatorTrack;
    //todo
    [Header("Command")]
    public ModeState modeState;
    [HideInInspector] public Invoker invoker;
    TileModeClient tileModeClient;
    [HideInInspector] public ObjectModeClient objectModeClient;

    [Header("Mouse")]
    bool inGridPlaneMousePosition;
    public LayerMask gridPlaneLayerMask;
    public Vector3Int gridPosition;
    private Vector3Int curGridPosition;

    private Vector3Int curPosition;
    private Vector3Int lastPosition;

    private Vector3 mousePosition;


    [Header("Indicator")]
    [SerializeField] GameObject curObj_ArrowIndicator;
    [SerializeField] GameObject ObjMove_Indicator;
    [SerializeField] GameObject ObjRotation_Indicator;
    [SerializeField] GameObject ObjSclae_Indicator;
    [SerializeField] GameObject ObjClear_Indicator;
    [SerializeField] GameObject Additional_Indicator;

    [Header("Current Placed Object")]
    public List<BuildObj> curPlaceObjList = new();

    [Header("Interaction State")]
    public bool onInteraction;

    private void Start()
    {
        onInteraction = true;
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
        objectModeClient = new ObjectModeClient();
    }
    #endregion

    public void Undo()
    {
        invoker.Undo();
    }


    #region Tile
    void TileMode()
    {
        switch (modeState)
        {
            case ModeState.Tile_Draw:
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
            case ModeState.Tile_Clear:
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
            case ModeState.Tile_BoxDraw:
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
            case ModeState.Tile_ClearBox:
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
            //first_holdingObj.transform.position = new Vector3(mousePosition.x, mousePosition.y, 0);
            first_holdingObj.transform.position = gridPosition;
            if (Input.GetMouseButtonDown(0) && CheckMousePosition_InGridBoundary())
            {
                curPlaceObjList.Add(first_holdingObj.GetComponent<BuildObj>());
                CurbuildObject = first_holdingObj;
                first_holdingObj = null;

                objectModeClient.Create();
                CreateIndicator(ModeState.Obj_Move);

            }
            if (Input.GetMouseButton(1))
            {
                Destroy(first_holdingObj);
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

    public void ObjectMode_Reset()
    {
        if(curIndicatior != null){ Destroy(curIndicatior); }
        if(first_holdingObj != null){ Destroy(first_holdingObj); }
        if(CurbuildObject != null) { CurbuildObject = null; }
        if(curArrowIndicatorTrack != null) { Destroy(curArrowIndicatorTrack); }
        if(curAdditionalIndicatorTrack != null) { Destroy(curAdditionalIndicatorTrack); }
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

    private void SelectCurBuildObj()
    {
        if(CurbuildObject != null)
        {
            curArrowIndicatorTrack = Instantiate(curObj_ArrowIndicator, CurbuildObject.transform);
            curArrowIndicatorTrack.GetComponent<Arrow_Indicator>().SetLinkObj(CurbuildObject);

            if (CurbuildObject.GetComponent<BuildObj>().id == 305 || CurbuildObject.GetComponent<BuildObj>().id == 306)
            {
                GameObject additionalIndicator = Instantiate(Additional_Indicator);
                curAdditionalIndicatorTrack = additionalIndicator;
                additionalIndicator.GetComponent<Additional_Indicator>().SetLinkObj(CurbuildObject);
            }

        }

    }


    public void CreateIndicator(ModeState modeState)
    {
        if(CurbuildObject != null)
        {
            switch (modeState)
            {
                case ModeState.Obj_Move:
                    this.modeState = modeState;
                    GameObject indicator = Instantiate(ObjMove_Indicator);
                    CurIndicatior = indicator;
                    //indicator.transform.SetParent(CurbuildObject.transform);
                    indicator.transform.position = CurbuildObject.transform.position;
                    indicator.GetComponent<Move_Indicator>().SetLinkObj(CurbuildObject);
                    break;
                case ModeState.Obj_Rotation:
                    this.modeState = modeState;
                    GameObject indicator_R = Instantiate(ObjRotation_Indicator);
                    CurIndicatior = indicator_R;
                    //indicator_R.transform.SetParent(CurbuildObject.transform);
                    indicator_R.transform.position = CurbuildObject.transform.position;
                    indicator_R.GetComponent<Rotate_Indicator>().SetLinkObj(CurbuildObject);
                    break;
            }
        }
    }




    #endregion


}
