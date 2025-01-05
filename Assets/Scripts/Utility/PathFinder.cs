
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PathFinder : MonoBehaviour
{
    //test
    public LayerMask obstacleLayer;
    private float cellSize = 1.0f; // 그리드 셀 크기

    Vector2Int[] directions_4 = { Vector2Int.down,Vector2Int.left,Vector2Int.up,Vector2Int.right};
    Vector2Int[] directions_8 = new Vector2Int[]
{
    new Vector2Int(0, 1),   
    new Vector2Int(0, -1),  
    new Vector2Int(1, 0),   
    new Vector2Int(-1, 0),  
    new Vector2Int(1, 1),   
    new Vector2Int(1, -1),  
    new Vector2Int(-1, 1),  
    new Vector2Int(-1, -1)  
};
    [ReadOnly]
    public Tilemap floorTileMap;

    public List<Vector2Int> FindPath(Vector2Int start, Vector2Int end)
    {

        floorTileMap = MapEditor.Instance.placeMentSystem.floorTileMap;
        PriorityQueue<Node> openSet = new();
        HashSet<Vector2Int> closedSet = new HashSet<Vector2Int>();

        openSet.Enqueue(new Node(start,0,null),Heuristic(start,end));

        while (openSet.Count > 0)
        {
            var current = openSet.Dequeue();
            Vector2Int curPosition = Vector2Int.RoundToInt(current.Position);

            if (curPosition == end)
            {
                Node endNode = new Node(end,current.GCost+1,current);
                return ReconstructPath_Vector2Int(endNode);
            }

            closedSet.Add(curPosition);

            foreach(var dir in directions_4)
            {
                Vector2Int nextPosition = curPosition + dir;

                if (closedSet.Contains(nextPosition) || IsObstacle(nextPosition)){ continue;}

                float nextGCost = current.GCost+1;
                if(openSet.Contains(nextPosition,out var existingNode))
                {
                    if (nextGCost < existingNode.GCost)
                    {
                        existingNode.Update(nextGCost, current);
                        openSet.UpdatePriority(existingNode, nextGCost + Heuristic(nextPosition, end));
                    }
                }
                else
                {
                    // 새 노드 추가
                    var nextNode = new Node(nextPosition, nextGCost, current);
                    openSet.Enqueue(nextNode, nextGCost + Heuristic(nextPosition, end));
                }
                  
            }
        }
        return null;
    }
  
    public List<Vector2> FindPath(Vector2 start, Vector2 end)
    {
        PriorityQueue<Node> openSet = new();
        HashSet<Vector2> closedSet = new HashSet<Vector2>();

        openSet.Enqueue(new Node(start, 0, null), Heuristic(start, end));

        while (openSet.Count > 0)
        {
            var current = openSet.Dequeue();
            Vector2 curPosition = current.Position;

            // 종료 조건: 목적지에 충분히 근접
            if (CheckDistance(curPosition, end))
            {
                Node endNode = new Node(end, current.GCost+1, current);
                return ReconstructPath(endNode);
            }

            closedSet.Add(curPosition);

            // 이웃 탐색
            foreach (var dir in directions_8)
            {
                Vector2 nextPosition = curPosition + dir;

                if (closedSet.Contains(nextPosition) || IsObstacle(nextPosition))
                    continue;

                float nextGCost = current.GCost + 1;

                if (openSet.Contains(nextPosition, out var existingNode))
                {
                    // 이미 존재하면 더 짧은 경로인지 확인
                    if (nextGCost < existingNode.GCost)
                    {
                        existingNode.Update(nextGCost, current);
                        openSet.UpdatePriority(existingNode, nextGCost + Heuristic(nextPosition, end));
                    }
                }
                else
                {
                    // 새 노드 추가
                    var nextNode = new Node(nextPosition, nextGCost, current);
                    openSet.Enqueue(nextNode, nextGCost + Heuristic(nextPosition, end));
                }
            }
        }

        // 경로를 찾지 못함
        return null;
    }
    
    private List<Vector2> ReconstructPath(Node node)
    {
        List<Vector2> path = new List<Vector2>();
        Vector2 previousPosition = node.Position;
        Vector2 curDir = Vector2.zero;

        Node curNode = node;

        while (node != null)
        {
            curNode = node;
            //1. 이전 위치
            //2. 현재 위치
            //3. 현재 진행방향
            if(!CheckDir(curDir,previousPosition,node.Position))
            {
                if (path.Count == 0 || path[^1] != previousPosition) // 중복 방지
                {
                    path.Add(previousPosition);
                }
                curDir = (node.Position - previousPosition).normalized;
                previousPosition = node.Position;

            }else
            {
                previousPosition = node.Position;
            }
            // if(CheckDir(previousDir,node.Position))
            // {
            //     accumulated =node.Position;
            // }
            // else
            // {
            //     path.Add(accumulated);
            //     previousDir = node.Position.normalized;
            //     accumulated = node.Position;

            // }

            node = node.Parent;

            // path.Add(node.Position);
            
        }
        if (path.Count == 0 || path[^1] != curNode.Position)
        {
            path.Add(curNode.Position);
        }

        path.Reverse();
        return path;
    }
    private bool CheckDir(Vector2 curDir ,Vector2 previousPosition,Vector2 curPosition)
    {
        Vector2 dir = (curPosition - previousPosition).normalized;

        return Vector2.Dot(curDir, dir) > 0.999f; 
    }
     private List<Vector2Int> ReconstructPath_Vector2Int(Node node)
    {
        List<Vector2Int> path = new List<Vector2Int>();
        while (node != null)
        {
            path.Add(Vector2Int.RoundToInt(node.Position));
            node = node.Parent;
        }
        path.Reverse();
        return path;
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
    private float Heuristic(Vector2 a,Vector2 b){
        return Vector2.Distance(a, b);
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


public class Node
{
    public Vector2 Position { get; }
    public float GCost { get; private set; }
    public Node Parent { get; private set; }

    public Node(Vector2 position, float gCost, Node parent)
    {
        Position = position;
        GCost = gCost;
        Parent = parent;
    }

    public void Update(float gCost, Node parent)
    {
        GCost = gCost;
        Parent = parent;
    }
  
}

public class PriorityQueue<T>
{
    private List<(T Item, float Priority)> heap = new();
    public int Count => heap.Count;

    public void Enqueue(T item, float priority) 
    {
        heap.Add((item,priority));
        int currentIndex = heap.Count-1;

        //while(currentIndex> 0)
        //{
        //    int parentIndex = (currentIndex - 1) / 2;
        //    if (heap[currentIndex].Priority >= heap[parentIndex].Priority) break;

        //    (heap[currentIndex], heap[parentIndex]) = (heap[parentIndex], heap[currentIndex]);
        //    currentIndex = parentIndex;
        //}
        HeapifyUp(currentIndex);

    }
    private void HeapifyUp(int index)
    {
        while (index > 0)
        {
            int parentIndex = (index - 1) / 2;
            if (heap[index].Priority >= heap[parentIndex].Priority)
                break;

            Swap(index, parentIndex);
            index = parentIndex;
        }
    }
    private void HeapifyDown(int index)
    {
        int lastIndex = heap.Count - 1;

        while (true)
        {
            int leftChildIndex = 2 * index + 1;
            int rightChildIndex = 2 * index + 2;
            int smallestIndex = index;

            if (leftChildIndex <= lastIndex && heap[leftChildIndex].Priority < heap[smallestIndex].Priority)
            {
                smallestIndex = leftChildIndex;
            }

            if (rightChildIndex <= lastIndex && heap[rightChildIndex].Priority < heap[smallestIndex].Priority)
            {
                smallestIndex = rightChildIndex;
            }

            if (smallestIndex == index)
                break;

            Swap(index, smallestIndex);
            index = smallestIndex;
        }
    }
    public T Dequeue()
    {
        if (heap.Count == 0) throw new System.InvalidOperationException("The queue is empty.");

        T root = heap[0].Item;
        heap[0] = heap[^1];
        heap.RemoveAt(heap.Count-1);


        int currentIndex = 0;
        HeapifyDown(currentIndex);
        //while (true)
        //{
        //    int leftChildIndex = 2 * currentIndex + 1;
        //    int rightChildIndex = 2 * currentIndex + 2;

        //    if (leftChildIndex >= heap.Count) break;

        //    int smallestChildIndex = (rightChildIndex < heap.Count && heap[rightChildIndex].Priority < heap[leftChildIndex].Priority)
        //        ? rightChildIndex
        //        : leftChildIndex;

        //    if (heap[currentIndex].Priority <= heap[smallestChildIndex].Priority) break;

        //    (heap[currentIndex], heap[smallestChildIndex]) = (heap[smallestChildIndex], heap[currentIndex]);
        //    currentIndex = smallestChildIndex;
        //}

        return root;

    }
    public bool Contains(Vector2 item,out Node node)
    {
        foreach(var el in heap)
        {
           if(el.Item is Node nodeItem && nodeItem.Position == item)
            {
                node = nodeItem;
                return true;
            }
        }
        node = default;
        return false;
    }

    public void UpdatePriority(T item, float newPriority)
    {
        for (int i = 0; i < heap.Count; i++)
        {
            if (EqualityComparer<T>.Default.Equals(heap[i].Item, item))
            {
                heap[i] = (item, newPriority);

                // 위로 또는 아래로 힙 구조를 복구
                HeapifyUp(i);
                HeapifyDown(i);
                return;
            }
        }

        throw new InvalidOperationException("Item not found in queue.");
    }

    private void Swap(int index1, int index2)
    {
        var temp = heap[index1];
        heap[index1] = heap[index2];
        heap[index2] = temp;
    }
}
