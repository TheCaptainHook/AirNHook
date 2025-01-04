
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.Tilemaps;

public class PathFinder : MonoBehaviour
{
    //test
    public LayerMask obstacleLayer;
    private float cellSize = 1.0f; // 그리드 셀 크기

    Vector2Int[] directions = { Vector2Int.down,Vector2Int.left,Vector2Int.up,Vector2Int.right};

    [ReadOnly]
    public Tilemap floorTileMap;

    public List<Vector2Int> FindPath(Vector2Int start, Vector2Int end)
    {

        floorTileMap = MapEditor.Instance.placeMentSystem.floorTileMap;

        PriorityQueue<(Vector2Int position,int gCost,List<Vector2Int> path)> openSet = new();

        HashSet<Vector2Int> closedSet = new HashSet<Vector2Int>();

        openSet.Enqueue((start, 0,new List<Vector2Int> { start }),0+Heuristic(start,end));

        while (openSet.Count > 0)
        {
            var current = openSet.Dequeue();
            Vector2Int curPosition = current.position;
            int curGCost = current.gCost;
            List<Vector2Int> curPath = current.path;

            if (curPosition == end) return current.path;

            closedSet.Add(curPosition);

            foreach(var dir in directions)
            {
                Vector2Int nextPosition = curPosition + dir;

                if (closedSet.Contains(nextPosition) || IsObstacle(nextPosition)){ continue;}

                int nextGCost = curGCost + 1;
                int nextHCost = Heuristic(nextPosition, end);

                List<Vector2Int> nextPath = new List<Vector2Int>(curPath) {nextPosition };
                openSet.Enqueue((nextPosition, nextGCost, nextPath), nextGCost + nextHCost);

                  
            }
        }
        return null;
    }
    public List<Vector2> FindPath(Vector2 start,Vector2 end)
    {
        PriorityQueue<(Vector2 position,int gCost,List<Vector2> path)> openSet = new();

        HashSet<Vector2> closedSet = new HashSet<Vector2>();
        openSet.Enqueue((start, 0,new List<Vector2> { start }),0+Heuristic(start,end));
        while(openSet.Count>0)
        {
            var current = openSet.Dequeue();
            Vector2 curPosition = current.position;
            int curGCost = current.gCost;
            List<Vector2> curPath = current.path;

            if(CheckDistance(curPosition,end))
            {
                List<Vector2> nextPath = new List<Vector2>(curPath){end};
                return nextPath;
            }

            closedSet.Add(curPosition);

            foreach(var dir in directions)
            {
                Vector2 nextPosition =curPosition + dir;
                if(closedSet.Contains(nextPosition)||IsObstacle(nextPosition)){continue;}

                int nextGCost = curGCost+1;
                int nextHCost = Heuristic(nextPosition,end);

                List<Vector2> nextPath = new List<Vector2>(curPath) {nextPosition };
                openSet.Enqueue((nextPosition, nextGCost, nextPath), nextGCost + nextHCost);
            }
        }

        return null;
    }

    private bool CheckDistance(Vector2 cur,Vector2 end)
    {
        return Vector2.Distance(cur,end) <= 1;
    }

    private bool IsObstacle(Vector2Int gridPosition)
    {
        // 월드 좌표로 변환
        Vector3 worldPosition = GridToWorld(gridPosition);
        
        // 해당 위치에 장애물이 있는지 확인
        
        TileBase tile = floorTileMap.GetTile(floorTileMap.WorldToCell(worldPosition));

        if (tile != null)
        {
            return true;
        }
        else return false;
    }

private bool IsObstacle(Vector2 position)
{
    Collider2D hit = Physics2D.OverlapPoint(position, obstacleLayer);
    return hit != null;
}

    private int Heuristic(Vector2Int a, Vector2Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }
    private int Heuristic(Vector2 a,Vector2 b){
        return Mathf.RoundToInt(Mathf.Abs(a.x-b.x)) +Mathf.RoundToInt(Mathf.Abs(a.y-b.y)); 
    }


    public Vector2Int WorldToGrid(Vector3 worldPosition)
    {
      
        // 월드 좌표를 그리드 좌표로 변환
        return new Vector2Int(
            Mathf.RoundToInt(worldPosition.x / cellSize),
            Mathf.RoundToInt(worldPosition.y / cellSize)
        );
    }

    public Vector3 GridToWorld(Vector2Int gridPosition)
    {
        // 그리드 좌표를 월드 좌표로 변환
        return new Vector3(gridPosition.x * cellSize, gridPosition.y * cellSize, 0);
    }
}


public class PriorityQueue<T>
{
    private List<(T Item, int Priority)> heap = new();
    public int Count => heap.Count;

    public void Enqueue(T item, int priority) 
    {
        heap.Add((item,priority));
        int currentIndex = heap.Count-1;

        while(currentIndex> 0)
        {
            int parentIndex = (currentIndex - 1) / 2;
            if (heap[currentIndex].Priority >= heap[parentIndex].Priority) break;

            (heap[currentIndex], heap[parentIndex]) = (heap[parentIndex], heap[currentIndex]);
            currentIndex = parentIndex;
        }

    }
    public T Dequeue()
    {
        if (heap.Count == 0) throw new System.InvalidOperationException("The queue is empty.");

        T root = heap[0].Item;
        heap[0] = heap[^1];
        heap.RemoveAt(heap.Count-1);


        int currentIndex = 0;
        while (true)
        {
            int leftChildIndex = 2 * currentIndex + 1;
            int rightChildIndex = 2 * currentIndex + 2;

            if (leftChildIndex >= heap.Count) break;

            int smallestChildIndex = (rightChildIndex < heap.Count && heap[rightChildIndex].Priority < heap[leftChildIndex].Priority)
                ? rightChildIndex
                : leftChildIndex;

            if (heap[currentIndex].Priority <= heap[smallestChildIndex].Priority) break;

            (heap[currentIndex], heap[smallestChildIndex]) = (heap[smallestChildIndex], heap[currentIndex]);
            currentIndex = smallestChildIndex;
        }

        return root;

    }
}
