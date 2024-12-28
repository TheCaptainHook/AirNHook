using Org.BouncyCastle.Asn1.Crmf;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinder : MonoBehaviour
{



    private float cellSize = 1.0f; // 그리드 셀 크기

    Vector2Int[] directions = { Vector2Int.down,Vector2Int.left,Vector2Int.up,Vector2Int.right};



  


    
    private Vector2Int WorldToGrid(Vector3 worldPosition)
    {
      
        // 월드 좌표를 그리드 좌표로 변환
        return new Vector2Int(
            Mathf.RoundToInt(worldPosition.x / cellSize),
            Mathf.RoundToInt(worldPosition.y / cellSize)
        );
    }

    private Vector3 GridToWorld(Vector2Int gridPosition)
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
