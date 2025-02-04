using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;


public class MovingPlatform_Net : NetworkBehaviour
{
    [SerializeField] GameObject rail_Node_Prefabs;
    [SerializeField] LineRenderer rail_Line_Prefabs;

    private LineRenderer lineRenderer;
    private LineRenderer LineRenderer 
    {
        get
        {
            if(!lineRenderer) lineRenderer = GetComponent<LineRenderer>();
            return lineRenderer;
        }
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


    [SyncVar] public DataPath dataPath;

    [Server]
    public void Server_CreateRail(Vector2[] paths)
    {
        dataPath = new DataPath(paths);
        CreateRail();
    }

    [ClientRpc]
    public void CreateRail()
    {
        Vector2[] paths = dataPath.paths;

        Transform parents = MapEditor.Instance.dontSaveObjectTransform;
        Transform container = new GameObject("Rail_Container").transform;
        container.SetParent(parents);
        
        //Rail Node
        LineRenderer line = Instantiate(rail_Line_Prefabs,container);
        //Draw Line
        DrawLine(line,paths);

        Vector2 startPot = line.GetPosition(0);
        Vector2 endPot = line.GetPosition(paths.Length-1);
        
        GameObject railNode_1;
        GameObject railNode_2;

        if(startPot == endPot){
            railNode_1 = Instantiate(rail_Node_Prefabs,container);
            railNode_1.transform.position = startPot;
        }else{
            railNode_1 = Instantiate(rail_Node_Prefabs,container);
            railNode_1.transform.position = startPot;
            railNode_2 = Instantiate(rail_Node_Prefabs,container);
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

}
