using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class TestPathFinder_0616 : MonoBehaviour
{
    [SerializeField] PathFinder pathFinder;

    [SerializeField] Transform end;


    [SerializeField] LineRenderer lineRenderer;
    private List<Vector2> pathList;


    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Q))
        {
            StartCoroutine(PathFinder());
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            StartCoroutine(Eraser());
        }
    }

    IEnumerator PathFinder()
    {
        yield return StartCoroutine(pathFinder.FindPathCoroutine(transform.position, end.position, path =>
        {
            if (path != null)
            {
                pathList = path;
                StartCoroutine(DrawOn(pathList));
            }
            else
            {
                Debug.Log("경로를 찾지 못함");
            }
        },false,Direction_Type.Four));
    }


    private void DrawLine(List<Vector2> path)
    {
        lineRenderer.positionCount = path.Count;

        for (int i = 0; i < path.Count; i++)
        {
            lineRenderer.SetPosition(i, path[i]);
        }
    }



    private IEnumerator DrawOn(List<Vector2> path, float drawSpeed = 30f)
    {
        if (path == null || path.Count < 2)
            yield break;

        lineRenderer.positionCount = 1;

        Vector3 start = path[0];
        lineRenderer.SetPosition(0, start);


        for (int i = 1; i < path.Count; i++)
        {
            lineRenderer.positionCount = i +1;
            Vector3 end = path[i];

            float distance = Vector3.Distance(start, end);
            float duration = distance / drawSpeed;
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                Vector3 pos = Vector3.Lerp(start, end, t);
                lineRenderer.SetPosition(i, pos);
                yield return null;
            }
            lineRenderer.SetPosition(i, end);
            // 다음 구간을 위해 현재 end를 start로 고정
            start = end;

        }

    }
    private IEnumerator Eraser(float drawSpeed = 30f)
    {
        Vector3 end = pathList[^1];
        for (int i = pathList.Count - 2; i >= 0; i--)
        {
            Vector3 start = pathList[i];
            float distance = Vector3.Distance(start, end);
            float duration = distance / drawSpeed;
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                Vector3 pos = Vector3.Lerp(end, start, t);
                lineRenderer.SetPosition(i+1, pos);
                yield return null;
            }
            //lineRenderer.SetPosition(i, start);
            lineRenderer.positionCount = i+1;
            end = start;
        }
        lineRenderer.positionCount = 0;
    }
}
