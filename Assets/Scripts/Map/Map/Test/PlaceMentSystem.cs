using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class PlaceMentSystem : MonoBehaviour
{
    Util Util = new Util();
    private Camera _camera;
    public LayerMask layerMask;

    public Vector3Int gridPosition;
    private Vector3Int curPosition;
    private Vector3Int lastPosition;
    public Vector3 mousePosition;
    Sprite default_MouseIndicatorSprite;
    [SerializeField] GameObject mouseIndicator,cellIndicator;
    public Dictionary<Vector3Int, int> tileDic = new();

    public GameObject MouseIndicator { 
        get { return mouseIndicator; }
        set
        {
            if (value == null) { mouseIndicator.GetComponent<SpriteRenderer>().sprite = default_MouseIndicatorSprite;}
            else if (mouseIndicator.GetComponent<SpriteRenderer>().sprite != value.GetComponent<SpriteRenderer>().sprite)
            {
                mouseIndicator.GetComponent<SpriteRenderer>().sprite = value.GetComponent<SpriteRenderer>().sprite;
             
            }}
        }

    [SerializeField] Tilemap preViewTileMap;
    public Tilemap floorTileMap;
    public TileBase tileBase;

    private void Awake()
    {
        _camera = Camera.main;
        default_MouseIndicatorSprite = mouseIndicator.GetComponent<SpriteRenderer>().sprite;
    }

    private void Update()
    {
        if(MapEditor.Instance.mapEditorState == MapEditorState.Tile)
        {
            mouseIndicator.transform.position = GetMousePosition();
            mouseIndicator.transform.position += new Vector3(1, 1);
        }
        else
        {
            mouseIndicator.SetActive(false);
        }

        if(curPosition != gridPosition)
        {
            lastPosition = curPosition;
            curPosition = gridPosition;
            UpdatePreview();
        }

        if(MapEditor.Instance.mapEditorState == MapEditorState.Tile)
        {
            if (Input.GetMouseButton(0))
            {
                DrawTile();
            }
        }
    }



    public Vector3 GetMousePosition()
    {
        Vector3 mousePot = Util.GetMouseWorldPosition(Input.mousePosition, _camera);
        Collider2D collider = Physics2D.OverlapPoint(mousePot);
        if(collider != null)
        {
            Vector3Int cellPot = floorTileMap.WorldToCell(mousePot);
            gridPosition = new Vector3Int(Mathf.FloorToInt(floorTileMap.CellToWorld(cellPot).x), Mathf.FloorToInt(floorTileMap.CellToWorld(cellPot).y));

            return gridPosition;
        }
        return gridPosition;
    }

    private void OnDrawGizmos()
    {
        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(mousePosition, 0.1f);
    }


    void UpdatePreview()
    {
        preViewTileMap.SetTile(lastPosition, null);
        preViewTileMap.SetTile(curPosition, tileBase);
    }

    void DrawTile()
    {
        if(tileBase == null)
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
}
