using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class PowerSupply_DrawLineUtility : MonoBehaviour
{
    private PathFinder pathFinder;
    private PathFinder PF { get { pathFinder ??= GetComponent<PathFinder>(); return pathFinder; } }
    private LineRenderer line;


    // public Material lineMat;
    public uint targetId;

    List<Vector2> pathList;
    public void Setting(Vector2 start, uint targetId, Material mat)
    {
        var target = NetworkClient.spawned.TryGetValue(targetId, out NetworkIdentity identity) ? identity : null;
        if (target == null) return;

        StartCoroutine(PF.FindPathCoroutine(start, target.transform.position,
            path =>
            {
                pathList = path;
                line = CreateLine(mat);
            },
            false, Direction_Type.Four));
    }
    #region  Clean
    public void Clean()
    {
        StopAllCoroutines();
    }
    #endregion
    #region  Draw,Erase
    private bool isDrawing;
    private bool isErasing;

    public void DrawOn()
    {
        if (isErasing)
        {
            StopCoroutine(eraseCoroutine);
            isErasing = false;
            eraseCoroutine = null;
        }

        if (drawCoroutine != null) StopCoroutine(drawCoroutine);
        drawCoroutine = StartCoroutine(DrawOn_MainToTarget());
    }
    private Coroutine drawCoroutine;
    private Coroutine eraseCoroutine;
    private float drawSpeed = 50f;
    private IEnumerator DrawOn_MainToTarget()
    {
        isDrawing = true;
        int index;
        Vector2 start;

        if (line.positionCount > 0) //paht 4(0,1,2,3)개 , line 2(0,1) 
        {
            index = line.positionCount - 1; //다음으로 시작해야할 path index
            start = line.GetPosition(index); //그리다만 라인렌더러 마지막 위치
        }
        else
        {
            line.positionCount = 1;
            start = pathList[0]; //첫번째 지점
            line.SetPosition(0, start);
            index = 1;  //다음 타겟 위치 path index
        }

        for (int i = index; i < pathList.Count; i++)
        {
            line.positionCount = i + 1;
            Vector2 end = pathList[i];
            float distance = Vector3.Distance(start, end);
            float duration = distance / drawSpeed;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                Vector3 pos = Vector3.Lerp(start, end, t);
                line.SetPosition(i, pos);
                yield return null;
            }
            line.SetPosition(i, end);
            start = end;
        }
        isDrawing = false;
        drawCoroutine = null;
    }

    public void EraseOn_TargetToMain()
    {
        if (isDrawing)
        {
            StopCoroutine(drawCoroutine);
            isDrawing = false;
            drawCoroutine = null;
        }

        if (eraseCoroutine != null) StopCoroutine(eraseCoroutine);
        eraseCoroutine = StartCoroutine(EraseOnCo());

    }
    private IEnumerator EraseOnCo() //line 3 (0,1,2) path 5(0,1,2,3,4)
    {
        isErasing = true;

        if (line.positionCount < 2)
        {
            isErasing = false;
            yield break;
        }

        // int index = pathList.Count - line.positionCount;
        Vector2 start = line.GetPosition(line.positionCount -1);

        for (int i = line.positionCount -2; i >= 0; i--)
        {
            Vector2 end = line.GetPosition(i);
            float distance = Vector3.Distance(start, end);
            float duration = distance / drawSpeed; // time = d/s
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                Vector3 pos = Vector3.Lerp(start, end, t);
                line.SetPosition(i + 1, pos);
                yield return null;
            }
            line.positionCount = i + 1;
            start = end;
        } 

        line.positionCount = 0;
        isErasing = false;
        eraseCoroutine = null;
    }
    #endregion

    #region  Create Line
    private LineRenderer CreateLine(Material mat)
    {
        LineRenderer lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.useWorldSpace = false;
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.material = mat;
        lineRenderer.positionCount = 0;
        lineRenderer.sortingLayerName = "Map/Tiles";
        lineRenderer.sortingOrder = 3;
        return lineRenderer;
    }
    #endregion

}
