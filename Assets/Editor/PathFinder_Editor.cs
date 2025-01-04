using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Mono.CecilX.Cil;
using Unity.VisualScripting;

[CustomEditor(typeof(PathFinder))]
public class PathFinder_Editor : Editor
{
    public GameObject targetObject;
    private PathFinder pathFinder;

    private Transform debugTr;
    private LineRenderer lineRenderer;

    private void OnEnable()
    {
        pathFinder = (PathFinder)target;
        if (pathFinder == null) return;

    }
    private void OnDisable()
    {
        if(debugTr != null)
        {
            lineRenderer = null;
            Undo.DestroyObjectImmediate(debugTr.gameObject);
        }
    }
    public override void OnInspectorGUI()
    {
        if (Application.isPlaying) return;

        base.OnInspectorGUI();
        EditorGUILayout.Space(50);
        GUILayout.BeginVertical(GUI.skin.window);

        if (GUILayout.Button("Create Debug Container"))
        {
            debugTr = CreateDebugSetting();
        }

        GUILayout.Label("Set Target GameObeject");
        targetObject = EditorGUILayout.ObjectField(targetObject, typeof(GameObject), true) as GameObject;
        if(GUILayout.Button("Draw Path"))
        {
            if (debugTr == null) 
            {
                EditorUtility.DisplayDialog(
                      "Can't find Debug Container",
                      "Please click the \"Create Debug Container\" button",
                      "OK"
                  );
                return;
            }
            if (targetObject == null)
            {
                EditorUtility.DisplayDialog(
                       "Can't find target object",
                       "Please set the target object",
                       "OK"
                   );
                return;
            }
            DrawLine(GetPath(targetObject));
        }
        if (GUILayout.Button("Reset"))
        {
            lineRenderer.positionCount = 0;
        }
        GUILayout.EndVertical();

    }


    private Transform CreateDebugSetting()
    {
        Transform debugContainer = new GameObject("DebugContainer").transform;
        lineRenderer = debugContainer.AddComponent<LineRenderer>();
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.positionCount = 0;
        lineRenderer.sortingLayerName = "ForeGround";
        lineRenderer.sortingOrder = 10000;
        debugContainer.SetParent(pathFinder.transform);
        return debugContainer;

    }

    private void DrawLine(List<Vector2> path)
    {
        lineRenderer.positionCount = path.Count;
        for(int i = 0; i < path.Count; i++)
        {
            lineRenderer.SetPosition(i, path[i]);
        }
    }
    
    private List<Vector2> GetPath(GameObject target)
    {
        return pathFinder.FindPath(pathFinder.transform.position,target.transform.position);
    }
}
