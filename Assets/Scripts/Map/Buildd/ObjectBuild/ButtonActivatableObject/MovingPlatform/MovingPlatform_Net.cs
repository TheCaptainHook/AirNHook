using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;


public class MovingPlatform_Net : NetworkBehaviour
{
    [SerializeField] GameObject rail_Node_Prefabs;
    [SerializeField] LineRenderer rail_Line_Prefabs;

    #region Components
    MovingPlatform main;
    MovingPlatform Main
    {
        get
        {
            main ??= GetComponent<MovingPlatform>();
            return main;
        }
    }
    Rigidbody2D rb;
    Rigidbody2D RB
    {
        get
        {
            rb ??= GetComponent<Rigidbody2D>();
            return rb;
        }
    }

    #endregion

    [Serializable]
    public struct DataPath
    {
        public Vector2[] paths;
        public float moveSpeed;
        public DataPath(Vector2[] paths, float moveSpeed)
        {
            this.paths = paths;
            this.moveSpeed = moveSpeed;
        }

    }
    #region  Init Sync

    [SyncVar(hook = nameof(OnDataPathUpdated))]
    public DataPath dataPath;

    [SyncVar] public bool onActive;

    public bool onSync;
    [Server]
    public void Server_InitSync()
    {
        if (Main.paths.Length > 0)
        {
            dataPath = new DataPath(Main.paths, Main.moveSpeed);
            maxIndex = dataPath.paths.Length;
            index = 0;
            increment = 1;
            targetPosition = dataPath.paths[index];

            Rpc_SetTargetPosition(RB.position, targetPosition);
            onFixedUpdataReady = true;
        }
        

        Rpc_InitSync(Main.ButtonActivatedObjectStruct);

    }

    [ClientRpc]
    private void Rpc_SetTargetPosition(Vector2 curPosition, Vector2 targetPosition)
    {
        RB.position = curPosition;
        this.targetPosition = targetPosition;
        Main.onArrivalPoint = false;
    }

    [Command(requiresAuthority = false)]
    private void Cmd_InitSync()
    {
        Server_InitSync();
    }
    [ClientRpc]
    private void Rpc_InitSync(ButtonActivatableObjectStruct data)
    {
        if (onSync) return;
        onSync = true;
        transform.position = data.position;
    }
    public override void OnStartClient()
    {
        base.OnStartClient();
        if (!onSync) Cmd_InitSync();
    }

    // [Server]
    // public void Server_CreateRail(Vector2[] paths, float moveSpeed)
    // {
    //     dataPath = new DataPath(paths, moveSpeed);
    // }

    public void CreateRail()
    {
        Vector2[] paths = dataPath.paths;

        Transform parents = MapEditor.Instance.dontSaveObjectTransform;
        Transform container = new GameObject("Rail_Container").transform;
        container.SetParent(parents);


        if (paths.Length == 0)
        {
            Debug.Log($"path : {paths.Length},Client : {isClient}");
            return;
        }

        //Rail Node
        LineRenderer line = Instantiate(rail_Line_Prefabs, container);
        //Draw Line
        DrawLine(line, paths);

        Vector2 startPot = line.GetPosition(0);
        Vector2 endPot = line.GetPosition(paths.Length - 1);

        GameObject railNode_1;
        GameObject railNode_2;

        if (startPot == endPot)
        {
            railNode_1 = Instantiate(rail_Node_Prefabs, container);
            railNode_1.transform.position = startPot;
        }
        else
        {
            railNode_1 = Instantiate(rail_Node_Prefabs, container);
            railNode_1.transform.position = startPot;
            railNode_2 = Instantiate(rail_Node_Prefabs, container);
            railNode_2.transform.position = endPot;
        }
    }

    private void DrawLine(LineRenderer line, Vector2[] path)
    {
        line.positionCount = path.Length;
        for (int i = 0; i < path.Length; i++)
        {
            line.SetPosition(i, path[i] + new Vector2(0, 0.25f));
        }

    }

    private void OnDataPathUpdated(DataPath oldPath, DataPath newPath)
    {
        if (newPath.paths != null)
        {
            CreateRail();

        }
    }

    #endregion


    #region Move Platform


    //--------------------------------------------------------------------------------------------------------Refectoring 0406
    private bool onFixedUpdataReady;
    private int maxIndex;
    private int index;
    private int increment;
    [ReadOnly]
    public Vector2 targetPosition;


  
    private Vector2 previousTargetPosition;

    private void Update()
    {
        if (!isServer) return;
        if (!onFixedUpdataReady) return;

        if (CheckDistance(RB.position, targetPosition))
        {
            // RB.position = targetPosition;
            previousTargetPosition = targetPosition;
            index += increment;

            if (index >= maxIndex || index < 0)
            {
                if (index >= maxIndex && dataPath.paths[maxIndex - 1] == dataPath.paths[0])
                {
                    index = 0;
                }
                else
                {
                    increment *= -1;
                    index += increment;
                }
            }

            targetPosition = dataPath.paths[index];
            //ClientRpc targetPositon sync
            Rpc_SetTargetPosition(previousTargetPosition, targetPosition);

        }
    }
    private bool CheckDistance(Vector2 curPos, Vector2 targetPos)
    {
        if (Vector3.Distance(curPos, targetPos) < 0.1f)
        {
            return true;
        }
        return false;
    }

  

    //--------------------------------------------------------------------------------------------------------Refectoring 0406
    #endregion

}
