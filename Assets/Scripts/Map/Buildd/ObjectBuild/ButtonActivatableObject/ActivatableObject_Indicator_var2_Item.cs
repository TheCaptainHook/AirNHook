using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
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

    public bool Setting(uint id, int inc)
    {
        Transform target = NetworkClient.spawned.TryGetValue(id, out var targetObject) ? targetObject.transform : null;
        if (target == null) return false;

        targetId = id;
        targetTr = target;
        StartCoroutine(SetPathCoroutine(inc));

        return true;
    }
    #region Fade
    private Color lineStartColor;
    private Color lineEndColor;
    #endregion

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

    private void Awake()
    {
        lineStartColor = lineRenderer.startColor;
        lineEndColor = lineRenderer.endColor;
    }

    /// <summary>
    /// </summary>
    /// <param name="inc">[-1] Erase(Deactive), [1] Draw(active)</param>
    public void SetActive(int inc)
    {
        switch (inc)
        {
            case -1:
                if (onDraw) Erase();
                onActive = false;
                onDraw = false;

                break;
            case 1:
                if (!onDraw) Draw(() => Mark_Green());
                onActive = true;
                onDraw = true;
                break;
            // case 2:
            //     if (onDraw) Erase(); //조건 충족, 단순 라인 제거용
            //     onActive = true;
            //     onDraw = false;
            //     break;
            case 3:
                if (!onDraw) Draw(()=>Mark_Red());
                onDraw = true;

                break;
        }
    }


    private Coroutine drawCoroutine;
    private Coroutine eraseCoroutine;

    public void Draw(Action markAction = null)
    {
        if (!gameObject.activeSelf) gameObject.SetActive(true);

        if (eraseCoroutine != null)
        {
            StopCoroutine(eraseCoroutine);
            eraseCoroutine = null;
        }

        // drawCoroutine = StartCoroutine(DrawOn_MainToTarget(pathList));
        drawCoroutine = StartCoroutine(DrawOn_TargetToMain(pathList, markAction));
    }
    public void Erase()
    {
        if (lineRenderer.positionCount == 0) return;

        if (drawCoroutine != null)
        {
            StopCoroutine(drawCoroutine);
            drawCoroutine = null;
        }

        eraseCoroutine = StartCoroutine(EraseCo_MainToTarget());
        // eraseCoroutine = StartCoroutine(EraseCo_MainToTarget());
    }


    #region Draw,Erase Coroutine
    public bool onPrograss;
    enum LINE_DIRECTION
    {
        MainToTarget,
        TargetToMain
    }
    LINE_DIRECTION line_direction;

    private float drawSpeed = 30f;
    #region Draw
    private IEnumerator DrawOn_MainToTarget(List<Vector2> path) //<-> [Eraser] TargetToMain 
    {
        line_direction = LINE_DIRECTION.MainToTarget;

        if (path == null || path.Count < 2) yield break;

        onPrograss = true;

        Vector3 start;
        int index;

        if (lineRenderer.positionCount > 0)
        {
            index = lineRenderer.positionCount - 1;
            start = lineRenderer.GetPosition(index);
        }
        else
        {
            lineRenderer.positionCount = 1;
            start = path[0];
            lineRenderer.SetPosition(0, start);
            index = 1;

        }

        for (int i = index; i < path.Count; i++)
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
        onPrograss = false;
        drawCoroutine = null;

    }
    private IEnumerator DrawOn_TargetToMain(List<Vector2> path, Action markEnableAction) //<-> MainToTarget
    {
        line_direction = LINE_DIRECTION.TargetToMain;
        if (path == null || path.Count < 2) yield break;

        onPrograss = true;
        int index;
        Vector2 start;

        if (lineRenderer.positionCount > 0)
        {
            index = path.Count - lineRenderer.positionCount; //다음으로 시작해야할 path index
            start = lineRenderer.GetPosition(lineRenderer.positionCount - 1); //그리다만 라인렌더러 마지막 위치
        }
        else
        {
            lineRenderer.positionCount = 1;
            start = path[path.Count - 1]; //마지막요소
            lineRenderer.SetPosition(0, start);
            index = path.Count - 2;   //다음 타겟 위치 path index
        }

        for (int i = lineRenderer.positionCount; i < path.Count; i++)
        {
            // int lineIndex = path.Count - i;
            lineRenderer.positionCount = i + 1;

            Vector2 end = path[index];
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
            index--;
            start = end;
        }
        //---------------------------TEST 0627 (Mark Change)
        markEnableAction?.Invoke();
        //---------------------------TEST 0627 (Mark Change)
        onPrograss = false;

        drawCoroutine = null;


    }
    #endregion

    #region Erase
    private IEnumerator EraseCo_TargetToMain()
    {
        if (line_direction == LINE_DIRECTION.TargetToMain) ReverseLineRendererPosition(false);
        line_direction = LINE_DIRECTION.TargetToMain;

        var index = lineRenderer.positionCount;
        if (index == 0)
        {
            eraseCoroutine = null;
            yield break;
        }

        onPrograss = true;

        Vector3 end = lineRenderer.GetPosition(index - 1);

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
            lineRenderer.positionCount = i + 1;
            end = start;
        }

        lineRenderer.positionCount = 0;
        eraseCoroutine = null;
        onPrograss = false;

    }

    private IEnumerator EraseCo_MainToTarget() //<-> Draw_TargetToMain
    {
        if (line_direction == LINE_DIRECTION.MainToTarget) ReverseLineRendererPosition(true);
        line_direction = LINE_DIRECTION.MainToTarget;

        //---------------------------TEST 0627 (Mark Change)
        Mark_ShutDown();
        //---------------------------TEST 0627 (Mark Change)

        if (lineRenderer.positionCount == 0)
        {
            eraseCoroutine = null;
            yield break;
        }

        onPrograss = true;

        Vector2 end = lineRenderer.GetPosition(lineRenderer.positionCount - 1);

        for (int i = lineRenderer.positionCount - 2; i >= 0; i--)
        {
            Vector2 start = lineRenderer.GetPosition(i);

            float distance = Vector3.Distance(start, end);
            float duration = distance / drawSpeed; // time = d/s
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                Vector3 pos = Vector3.Lerp(end, start, t);
                lineRenderer.SetPosition(i + 1, pos);
                yield return null;
            }

            lineRenderer.positionCount = i + 1;
            end = start;
        }

        lineRenderer.positionCount = 0;
        eraseCoroutine = null;
        onPrograss = false;

    }
    #endregion

    private void ReverseLineRendererPosition(bool onReverse)
    {
        if (onReverse)
        {
            for (int i = 0; i < lineRenderer.positionCount; i++)
            {
                var pot = pathList[pathList.Count - 1 - i];
                lineRenderer.SetPosition(i, pot);
            }
        }
        else
        {
            for (int i = 0; i < lineRenderer.positionCount; i++)
            {
                var pot = pathList[i];
                lineRenderer.SetPosition(i, pot);
            }
        }
        //MainToTarget : 0 -> end
        //TargetToMain : end -> 0
    }
    #endregion

    #region  Mark. (TEST/0627)
    public void Mark_Red()
    {
        mainSprite.enabled = true;
        mainSprite.color = Color.red;
    }
    public void Mark_Green()
    {
        mainSprite.enabled = true;
        mainSprite.color = Color.green;
    }
    public void Mark_ShutDown()
    {
        mainSprite.color = Color.red;
        mainSprite.enabled = false;
    }
    #endregion


    #region Fade
    public bool isFading;
    float maxAlpha = 1;
    float minAlpha = 0.05f;
    float duration = 1;
    public Coroutine satisfyConditionCoroutine;
    public void SatisfyCondition_FadeOutLine()
    {
        if (satisfyConditionCoroutine != null) StopCoroutine(satisfyConditionCoroutine);
        satisfyConditionCoroutine = StartCoroutine(SatisfyCondition_FadeOutLineCo());
    }

    private IEnumerator SatisfyCondition_FadeOutLineCo()
    {
        isFading = true;
        float elapsed = 0;
        Color curStartCol = lineRenderer.startColor;
        Color fade_Start = new Color(curStartCol.r, curStartCol.g, curStartCol.b, minAlpha);

        Color curEndColr = lineRenderer.endColor;
        Color fade_End = new Color(curEndColr.r, curEndColr.g, curEndColr.b, minAlpha);

        while(elapsed < duration)
        {
            elapsed += Time.deltaTime;
            lineRenderer.startColor = Color.Lerp(curStartCol, fade_Start, elapsed);
            lineRenderer.endColor = Color.Lerp(curEndColr, fade_End, elapsed);
            yield return null;
        }

        lineRenderer.startColor = fade_Start;
        lineRenderer.endColor = fade_End;
        isFading = false;
    }
    public void FadeRecover()
    {
        if (satisfyConditionCoroutine != null)
        {
            StopCoroutine(satisfyConditionCoroutine);
            satisfyConditionCoroutine = null;
        }

        lineRenderer.startColor = new Color(lineStartColor.r, lineStartColor.g, lineStartColor.b, maxAlpha);
        lineRenderer.endColor = new Color(lineEndColor.r, lineEndColor.g, lineEndColor.b, maxAlpha);
    }
    #endregion
}
