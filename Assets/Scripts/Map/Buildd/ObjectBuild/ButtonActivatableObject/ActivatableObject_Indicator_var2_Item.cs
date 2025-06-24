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
    [SerializeField] SpriteRenderer mainSprite;

    [ReadOnly]
    public uint targetId;


    [ReadOnly]
    public bool onActive = false;
    [ReadOnly]
    public bool onDraw = false;

    public bool Setting(uint id,int inc)
    {
        Transform target = NetworkClient.spawned.TryGetValue(id, out var targetObject) ? targetObject.transform : null;
        if (target == null) return false;

        targetId = id;
        targetTr = target;
        StartCoroutine(SetPathCoroutine(inc));

        return true;
    }

    #region PathFinder
    public List<Vector2> pathList;
    private IEnumerator SetPathCoroutine(int inc)
    {
        yield return StartCoroutine(pathFinder.FindPathCoroutine(transform.position, targetTr.position, path =>
        {
            if (path != null)
            {
                pathList = path;
                SetActive(inc);
            }
            else
            {
                Debug.Log("경로를 찾지 못함");
            }
        }, false, Direction_Type.Four));
    }
    #endregion


    /// <summary>
    /// </summary>
    /// <param name="inc">[-1] Erase(Deactive), [1] Draw(active)</param>
    public void SetActive(int inc)
    {
        switch (inc)
        {
            case -1:
                if (onActive && onDraw) Erase();
                onActive = false;
                onDraw = false;

                break;
            case 1:
                if (!onDraw) Draw();
                onActive = true;
                onDraw = true;
                break;
            case 2:
                if (onDraw) Erase(); //조건 충족, 단순 라인 제거용
                onActive = true;
                onDraw = false;
                break;
            case 3:
                if (!onDraw) Draw(); //조건 충족,
                onDraw = true;

                break;
        }
    }


    private Coroutine drawCoroutine;
    private Coroutine eraseCoroutine;

    public void Draw()
    {
        if(!gameObject.activeSelf) gameObject.SetActive(true);

        if(eraseCoroutine != null)
        {
            StopCoroutine(eraseCoroutine);
            eraseCoroutine = null;
        }
      
        drawCoroutine = StartCoroutine(DrawOn(pathList));
    }
    public void Erase()
    {
        if (lineRenderer.positionCount == 0) return;

        if (drawCoroutine != null)
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

        if (index == 0)
        {
            eraseCoroutine = null;
            yield break;
        }

        Vector3 end = lineRenderer.GetPosition(index-1);

        for (int i = index - 2; i >= 0; i--)
        {
            Vector3 start = pathList[i];
            float distance = Vector3.Distance(start, end);
            float duration = distance / drawSpeed;
            float elapsed = 0f;

            lineRenderer.positionCount = i + 2;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                Vector3 pos = Vector3.Lerp(end, start, t);
                lineRenderer.SetPosition(i + 1, pos);
                yield return null;
            }

            end = start;
        }
        lineRenderer.positionCount = 0;
        eraseCoroutine = null;
        
    }
    #endregion

}
