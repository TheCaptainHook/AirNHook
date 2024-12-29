using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawPathTest : MonoBehaviour
{
    PathFinder pathFinder;
    LineRenderer lineRenderer;

    public Transform start;
    public Transform end;



    private void Awake()
    {
        pathFinder = GetComponent<PathFinder>();
        lineRenderer = GetComponent<LineRenderer>();
    }



    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            DrawPath(start, end);
        }
    }

    public void DrawPath(Transform start,Transform end)
    {
        Vector2Int startPot = pathFinder.WorldToGrid(start.position);
        Vector2Int endPot = pathFinder.WorldToGrid(end.position);

        List<Vector2Int> path = pathFinder.FindPath(startPot, endPot);

        if(path== null) { return; }

        lineRenderer.positionCount = path.Count;
        for (int i = 0; i < path.Count; i++)
        {
            Vector3 worldPosition = pathFinder.GridToWorld(path[i]);
            lineRenderer.SetPosition(i, worldPosition);
        }
    }


}
