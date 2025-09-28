using ANH_MapEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Mirror;


public class Indicator_var2_DrawLineUtility : MonoBehaviour
{
    private Coroutine coroutine;
    private LineRenderer lineRenderer;
    private LineRenderer Line { get { lineRenderer ??= GetComponent<LineRenderer>(); return lineRenderer; } }

    #region ID
    public uint id;
    public ActivatableObject_Indicator_var2_Item item;
    public Transform target;
 
    #endregion

    public int curApplyActive;
    public void SettingAndDrawLine(uint id,
        ActivatableObject_Indicator_var2_Item item,
        Transform target,
        Action applyActiveAction,
        bool notObstacle
        )
    {
        this.id = id;
        this.item = item;
        this.target = target;
        this.notObstacle = notObstacle;

        StartCoroutine(SetPathCoroutine(applyActiveAction));
         
    }
    #region  Path Chaking
    public void PathChacking(ActivatableObject_Indicator_var2 indicator ,uint targetId,ActivatableObject_Indicator_var2_Item item)
    {
        var target = NetworkClient.spawned.TryGetValue(targetId, out NetworkIdentity identity) ? identity.gameObject : null;
        if (target == null) return;
        notObstacle = indicator.notObstacle;

        StartCoroutine(PathChackingCo(indicator,item,target.transform.position));
    }

    private bool InnerFloorTileChack(Vector2 itemPot,Vector2 targetPot)
    {
        Vector2 probeSize = new Vector2(0.5f, 0.5f);
        var hit = Physics2D.OverlapBox(itemPot, probeSize, 0f, PathFinder.obstacleLayer);
        var hit2 = Physics2D.OverlapBox(targetPot, probeSize, 0f, PathFinder.obstacleLayer);

        if (hit == null && hit2 == null)
        {
            Debug.Log("Not Found hit");
            return false;
        }
        else
        {
            Debug.Log("Found hit");
            return true;
        }

        
    }
    
    private IEnumerator PathChackingCo(ActivatableObject_Indicator_var2 indicator, ActivatableObject_Indicator_var2_Item item, Vector2 targetPosition)
    {
        //바닥타일 내부에 있는지 체크,
        if (InnerFloorTileChack(item.transform.position,targetPosition))
        {
            indicator.notObstacle = true;
            indicator.itemWaitStack.Push(item);
            indicator.lineQueue.Enqueue(this);
            yield break;
        }

        //바닥타일 내부에 있는지 체크,
        yield return StartCoroutine(PathFinder.FindPathCoroutine(item.transform.position, targetPosition, path =>
        {
            if (path != null)
            {
                indicator.notObstacle = false;
            }
            else
            {
                indicator.notObstacle = true;
            }

            indicator.itemWaitStack.Push(item);
            indicator.lineQueue.Enqueue(this);

        }, false, Direction_Type.Four));

    }
    #endregion

    #region PathFind
    public bool onPathFind;
    private PathFinder pathFinder;
    private bool notObstacle = false;
    private PathFinder PathFinder { get { pathFinder ??= GetComponent<PathFinder>(); return pathFinder; } }
    private IEnumerator SetPathCoroutine(Action applyActiveAction)
    {
        onPathFind = true;
        yield return StartCoroutine(PathFinder.FindPathCoroutine(item.transform.position, target.position, path =>
        {
            if (path != null)
            {
                //StartCoroutine(DrawOn_TargetToMain(data, path, () => Mark_Green()));
                coroutine = StartCoroutine(DrawOn_TargetToMain(path, applyActiveAction));

            }
            else
            {
                Debug.Log("경로를 찾지 못함");
                notObstacle = true;
            }
        }, notObstacle, Direction_Type.Four));

        onPathFind = false;
    }


    #endregion


    private float drawSpeed = 30f;

    #region Draw
    private IEnumerator DrawOn_TargetToMain(List<Vector2> path ,Action applyActive) //<-> MainToTarget
    {
        int index;
        Vector2 start;

        if (Line.positionCount > 0)
        {
            index = path.Count - Line.positionCount; //다음으로 시작해야할 path index
            start = Line.GetPosition(Line.positionCount - 1); //그리다만 라인렌더러 마지막 위치
        }
        else
        {
            Line.positionCount = 1;
            start = path[path.Count - 1]; //마지막요소
            Line.SetPosition(0, start);
            index = path.Count - 2;   //다음 타겟 위치 path index
        }

        for (int i = Line.positionCount; i < path.Count; i++)
        {
            // int lineIndex = path.Count - i;
            Line.positionCount = i + 1;

            Vector2 end = path[index];
            float distance = Vector3.Distance(start, end);
            float duration = distance / drawSpeed;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                Vector3 pos = Vector3.Lerp(start, end, t);
                Line.SetPosition(i, pos);
                yield return null;
            }
            Line.SetPosition(i, end);
            index--;
            start = end;
        }
        //---------------------------TEST 0627 (Mark Change)
        item.Mark_Green(); 
        //---------------------------TEST 0627 (Mark Change)

        applyActive?.Invoke();
        curApplyActive = 1;
        coroutine = null;
    }
    #endregion

    #region Erase
    public void Erase(ActivatableObject_Indicator_var2 indicator, Action applyActive)
    {
     StartCoroutine(EraseCo_MainToTarget(indicator, applyActive));
    }
    private IEnumerator EraseCo_MainToTarget(ActivatableObject_Indicator_var2 indicator,Action applyAction) //<-> Draw_TargetToMain
    {
        yield return new WaitUntil(() => !onPathFind);

        if (coroutine != null)
        {
            StopCoroutine(coroutine);
            coroutine = null;
        }

        //---------------------------TEST 0627 (Mark Change)
        item.Mark_ShutDown();
        indicator.itemWaitStack.Push(item);
        
        if (curApplyActive == 1)
        {
            applyAction?.Invoke();
            curApplyActive = 0;
        }
        
        //---------------------------TEST 0627 (Mark Change)

        if (Line.positionCount == 0)
        {
            // eraseCoroutine = null;
            StopAllCoroutines();
            indicator.lineQueue.Enqueue(this);
            yield break;
        }

        Vector2 end = Line.GetPosition(Line.positionCount - 1);

        for (int i = Line.positionCount - 2; i >= 0; i--)
        {
            Vector2 start = Line.GetPosition(i);

            float distance = Vector3.Distance(start, end);
            float duration = distance / drawSpeed; // time = d/s
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                Vector3 pos = Vector3.Lerp(end, start, t);
                Line.SetPosition(i + 1, pos);
                yield return null;
            }

            lineRenderer.positionCount = i + 1;
            end = start;
        }
        Fade(false);
        Line.positionCount = 0;
        indicator.lineQueue.Enqueue(this);

    }
    #endregion

    #region Fade
    private Coroutine fadeCoroutine;
    float maxAlpha = 1;
    float minAlpha = 0.05f;
    float duration = 1;
    public void Fade(bool onOff)
    {
        if(fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }

        fadeCoroutine = StartCoroutine(SatisfyCondition_FadeOutLineCo(onOff));
    }
    private IEnumerator SatisfyCondition_FadeOutLineCo(bool onOff)
    {

        float elapsed = 0;
        Color curStartCol = Line.startColor;
        Color fade_Start = new Color(curStartCol.r, curStartCol.g, curStartCol.b, onOff ? minAlpha : maxAlpha );

        Color curEndColr = Line.endColor;
        Color fade_End = new Color(curEndColr.r, curEndColr.g, curEndColr.b, onOff ? minAlpha : maxAlpha);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            Line.startColor = Color.Lerp(curStartCol, fade_Start, elapsed);
            Line.endColor = Color.Lerp(curEndColr, fade_End, elapsed);
            yield return null;
        }

        Line.startColor = fade_Start;
        Line.endColor = fade_End;

        fadeCoroutine = null;
    }

    #endregion
}
