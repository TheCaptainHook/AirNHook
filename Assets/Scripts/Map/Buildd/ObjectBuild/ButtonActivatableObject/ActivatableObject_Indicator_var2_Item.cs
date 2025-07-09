
using UnityEngine;

public class ActivatableObject_Indicator_var2_Item : MonoBehaviour
{
    // [ReadOnly]
    // public Transform targetTr;

    // [SerializeField] PathFinder pathFinder;

    // [ReadOnly]
    // public LineRenderer lineRenderer;
    [SerializeField] SpriteRenderer mainSprite;

    // [ReadOnly]
    // public uint targetId;


    // [ReadOnly]
    // public bool onActive = false;
    // [ReadOnly]
    // public bool onDraw = false;

    // public bool Setting(uint id, int inc)
    // {
    //     Transform target = NetworkClient.spawned.TryGetValue(id, out var targetObject) ? targetObject.transform : null;
    //     if (target == null) return false;

    //     targetId = id;
    //     targetTr = target;
    //     StartCoroutine(SetPathCoroutine(inc));

    //     return true;
    // }

    //--------------------------------------------------------------------------------Renewal 0704
    // private Queue<(Indicator_2_DrawLineStruct data,List<Vector2> path)> drawQueue;
    
    //public ActivatableObject_Indicator_var2 indicator_Var2;
    //public void SettingAndDraw(Indicator_2_DrawLineStruct data)
    //{
    //    StartCoroutine(SetPathCoroutine(data));      

    //}
    //public void Erase((uint id, ActivatableObject_Indicator_var2_Item item, Indicator_2_DrawLineStruct data) data)
    //{
    //    StartCoroutine(EraseCo_MainToTarget(data));
    //}

    //private IEnumerator SetPathCoroutine(Indicator_2_DrawLineStruct data)
    //{
    //    yield return StartCoroutine(pathFinder.FindPathCoroutine(transform.position, data.target.position, path =>
    //    {
    //        if (path != null)
    //        {
    //            //StartCoroutine(DrawOn_TargetToMain(data, path, () => Mark_Green()));
    //        }
    //        else
    //        {
    //            Debug.Log("경로를 찾지 못함");
    //        }
    //    }, false, Direction_Type.Four));
    //}
    //--------------------------------------------------------------------------------Renewal 0704

    #region Draw,Erase Coroutine
    // public bool onPrograss;


    #region Draw

    //private IEnumerator DrawOn_TargetToMain(Indicator_2_DrawLineStruct data,List<Vector2> path, Action markEnableAction) //<-> MainToTarget
    //{
    //    // line_direction = LINE_DIRECTION.TargetToMain;
    //    // if (path == null || path.Count < 2) yield break;

    //    LineRenderer lineRenderer = data.line;

    //    onPrograss = true;
    //    int index;
    //    Vector2 start;

    //    if (lineRenderer.positionCount > 0)
    //    {
    //        index = path.Count - lineRenderer.positionCount; //다음으로 시작해야할 path index
    //        start = lineRenderer.GetPosition(lineRenderer.positionCount - 1); //그리다만 라인렌더러 마지막 위치
    //    }
    //    else
    //    {
    //        lineRenderer.positionCount = 1;
    //        start = path[path.Count - 1]; //마지막요소
    //        lineRenderer.SetPosition(0, start);
    //        index = path.Count - 2;   //다음 타겟 위치 path index
    //    }

    //    for (int i = lineRenderer.positionCount; i < path.Count; i++)
    //    {
    //        // int lineIndex = path.Count - i;
    //        lineRenderer.positionCount = i + 1;

    //        Vector2 end = path[index];
    //        float distance = Vector3.Distance(start, end);
    //        float duration = distance / drawSpeed;
    //        float elapsed = 0f;

    //        while (elapsed < duration)
    //        {
    //            elapsed += Time.deltaTime;
    //            float t = Mathf.Clamp01(elapsed / duration);
    //            Vector3 pos = Vector3.Lerp(start, end, t);
    //            lineRenderer.SetPosition(i, pos);
    //            yield return null;
    //        }
    //        lineRenderer.SetPosition(i, end);
    //        index--;
    //        start = end;
    //    }
    //    //---------------------------TEST 0627 (Mark Change)
    //    markEnableAction?.Invoke();
    //    //---------------------------TEST 0627 (Mark Change)
    //    indicator_Var2.SetApplyActive(1);

    //    onPrograss = false;
    //    // drawCoroutine = null;


    //}
    #endregion

    #region Erase
    //private IEnumerator EraseCo_MainToTarget((uint id, ActivatableObject_Indicator_var2_Item item,Indicator_2_DrawLineStruct data) data) //<-> Draw_TargetToMain
    //{
    //    // if (line_direction == LINE_DIRECTION.MainToTarget) ReverseLineRendererPosition(true);
    //    // line_direction = LINE_DIRECTION.MainToTarget;

    //    //---------------------------TEST 0627 (Mark Change)
    //    Mark_ShutDown();
    //    indicator_Var2.itemWaitStack.Push(data.item);
    //    indicator_Var2.SetApplyActive(-1);

    //    //---------------------------TEST 0627 (Mark Change)
    //    LineRenderer lineRenderer = data.data.line;

    //    if (lineRenderer.positionCount == 0)
    //    {
    //        // eraseCoroutine = null;
    //        yield break;
    //    }

    //    onPrograss = true;

    //    Vector2 end = lineRenderer.GetPosition(lineRenderer.positionCount - 1);

    //    for (int i = lineRenderer.positionCount - 2; i >= 0; i--)
    //    {
    //        Vector2 start = lineRenderer.GetPosition(i);

    //        float distance = Vector3.Distance(start, end);
    //        float duration = distance / drawSpeed; // time = d/s
    //        float elapsed = 0f;

    //        while (elapsed < duration)
    //        {
    //            elapsed += Time.deltaTime;
    //            float t = Mathf.Clamp01(elapsed / duration);
    //            Vector3 pos = Vector3.Lerp(end, start, t);
    //            lineRenderer.SetPosition(i + 1, pos);
    //            yield return null;
    //        }

    //        lineRenderer.positionCount = i + 1;
    //        end = start;
    //    }

    //    lineRenderer.positionCount = 0;
    //    indicator_Var2.lineQueue.Enqueue(data.data.line);
    //    // eraseCoroutine = null;
    //    onPrograss = false;

    //}
    #endregion

    #endregion

    #region  Mark. (TEST/0627)
    // public void Mark_Red()
    // {
    //     mainSprite.enabled = true;
    //     mainSprite.color = Color.red;
    // }
    public void Mark_Green()
    {
        mainSprite.enabled = true;
        mainSprite.color = Color.green;
    }
    public void Mark_ShutDown()
    {
        mainSprite.color = Color.red;
        //mainSprite.enabled = false;
    }
    #endregion


    #region Fade
    // public bool isFading;
    // float maxAlpha = 1;
    // float minAlpha = 0.05f;
    // float duration = 1;
    // public Coroutine satisfyConditionCoroutine;
    // public void SatisfyCondition_FadeOutLine()
    // {
    //     if (satisfyConditionCoroutine != null) StopCoroutine(satisfyConditionCoroutine);
    //     satisfyConditionCoroutine = StartCoroutine(SatisfyCondition_FadeOutLineCo());
    // }

    // private IEnumerator SatisfyCondition_FadeOutLineCo()
    // {
    //     isFading = true;
    //     float elapsed = 0;
    //     Color curStartCol = lineRenderer.startColor;
    //     Color fade_Start = new Color(curStartCol.r, curStartCol.g, curStartCol.b, minAlpha);

    //     Color curEndColr = lineRenderer.endColor;
    //     Color fade_End = new Color(curEndColr.r, curEndColr.g, curEndColr.b, minAlpha);

    //     while(elapsed < duration)
    //     {
    //         elapsed += Time.deltaTime;
    //         lineRenderer.startColor = Color.Lerp(curStartCol, fade_Start, elapsed);
    //         lineRenderer.endColor = Color.Lerp(curEndColr, fade_End, elapsed);
    //         yield return null;
    //     }

    //     lineRenderer.startColor = fade_Start;
    //     lineRenderer.endColor = fade_End;
    //     isFading = false;
    // }
    // public void FadeRecover()
    // {
    //     if (satisfyConditionCoroutine != null)
    //     {
    //         StopCoroutine(satisfyConditionCoroutine);
    //         satisfyConditionCoroutine = null;
    //     }

    //     lineRenderer.startColor = new Color(lineStartColor.r, lineStartColor.g, lineStartColor.b, maxAlpha);
    //     lineRenderer.endColor = new Color(lineEndColor.r, lineEndColor.g, lineEndColor.b, maxAlpha);
    // }
    #endregion
}
