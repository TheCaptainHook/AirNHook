#if UNITY_EDITOR


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Unity.EditorCoroutines.Editor;
using UnityEngine.SceneManagement;

[ExecuteInEditMode]
public class TeslaNodeRod_LineToTarget : MonoBehaviour
{
    private TeslaNodeRod main;

    private Transform debugTr;
    private List<LineRenderer> targetLineList;
    private List<LineRenderer> lightLineList;
    Vector3Int previousePosition;


    public void Setting()
    {
        StopAllCoroutines();

        main = GetComponent<TeslaNodeRod>();
        previousePosition = ConvertPosition(transform.position);

        transform.position = previousePosition;


        CreateDebugTransform();

        //Target Object
        targetLineList = new();
        for (int i = 0; i < main.targetObjects.Count; i++)
        {
            LineRenderer line = GeneratorLineRenderer(targetLineList, Color.red);
            SetLine(line, ConvertPosition(main.targetObjects[i].transform.position));
        }
        //Light Object
        lightLineList = new();
        for (int i = 0; i < lightLineList.Count; i++)
        {
            LineRenderer line = GeneratorLineRenderer(lightLineList, Color.blue);
            SetLine(line, main.lightObjects[i].transform.position);
        }

    }

    Coroutine coroutine;

    public void StartRefrash()
    {
        coroutine = StartCoroutine(RefrashCo());
    }
    public void StopRefrash()
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
            Destroy();
        }
    }

    private IEnumerator RefrashCo()
    {

        while (true)
        {
            Refrash();
            yield return null;
        }

    }

    private void Refrash()
    {
        if (main == null) return;

        if (previousePosition != transform.position)
        {
            previousePosition = ConvertPosition(transform.position);
            transform.position = previousePosition;
        }

        CheckIPowerConsumers();

        CompareTargetListToLineList();
        CompareLightListToLineList();

        for (int i = 0; i < main.targetObjects.Count; i++)
        {
            Vector3Int pot = ConvertPosition(main.targetObjects[i].transform.position);
            main.targetObjects[i].transform.position = pot;
            SetLine(targetLineList[i], pot);
        }
        for (int i = 0; i < main.lightObjects.Count; i++)
        {
            Vector3 pot = main.lightObjects[i].transform.position;
            main.lightObjects[i].transform.position = pot;
            SetLine(lightLineList[i], pot);
        }
    }



    private void CompareTargetListToLineList()
    {
        if (main.targetObjects.Count > targetLineList.Count)
        {
            for (int i = targetLineList.Count; i < main.targetObjects.Count; i++)
            {
                GeneratorLineRenderer(targetLineList, Color.red);
            }
        }
        else if (main.targetObjects.Count < targetLineList.Count)
        {
            for (int i = targetLineList.Count - 1; i >= main.targetObjects.Count; i--)
            {
                GameObject lineObj = targetLineList[i].gameObject;
                targetLineList.RemoveAt(i);
                Undo.DestroyObjectImmediate(lineObj);
            }
        }
    }
    private void CompareLightListToLineList()
    {
        if (main.lightObjects.Count > lightLineList.Count)
        {
            for (int i = lightLineList.Count; i < main.lightObjects.Count; i++)
            {
                GeneratorLineRenderer(lightLineList, Color.blue);
            }
        }
        else if (main.lightObjects.Count < lightLineList.Count)
        {
            for (int i = lightLineList.Count - 1; i >= main.lightObjects.Count; i--)
            {
                GameObject lineObj = lightLineList[i].gameObject;
                lightLineList.RemoveAt(i);
                Undo.DestroyObjectImmediate(lineObj);
            }
        }
    }
    private void CheckIPowerConsumers()
    {
        //Target Object
        if (main.targetObjects.Count == 0) return;

        for (int i = 0; i < main.targetObjects.Count; i++)
        {
            //if (main.targetObjects[i] == null || !main.targetObjects[i].TryGetComponent(out IPowerConsumer _))
            //{
            //    main.targetObjects.RemoveAt(i);
            //    continue;
            //}

            if(main.targetObjects[i] != null || 
                main.targetObjects[i].TryGetComponent(out IPowerConsumer _) || 
                main.targetObjects[i].TryGetComponent(out ActivatableObjectEntity _))
            {
                continue;       
            }

            main.targetObjects.RemoveAt(i);

        }

        //Light Object
        if (main.lightObjects.Count == 0) return;
        for (int i = 0; i < main.lightObjects.Count; i++)
        {
            if (main.lightObjects[i] == null || !main.lightObjects[i].TryGetComponent(out IPowerConsumer _))
            {
                main.lightObjects.RemoveAt(i);
                continue;
            }
        }

    }
    public void Destroy()
    {
        if (debugTr == null) return;
        Undo.DestroyObjectImmediate(debugTr.gameObject);

    }

    #region  Util
    private Vector3Int ConvertPosition(Vector3 pot)
    {
        return new Vector3Int(
            Mathf.RoundToInt(pot.x),
            Mathf.RoundToInt(pot.y),
            Mathf.RoundToInt(pot.z)
        );
    }
    private void CreateDebugTransform()
    {
        foreach (Transform tr in transform)
        {
            if (tr.name == "DebugTransform")
            {
                Undo.DestroyObjectImmediate(tr.gameObject);
                break;
            }
        }
        debugTr = new GameObject("DebugTransform").transform;
        debugTr.SetParent(transform);
    }
    public bool IsObjectVisibleInInspector()
    {
        try
        {
            return gameObject.scene.IsValid() && gameObject.scene == SceneManager.GetActiveScene();
        }
        catch
        {
            return false;
        }

    }

    #endregion

    #region Draw Line
    private LineRenderer GeneratorLineRenderer(List<LineRenderer> list, Color color)
    {

        GameObject obj = new GameObject("LineRenderer");
        LineRenderer lineRenderer = obj.AddComponent<LineRenderer>();
        list.Add(lineRenderer);

        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = color;
        lineRenderer.endColor = color;

        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.positionCount = 0;
        lineRenderer.sortingLayerName = "ForeGround";
        lineRenderer.sortingOrder = 10000;
        obj.transform.SetParent(debugTr);

        return lineRenderer;
    }
    private void SetLine(LineRenderer lineRenderer, Vector3Int end)
    {
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, end);
    }
    private void SetLine(LineRenderer lineRenderer, Vector3 end)
    {
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, end);
    }


    #endregion
}

#endif