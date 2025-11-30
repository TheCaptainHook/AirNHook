using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

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
    public Tilemap halfTileMap;
    public Tilemap backgroundTileMap;
    // 1022
    public Tilemap ropeTileMap;
    public Tilemap accessoryTileMap;
    // 1022
    public Tilemap hiddentTIleMap;
    //1130
    public Tilemap specialTileMap;

    [HideInInspector] public TileBase tileBase;
    public TileBase previewTileBase;
    [HideInInspector] public bool getTarget;
    [HideInInspector] public Vector3Int startPosition; //used Tile Draw Box
    [HideInInspector] public Vector3Int endPosition; //used Tile Draw Box

    [Header("Object")]
    private GameObject curBuildObject;
    public GameObject CurbuildObject {
        get { return curBuildObject;}
        set
        {
            if (curArrowIndicatorTrack != null) { Destroy(curArrowIndicatorTrack); }
            if(curAdditionalIndicatorTrack != null) { Destroy(curAdditionalIndicatorTrack);}
            if(curBuildObject != null) { ResetColorObj(); }

            CurIndicatior = null;
            curBuildObject = value;
            SelectCurBuildObj();//after
            MapEditor.Instance.editorUIController.RotateAndScaleBtn();
            
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
    public Vector3 mousePosition;

    private Vector3Int curGridPosition;

    private Vector3Int curPosition;
    private Vector3Int lastPosition;

    

    [Header("Indicator")]
    [SerializeField] GameObject curObj_ArrowIndicator;
    [SerializeField] GameObject ObjMove_Indicator;
    [SerializeField] GameObject ObjRotation_Indicator;
    [SerializeField] GameObject Additional_Indicator;

    [Header("Current Placed Object")]
    public List<BuildObj> curPlaceObjList = new(); //use Object Mode, placed object all turn on / turn off

    [Header("Interaction State")]
    public bool onInteraction; //Only interaction with the UI if this value is true. ex) interation infoUI.
    //todo 24.0520
    /// <summary>
    /// If the mouse pointer enters the Editor UI Controller Ui
    /// cant build tile and object
    /// </summary>
    public bool onEnterMapEditorUi; 

    [Header("Effect")]
    public ParticleSystem particleEffect_ObejctClear;
    public GraphicRaycaster uiRaycaster;


    //todo Test 0426 Ui Grapic raycast



    private void Start()
    {
        onInteraction = true;
        invoker = new Invoker();
    }

    //private void Update()
    //{

    //    //if (MapEditor.Instance.mapEditorState != MapEditorState.NoEditor)
    //    //{


    //    //}


    //    //tile
    //    if (!onEnterMapEditorUi)
    //    {
    //        if (MapEditor.Instance.mapEditorState == MapEditorState.Tile)
    //        {
    //            GetMousePosition();
    //            TileMode();
    //        }
    //        else if (MapEditor.Instance.mapEditorState == MapEditorState.Object)
    //        {
    //            mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    //            mousePosition = new Vector3(mousePosition.x, mousePosition.y, 0);
    //            ObjectMode();
    //        }
    //        //else if(MapEditor.Instance.mapEditorState == MapEditorState.Background){ BackgroundMode();} // todo 0427
    //    }


    //}

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
                    if (Input.GetMouseButton(0)  && inGridPlaneMousePosition)
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
                    if (Input.GetMouseButton(0)  && inGridPlaneMousePosition)
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
        preViewTileMap.ClearAllTiles();
    }
    #endregion

    #region Object
    private void ObjectMode()
    {

        if (onEnterMapEditorUi) return;

        if (first_holdingObj != null)
        {
            //first_holdingObj.transform.position = new Vector3(mousePosition.x, mousePosition.y, 0);
            //first_holdingObj.transform.position = gridPosition;
            first_holdingObj.transform.position = mousePosition;
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
    //todo 0427
    #region Background
    private void BackgroundMode()
    {
        if(first_holdingObj != null)
        {
            Vector3 mousePot = Util.GetMouseWorldPosition(Input.mousePosition, _camera);
            if (Input.GetMouseButtonDown(0))
            {
                //Not need when add curPlaceObjList,
                CurbuildObject = first_holdingObj;
                first_holdingObj = null;

                objectModeClient.Create();

            }

            if (Input.GetMouseButton(1))
            {
                Destroy(first_holdingObj);
            }

        }
    }
    //todo 0427


    #endregion

    #region util
    public void GetMousePosition()
    {
        Vector3 mousePot = Util.GetMouseWorldPosition(Input.mousePosition, _camera);
        Collider2D collider = Physics2D.OverlapPoint(mousePot, gridPlaneLayerMask);


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

    //private void CheckUiMouseHover(Vector3 pot)
    //{

    //    PointerEventData pointerEventData = new PointerEventData(EventSystem.current); // PointerEventData 객체 생성
    //    pointerEventData.position = pot; // 마우스 위치 설정
    //    List<RaycastResult> results = new List<RaycastResult>(); // Raycast 결과 저장할 리스트

    //    uiRaycaster.Raycast(pointerEventData, results); // Raycast 수행

    //    // Raycast 결과가 있다면...
    //    if (results.Count > 0)
    //    {
    //        // 충돌된 UI 요소를 확인합니다.
    //        GameObject clickedObject = results[0].gameObject;
    //        Debug.Log("마우스가 UI 요소 " + clickedObject.name + "에 충돌했습니다!");
    //    }
    //    // Raycast 결과가 없다면...
    //    else
    //    {
    //        // 마우스가 UI 요소 밖에 있습니다.
    //        Debug.Log("마우스가 UI 요소 밖에 있습니다.");
    //    }


    //}

    // private void OnDrawGizmos()
    // {
    //     //mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    //     Gizmos.color = Color.red;
    //     Gizmos.DrawSphere(mousePosition, 0.1f);
    // }

    public void ObjectMode_Reset() //Indicatior,mouseHolding Object, curObj, arrow, additional Reset
    {
        if(curIndicatior != null){ Destroy(curIndicatior); }
        if(first_holdingObj != null){ Destroy(first_holdingObj); }
        if(CurbuildObject != null) { ResetColorObj();  CurbuildObject = null; }
        //if(curArrowIndicatorTrack != null) { Destroy(curArrowIndicatorTrack); }
        if(curAdditionalIndicatorTrack != null) { Destroy(curAdditionalIndicatorTrack); }





    }

    public void CurPlacedObjTurnOff()
    {
        foreach(BuildObj build in curPlaceObjList)
        {
            build.TurnOff();
        }
    }
    public void CurPlacedObjTurnOn()
    {
        foreach (BuildObj build in curPlaceObjList)
        {
            build.TurnOn();
        }
    }

    public bool CheckMousePosition_InGridBoundary(bool isBackgroundObj = false)
    {
        if (isBackgroundObj == true) return true; // if Background Object then need not check grid boundary.

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
            //curArrowIndicatorTrack = Instantiate(curObj_ArrowIndicator, CurbuildObject.transform);
            //curArrowIndicatorTrack.GetComponent<Arrow_Indicator>().SetLinkObj(CurbuildObject);

            //todo 0603
            SelectColorObj();
            //todo 0603

            BuildObj buildObj = CurbuildObject.GetComponent<BuildObj>();

            if (buildObj.id == 305 || buildObj.id == 306 || buildObj.id == 312)
            {
                GameObject additionalIndicator = Instantiate(Additional_Indicator);
                curAdditionalIndicatorTrack = additionalIndicator;
                additionalIndicator.GetComponent<Additional_Indicator>().SetLinkObj(CurbuildObject);
            }

        }

    }



    //todo 0603
    private void SelectColorObj()
    {
        ChangeColorRecursively(CurbuildObject.transform,Color.green);
    }

    private void ResetColorObj()
    {
        ChangeColorRecursively(CurbuildObject.transform, Color.white);
    }

    void ChangeColorRecursively(Transform parent,Color color)
    {
        SpriteRenderer spriteRenderer = parent.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.color = color;
        }
        foreach (Transform child in parent)
        {
            ChangeColorRecursively(child,color);
        }
    }
    //todo 0603



    public void CreateIndicator(ModeState modeState)
    {
        if(CurbuildObject != null)
        {
            switch (modeState)
            {
                case ModeState.Obj_Move:
                    //this.modeState = modeState;
                    //GameObject indicator = Instantiate(ObjMove_Indicator);
                    //CurIndicatior = indicator;
                    ////indicator.transform.SetParent(CurbuildObject.transform);
                    //indicator.transform.position = CurbuildObject.transform.position;
                    //indicator.GetComponent<Move_Indicator>().SetLinkObj(CurbuildObject);
                    //todo 0427
                    this.modeState = modeState;
                    GameObject indicator = Instantiate(ObjMove_Indicator);
                    CurIndicatior = indicator;
                    //indicator.transform.SetParent(CurbuildObject.transform);
                    indicator.transform.position = CurbuildObject.transform.position + (Vector3)CurbuildObject.GetComponent<BuildObj>().offset;
                    indicator.GetComponent<Move_Indicator>().SetLinkObj(CurbuildObject);
                    //todo 0427
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
