using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;
using UnityEngine.Assertions.Must;

public class TeslaNodeRod_Net : NetworkBehaviour
{
    private TeslaNodeRod main;
    private ButtonObjectStruct data;

    private PathFinder pathFinder;
    [SerializeField] Transform lineContainer;
    [SerializeField] Material lineMat;
    private void Awake()
    {
        main = GetComponent<TeslaNodeRod>();
        pathFinder = GetComponent<PathFinder>();
    }

    #region Init Sync
    public bool onSync;
    [Server]
    public void Server_InitSync()
    {
        Rpc_InitSync(main.ButtonObjectData);
    }
    [ClientRpc]
    private void Rpc_InitSync(ButtonObjectStruct data)
    {
        if (onSync) return;
        this.data = data;
        transform.position = data.position;
        transform.rotation = data.quaternion;
        // CreateLine();
        onSync = true;

    }
    [Command(requiresAuthority = false)]
    private void Cmd_InitSync()
    {
        Server_InitSync();
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (!onSync) Cmd_InitSync();
    }

    #endregion

    #region IPowerConsumer
    [SyncVar(hook = nameof(Hook_OnChangeHasPower))]
    public int hasPower;
    public bool isActive;
    private void Hook_OnChangeHasPower(int old,int newVal)
    {
        if (newVal == 1)
        {
            if (!isActive)
            {
                isActive = true;
                main.Net_Active();
            }
            //Active
        }else if(newVal == 0)
        {
            isActive = false;
            main.Net_DeActive();
            //Deactive
        }
    }
   
    [Server]
    public void Server_SetHasPower(bool value)
    {
        if(value)
        {   
            hasPower++;

        }
        else
        {
            hasPower--;
        }
    }

    #endregion

    #region Line
    private LineRenderer[] lineArr;
  
    private void CreateLine()
    {
        lineArr = new LineRenderer[data.targetPositions.Count];
        Vector2 startPot = lineContainer.position;

        for(int i = 0; i< lineArr.Length; i++)
        {
            Vector2 endPot = data.targetPositions[i];
            LineRenderer line = GeneratorLineRenderer();
            lineArr[i] = line;

            StartCoroutine(pathFinder.FindPathCoroutine(startPot, endPot, path =>
            {
                if (path != null)
                {
                    SetLine(line,path);
                }   
                
            }));

            // SetLine(line, pathFinder.FindPath(startPot, endPot, false, Direction_Type.Four));
        }

    }
    private void SetLine(LineRenderer lineRenderer, List<Vector2> path)
    {
        lineRenderer.positionCount = path.Count;
        for (int i = 0; i < path.Count; i++)
        {
            Vector3 worldPosition = path[i];
            lineRenderer.SetPosition(i, worldPosition);
        }
    }

    private LineRenderer GeneratorLineRenderer()
    {
        GameObject obj = new GameObject("LineRenderer");
        LineRenderer lineRenderer = obj.AddComponent<LineRenderer>();
        lineRenderer.useWorldSpace = false;
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
            //TEST
        lineRenderer.material = lineMat;
            //TEST
        lineRenderer.positionCount = 0;
        lineRenderer.sortingLayerName = "BackGround";
        lineRenderer.sortingOrder = 1;
        obj.transform.SetParent(lineContainer);

        return lineRenderer;
    }
    [SerializeField] Material activeMat;
    [SerializeField] Material deactiveMat;
    public void LineActive()
    {
        if(lineArr == null || lineArr.Length == 0)return;

        for (int i = 0; i < lineArr.Length; i++)
        {
            var line = lineArr[i];
            //TEST
            line.material = activeMat;
            line.startWidth = 0.5f;
            line.endWidth = 0.5f;
            // line.startColor = Color.blue;
            // line.endColor = Color.blue;
            //TEST
        }
    }
    public void LineDeActive()
    {
        if(lineArr == null || lineArr.Length == 0)return;
        
        for (int i = 0; i < lineArr.Length; i++)
        {
            var line = lineArr[i];
            //TEST
            line.material = deactiveMat;
            line.startWidth = 0.05f;
            line.endWidth = 0.05f;
            // line.startColor = Color.white;
            // line.endColor = Color.white;
            //TEST
        }
    }
    #endregion

}
