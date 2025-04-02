using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;


public class MovingPlatform_Net : NetworkBehaviour
{
    [SerializeField] GameObject rail_Node_Prefabs;
    [SerializeField] LineRenderer rail_Line_Prefabs;

    // private LineRenderer lineRenderer;
    // private LineRenderer LineRenderer 
    // {
    //     get
    //     {
    //         if(!lineRenderer) lineRenderer = GetComponent<LineRenderer>();
    //         return lineRenderer;
    //     }
    // }

    // MovingPlatform MovingPlatform => GetComponent<MovingPlatform>();

    [SyncVar(hook = nameof(Hook_Addforce_OnReady))] public bool addForce_OnReady;
    private void Hook_Addforce_OnReady(bool old,bool newVal)
    {
        if(newVal) AddForcePlatform.onReady = true;
    }

    [Serializable]
    public struct DataPath
    {
        public Vector2[] paths;
        public DataPath(Vector2[] paths)
        {
            this.paths = paths;
        } 
    }

    [SyncVar(hook =nameof(OnDataPathUpdated))]
    public DataPath dataPath;
//----------------------------------------------------------------040
    // [SyncVar] public Vector2 velocity;
    // [SyncVar] public float step;
    private AddForcePlatform addForcePlatform;
    private AddForcePlatform AddForcePlatform
    {
        get
        {
            addForcePlatform ??= GetComponent<AddForcePlatform>();
            return addForcePlatform;
        }
    }
//----------------------------------------------------------------0402
    [Server]
    public void Server_CreateRail(Vector2[] paths)
    {
        dataPath = new DataPath(paths);

    }

//----------------------------------------------------------------0402
    // [Server]
    // public void Server_SetVelocity(Vector2 velocity,float step)
    // {
    //     this.velocity = velocity;
    //     this.step = step;
    // }
//----------------------------------------------------------------0402

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

    private void DrawLine(LineRenderer line,Vector2[] path)
    {
        line.positionCount = path.Length;
        for(int i = 0; i<path.Length;i++){
            line.SetPosition(i,path[i]+new Vector2(0,0.25f));
        }
        
    }

    private void OnDataPathUpdated(DataPath oldPath, DataPath newPath)
    {
        if (newPath.paths != null)
        {
            CreateRail();
            // MovingPlatform.AddForce();
        }
    }
    


    #region AddForce Platform
    [Server]
    public void Server_AddForce(Vector2 velocity,float step)
    {
        Rpc_AddForce(velocity,step);
    }
    [ClientRpc]
    private void Rpc_AddForce(Vector2 velocity,float step)
    {
        Debug.Log($"[Rpc_AddForce] velocoty : {velocity}, step : {step}\nisServer : {isServer}\nisClient : {isClient}");
        AddForcePlatform.AddForce(velocity,step);
    }
    #endregion

}
