using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Mirror;
using UnityEngine;


public class WDMP_Net : NetworkBehaviour
{
    [SerializeField] GameObject rail_Node_Prefabs;
    [SerializeField] LineRenderer rail_Line_Prefabs;

    // private Collider2D Collider => GetComponent<Collider2D>();

    [SyncVar] public float moveDistance;
    [SyncVar(hook =nameof(OnDataPathUpdated))] 
    public Vector2 position;
    [SyncVar] public float rayLength;

    [SyncVar] public Vector2 dir;
    [SyncVar] public float step;


    [SyncVar] public float minDis_Clamp;
    [SyncVar] public float maxDis_Clamp;

    [Server]
    public void Server_SetMoveDistance(float moveDistance,Vector2 position)
    {
       this.moveDistance = moveDistance;   
       this.position = position;
    }

    [Server]
    public void Server_SetDir(Vector2 dir)
    {
        this.dir = dir;
    }
    [Server]
    public void Server_SetStep(float step)
    {
        this.step = step;
    }
    [Server]
    public void Server_SetClamp(float min,float max)
    {
        this.minDis_Clamp = min;
        this.maxDis_Clamp = max;
    }
    [Server]
    public void Server_SetRayLength(float rayLength)
    {
        this.rayLength = rayLength;
    }

    Transform container;
    GameObject railNode_1;
    GameObject railNode_2;
    LineRenderer line;

    private void CreateRail() //rail node, rail lineRenderer
    { 
        Transform parents = MapEditor.Instance.dontSaveObjectTransform;
        container = new GameObject("Rail_Container").transform;
        container.SetParent(parents);

        line = Instantiate(rail_Line_Prefabs,container);
        //Draw Line
        DrawLine(line);

        railNode_1 = Instantiate(rail_Node_Prefabs,container);
        railNode_1.transform.position = line.GetPosition(0);

        railNode_2 = Instantiate(rail_Node_Prefabs,container);
        railNode_2.transform.position = line.GetPosition(1);
    }
    private void DrawLine(LineRenderer line)
    {
        line.positionCount = 2;
        line.SetPosition(0,position);
        Vector2 target = new Vector2(position.x + moveDistance, position.y);
        line.SetPosition(1,target);
    }


    //private void CreateNode()
    //{
    //    DrawLine(line);
    //    railNode_1.transform.position = line.GetPosition(0);
    //    railNode_2.transform.position = line.GetPosition(1);
    //}

    private void OnDataPathUpdated(Vector2 old, Vector2 newVal)
    {
        if (newVal != Vector2.zero)
        {
            CreateRail();
        }
    }




}
