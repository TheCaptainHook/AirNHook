using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivatableObject_Indicator_var2_Item : MonoBehaviour
{
    [ReadOnly]
    public Transform targetTr;

    [SerializeField] PathFinder pathFinder;

    [SerializeField] LineRenderer lineRenderer;

    public bool Setting(uint id)
    {
        Transform target = NetworkClient.spawned.TryGetValue(id, out var targetObject) ? targetObject.transform : null;
        if (target == null) return false;

        targetTr = target;
        StartCoroutine(SetPathCoroutine());

        return true;
    }

    #region PathFinder
    public List<Vector2> pathList;
    private IEnumerator SetPathCoroutine()
    {
        yield return StartCoroutine(pathFinder.FindPathCoroutine(transform.position, targetTr.position, path =>
        {
            if (path != null)
            {
                pathList = path;
                Draw();
            }
            else
            {
                Debug.Log("경로를 찾지 못함");
            }
        }, false, Direction_Type.Four));
    }
    #endregion



    public void SetActive(int inc)
    {
        if (inc > 0)
        {
            Draw();
        }
        else
        {
            Erase();
        }

    }

    private Coroutine drawCoroutine;
    private Coroutine eraseCoroutine;

    private void Draw()
    {
        if(!gameObject.activeSelf) gameObject.SetActive(true);

        if(eraseCoroutine != null)
        {
            StopCoroutine(eraseCoroutine);
            eraseCoroutine = null;
        }

        drawCoroutine = StartCoroutine(DrawOn(pathList));
     

    }
    private void Erase()
    {
        if(drawCoroutine != null)
        {
            StopCoroutine(drawCoroutine);
            drawCoroutine = null;
        }

        eraseCoroutine = StartCoroutine(EraseCo());
    }


    #region Coroutine
    private float drawSpeed = 30f;
    private IEnumerator DrawOn(List<Vector2> path)
    {
        if (path == null || path.Count < 2)
            yield break;

        lineRenderer.positionCount = 1;

        Vector3 start = path[0];
        lineRenderer.SetPosition(0, start);


        for (int i = 1; i < path.Count; i++)
        {
            lineRenderer.positionCount = i + 1;
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
        drawCoroutine = null;
    }
    private IEnumerator EraseCo()
    {
        var index = lineRenderer.positionCount;
        Vector3 end = lineRenderer.GetPosition(index-1);

        for (int i = index - 2; i >= 0; i--)
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
                lineRenderer.SetPosition(i + 1, pos);
                yield return null;
            }
            //lineRenderer.SetPosition(i, start);
            lineRenderer.positionCount = i + 1;
            end = start;
        }
        lineRenderer.positionCount = 0;
        eraseCoroutine = null;
        gameObject.SetActive(false);
    }
    #endregion

}
