using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class PlaceMentSystem : MonoBehaviour
{
    Util Util = new Util();
    private Camera _camera;
    public LayerMask layerMask;

    private Vector3 lastPosition;
    

    [SerializeField] GameObject mouseIndicator,cellIndicator;
    [SerializeField] Tilemap tilemap;
    private void Awake()
    {
        _camera = Camera.main;
        
    }

    private void Update()
    {
        mouseIndicator.transform.position = GetMousePosition();
        mouseIndicator.transform.position += new Vector3(1,1);

    }


    Vector3 GetMousePosition()
    {
        Vector3 mousePot = Util.GetMouseWorldPosition(Input.mousePosition, _camera);
        Collider2D collider = Physics2D.OverlapPoint(mousePot);
        if(collider != null)
        {
            mouseIndicator.SetActive(true);
            Vector3Int cellPot = tilemap.WorldToCell(mousePot);
            lastPosition = tilemap.CellToWorld(cellPot);
            return lastPosition;
        }
        mouseIndicator.SetActive(false);
        return lastPosition;
    }

    private void OnDrawGizmos()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(mousePosition, 0.1f);
    }
}
